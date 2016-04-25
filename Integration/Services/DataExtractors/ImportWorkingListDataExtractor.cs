namespace Bars.Gkh.Ris.Integration.Services.DataExtractors
{
    using B4.DataAccess;
    using B4.Utils;
    using Castle.Windsor;
    using Domain;
    using Entities.HouseManagement;
    using Entities.Services;
    using Enums;
    using FileService;
    using Repair.Entities;
    using Ris.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.Gkh.Ris.Entities;

    using Entities.Nsi;

    public class ImportWorkingListDataExtractor : GisIntegrationDataExtractorBase
    {
        private Dictionary<string, RisHouse> risHouseByFiasGuid = null;
        private Dictionary<string, RisContract> actualContractsByHouseFias = null;
        private Dictionary<long, ServiceType> serviceTypeByGkhId = null;

        public override string Code
        {
            get {return "importWorkingList"; }
        }

        /// <summary>
        /// Подготовка данных.
        /// </summary>
        protected override void FillDictionaries()
        {
            var risHouseDomain = this.Container.ResolveDomain<RisHouse>();
            var contractObjectDomain = this.Container.ResolveDomain<ContractObject>();
            var serviceTypeDomain = this.Container.ResolveDomain<ServiceType>();

            try
            {
                this.risHouseByFiasGuid = risHouseDomain.GetAll()
                    .Where(x => x.FiasHouseGuid != "")
                    .GroupBy(x => x.FiasHouseGuid)
                    .ToDictionary(x => x.Key, x => x.First());

                this.actualContractsByHouseFias = contractObjectDomain.GetAll()
                    .Where(x => x.FiasHouseGuid != "")
                    .Where(x => x.Contract != null) // у нас загружены только актуальные договоры
                    .ToArray()
                    .Select(x => new
                    {
                        x.FiasHouseGuid,
                        x.Contract
                    })
                    .GroupBy(x => x.FiasHouseGuid)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Contract).First());

                // всё это должно формироваться и сохраняться в методе importOrganizationWorks
                //this.serviceTypeByGkhId = serviceTypeDomain.GetAll()
                //    .Select(x => new
                //    {
                //        x.GkhId,
                //        x.GisId,
                //        x.GisGuid
                //    })
                //    .ToList()
                //    .GroupBy(x => x.GkhId)
                //    .ToDictionary(x => x.Key, x => x.Select(y => new nsiRef
                //    {
                //        Code = y.GisId,
                //        GUID = y.GisGuid
                //    })
                //    .First());

                this.serviceTypeByGkhId = serviceTypeDomain.GetAll()
                    .Select(x => new
                    {
                        x.GisDictRef.GkhId,
                        ServiceType = x
                    })
                    .GroupBy(x => x.GkhId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.ServiceType).First());
            }
            finally
            {
                this.Container.Release(risHouseDomain);
                this.Container.Release(contractObjectDomain);
                this.Container.Release(serviceTypeDomain);
            }
        }
        /// <summary>
        /// Сохранение объектов РИС.
        /// </summary>
        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var serviceDomain = this.Container.ResolveDomain<RepairObject>();
            var fileUploadService = this.Container.Resolve<IFileUploadService>();
            var workingListAttachmentDomain = this.Container.ResolveDomain<WorkingListAttachment>();
            var repairWorkDomain = this.Container.ResolveDomain<RepairWork>();

            try
            {
                var workingListsRepairObject = serviceDomain.GetAll()
                    .Where(x => x.RealityObject != null && x.RealityObject.HouseGuid != "")
                    .Where(x => x.RepairProgram != null && x.RepairProgram.Period != null)
                    .ToArray()
                    .Select(x => new
                    {
                        RepairObject = x,
                        WorkList = new WorkList
                        {
                            ExternalSystemEntityId = x.Id,
                            ExternalSystemName = "gkh",
                            Contract = this.actualContractsByHouseFias.Get(x.RealityObject.HouseGuid),
                            House = this.risHouseByFiasGuid.Get(x.RealityObject.HouseGuid),
                            MonthFrom = x.RepairProgram.Period.DateStart.Month,
                            YearFrom = (short)x.RepairProgram.Period.DateStart.Year,
                            MonthTo = x.RepairProgram.Period.DateEnd.HasValue ? ((DateTime)x.RepairProgram.Period.DateEnd).Month : 0,
                            YearTo = (short)(x.RepairProgram.Period.DateEnd.HasValue ? ((DateTime)x.RepairProgram.Period.DateEnd).Year : 0)
                        }
                    }).ToList();

                var workListByRepairObjectId = workingListsRepairObject
                    .Select(x => new
                    {
                        RepairObjectId = x.RepairObject.Id,
                        WorkList = x.WorkList
                    })
                    .GroupBy(x => x.RepairObjectId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.WorkList).First());

                var workingListsToSave = workingListsRepairObject.Select(x => x.WorkList).ToList();
                this.SaveRisEntities<WorkList, RepairObject>(workingListsToSave);

                foreach (var item in workingListsRepairObject)
                {
                    var file = item.RepairObject.ReasonDocument;
                    WorkingListAttachment newAttachment = null;

                    try
                    {
                        newAttachment = new WorkingListAttachment
                        {
                            WorkList = item.WorkList,
                            Attachment = fileUploadService.SaveAttachment(
                                file, item.RepairObject.Comment, FileStorageName.Services, this.Contragent.SenderId)
                        };
                    }
                    catch// в локальном хранилище нет файлов
                    { }

                    if (newAttachment != null)
                    {
                        workingListAttachmentDomain.Save(newAttachment);
                    }
                }

                var workListItemsToSave = repairWorkDomain.GetAll()
                    .Where(x => x.RepairObject != null)
                    .ToArray()
                    .Select((x, index) => new WorkListItem
                    {
                        WorkList = workListByRepairObjectId.Get(x.RepairObject.Id),
                        TotalCost = x.CostSum ?? 0m,
                        WorkItemNsi = this.serviceTypeByGkhId.Get(x.Work.Id),
                        Index = index
                    }).ToList();

                this.SaveRisEntities<WorkListItem, RepairWork>(workListItemsToSave);

                return new Dictionary<Type, List<BaseRisEntity>>();
            }
            finally
            {
                this.Container.Release(serviceDomain);
                this.Container.Release(fileUploadService);
                this.Container.Release(workingListAttachmentDomain);
                this.Container.Release(repairWorkDomain);
            }
        }

    }
}
