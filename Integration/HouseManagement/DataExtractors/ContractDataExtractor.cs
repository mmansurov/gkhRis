namespace Bars.Gkh.Ris.Integration.HouseManagement.DataExtractors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4.DataAccess;
    using B4.Utils;

    using Bars.Gkh.Domain;

    using Entities;
    using Entities.GisIntegration.Ref;
    using Entities.HouseManagement;
    using Enums;
    using Enums.HouseManagement;
    using FileService;
    using Gkh.Entities;
    using Gkh.Enums;
    using Gis.DomainService.House;
    using Gis.Entities.Dict;
    using Gis.Entities.Kp50;
    using Gis.Enum;
    using Ris.HouseManagement;

    public class ContractDataExtractor : GisIntegrationDataExtractorBase
    {
        private Dictionary<long, nsiRef> contractBaseDict;
        private Dictionary<long, nsiRef> houseServiceDict;
        private Dictionary<long, nsiRef> addServiceDict;
        private Dictionary<long, RealityObject> realityObjsByContractIds;

        protected override void FillDictionaries()
        {
            var gisDictRefDomain = this.Container.ResolveDomain<GisDictRef>();
            var gisDictRefAddServDomain = this.Container.ResolveDomain<GisDictRef>();
            var gisDictRefHouseServDomain = this.Container.ResolveDomain<GisDictRef>();
            var manOrgRoDomain = this.Container.ResolveDomain<ManagingOrgRealityObject>();
            var risHouse = this.Container.ResolveDomain<RisHouse>();

            try
            {
                this.realityObjsByContractIds = manOrgRoDomain.GetAll()
                    .Where(
                        x =>
                            x.ManagingOrganization != null &&
                            x.RealityObject != null &&
                            x.RealityObject.HouseGuid != null)

                    .Select(x => new
                    {
                        ManOrgId = x.ManagingOrganization.Id,
                        Ro = x.RealityObject
                    })
                    .ToList()
                    .GroupBy(x => x.ManOrgId)
                    .ToDictionary(x => x.Key, x => x.First().Ro);

                this.contractBaseDict = gisDictRefDomain.GetAll()
                    .Where(x => x.Dict.ActionCode == "Основание заключения договора")
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

                this.houseServiceDict = gisDictRefDomain.GetAll()
                   .Where(x => x.Dict.NsiRegistryNumber == "3") // Коммунальные услуги
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

                this.addServiceDict = gisDictRefDomain.GetAll()
                   .Where(x => x.Dict.NsiRegistryNumber == "1") // Прочие услуги
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
                this.Container.Release(gisDictRefAddServDomain);
                this.Container.Release(gisDictRefHouseServDomain);
                this.Container.Release(manOrgRoDomain);
                this.Container.Release(risHouse);
            }
        }

        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var contractDomain = this.Container.ResolveDomain<ManOrgContractOwners>();
            var fileUploadService = this.Container.Resolve<IFileUploadService>();
            var contractAttachmentDomain = this.Container.ResolveDomain<RisContractAttachment>();
            var contragentDomain = this.Container.ResolveDomain<RisContragent>();
            var protocolOkDomain = this.Container.ResolveDomain<RisProtocolOk>();
            var protocolMeetingOwnersDomain = this.Container.ResolveDomain<ProtocolMeetingOwner>();

            try
            {
                long[] selectedIds = { };

                var selectedValue = parameters.GetAs("selectedList", string.Empty);
                if (selectedValue.ToUpper() == "ALL")
                {
                    selectedIds = null; // выбраны все, фильтрацию не накладываем
                }
                else
                {
                    selectedIds = selectedValue.ToLongArray();
                }

                var contragentsByExternalId = contragentDomain.GetAll()
                    .Select(x => new
                    {
                        ExternalId = x.GkhId,
                        Contragent = x
                    })
                    .GroupBy(x => x.ExternalId)
                    .ToDictionary(x => x.Key, x => x.First());

                var gkhContracts = contractDomain.GetAll()
                    .WhereIf(selectedIds != null, x => selectedIds.Contains(x.Id))
                    .Where(x => x.TerminateReason == "")
                    .Where(x => (x.EndDate == null) || (x.EndDate != null && x.EndDate > DateTime.Now))
                    .Where(x => x.TypeContractManOrgRealObj == TypeContractManOrg.ManagingOrgJskTsj || x.TypeContractManOrgRealObj == TypeContractManOrg.ManagingOrgOwners)
                    .Where(x =>
                           x.ManagingOrganization != null &&
                           x.ManagingOrganization.ActivityGroundsTermination == GroundsTermination.NotSet &&
                           x.ManagingOrganization.TypeManagement == TypeManagementManOrg.UK)
                    .Where(x => x.ManagingOrganization.Contragent != null)
                    .WhereIf(this.Contragent != null, x => x.ManagingOrganization.Contragent.Id == this.Contragent.GkhId)
                    .ToList();

                var risContracts = gkhContracts
                    .Select(x => new
                    {
                        ContractId = x.Id,
                        RisContract = new RisContract
                        {
                            ExternalSystemEntityId = x.Id,
                            ExternalSystemName = "gkh",
                            DocNum = x.DocumentNumber,
                            SigningDate = x.DocumentDate,
                            EffectiveDate = x.StartDate,
                            PlanDateComptetion = x.PlannedEndDate,
                            ValidityYear = this.GetYearsDifference(x.StartDate, x.PlannedEndDate),
                            ValidityMonth = this.GetMonthsDifference(x.StartDate, x.PlannedEndDate),
                            OwnersType = this.GetOwnersType(x.ManagingOrganization.TypeManagement),
                            ContractBaseCode = this.contractBaseDict.ContainsKey((long)x.ContractFoundation)
                                ? this.contractBaseDict[(long)x.ContractFoundation].Code
                                : null,
                            ContractBaseGuid = this.contractBaseDict.ContainsKey((long)x.ContractFoundation)
                                ? this.contractBaseDict[(long)x.ContractFoundation].GUID
                                : null,
                            Org = contragentsByExternalId.ContainsKey(x.ManagingOrganization.Contragent.Id) ?
                                    contragentsByExternalId.Get(x.ManagingOrganization.Contragent.Id).Contragent : null,
                            InputMeteringDeviceValuesBeginDate = x.InputMeteringDeviceValuesBeginDate,
                            InputMeteringDeviceValuesEndDate = x.InputMeteringDeviceValuesEndDate,
                            DrawingPaymentDocumentDate = x.DrawingPaymentDocumentDate,
                            ThisMonthPaymentDocDate = x.ThisMonthPaymentDocDate
                        }
                    }).ToList();

                var contractsToSave = risContracts.Select(x => x.RisContract).ToList();
                this.SaveRisEntities<RisContract, ManOrgContractOwners>(contractsToSave);

                var contractsById = risContracts.GroupBy(x => x.ContractId)
                   .ToDictionary(x => x.Key, x => x.First().RisContract);

                var risContractsObjectsToSave = gkhContracts
                    .Select(x => new ContractObject
                    {
                        ExternalSystemEntityId = x.Id,
                        ExternalSystemName = "gkh",
                        FiasHouseGuid = this.realityObjsByContractIds.Get(x.ManagingOrganization.Id).HouseGuid,
                        RealityObjectId = this.realityObjsByContractIds.Get(x.ManagingOrganization.Id).Id,
                        Contract = contractsById.Get(x.Id),
                        StartDate = x.StartDate
                    }).ToList();

                this.SaveRisEntities<ContractObject, ManOrgContractOwners>(risContractsObjectsToSave);

                var communalServicesToSave = new List<HouseManService>();
                var addServicesToSave = new List<AddService>();

                foreach (var contractObject in risContractsObjectsToSave)
                {
                    var services = this.GetAllServices(contractObject.RealityObjectId);

                    foreach (var communalService in services.Where(x => x.TypeService == TypeServiceGis.Communal))
                    {
                        communalServicesToSave.Add(new HouseManService
                        {
                            ExternalSystemEntityId = communalService.Id,
                            ExternalSystemName = "gkh",
                            ServiceTypeCode = this.houseServiceDict.Get(communalService.Code).ReturnSafe(x => x.Code),
                            ServiceTypeGuid = this.houseServiceDict.Get(communalService.Code).ReturnSafe(x => x.GUID),
                            StartDate = contractObject.StartDate,
                            EndDate = contractObject.EndDate,
                            BaseServiceCurrentDoc = true,
                            ContractObject = contractObject
                        });
                    }

                    foreach (var communalService in services.Where(x => x.TypeService != TypeServiceGis.Communal))
                    {
                        addServicesToSave.Add(new AddService
                        {
                            ExternalSystemEntityId = communalService.Id,
                            ExternalSystemName = "gkh",
                            ServiceTypeCode = this.addServiceDict.Get(communalService.Code).ReturnSafe(x => x.Code),
                            ServiceTypeGuid = this.addServiceDict.Get(communalService.Code).ReturnSafe(x => x.GUID),
                            StartDate = contractObject.StartDate,
                            EndDate = contractObject.EndDate,
                            BaseServiceCurrentDoc = true,
                            ContractObject = contractObject
                        });
                    }
                }

                this.SaveRisEntities<HouseManService, BilServiceDictionary>(communalServicesToSave);
                this.SaveRisEntities<AddService, BilServiceDictionary>(addServicesToSave);

                var contractAttachmentList = new List<RisContractAttachment>();
                foreach (var contract in gkhContracts)
                {
                    var file = contract.FileInfo;

                    if (file == null)
                    {
                        continue;
                    }

                    RisContractAttachment newContractAttachment = null;

                    try
                    {
                        newContractAttachment = new RisContractAttachment
                        {
                            Contract = contractsById.Get(contract.Id),
                            ExternalSystemEntityId = file.Id,
                            ExternalSystemName = "gkh",
                            Attachment = fileUploadService.SaveAttachment(
                                file, contract.Note, FileStorageName.HomeManagement, this.Contragent.SenderId),
                        };
                    }
                    catch
                    {
                        // в локальном хранилище нет файлов
                    }

                    if (newContractAttachment != null)
                    {
                        contractAttachmentDomain.Save(newContractAttachment);
                        contractAttachmentList.Add(newContractAttachment);
                    }
                }

                var protocolOkList = new List<RisProtocolOk>();
                foreach (var contract in gkhContracts
                    .Where(x => x.ManagingOrganization != null && x.ManagingOrganization.TypeManagement == TypeManagementManOrg.UK)
                    .Where(x => x.ContractFoundation == ManOrgContractOwnersFoundation.OpenTenderResult))
                {
                    var file = contract.ProtocolFileInfo;

                    if (file == null)
                    {
                        continue;
                    }

                    RisProtocolOk newProtocolOk = null;

                    try
                    {
                        newProtocolOk = new RisProtocolOk
                        {
                            Contract = contractsById.Get(contract.Id),
                            ExternalSystemEntityId = file.Id,
                            ExternalSystemName = "gkh",
                            Attachment = fileUploadService.SaveAttachment(
                                file, contract.Note, FileStorageName.HomeManagement, this.Contragent.SenderId),
                        };
                    }
                    catch
                    {
                        // в локальном хранилище нет файлов
                    }

                    if (newProtocolOk != null)
                    {
                        protocolOkDomain.Save(newProtocolOk);
                        protocolOkList.Add(newProtocolOk);
                    }
                }

                var protocolMeetOwnerList = new List<ProtocolMeetingOwner>();
                foreach (var contract in gkhContracts
                    .Where(x => x.ManagingOrganization != null && x.ManagingOrganization.TypeManagement == TypeManagementManOrg.UK)
                    .Where(x => x.ContractFoundation == ManOrgContractOwnersFoundation.OwnersMeetingProtocol))
                {
                    var file = contract.ProtocolFileInfo;

                    if (file == null)
                    {
                        continue;
                    }

                    ProtocolMeetingOwner newProtocolMeetingOwner = null;

                    try
                    {
                        newProtocolMeetingOwner = new ProtocolMeetingOwner
                        {
                            Contract = contractsById.Get(contract.Id),
                            ExternalSystemEntityId = file.Id,
                            ExternalSystemName = "gkh",
                            Attachment = fileUploadService.SaveAttachment(
                                file, contract.Note, FileStorageName.HomeManagement, this.Contragent.SenderId),
                        };
                    }
                    catch
                    {
                        // в локальном хранилище нет файлов
                    }

                    if (newProtocolMeetingOwner != null)
                    {
                        protocolMeetingOwnersDomain.Save(newProtocolMeetingOwner);
                        protocolMeetOwnerList.Add(newProtocolMeetingOwner);
                    }
                }

                var extractedDataDict = new Dictionary<Type, List<BaseRisEntity>>()
                {
                    {typeof(RisContract), new List<BaseRisEntity>()},
                    {typeof(RisContractAttachment), new List<BaseRisEntity>()},
                    {typeof(ContractObject), new List<BaseRisEntity>()},
                    {typeof(HouseManService), new List<BaseRisEntity>()},
                    {typeof(AddService), new List<BaseRisEntity>()},
                    {typeof(RisProtocolOk), new List<BaseRisEntity>()},
                    {typeof(ProtocolMeetingOwner), new List<BaseRisEntity>()}
                };

                extractedDataDict[typeof(RisContract)].AddRange(contractsToSave);
                extractedDataDict[typeof(RisContractAttachment)].AddRange(contractAttachmentList);
                extractedDataDict[typeof(ContractObject)].AddRange(risContractsObjectsToSave);
                extractedDataDict[typeof(HouseManService)].AddRange(communalServicesToSave);
                extractedDataDict[typeof(AddService)].AddRange(addServicesToSave);
                extractedDataDict[typeof(RisProtocolOk)].AddRange(protocolOkList);
                extractedDataDict[typeof(ProtocolMeetingOwner)].AddRange(protocolMeetOwnerList);

                return extractedDataDict;
            }
            finally
            {
                this.Container.Release(contragentDomain);
                this.Container.Release(contractDomain);
                this.Container.Release(fileUploadService);
                this.Container.Release(contractAttachmentDomain);
                this.Container.Release(protocolOkDomain);
                this.Container.Release(protocolMeetingOwnersDomain);
            }
        }

        /// <summary>
        /// Получает разницу между двумя датами в полных годах.
        /// </summary>
        /// <param name="start">Дата начала</param>
        /// <param name="end">Дата окончания</param>
        /// <returns>Количество лет</returns>
        private int? GetYearsDifference(DateTime? start, DateTime? end)
        {
            int? result = null;

            if (start.HasValue && end.HasValue)
            {
                result = Math.Truncate((((DateTime)end - (DateTime)start).Days / 365.25m)).ToInt();
            }

            return result;
        }

        /// <summary>
        /// Получает разницу между двумя датами в месяцах (без учёта годов).
        /// </summary>
        /// <param name="start">Дата начала</param>
        /// <param name="end">Дата окончания</param>
        /// <returns>Количество месяцев</returns>
        private int? GetMonthsDifference(DateTime? start, DateTime? end)
        {
            int? result = null;

            if (start.HasValue && end.HasValue)
            {
                var startDate = (DateTime)start;
                var endDate = (DateTime)end;
                var differenece = endDate.Month - startDate.Month;

                result = differenece > 0 ? differenece : 0;
            }

            return result;
        }

        /// <summary>
        /// Переодит значения перечисления "Тип управления управляющей организацией" в значения "Тип владельца договора".
        /// </summary>
        /// <param name="type">Тип управления управляющей организацией</param>
        /// <returns>Тип владельца договора</returns>
        private RisContractOwnersType? GetOwnersType(TypeManagementManOrg type)
        {
            RisContractOwnersType? result = null;

            switch (type)
            {
                case TypeManagementManOrg.JSK:
                    result = RisContractOwnersType.BuildingOwner;
                    break;

                case TypeManagementManOrg.Other:
                    result = RisContractOwnersType.MunicipalHousing;
                    break;

                case TypeManagementManOrg.UK:
                    result = RisContractOwnersType.Owners;
                    break;
                case TypeManagementManOrg.TSJ:
                    result = RisContractOwnersType.Cooperative;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Получить все услуги ГИС для дома
        /// </summary>
        /// <param name="realityObjectId">Идентификатор дома</param>
        /// <returns>Все услуги ГИС для дома</returns>
        private IEnumerable<ServiceDictionary> GetAllServices(long realityObjectId)
        {
            var roDomain = this.Container.ResolveDomain<RealityObject>();
            var fiasAddressUidDomain = this.Container.ResolveDomain<FiasAddressUid>();
            var bilHouseCodeStorageDomain = this.Container.ResolveDomain<BilHouseCodeStorage>();
            var gisHouseService = this.Container.Resolve<IGisHouseService>("GisHouseService");
            var kpHouseService = this.Container.Resolve<IGisHouseService>("KpHouseService");
            var bilServiceDictionaryDomain = this.Container.ResolveDomain<BilServiceDictionary>();

            try
            {
                var realityObject = roDomain.Get(realityObjectId);

                var billingIds = fiasAddressUidDomain.GetAll()
                    .Where(x => x.Address == realityObject.FiasAddress)
                    .Select(x => x.BillingId)
                    .ToList();

                var schemaIds = bilHouseCodeStorageDomain.GetAll()
                    .Where(x => billingIds.Contains(x.BillingHouseCode.ToString()) && x.Schema != null)
                    .Select(x => x.Schema.Id)
                    .ToList();

                var houseService = gisHouseService ?? kpHouseService;
                var houseServiceIds = new List<long>();

                if (houseService != null)
                {
                    // услуги есть начиная с 2013
                    for (int year = 2013; year <= DateTime.Now.Year; year++)
                    {
                        for (int month = 1; month <= 12; month++)
                        {
                            var houseServices = houseService.GetHouseServices(realityObjectId, new DateTime(year, month, 1));

                            if (houseServices != null)
                            {
                                houseServiceIds.AddRange(houseServices.Select(x => x.Id));
                            }
                        }
                    }
                }

                return bilServiceDictionaryDomain.GetAll()
                    .Where(x =>
                        x.Schema != null &&
                        schemaIds.Contains(x.Schema.Id) &&
                        houseServiceIds.Contains(x.ServiceCode))
                    .Select(x => x.Service)
                    .ToList();

            }
            finally
            {
                this.Container.Release(roDomain);
                this.Container.Release(fiasAddressUidDomain);
                this.Container.Release(bilHouseCodeStorageDomain);
                this.Container.Release(gisHouseService);
                this.Container.Release(kpHouseService);
                this.Container.Release(bilServiceDictionaryDomain);
            }
        }
    }
}