namespace Bars.Gkh.Ris.Integration.HouseManagement.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using B4.Utils;

    using Entities.HouseManagement;
    using Enums;
    using Enums.HouseManagement;
    using HouseManagementAsync;
    using HouseManagement.DataExtractors;
    using Tasks.HouseManagement;

    public class ContractDataExporter : BaseDataExporter<importContractRequest, HouseManagementPortsTypeAsyncClient>
    {
        private Dictionary<long, List<ContractObject>> contractObjectsByContractId = new Dictionary<long, List<ContractObject>>();
        private Dictionary<long, List<RisContractAttachment>> attachmentsByContractId = new Dictionary<long, List<RisContractAttachment>>();
        private Dictionary<long, List<RisProtocolOk>> protocolOksByContractId = new Dictionary<long, List<RisProtocolOk>>();
        private Dictionary<long, List<ProtocolMeetingOwner>> protocolMeetingOwnersByContractId = new Dictionary<long, List<ProtocolMeetingOwner>>();
        private Dictionary<long, List<HouseManService>> houseServByContractObjId = new Dictionary<long, List<HouseManService>>();
        private Dictionary<long, List<AddService>> addServByContractObjId = new Dictionary<long, List<AddService>>();

        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        private const int Portion = 100;

        private List<RisContract> contractsToExport;

        /// <summary>
        /// Наименование экспортера
        /// </summary>
        public override string Name => "Экспорт договоров управления";

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get { return 10; }
        }

        /// <summary>
        /// Описание экспортера
        /// </summary>
        public override string Description => "Экспорт договоров управления";

        protected override void ExtractData(DynamicDictionary parameters)
        {
            var extractor = this.Container.Resolve<IGisIntegrationDataExtractor>("ContractDataExtractor");

            try
            {
                extractor.Contragent = this.Contragent;

                var extractedDataDict = extractor.Extract(parameters);

                this.contractsToExport = extractedDataDict.ContainsKey(typeof(RisContract))
                    ? extractedDataDict[typeof(RisContract)].Cast<RisContract>().ToList()
                    : new List<RisContract>();

                this.contractObjectsByContractId = extractedDataDict.ContainsKey(typeof(ContractObject))
                    ? extractedDataDict[typeof(ContractObject)]
                        .Cast<ContractObject>()
                        .GroupBy(x => x.Contract.Id)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    : new Dictionary<long, List<ContractObject>>();

                this.attachmentsByContractId = extractedDataDict.ContainsKey(typeof(RisContractAttachment))
                    ? extractedDataDict[typeof(RisContractAttachment)]
                        .Cast<RisContractAttachment>()
                        .GroupBy(x => x.Contract.Id)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    : new Dictionary<long, List<RisContractAttachment>>();

                this.protocolOksByContractId = extractedDataDict.ContainsKey(typeof(RisProtocolOk))
                    ? extractedDataDict[typeof(RisProtocolOk)]
                        .Cast<RisProtocolOk>()
                        .GroupBy(x => x.Contract.Id)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    : new Dictionary<long, List<RisProtocolOk>>();

                this.protocolMeetingOwnersByContractId = extractedDataDict.ContainsKey(typeof(ProtocolMeetingOwner))
                    ? extractedDataDict[typeof(ProtocolMeetingOwner)]
                        .Cast<ProtocolMeetingOwner>()
                        .GroupBy(x => x.Contract.Id)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    : new Dictionary<long, List<ProtocolMeetingOwner>>();

                this.houseServByContractObjId = extractedDataDict.ContainsKey(typeof(HouseManService))
                    ? extractedDataDict[typeof(HouseManService)]
                        .Cast<HouseManService>()
                        .GroupBy(x => x.ContractObject.Id)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    : new Dictionary<long, List<HouseManService>>();

                this.addServByContractObjId = extractedDataDict.ContainsKey(typeof(AddService))
                    ? extractedDataDict[typeof(AddService)]
                        .Cast<AddService>()
                        .GroupBy(x => x.ContractObject.Id)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    : new Dictionary<long, List<AddService>>();
            }
            finally
            {
                this.Container.Release(extractor);
            }

        }

        /// <summary>
        /// Валидация данных
        /// </summary>
        /// <returns>Результат валидации</returns>
        protected override List<ValidateObjectResult> ValidateData()
        {
            var result = new List<ValidateObjectResult>();

            var itemsToRemove = new List<RisContract>();

            foreach (var item in this.contractsToExport)
            {
                var validateResult = this.CheckListItem(item);

                if (validateResult.State != ObjectValidateState.Success)
                {
                    result.Add(validateResult);
                    itemsToRemove.Add(item);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                this.contractsToExport.Remove(itemToRemove);
            }

            return result;
        }

        /// <summary>
        /// Сформировать объекты запросов к асинхронному сервису ГИС
        /// </summary>
        /// <returns>Словарь Объект запроса - Словарь Транспортных идентификаторов: Тип обектов - Словарь: Транспортный идентификатор - Идентификатор объекта</returns>
        protected override Dictionary<importContractRequest, Dictionary<Type, Dictionary<string, long>>> GetRequestData()
        {
            var result = new Dictionary<importContractRequest, Dictionary<Type, Dictionary<string, long>>>();

            foreach (var iterationList in this.GetPortions())
            {
                var transportGuidDictionary = new Dictionary<Type, Dictionary<string, long>>();
                var request = this.GetRequestObject(iterationList, transportGuidDictionary);

                result.Add(request, transportGuidDictionary);
            }

            return result;
        }

        /// <summary>
        /// Выполнить запрос к асинхронному сервису ГИС
        /// </summary>
        /// <param name="request">Объект запроса</param>
        /// <returns>Идентификатор сообщения для получения результата</returns>
        protected override string ExecuteRequest(importContractRequest request)
        {
            AckRequest result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importContractData(this.GetNewRequestHeader(), request, out result);
            }

            return result.Ack.MessageGUID;
        }

        /// <summary>
        /// Получить тип задачи получения результатов экспорта
        /// </summary>
        /// <returns>Тип задачи</returns>
        protected override Type GetTaskType()
        {
            return typeof(ExportContractDataTask);
        }

        /// <summary>
        /// Получить новый заголовок запроса
        /// </summary>
        /// <returns>Заголовок запроса</returns>
        private RequestHeader GetNewRequestHeader()
        {
            return new RequestHeader
            {
                Date = DateTime.Now,
                MessageGUID = Guid.NewGuid().ToStr(),
                SenderID = this.SenderId
            };
        }

        /// <summary>
        /// Проверить валидность объекта RisContract
        /// </summary>
        /// <param name="item">Объект RisContract</param>
        /// <returns>Результат валидации</returns>
        private ValidateObjectResult CheckListItem(RisContract item)
        {
            StringBuilder messages = new StringBuilder();

            if (item.DocNum.IsEmpty())
            {
                messages.Append("PlacingContract/DocNum ");
            }

            if (!item.SigningDate.HasValue)
            {
                messages.Append("PlacingContract/SigningDate ");
            }

            if (!item.EffectiveDate.HasValue)
            {
                messages.Append("PlacingContract/EffectiveDate ");
            }

            if (!item.PlanDateComptetion.HasValue)
            {
                messages.Append("PlacingContract/PlanDateComptetion ");
            }

            if (item.ContractBaseCode.IsEmpty())
            {
                messages.Append("PlacingContract/ContractBase/Code ");
            }

            if (item.ContractBaseGuid.IsEmpty())
            {
                messages.Append("PlacingContract/ContractBase/GUID ");
            }

            if (!this.contractObjectsByContractId.ContainsKey(item.Id) ||
                (item.OwnersType == RisContractOwnersType.Owners && this.contractObjectsByContractId[item.Id].Count != 1))
            {
                messages.Append("PlacingContract/ContractObject ");
            }

            foreach (var contractObject in this.contractObjectsByContractId[item.Id])
            {
                if (contractObject.FiasHouseGuid.IsEmpty())
                {
                    messages.Append("PlacingContract/ContractObject/FiasHouseGUID");
                }
            }

            if (!this.attachmentsByContractId.ContainsKey(item.Id))
            {
                messages.Append("PlacingContract/ContractAttachment ");
            }

            return new ValidateObjectResult
            {
                Id = item.Id,
                State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                Message = messages.ToString(),
                Description = "Договор управления"
            };
        }

        /// <summary>
        /// Получает список порций объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список порций объектов ГИС</returns>
        private List<IEnumerable<RisContract>> GetPortions()
        {
            var result = new List<IEnumerable<RisContract>>();

            if (this.contractsToExport.Count > 0)
            {
                var startIndex = 0;
                do
                {
                    result.Add(this.contractsToExport.Skip(startIndex).Take(ContractDataExporter.Portion));
                    startIndex += ContractDataExporter.Portion;
                }
                while (startIndex < this.contractsToExport.Count);
            }

            return result;
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта</param>
        /// <param name="transportGuidDictionary">Список объектов для импорта</param>
        /// <returns>Объект запроса</returns>
        private importContractRequest GetRequestObject(IEnumerable<RisContract> listForImport, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var contractList = new List<importContractRequestContract>();

            var contractTransportGuidDictionary = new Dictionary<string, long>();

            foreach (var contract in listForImport)
            {
                var listItem = this.GetImportContractRequestContract(contract);
                contractList.Add(listItem);

                contractTransportGuidDictionary.Add(listItem.TransportGUID, contract.Id);
            }

            transportGuidDictionary.Add(typeof(RisContract), contractTransportGuidDictionary);

            return new importContractRequest { Contract = contractList.ToArray() };
        }

        /// <summary>
        /// Создать объект importContractRequestContract по RisContract
        /// </summary>
        /// <param name="contract">Объект типа RisContract</param>
        /// <returns>Объект типа importContractRequestContract</returns>
        private importContractRequestContract GetImportContractRequestContract(RisContract contract)
        {
            object item = null;
            ItemChoiceType2 itemName = ItemChoiceType2.Owners;

            switch (contract.OwnersType)
            {
                case RisContractOwnersType.BuildingOwner:
                    item = new RegOrgType
                    {
                        orgRootEntityGUID = contract.Org.ReturnSafe(x => x.OrgRootEntityGuid)
                    };
                    itemName = ItemChoiceType2.BuildingOwner;
                    break;
                case RisContractOwnersType.Cooperative:
                    item = new RegOrgType
                    {
                        orgRootEntityGUID = contract.Org.ReturnSafe(x => x.OrgRootEntityGuid)
                    };
                    itemName = ItemChoiceType2.Cooperative;
                    break;
                case RisContractOwnersType.MunicipalHousing:
                    item = new RegOrgType
                    {
                        orgRootEntityGUID = contract.Org.ReturnSafe(x => x.OrgRootEntityGuid)
                    };
                    itemName = ItemChoiceType2.MunicipalHousing;
                    break;
                case RisContractOwnersType.Owners:
                    item = true;
                    break;
            }

            var protocolOKs = this.GetProtocolOKs(contract.Id);
            var protocolMeetingOwners = this.GetProtocolMeetingOwners(contract.Id);

            var items = new List<object>();

            if (protocolOKs.Length > 0)
            {
                items.Add(new ContractTypeProtocolProtocolAdd
                {
                    Items = this.GetProtocolOKs(contract.Id),
                    ItemsElementName = new[] { ItemsChoiceType5.ProtocolOK }
                });
            }

            if (protocolMeetingOwners.Length > 0)
            {
                items.Add(new ContractTypeProtocolProtocolAdd
                {
                    Items = this.GetProtocolMeetingOwners(contract.Id),
                    ItemsElementName = new[] { ItemsChoiceType5.ProtocolMeetingOwners }
                });
            }

            var importContractRequest = new importContractRequestContract
            {
                TransportGUID = Guid.NewGuid().ToString(),
                Item = new importContractRequestContractPlacingContract
                {
                    DocNum = contract.DocNum,
                    SigningDate = contract.SigningDate ?? DateTime.MinValue,
                    EffectiveDate = contract.EffectiveDate ?? DateTime.MinValue,
                    PlanDateComptetion = contract.PlanDateComptetion ?? DateTime.MinValue,
                    Validity = new ContractTypeValidity(),
                    Item = item,
                    ItemElementName = itemName,
                    Protocol = new ContractTypeProtocol
                    {
                        Items = items.ToArray()
                    },
                    ContractBase = new nsiRef
                    {
                        Code = contract.ContractBaseCode,
                        GUID = contract.ContractBaseGuid
                    },
                    ContractObject = this.GetContractObjects(contract.Id),
                    DateDetails = new DateDetailsType
                    {
                        PeriodMetering = new DateDetailsTypePeriodMetering
                        {
                            StartDate = (sbyte)(contract.InputMeteringDeviceValuesBeginDate ?? 0),
                            Item = (sbyte)(contract.InputMeteringDeviceValuesEndDate ?? 0)
                        },
                        PaymentDate = new DateDetailsTypePaymentDate
                        {
                            Item = (sbyte)(contract.DrawingPaymentDocumentDate ?? 0),
                            Item1ElementName =
                                contract.ThisMonthPaymentDocDate
                                    ? Item1ChoiceType.CurrentMounth
                                    : Item1ChoiceType.NextMounth,
                            Item1 = true
                        }
                    },
                    ContractAttachment = this.GetContractAttachments(contract.Id)
                }
            };

            if (contract.Operation == RisEntityOperation.Update)
            {
                importContractRequest.ContractVersionGUID = contract.Guid;
            }

            return importContractRequest;
        }

        /// <summary>
        /// Получить массив объектов ContractTypeContractObject по заданному contractId
        /// </summary>
        /// <param name="contractId">Идентификатор контракта</param>
        /// <returns>Массив объектов ContractTypeContractObject</returns>
        private ContractTypeContractObject[] GetContractObjects(long contractId)
        {
            List<ContractTypeContractObject> result = new List<ContractTypeContractObject>();

            if (this.contractObjectsByContractId.ContainsKey(contractId))
            {
                foreach (var contractObject in this.contractObjectsByContractId[contractId])
                {
                    result.Add(new ContractTypeContractObject
                    {
                        FIASHouseGuid = contractObject.FiasHouseGuid,
                        StartDate = contractObject.StartDate ?? DateTime.MinValue,
                        EndDate = contractObject.EndDate ?? DateTime.MinValue,
                        BaseMService = new BaseServiceType
                        {
                            Item = true
                        },
                        AddService = this.GetAddServices(contractObject.Id),
                        HouseService = this.GetHouseServices(contractObject.Id)
                    });
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Сформировать массив прочих услуг по дому
        /// </summary>
        /// <param name="contractObjectId">Иднетификатор объекта контракта</param>
        /// <returns>Массив услуг</returns>
        private ContractTypeContractObjectAddService[] GetAddServices(long contractObjectId)
        {
            var result = new List<ContractTypeContractObjectAddService>();

            var listServices = this.addServByContractObjId.Get(contractObjectId) ?? new List<AddService>();

            foreach (var service in listServices)
            {
                result.Add(new ContractTypeContractObjectAddService
                {
                    ServiceType = new nsiRef
                    {
                        Code = service.ServiceTypeCode,
                        GUID = service.ServiceTypeGuid
                    },
                    StartDate = service.StartDate ?? DateTime.MinValue,
                    EndDate = service.EndDate ?? DateTime.MinValue,
                    BaseService = new BaseServiceType
                    {
                        Item = service.BaseServiceCurrentDoc
                    }
                });
            }

            return result.ToArray();
        }

        /// <summary>
        /// Сформировать массив коммунальных услуг по дому
        /// </summary>
        /// <param name="contractObjectId">Иднетификатор объекта контракта</param>
        /// <returns>Массив услуг</returns>
        private ContractTypeContractObjectHouseService[] GetHouseServices(long contractObjectId)
        {
            var result = new List<ContractTypeContractObjectHouseService>();

            var listHouseManServices = this.houseServByContractObjId.Get(contractObjectId) ?? new List<HouseManService>();

            foreach (var service in listHouseManServices)
            {
                result.Add(new ContractTypeContractObjectHouseService
                {
                    ServiceType = new nsiRef
                    {
                        Code = service.ServiceTypeCode,
                        GUID = service.ServiceTypeGuid
                    },
                    StartDate = service.StartDate ?? DateTime.MinValue,
                    EndDate = service.EndDate ?? DateTime.MinValue,
                    BaseService = new BaseServiceType
                    {
                        Item = service.BaseServiceCurrentDoc
                    }
                });
            }

            return result.ToArray();
        }

        /// <summary>
        /// Сформировать массив приложений контракта
        /// </summary>
        /// <param name="contractId">Иднетификатор объекта контракта</param>
        /// <returns>Массив приложений контракта</returns>
        private AttachmentType[] GetContractAttachments(long contractId)
        {
            List<AttachmentType> result = new List<AttachmentType>();

            if (this.attachmentsByContractId.ContainsKey(contractId))
            {
                var listContractAttachment = this.attachmentsByContractId[contractId] ?? new List<RisContractAttachment>();

                foreach (var attachment in listContractAttachment)
                {
                    result.Add(new AttachmentType
                    {
                        Name = attachment.Attachment.Name,
                        Description = attachment.Attachment.Description.IsEmpty() ? attachment.Attachment.Name : attachment.Attachment.Description,
                        Attachment = new Attachment
                        {
                            AttachmentGUID = attachment.Attachment.Guid
                        },
                        AttachmentHASH = attachment.Attachment.Hash
                    });
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Сформировать массив протоколов контракта
        /// </summary>
        /// <param name="contractId">Иднетификатор объекта контракта</param>
        /// <returns>Массив протоколов контракта</returns>
        private object[] GetProtocolOKs(long contractId)
        {
            var result = new List<object>();

            if (this.protocolOksByContractId.ContainsKey(contractId))
            {
                var listProtocolOk = this.protocolOksByContractId[contractId] ?? new List<RisProtocolOk>();

                foreach (var attachment in listProtocolOk)
                {
                    result.Add(new AttachmentType
                    {
                        Name = attachment.Attachment.Name,
                        Description = attachment.Attachment.Description.IsEmpty() ? attachment.Attachment.Name : attachment.Attachment.Description,
                        Attachment = new Attachment
                        {
                            AttachmentGUID = attachment.Attachment.Guid
                        },
                        AttachmentHASH = attachment.Attachment.Hash
                    });
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Сформировать массив протоколов контракта
        /// </summary>
        /// <param name="contractId">Иднетификатор объекта контракта</param>
        /// <returns>Массив протоколов контракта</returns>
        private object[] GetProtocolMeetingOwners(long contractId)
        {
            var result = new List<object>();

            if (this.protocolMeetingOwnersByContractId.ContainsKey(contractId))
            {
                var listProtocolMeetingOwner = this.protocolMeetingOwnersByContractId[contractId] ?? new List<ProtocolMeetingOwner>();

                foreach (var attachment in listProtocolMeetingOwner)
                {
                    result.Add(new AttachmentType
                    {
                        Name = attachment.Attachment.Name,
                        Description = attachment.Attachment.Description.IsEmpty() ? attachment.Attachment.Name : attachment.Attachment.Description,
                        Attachment = new Attachment
                        {
                            AttachmentGUID = attachment.Attachment.Guid
                        },
                        AttachmentHASH = attachment.Attachment.Hash
                    });
                }
            }

            return result.ToArray();
        }
    }
}
