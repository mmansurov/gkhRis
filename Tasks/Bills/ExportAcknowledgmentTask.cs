namespace Bars.Gkh.Ris.Tasks.Bills
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4;
    using Bars.B4.DataAccess;
    using Bars.B4.Utils;

    using Bars.Gkh.Ris.BillsAsync;
    using Bars.Gkh.Ris.Entities.Payment;
    using Bars.Gkh.Ris.Enums;

    /// <summary>
    /// Задача получения результатов для экспортера данных о квитировании
    /// </summary>
    public class ExportAcknowledgmentTask: BaseExportTask<getStateResult, BillsPortsTypeAsyncClient>
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
            var acknowledgmentDomain = this.Container.ResolveDomain<RisAcknowledgment>();

            if (!transportGuidDictByType.ContainsKey(typeof(RisAcknowledgment)))
            {
                throw new Exception("Не удалось обработать результат выполнения метода getState");
            }
            var acknowledgmentsByTransportGuid = transportGuidDictByType[typeof(RisAcknowledgment)];

            try
            {
                return this.ParseStateResult(responce, acknowledgmentDomain, acknowledgmentsByTransportGuid);
            }
            finally
            {
                this.Container.Release(acknowledgmentDomain);
            }
        }

        private PackageProcessingResult ParseStateResult(
            getStateResult responce, 
            IDomainService<RisAcknowledgment> acknowledgmentDomain,
            Dictionary<string, long> transportGuidDict)
        {
            var result = new PackageProcessingResult { State = PackageProcessingState.Success, Objects = new List<ObjectProcessingResult>()};

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
                   var processingResult = this.CheckResponseItem(responseItem, acknowledgmentDomain, transportGuidDict);
                   result.Objects.Add(processingResult);
                }
            }

            return result;
        }

        /// <summary>
        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент response</param>
        /// <param name="acknowledgmentDomain">DomainService</param>
        /// <param name="transportGuidDict">Словарь transportGuid-ов</param>
        private ObjectProcessingResult CheckResponseItem(
            CommonResultType responseItem, 
            IDomainService<RisAcknowledgment> acknowledgmentDomain,
            Dictionary<string, long> transportGuidDict)
        {
            if (!transportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
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

            var acknowledgmentId = transportGuidDict[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = error != null ? error.Description : "Вернулся пустой GUID";

                return new ObjectProcessingResult
                {
                    Description = "Сведения о квитировании",
                    RisId = acknowledgmentId,
                    State = ObjectProcessingState.Error,
                    Message = errorNotation
                };
            }

            var acknowledgment = acknowledgmentDomain.Get(acknowledgmentId);

            acknowledgment.Guid = responseItem.GUID;

            return new ObjectProcessingResult
            {
                Description = "Сведения о квитировании",
                RisId = acknowledgmentId,
                State = ObjectProcessingState.Success,
                ObjectsToSave = new List<PersistentObject>
                                {
                                    acknowledgment
                                }
            };
        }
    }
}
