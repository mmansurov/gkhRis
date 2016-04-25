namespace Bars.Gkh.Ris.Integration.Services.DataExtractors
{
    using System;

    using B4.DataAccess;
    using B4.Utils;
    using Entities.GisIntegration.Ref;
    using Entities.HouseManagement;
    using Entities.Services;
    using Gkh.Entities;
    using Gkh.Entities;
    using Ris.Services;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.Gkh.Ris.Entities;

    using Gkh1468.Entities;
    using GkhDi.Entities;

    public class ImportMSRSODataExtractor : GisIntegrationDataExtractorBase
    {
        private Dictionary<long, nsiRef> contractBaseDict = null;
        private Dictionary<long, RisHouse> risHouseByContractIds = null;

        public override string Code
        {
            get { return "importMSRSO"; }
        }

        /// <summary>
        /// Подготовка данных.
        /// </summary>
        protected override void FillDictionaries()
        {
            var contractDomain = this.Container.ResolveDomain<RealityObjectResOrg>();
            var gisDictRefDomain = this.Container.ResolveDomain<GisDictRef>();
            var risHouse = this.Container.ResolveDomain<RisHouse>();

            try
            {
                var risHouseByFiasGuid = risHouse.GetAll()
                    .Where(x => x.FiasHouseGuid != "")
                    .GroupBy(x => x.FiasHouseGuid)
                    .ToDictionary(x => x.Key, x => x.First());

                this.risHouseByContractIds = contractDomain.GetAll()
                    .Where(
                        x =>
                            x.RealityObject != null &&
                            x.RealityObject.HouseGuid != null)

                    .Select(x => new
                    {
                        ContractId = x.Id,
                        RoHouseGuid = x.RealityObject.HouseGuid
                    })
                    .AsEnumerable()
                    .Select(x => new
                    {
                        x.ContractId,
                        RisHouse = risHouseByFiasGuid.Get(x.RoHouseGuid)
                    })
                    .Where(x => x.RisHouse != null)
                    .ToList()
                    .GroupBy(x => x.ContractId)
                    .ToDictionary(x => x.Key, x => x.First().RisHouse);

                this.contractBaseDict = gisDictRefDomain.GetAll()
                    .Where(x => x.Dict.ActionCode == "Услуга")
                    .Select(x => new
                    {
                        x.GkhId,
                        x.GisId,
                        x.GisGuid
                    })
                    .ToList()
                    .GroupBy(x => x.GkhId)
                    .ToDictionary(x => x.Key, x => x.Select(y => new nsiRef
                    {
                        Code = y.GisId,
                        GUID = y.GisGuid
                    })
                    .First());
            }
            finally
            {
                this.Container.Release(gisDictRefDomain);
                this.Container.Release(risHouse);
                this.Container.Release(contractDomain);
            }
        }

        /// <summary>
        /// Сохранение объектов РИС.
        /// </summary>
        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var serviceDomain = this.Container.ResolveDomain<RealityObjectResOrgService>();

            try
            {
                var servicesToSave = serviceDomain.GetAll()
                    .Where(x => x.RoResOrg != null && x.Service != null)
                    .ToArray()
                    .Select(x => new RisHouseService
                    {
                        ExternalSystemEntityId = x.Id,
                        ExternalSystemName = "gkh",
                        House = this.risHouseByContractIds.Get(x.RoResOrg.Id, null),
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        MsTypeCode = this.contractBaseDict.ContainsKey(x.Service.Id)
                            ? this.contractBaseDict[x.Service.Id].Code
                            : null,
                        MsTypeGuid = this.contractBaseDict.ContainsKey(x.Service.Id)
                            ? this.contractBaseDict[x.Service.Id].GUID
                            : null
                    }).ToList();

                this.SaveRisEntities<RisHouseService, RealityObjectResOrgService>(servicesToSave);

                return new Dictionary<Type, List<BaseRisEntity>>
                {
                    { typeof(RisHouseService), servicesToSave.Cast<BaseRisEntity>().ToList() }
                };
            }
            finally
            {
                this.Container.Release(serviceDomain);
            }
        }
    }
}
