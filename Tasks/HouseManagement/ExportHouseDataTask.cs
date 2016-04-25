namespace Bars.Gkh.Ris.Tasks.HouseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4;
    using Bars.B4.DataAccess;
    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Entities;
    using Bars.Gkh.Ris.Entities.HouseManagement;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.HouseManagementAsync;

    /// <summary>
    /// Задача получения результатов для экспортера данных по домам
    /// </summary>
    public class ExportHouseDataTask : BaseExportTask<getStateResult, HouseManagementPortsTypeAsyncClient>
    {
        /// <summary>
        /// Получить результат экспорта пакета данных
        /// </summary>
        /// <param name="messageGuid">Идентификатор сообщения</param>
        /// <param name="result">Результат экспорта</param>
        /// <returns>Статус обработки запроса</returns>
        protected override sbyte GetSatateResult(string messageGuid, out getStateResult result)
        {
            var request = new getStateRequest { MessageGUID = messageGuid };

            var requestHeader = new RequestHeader
            {
                Date = DateTime.Now,
                MessageGUID = Guid.NewGuid().ToStr(),
                SenderID = this.SenderId
            };

            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient == null)
            {
                throw new Exception("Не удалось получить SOAP клиент");
            }

            soapClient.getState(requestHeader, request, out result);

            if (result == null)
            {
                throw new Exception("Пустой результат выполенния запроса");
            }

            return result.RequestState;
        }

        /// <summary>
        /// Обработать результат экспорта пакета данных
        /// </summary>
        /// <param name="responce">Ответ от сервиса</param>
        /// <param name="transportGuidDictByType">Словаь транспортных идентификаторов в разрезе типов</param>
        /// <returns>Результат обработки пакета</returns>
        protected override PackageProcessingResult ProcessResult(getStateResult responce,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictByType)
        {
            var houseDomain = this.Container.ResolveDomain<RisHouse>();
            var residentialPremisesDomain = this.Container.ResolveDomain<ResidentialPremises>();
            var nonResidentialPremisesDomain = this.Container.ResolveDomain<NonResidentialPremises>();
            var entranceDomain = this.Container.ResolveDomain<RisEntrance>();
            var livingRoomDomain = this.Container.ResolveDomain<LivingRoom>();

            try
            {
                return this.ParseStateResult(
                    responce,
                    transportGuidDictByType,
                    houseDomain,
                    residentialPremisesDomain,
                    nonResidentialPremisesDomain,
                    entranceDomain,
                    livingRoomDomain);
            }
            finally
            {
                this.Container.Release(houseDomain);
                this.Container.Release(residentialPremisesDomain);
                this.Container.Release(nonResidentialPremisesDomain);
                this.Container.Release(entranceDomain);
                this.Container.Release(livingRoomDomain);
            }
        }

        private PackageProcessingResult ParseStateResult(
            getStateResult responce,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictByType,
            IDomainService<RisHouse> houseDomain,
            IDomainService<ResidentialPremises> residentialPremisesDomain,
            IDomainService<NonResidentialPremises> nonResidentialPremisesDomain,
            IDomainService<RisEntrance> entranceDomain,
            IDomainService<LivingRoom> livingRoomDomain)
        {
            var result = new PackageProcessingResult { State = PackageProcessingState.Success, Objects = new List<ObjectProcessingResult>() };

            foreach (var item in responce.Items)
            {
                var errorMessageItem = item as ErrorMessageType;
                var importResult = item as ImportResult;

                if (errorMessageItem != null)
                {
                    result.Objects.Add(new ObjectProcessingResult
                    {
                        State = ObjectProcessingState.Error,
                        Message = errorMessageItem.Description
                    });
                }
                else if (importResult != null)
                {
                    foreach (var responseItem in importResult.Items)
                    {
                        var importResultErrorMessageItem = responseItem as ErrorMessageType;
                        var importResultCommonResultItem = responseItem as ImportResultCommonResult;

                        if (importResultErrorMessageItem != null)
                        {
                            result.Objects.Add(new ObjectProcessingResult
                            {
                                State = ObjectProcessingState.Error,
                                Message = importResultErrorMessageItem.Description
                            });
                        }
                        else if (importResultCommonResultItem != null)
                        {
                            var processingResult = this.CheckResponseItem(
                                importResultCommonResultItem,
                                transportGuidDictByType,
                                houseDomain,
                                residentialPremisesDomain,
                                nonResidentialPremisesDomain,
                                entranceDomain,
                                livingRoomDomain);

                            result.Objects.Add(processingResult);
                        }
                        else
                        {
                            result.Objects.Add(new ObjectProcessingResult
                            {
                                State = ObjectProcessingState.Error,
                                Message = "Не удалось разобрать getStateResult"
                            });
                        }
                    }                    
                }
                else
                {
                    result.Objects.Add(new ObjectProcessingResult
                    {
                        State = ObjectProcessingState.Error,
                        Message = "Не удалось разобрать getStateResult"
                    });
                }
            }

            return result;
        }

        private ObjectProcessingResult CheckResponseItem(
            ImportResultCommonResult responseItem,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictByType,
            IDomainService<RisHouse> houseDomain,
            IDomainService<ResidentialPremises> residentialPremisesDomain,
            IDomainService<NonResidentialPremises> nonResidentialPremisesDomain,
            IDomainService<RisEntrance> entranceDomain,
            IDomainService<LivingRoom> livingRoomDomain)
        {
            var transportGuid = responseItem.TransportGUID;

            if (transportGuidDictByType.ContainsKey(typeof(RisHouse)) &&
                transportGuidDictByType[typeof(RisHouse)].ContainsKey(transportGuid))
            {
                var houseId = transportGuidDictByType[typeof(RisHouse)][transportGuid];

                return this.CheckResponseItem(houseDomain, responseItem, houseId);
            }

            if (transportGuidDictByType.ContainsKey(typeof(ResidentialPremises)) &&
                transportGuidDictByType[typeof(ResidentialPremises)].ContainsKey(transportGuid))
            {
                var residentialPremisesId = transportGuidDictByType[typeof(ResidentialPremises)][transportGuid];

                return this.CheckResponseItem(residentialPremisesDomain, responseItem, residentialPremisesId);
            }

            if (transportGuidDictByType.ContainsKey(typeof(NonResidentialPremises)) &&
                transportGuidDictByType[typeof(NonResidentialPremises)].ContainsKey(transportGuid))
            {
                var nonResidentialPremisesId = transportGuidDictByType[typeof(NonResidentialPremises)][transportGuid];

                return this.CheckResponseItem(nonResidentialPremisesDomain, responseItem, nonResidentialPremisesId);
            }

            if (transportGuidDictByType.ContainsKey(typeof(RisEntrance)) &&
                transportGuidDictByType[typeof(RisEntrance)].ContainsKey(transportGuid))
            {
                var entranceId = transportGuidDictByType[typeof(RisEntrance)][transportGuid];

                return this.CheckResponseItem(entranceDomain, responseItem, entranceId);
            }

            if (transportGuidDictByType.ContainsKey(typeof(LivingRoom)) &&
                transportGuidDictByType[typeof(LivingRoom)].ContainsKey(transportGuid))
            {
                var livingRoomId = transportGuidDictByType[typeof(LivingRoom)][transportGuid];

                return this.CheckResponseItem(livingRoomDomain, responseItem, livingRoomId);
            }

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = error != null ? error.Description : "Вернулся пустой GUID";

                return new ObjectProcessingResult
                {
                    Description = string.Format("Не найден объект с TransportGuid = {0}", responseItem.TransportGUID),
                    State = ObjectProcessingState.Error,
                    Message = errorNotation
                };
            }

            return new ObjectProcessingResult
            {
                GisId = responseItem.GUID,
                Description =
                           string.Format(
                               "Не найден объект с TransportGuid = {0}",
                               responseItem.TransportGUID),
                State = ObjectProcessingState.Success
            };
        }

        private ObjectProcessingResult CheckResponseItem<T>(
            IDomainService<T> domain, 
            CommonResultType responseItem, 
            long entityId)
           where T : BaseRisEntity
        {
            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = error != null ? error.Description : "Вернулся пустой GUID";

                return new ObjectProcessingResult
                {
                    Description = string.Format("Объект типа {0}", typeof(T).Name),
                    RisId = entityId,
                    State = ObjectProcessingState.Error,
                    Message = errorNotation
                };
            }

            var entity = domain.Get(entityId);

            entity.Guid = responseItem.GUID;

            return new ObjectProcessingResult
            {
                Description = string.Format("Объект типа {0}", typeof(T).Name),
                RisId = entityId,
                State = ObjectProcessingState.Success,
                ObjectsToSave = new List<PersistentObject>
                                {
                                    entity
                                }
            };
        }
    }
}
