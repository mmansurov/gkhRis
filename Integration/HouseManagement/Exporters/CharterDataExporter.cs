namespace Bars.Gkh.Ris.Integration.HouseManagement.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.Utils;
    using Entities.HouseManagement;
    using Enums;
    using HouseManagementAsync;
    using DataExtractors;
    using Tasks.HouseManagement;

    public class CharterDataExporter : BaseDataExporter<importCharterRequest, HouseManagementPortsTypeAsyncClient>
    {
        private Dictionary<long, List<ContractObject>> contractObjectsByCharterId = new Dictionary<long, List<ContractObject>>();
        private Dictionary<long, List<ProtocolMeetingOwner>> protocolMeetingOwnersByCharterId = new Dictionary<long, List<ProtocolMeetingOwner>>();

        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        private const int Portion = 100;

        private List<Charter> chartersToExport;

        /// <summary>
        /// Наименование экспортера
        /// </summary>
        public override string Name => "Экспорт уставов";

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get { return 11; }
        }

        /// <summary>
        /// Описание экспортера
        /// </summary>
        public override string Description => "Экспорт уставов";

        /// <summary>
        /// Извлечение данных из таблиц ЖКХ
        /// </summary>
        /// <param name="parameters"></param>
        protected override void ExtractData(DynamicDictionary parameters)
        {
            var extractor = this.Container.Resolve<IGisIntegrationDataExtractor>(typeof(CharterDataExtractor));

            try
            {
                extractor.Contragent = this.Contragent;

                var extractedDataDict = extractor.Extract(parameters);

                this.chartersToExport = extractedDataDict.ContainsKey(typeof(Charter))
                    ? extractedDataDict[typeof(Charter)].Cast<Charter>().ToList()
                    : new List<Charter>();

                this.protocolMeetingOwnersByCharterId = extractedDataDict.ContainsKey(typeof(ProtocolMeetingOwner))
                    ? extractedDataDict[typeof(ProtocolMeetingOwner)]
                        .Cast<ProtocolMeetingOwner>()
                        .GroupBy(x => x.Charter.Id)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    : new Dictionary<long, List<ProtocolMeetingOwner>>();

                this.protocolMeetingOwnersByCharterId = extractedDataDict.ContainsKey(typeof(ProtocolMeetingOwner))
                    ? extractedDataDict[typeof(ProtocolMeetingOwner)]
                        .Cast<ProtocolMeetingOwner>()
                        .GroupBy(x => x.Charter.Id)
                        .ToDictionary(x => x.Key, x => x.ToList())
                    : new Dictionary<long, List<ProtocolMeetingOwner>>();
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

            var itemsToRemove = new List<Charter>();

            foreach (var item in this.chartersToExport)
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
                this.chartersToExport.Remove(itemToRemove);
            }

            return result;
        }

        /// <summary>
        /// Сформировать объекты запросов к асинхронному сервису ГИС
        /// </summary>
        /// <returns>Словарь Объект запроса - Словарь Транспортных идентификаторов: Тип обектов - Словарь: Транспортный идентификатор - Идентификатор объекта</returns>
        protected override Dictionary<importCharterRequest, Dictionary<Type, Dictionary<string, long>>> GetRequestData()
        {
            var result = new Dictionary<importCharterRequest, Dictionary<Type, Dictionary<string, long>>>();

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
        protected override string ExecuteRequest(importCharterRequest request)
        {
            AckRequest result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importCharterData(this.GetNewRequestHeader(), request, out result);
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
        private ValidateObjectResult CheckListItem(Charter item)
        {
            StringBuilder messages = new StringBuilder();

            if (item.DocNum.IsEmpty())
            {
                messages.Append("DOCNUM ");
            }

            if (!item.DocDate.HasValue)
            {
                messages.Append("DOCDATE ");
            }

            if (!this.contractObjectsByCharterId.ContainsKey(item.Id))
            {
                messages.Append("RIS_CONTRACTOBJECT ");
            }

            if (item.Attachment == null)
            {
                messages.Append("ATTACHMENT_ID ");
            }

            var headInd = item.Head;

            if (headInd == null)
            {
                messages.Append("HEAD ");
            }
            else
            {
                if (headInd.Surname.IsEmpty())
                {
                    messages.Append("HEAD_SURNAME ");
                }

                if (headInd.FirstName.IsEmpty())
                {
                    messages.Append("HEAD_FIRSTNAME ");
                }

                if (!headInd.DateOfBirth.HasValue)
                {
                    messages.Append("HEAD_DATEOFBIRTH ");
                }

                if (headInd.Snils.IsEmpty())
                {
                    messages.Append("HEAD_SNILS ");
                }
            }

            var protMeetOwners = this.protocolMeetingOwnersByCharterId.ContainsKey(item.Id)
                ? this.protocolMeetingOwnersByCharterId[item.Id]
                : null;

            var protMeetOwnRequestList = new List<AttachmentType>();

            if (protMeetOwners != null)
            {
                foreach (var protMeetOwner in protMeetOwners)
                {
                    if (protMeetOwner.Attachment.Name.IsEmpty())
                    {
                        messages.Append("PROTMEETINGOWNER_NAME ");
                    }

                    if (protMeetOwner.Attachment.Description.IsEmpty())
                    {
                        messages.Append("PROTMEETINGOWNER_DESCRIPTION ");
                    }

                    if (protMeetOwner.Attachment.Guid.IsEmpty())
                    {
                        messages.Append("PROTMEETINGOWNER_GUID ");
                    }

                    if (protMeetOwner.Attachment.Hash.IsEmpty())
                    {
                        messages.Append("PROTMEETINGOWNER_HASH ");
                    }
                    //var protMeetOwnRequest = new AttachmentType
                    //{
                    //    Name = protMeetOwner.Attachment.Name,
                    //    Description = protMeetOwner.Attachment.Description,
                    //    Attachment = new Attachment { AttachmentGUID = protMeetOwner.Attachment.Guid },
                    //    AttachmentHASH = protMeetOwner.Attachment.Hash
                    //};

                    //protMeetOwnRequestList.Add(protMeetOwnRequest);
                }
            }

            if (!protMeetOwnRequestList.Any())
            {
                messages.Append("RIS_PROTOCOLMEETINGOWNER ");
            }

            var contrObjectList = this.contractObjectsByCharterId.ContainsKey(item.Id)
                    ? this.contractObjectsByCharterId[item.Id]
                    : null;

            var contractObjectRequestList = new List<CharterTypeContractObjectList>();

            if (contrObjectList != null)
            {
                foreach (var contrObject in contrObjectList)
                {
                    if (contrObject.House == null)
                    {
                        messages.Append("CONTROBJECT_HOUSE ");
                    }

                    if (!contrObject.StartDate.HasValue)
                    {
                        messages.Append("CONTROBJECT_STARTDATE ");
                    }

                    if (!contrObject.BaseMServiseCurrentCharter)
                    {
                        messages.Append("CONTROBJECT_BASEMSERVICECURRCHARTER ");
                    }

                    var contractObjectRequest = new CharterTypeContractObjectList
                    {
                        ContractObject = new CharterTypeContractObjectListContractObject
                        {
                            FIASHouseGuid = contrObject.House != null ? contrObject.House.FiasHouseGuid : "",
                            StartDate = contrObject.StartDate ?? DateTime.MinValue,
                            BaseMService = new BaseServiceCharterType
                            {
                                Item = true
                            }
                        }
                    };

                    contractObjectRequestList.Add(contractObjectRequest);
                }
            }

            if (!contractObjectRequestList.Any())
            {
                messages.Append("RIS_CONTRACTOBJECT ");
            }

            return new ValidateObjectResult
            {
                Id = item.Id,
                State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                Message = messages.ToString(),
                Description = "Устав"
            };
        }
        
        /// <summary>
        /// Получает список порций объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список порций объектов ГИС</returns>
        private List<IEnumerable<Charter>> GetPortions()
        {
            var result = new List<IEnumerable<Charter>>();

            if (this.chartersToExport.Count > 0)
            {
                var startIndex = 0;
                do
                {
                    result.Add(this.chartersToExport.Skip(startIndex).Take(CharterDataExporter.Portion));
                    startIndex += CharterDataExporter.Portion;
                }
                while (startIndex < this.chartersToExport.Count);
            }

            return result;
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта</param>
        /// <param name="transportGuidDictionary">Список объектов для импорта</param>
        /// <returns>Объект запроса</returns>
        private importCharterRequest GetRequestObject(IEnumerable<Charter> listForImport, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var charterList = new List<importCharterRequestCharter>();

            var charterTransportGuidDictionary = new Dictionary<string, long>();

            foreach (var charter in listForImport)
            {
                var listItem = this.GetImportCharterRequestContract(charter);
                charterList.Add(listItem);

                //charterTransportGuidDictionary.Add(listItem.TransportGUID, contract.Id);
            }

            transportGuidDictionary.Add(typeof(Charter), charterTransportGuidDictionary);

            return new importCharterRequest { Item = charterList.ToArray() };
        }

        /// <summary>
        /// Создать объект importCharterRequestCharter по Charter
        /// </summary>
        /// <param name="contract">Объект типа Charter</param>
        /// <returns>Объект типа importCharterRequestCharter</returns>
        private importCharterRequestCharter GetImportCharterRequestContract(Charter charter)
        {
            object item = null;
            ItemChoiceType2 itemName = ItemChoiceType2.Owners;

            var protocolMeetingOwners = this.GetProtocolMeetingOwners(charter.Id);

            var attachmentArray = new[]
            {
                new AttachmentType
                {
                    Name = charter.Attachment.Name,
                    Description = charter.Attachment.Description,
                    Attachment = new Attachment { AttachmentGUID = charter.Attachment.Guid },
                    AttachmentHASH = charter.Attachment.Hash
                }
            };

            var headInd = charter.Head;
            IndType indType = null;

            if (headInd != null)
            {
                indType = new IndType
                {
                    Surname = headInd.Surname,
                    FirstName = headInd.FirstName,
                    Sex = headInd.Sex == RisGender.F ? IndTypeSex.F : IndTypeSex.M,
                    DateOfBirth = headInd.DateOfBirth ?? DateTime.MinValue,
                    Item = headInd.Snils
                };
            }

            var importCharterRequest = new importCharterRequestCharter
            {
                DocNum = charter.DocNum,
                Date = charter.DocDate ?? DateTime.MinValue,
                MeetingProtocol = new CharterTypeMeetingProtocol
                {
                    Items = protocolMeetingOwners.ToArray()
                },
                ChiefExecutive = new CharterTypeChiefExecutive
                {
                    Head = new CharterTypeChiefExecutiveHead
                    {
                        Item = indType,
                        ItemElementName = ItemChoiceType3.IndOwner
                    },
                    Managers = charter.Managers
                },
                ContractObjectList = this.GetContractObjects(charter.Id).ToArray(),
                AttachmentCharter = attachmentArray
            };

            //if (charter.Operation == RisEntityOperation.Update)
            //{
            //    importContractRequest.ContractVersionGUID = charter.Guid;
            //}

            return importCharterRequest;
        }

        /// <summary>
        /// Получить массив объектов ContractTypeContractObject по заданному contractId
        /// </summary>
        /// <param name="contractId">Идентификатор контракта</param>
        /// <returns>Массив объектов ContractTypeContractObject</returns>
        private CharterTypeContractObjectList[] GetContractObjects(long contractId)
        {
            List<CharterTypeContractObjectList> result = new List<CharterTypeContractObjectList>();

            if (this.contractObjectsByCharterId.ContainsKey(contractId))
            {
                foreach (var contractObject in this.contractObjectsByCharterId[contractId])
                {
                    result.Add(new CharterTypeContractObjectList
                    {
                        ContractObject = new CharterTypeContractObjectListContractObject
                        {
                            FIASHouseGuid = contractObject.FiasHouseGuid,
                            StartDate = contractObject.StartDate ?? DateTime.MinValue,
                            EndDate = contractObject.EndDate ?? DateTime.MinValue,
                            //BaseMService = new BaseServiceType
                            //{
                            //    Item = true
                            //},
                            //AddService = this.GetAddServices(contractObject.Id),
                            //HouseService = this.GetHouseServices(contractObject.Id)
                        }
                    });
                }
            }

            return result.ToArray();
        }

        ///// <summary>
        ///// Сформировать массив прочих услуг по дому
        ///// </summary>
        ///// <param name="contractObjectId">Иднетификатор объекта контракта</param>
        ///// <returns>Массив услуг</returns>
        //private ContractTypeContractObjectAddService[] GetAddServices(long contractObjectId)
        //{
        //    var result = new List<ContractTypeContractObjectAddService>();

        //    var listServices = this.addServByContractObjId.Get(contractObjectId) ?? new List<AddService>();

        //    foreach (var service in listServices)
        //    {
        //        result.Add(new ContractTypeContractObjectAddService
        //        {
        //            ServiceType = new nsiRef
        //            {
        //                Code = service.ServiceTypeCode,
        //                GUID = service.ServiceTypeGuid
        //            },
        //            StartDate = service.StartDate ?? DateTime.MinValue,
        //            EndDate = service.EndDate ?? DateTime.MinValue,
        //            BaseService = new BaseServiceType
        //            {
        //                Item = service.BaseServiceCurrentDoc
        //            }
        //        });
        //    }

        //    return result.ToArray();
        //}

        ///// <summary>
        ///// Сформировать массив коммунальных услуг по дому
        ///// </summary>
        ///// <param name="contractObjectId">Иднетификатор объекта контракта</param>
        ///// <returns>Массив услуг</returns>
        //private ContractTypeContractObjectHouseService[] GetHouseServices(long contractObjectId)
        //{
        //    var result = new List<ContractTypeContractObjectHouseService>();

        //    var listHouseManServices = this.houseServByContractObjId.Get(contractObjectId) ?? new List<HouseManService>();

        //    foreach (var service in listHouseManServices)
        //    {
        //        result.Add(new ContractTypeContractObjectHouseService
        //        {
        //            ServiceType = new nsiRef
        //            {
        //                Code = service.ServiceTypeCode,
        //                GUID = service.ServiceTypeGuid
        //            },
        //            StartDate = service.StartDate ?? DateTime.MinValue,
        //            EndDate = service.EndDate ?? DateTime.MinValue,
        //            BaseService = new BaseServiceType
        //            {
        //                Item = service.BaseServiceCurrentDoc
        //            }
        //        });
        //    }

        //    return result.ToArray();
        //}

        /// <summary>
        /// Сформировать массив протоколов контракта
        /// </summary>
        /// <param name="contractId">Иднетификатор объекта контракта</param>
        /// <returns>Массив протоколов контракта</returns>
        private object[] GetProtocolMeetingOwners(long contractId)
        {
            var result = new List<object>();

            if (this.protocolMeetingOwnersByCharterId.ContainsKey(contractId))
            {
                var listProtocolMeetingOwner = this.protocolMeetingOwnersByCharterId[contractId] ?? new List<ProtocolMeetingOwner>();

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