namespace Bars.Gkh.Ris.Integration.Nsi.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Bars.B4.Utils;

    using Bars.Gkh.Ris.Entities.Nsi;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.NsiAsync;
    using Bars.Gkh.Ris.Tasks.Nsi;

    public class AdditionalServicesExporter : BaseDataExporter<importAdditionalServicesRequest, NsiPortsTypeAsyncClient>
    {
        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        private const int Portion = 1000;

        private List<RisAdditionalService> additionalServicesToExport;

        /// <summary>
        /// Наименование экспортера
        /// </summary>
        public override string Name => "Экспорт записей справочника «Дополнительные услуги»";

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order => 5;

        /// <summary>
        /// Описание экспортера
        /// </summary>
        public override string Description => "Экспорт записей справочника «Дополнительные услуги»";

        protected override void ExtractData(DynamicDictionary parameters)
        {
            var extractor = this.Container.Resolve<IGisIntegrationDataExtractor>("AdditionalServicesDataExtractor");

            try
            {
                extractor.Contragent = this.Contragent;

                var extractedDataDict = extractor.Extract(parameters);

                this.additionalServicesToExport = extractedDataDict.ContainsKey(typeof(RisAdditionalService))
                    ? extractedDataDict[typeof(RisAdditionalService)].Cast<RisAdditionalService>().ToList()
                    : new List<RisAdditionalService>();
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

            var itemsToRemove = new List<RisAdditionalService>();

            foreach (var item in this.additionalServicesToExport)
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
                this.additionalServicesToExport.Remove(itemToRemove);
            }

            return result;
        }

        /// <summary>
        /// Сформировать объекты запросов к асинхронному сервису ГИС
        /// </summary>
        /// <returns>Словарь Объект запроса - Словарь Транспортных идентификаторов: Тип обектов - Словарь: Транспортный идентификатор - Идентификатор объекта</returns>
        protected override Dictionary<importAdditionalServicesRequest, Dictionary<Type, Dictionary<string, long>>> GetRequestData()
        {
            var result = new Dictionary<importAdditionalServicesRequest, Dictionary<Type, Dictionary<string, long>>>();

            foreach (var iterationList in this.GetPortions())
            {
                var transportGuidDictionary = new Dictionary<Type, Dictionary<string, long>>();
                var request = this.GetRequestObject(iterationList, transportGuidDictionary);
                request.Id = Guid.NewGuid().ToString();

                result.Add(request, transportGuidDictionary);
            }

            return result;
        }

        /// <summary>
        /// Выполнить запрос к асинхронному сервису ГИС
        /// </summary>
        /// <param name="request">Объект запроса</param>
        /// <returns>Идентификатор сообщения для получения результата</returns>
        protected override string ExecuteRequest(importAdditionalServicesRequest request)
        {
            AckRequest result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importAdditionalServices(this.GetNewRequestHeader(), request, out result);
            }

            return result.Ack.MessageGUID;
        }

        /// <summary>
        /// Получить тип задачи получения результатов экспорта
        /// </summary>
        /// <returns>Тип задачи</returns>
        protected override Type GetTaskType()
        {
            return typeof(ExportAdditionalServicesTask);
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
        private ValidateObjectResult CheckListItem(RisAdditionalService item)
        {
            var messages = new StringBuilder();

            if (item.AdditionalServiceTypeName.IsEmpty())
            {
                messages.Append("AdditionalServiceTypeName ");
            }

            if (item.Okei.IsEmpty() && item.StringDimensionUnit.IsEmpty())
            {
                messages.Append("Okei or StringDimensionUnit ");
            }

            return new ValidateObjectResult
            {
                Id = item.Id,
                State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                Message = messages.ToString(),
                Description = "Дополнительная услуга"
            };
        }

        /// <summary>
        /// Получает список порций объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список порций объектов ГИС</returns>
        private List<IEnumerable<RisAdditionalService>> GetPortions()
        {
            var result = new List<IEnumerable<RisAdditionalService>>();

            if (this.additionalServicesToExport.Count > 0)
            {
                var startIndex = 0;
                do
                {
                    result.Add(this.additionalServicesToExport.Skip(startIndex).Take(Portion));
                    startIndex += Portion;
                }
                while (startIndex < this.additionalServicesToExport.Count);
            }

            return result;
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта</param>
        /// <param name="transportGuidDictionary">Список объектов для импорта</param>
        /// <returns>Объект запроса</returns>
        private importAdditionalServicesRequest GetRequestObject(IEnumerable<RisAdditionalService> listForImport, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var addServList = new List<importAdditionalServicesRequestImportAdditionalServiceType>();

            var addServTransportGuidDictionary = new Dictionary<string, long>();

            foreach (var addServ in listForImport)
            {
                var listItem = this.GetImportAdditionalServiceImportType(addServ);
                addServList.Add(listItem);

                addServTransportGuidDictionary.Add(listItem.TransportGUID, addServ.Id);
            }

            transportGuidDictionary.Add(typeof(RisAdditionalService), addServTransportGuidDictionary);

            return new importAdditionalServicesRequest { ImportAdditionalServiceType = addServList.ToArray() };
        }

        /// <summary>
        /// Создать объект importAdditionalServicesRequestImportAdditionalServiceType по RisAdditionalService
        /// </summary>
        /// <param name="addServ">Объект типа RisAdditionalService</param>
        /// <returns>Объект типа importAdditionalServicesRequestImportAdditionalServiceType</returns>
        private importAdditionalServicesRequestImportAdditionalServiceType GetImportAdditionalServiceImportType(RisAdditionalService addServ)
        {
            var transportGuid = Guid.NewGuid().ToString();

            string dimension;
            ItemChoiceType dimensionType;

            if (string.IsNullOrEmpty(addServ.Okei))
            {
                dimension = addServ.StringDimensionUnit;
                dimensionType = ItemChoiceType.StringDimensionUnit;
            }
            else
            {
                dimension = addServ.Okei;
                dimensionType = ItemChoiceType.OKEI;
            }

            return new importAdditionalServicesRequestImportAdditionalServiceType
            {
                TransportGUID = transportGuid,
                ElementGuid = addServ.Operation == RisEntityOperation.Update ? addServ.Guid : null,
                AdditionalServiceTypeName = addServ.AdditionalServiceTypeName,
                Item = dimension,
                ItemElementName = dimensionType
            };
        }
    }
}