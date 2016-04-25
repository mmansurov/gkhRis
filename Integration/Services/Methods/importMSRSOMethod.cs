namespace Bars.Gkh.Ris.Integration.Services.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.Services;
    using Ris.Services;

    public class ImportMSRSOMethod : GisIntegrationServicesMethod<RisHouseService, importMSRSORequest>
    {
        private Dictionary<string, RisHouseService> servicesByTransportGuid = new Dictionary<string, RisHouseService>();
        private List<RisHouseService> servicesToSave = new List<RisHouseService>();

        /// <summary>
        /// Количество объектов для сохранения. Переопределять как количество объектов для сохранения
        /// </summary>
        protected override int ProcessedObjects
        {
            get { return this.servicesToSave.Count; }
        }

        /// <summary>
        /// Код импорта.
        /// </summary>
        public override string Code
        {
            get { return "importMSRSO"; }
        }

        /// <summary>
        /// Наименование импорта в списке.
        /// </summary>
        public override string Name
        {
            get { return "Импорт КУ по прямым договорам с РСО"; }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 35;
            }
        }

        /// <summary>
        /// Максимаьлное количество объектов ГИС в одном файле импорта.
        /// </summary>
        protected override int Portion
        {
            get { return 1000; }
        }

        /// <summary>
        /// Список объектов ГИС, по которым производится импорт.
        /// </summary>
        protected override IList<RisHouseService> MainList { get; set; }


        /// <summary>
        /// Подготовка кэша данных для PrepareRequest. Заполнить MainList.
        /// </summary>
        protected override void Prepare()
        {
            var serviceDomain = this.Container.ResolveDomain<RisHouseService>();

            try
            {
                this.MainList = serviceDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(serviceDomain);
            }
        }

        /// <summary>
        /// Проверка объекта
        /// </summary>
        /// <param name="item">Объект</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(RisHouseService item)
        {
            StringBuilder messages = new StringBuilder();

            if (item.MsTypeCode.IsNull())
            {
                messages.Append("MSRSO/MSTYPE/CODE ");
            }

            if (item.MsTypeGuid.IsNull())
            {
                messages.Append("MSRSO/MSTYPE/GUID ");
            }

            if (!item.StartDate.HasValue)
            {
                messages.Append("MSRSO/STARTDATEFROM ");
            }

            if (!item.EndDate.HasValue)
            {
                messages.Append("MSRSO/STARTDATETO ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        /// <summary>
        /// Получает список порций объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список порций объектов ГИС</returns>
        protected override List<IEnumerable<RisHouseService>> GetPortions()
        {
            List<IEnumerable<RisHouseService>> result = new List<IEnumerable<RisHouseService>>();
            Dictionary<string, List<RisHouseService>> meteringDeviceByFiasHouseGuidDict = 
                this.MainList
                .GroupBy(x => x.House.FiasHouseGuid)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var deviceList in meteringDeviceByFiasHouseGuidDict.Values)
            {
                var startIndex = 0;
                do
                {
                    result.Add(deviceList.Skip(startIndex).Take(this.Portion));
                    startIndex += this.Portion;
                }
                while (startIndex <= deviceList.Count);
            }

            return result;
        }

        /// <summary>
        /// Получить объект для запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов</param>
        /// <returns>Объект для запроса</returns>
        protected override importMSRSORequest GetRequestObject(IEnumerable<RisHouseService> listForImport)
        {
            var importRequestList = new List<importMSRSORequestMSRSO>();

            foreach (var item in listForImport)
            {
                var itemToAdd = new importMSRSORequestMSRSO
                {
                    TransportGUID = Guid.NewGuid().ToString(),
                    StartDateFrom = item.StartDate ?? DateTime.MinValue,
                    StartDateTo = item.EndDate ?? DateTime.MinValue,
                    MSType = new nsiRef
                    {
                        Code = item.MsTypeCode,
                        GUID = item.MsTypeGuid
                    }
                };

                importRequestList.Add(itemToAdd);
                this.servicesByTransportGuid.Add(itemToAdd.TransportGUID, item);
            }

            this.CountObjects += importRequestList.Count;

            return new importMSRSORequest { FIASHouseGuid = listForImport.First().House.FiasHouseGuid, MSRSO = importRequestList.ToArray() };
        }

        /// <summary>
        /// Проверить строку response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент списка из response</param>
        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (!this.servicesByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                return;
            }

            var service = this.servicesByTransportGuid[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("Коммунальная услуга", service.Id, "Не загружена", errorNotation);

                return;
            }

            service.Guid = responseItem.GUID;
            this.servicesToSave.Add(service);
            this.AddLineToLog("Коммунальная услуга", service.Id, "Загружена", responseItem.GUID);
        }

        /// <summary>
        /// Получить ответ от сервиса.
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns>Результат выполнения запроса</returns>
        protected override ImportResult GetRequestResult(importMSRSORequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importMSRSO(this.RequestHeader, request, out result);
            }

            return result;
        }

        /// <summary>
        /// Сохранение объектов ГИС после импорта.
        /// </summary>
        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.servicesToSave, 1000, true, true);
        }
    }
}
