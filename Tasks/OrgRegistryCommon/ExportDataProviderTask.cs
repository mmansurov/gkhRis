namespace Bars.Gkh.Ris.Tasks.OrgRegistryCommon
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4;
    using Bars.B4.DataAccess;
    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Entities;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.OrgRegistryCommonAsync;

    /// <summary>
    /// Задача получения результатов для экспортера данных о квитировании
    /// </summary>
    public class ExportDataProviderTask : BaseExportTask<getStateResult, RegOrgPortsTypeAsyncClient>
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

            var requestHeader = new HeaderType
            {
                Date = DateTime.Now,
                MessageGUID = Guid.NewGuid().ToStr()
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
            var dataProviderDomain = this.Container.ResolveDomain<RisContragent>();

            if (!transportGuidDictByType.ContainsKey(typeof(RisContragent)))
            {
                throw new Exception("Не удалось обработать результат выполнения метода getState");
            }
            var dataProvidersByTransportGuid = transportGuidDictByType[typeof(RisContragent)];

            try
            {
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
                        var processingResult = this.CheckResponseItem(responseItem, dataProviderDomain, dataProvidersByTransportGuid);
                        result.Objects.Add(processingResult);
                    }
                }

                return result;
            }
            finally
            {
                this.Container.Release(dataProviderDomain);
            }
        }

        /// <summary>
        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент response</param>
        /// <param name="dataProviderDomain">DomainService</param>
        /// <param name="transportGuidDict">Словарь transportGuid-ов</param>
        private ObjectProcessingResult CheckResponseItem(
            CommonResultType responseItem, 
            IDomainService<RisContragent> dataProviderDomain,
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

            var dataProviderId = transportGuidDict[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = error != null ? error.Description : "Вернулся пустой GUID";

                return new ObjectProcessingResult
                {
                    Description = "Поставщик информации",
                    RisId = dataProviderId,
                    State = ObjectProcessingState.Error,
                    Message = errorNotation
                };
            }

            var dataProvider = dataProviderDomain.Get(dataProviderId);

            dataProvider.SenderId = responseItem.GUID;

            return new ObjectProcessingResult
            {
                Description = "Поставщик информации",
                RisId = dataProviderId,
                State = ObjectProcessingState.Success,
                ObjectsToSave = new List<PersistentObject>
                                {
                                    dataProvider
                                }
            };
        }
    }
}
