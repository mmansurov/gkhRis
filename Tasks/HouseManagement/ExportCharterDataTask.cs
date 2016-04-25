namespace Bars.Gkh.Ris.Tasks.HouseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using B4.DataAccess;
    using B4.Utils;

    using Entities.HouseManagement;
    using Enums;
    using HouseManagementAsync;

    /// <summary>
    /// Задача получения результатов для экспортера договоров управления
    /// </summary>
    public class ExportCharterDataTask : BaseExportTask<getStateResult, HouseManagementPortsTypeAsyncClient>
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
            result = null;

            if (soapClient != null)
            {
                soapClient.getState(requestHeader, request, out result);
            }

            return result?.RequestState ?? 0;
        }

        /// <summary>
        /// Обработать результат экспорта пакета данных
        /// </summary>
        /// <param name="responce">Ответ от сервиса</param>
        /// <param name="transportGuidDictByType">Словарь transportGuid-ов для типа</param>
        /// <returns>Результат обработки пакета</returns>
        protected override PackageProcessingResult ProcessResult(
            getStateResult responce,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictByType)
        {
            if (!transportGuidDictByType.ContainsKey(typeof(Charter)))
            {
                throw new Exception("Не удалось обработать результат выполнения метода getState");
            }
            var chartersByTransportGuid = transportGuidDictByType[typeof(Charter)];

            var result = new PackageProcessingResult { State = PackageProcessingState.Success, Objects = new List<ObjectProcessingResult>() };

            foreach (var item in responce.Items)
            {
                var errorItem = item as CommonResultTypeError;
                var errorMessageTypeItem = item as ErrorMessageType;
                var responseItem = item as CommonResultType;

                if (errorItem != null)
                {
                    result.Objects.Add(new ObjectProcessingResult
                    {
                        State = ObjectProcessingState.Error,
                        Message = errorItem.Description
                    });
                }
                else if (errorMessageTypeItem != null)
                {
                    result.Objects.Add(new ObjectProcessingResult
                    {
                        State = ObjectProcessingState.Error,
                        Message = errorMessageTypeItem.Description
                    });
                }
                else if (responseItem != null)
                {
                    var processingResult = this.CheckResponseItem(responseItem, chartersByTransportGuid);
                    result.Objects.Add(processingResult);
                }
            }

            return result;
        }

        /// <summary>
        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент response</param>
        /// <param name="transportGuidDict">Словарь transportGuid-ов</param>
        private ObjectProcessingResult CheckResponseItem(CommonResultType responseItem, Dictionary<string, long> transportGuidDict)
        {
            var domain = this.Container.ResolveDomain<Charter>();

            try
            {

                if (!transportGuidDict.ContainsKey(responseItem.TransportGUID))
                {
                    if (responseItem.GUID.IsEmpty())
                    {
                        var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                        var errorNotation = error != null ? error.Description : "Вернулся пустой GUID";

                        return new ObjectProcessingResult
                        {
                            Description = $"Не найден объект с TransportGuid = { responseItem.TransportGUID }",
                            State = ObjectProcessingState.Error,
                            Message = errorNotation
                        };
                    }

                    return new ObjectProcessingResult
                    {
                        GisId = responseItem.GUID,
                        Description = $"Не найден объект с TransportGuid = { responseItem.TransportGUID }",
                        State = ObjectProcessingState.Success
                    };
                }

                var charterId = transportGuidDict[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;
                    var errorNotation = error != null ? error.Description : "Вернулся пустой GUID";

                    return new ObjectProcessingResult
                    {
                        Description = "Устав",
                        RisId = charterId,
                        State = ObjectProcessingState.Error,
                        Message = errorNotation
                    };
                }

                var charter = domain.Get(charterId);
                charter.Guid = responseItem.GUID;

                return new ObjectProcessingResult
                {
                    Description = "Устав",
                    RisId = charterId,
                    State = ObjectProcessingState.Success,
                    ObjectsToSave = new List<PersistentObject> { charter }
                };
            }
            finally
            {
                this.Container.Release(domain);
            }
        }
    }
}