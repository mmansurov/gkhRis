namespace Bars.Gkh.Ris.Integration.Nsi.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using B4.Utils;

    using Bars.Gkh.Ris.NsiAsync;
    
    using Domain;
    using Entities.Nsi;

    /// <summary>
    /// Класс - экспортер записей справочника «Коммунальные услуги»
    /// </summary>
    public class ImportMunicipalServiceMethod: GisIntegrationNsiMethod<RisMunicipalService, importMunicipalServicesRequest>
    {
        private readonly List<RisMunicipalService> municipalServicesToSave;

        private readonly Dictionary<string, RisMunicipalService> municipalServicesByTransportGuid;

        /// <summary>
        /// Конструктор класса экспортера
        /// </summary>
        public ImportMunicipalServiceMethod()
        {
            this.municipalServicesToSave = new List<RisMunicipalService>();
            this.municipalServicesByTransportGuid = new Dictionary<string, RisMunicipalService>();
        }

        /// <summary>
        /// Код метода
        /// </summary>
        public override string Code
        {
            get
            {
                return "importMunicipalService";
            }
        }

        /// <summary>
        /// Наименование метода
        /// </summary>
        public override string Name
        {
            get
            {
                return "Импорт записей справочника «Коммунальные услуги»";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get { return 6; }
        }

        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        protected override int Portion
        {
            get
            {
                return 1000;
            }
        }

        /// <summary>
        /// Количество объектов для сохранения
        /// </summary>
        protected override int ProcessedObjects
        {
            get
            {
                return this.municipalServicesToSave.Count;
            }
        }

        /// <summary>
        /// Общее количество импортируемых объектов
        /// </summary>
        new protected int CountObjects
        {
            get
            {
                return this.municipalServicesByTransportGuid.Count;
            }
        }

        /// <summary>
        /// Список записей, по которым производится импорт.
        /// </summary>
        protected override IList<RisMunicipalService> MainList { get; set; }

        /// <summary>
        /// Подготовка кэша данных 
        /// </summary>
        protected override void Prepare()
        {
            this.MainList = this.GetMainList();
        }

        /// <summary>
        /// Проверка записей справочника
        /// </summary>
        /// <param name="item">Запись справочника</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(RisMunicipalService item)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(item.MunicipalServiceRefCode) || string.IsNullOrEmpty(item.MunicipalServiceRefGuid))
            {
                messages.Append("MunicipalServiceRef ");
            }

            if (string.IsNullOrEmpty(item.MainMunicipalServiceName))
            {
                messages.Append("MainMunicipalServiceName ");
            }

            if (string.IsNullOrEmpty(item.MunicipalResourceRefCode) || string.IsNullOrEmpty(item.MunicipalResourceRefGuid))
            {
                messages.Append("MunicipalResourceRef ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта</param>
        /// <returns>Объект запроса</returns>
        protected override importMunicipalServicesRequest GetRequestObject(
            IEnumerable<RisMunicipalService> listForImport)
        {
            var importRequestList = new List<importMunicipalServicesRequestImportMainMunicipalService>();

            foreach (var item in listForImport)
            {
                importRequestList.Add(this.PrepareMunicipalServicesRequest(item));
            }

            return new importMunicipalServicesRequest { ImportMainMunicipalService = importRequestList.ToArray() };
        }


        /// <summary>
        /// Получить ответ от сервиса.
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns>Результат выполнения запроса</returns>
        protected override AckRequest GetRequestAckRequest(importMunicipalServicesRequest request)
        {
            AckRequest result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importMunicipalServices(this.RequestHeader, request, out result);
            }

            return result;
        }

        /// <summary>
        /// Сохранение объектов ГИС после импорта.
        /// </summary>
        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.municipalServicesToSave, 1000, true, true);
        }

        /// <summary>
        /// Распарсить полученный результат и записать в сущности 
        /// или лог в случае ошибки
        /// </summary>
        /// <param name="stateResult">Результат выполнения getState</param>
        protected override void ParseStateResult(getStateResult stateResult)
        {
            foreach (var item in stateResult.Items)
            {
                var errorMessageTypeItem = item as ErrorMessageType;
                var responseItem = item as CommonResultType;
                
                if (errorMessageTypeItem != null)
                {
                    this.AddLineToLog(string.Empty, 0, string.Empty, errorMessageTypeItem.Description);
                }
                else if (responseItem != null)
                {
                    this.CheckResponseItem(responseItem);
                }
            }
        }

        /// <summary>
        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент response</param>
        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (!this.municipalServicesByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                return;
            }

            var municipalService = this.municipalServicesByTransportGuid[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("Запись справочника", municipalService.Id, "Не загружена", errorNotation);

                return;
            }

            municipalService.Guid = responseItem.GUID;
            this.municipalServicesToSave.Add(municipalService);

            this.AddLineToLog("Запись справочника", municipalService.Id, "Загружена", responseItem.GUID);
        }

        private importMunicipalServicesRequestImportMainMunicipalService PrepareMunicipalServicesRequest(
            RisMunicipalService municipalService)
        {
            var transportGuid = Guid.NewGuid().ToString();

            object item = null;

            if (municipalService.SortOrderNotDefined)
            {
                item = true;
            }
            else
            {
                item = municipalService.SortOrder;
            }

            this.municipalServicesByTransportGuid.Add(transportGuid, municipalService);

            return new importMunicipalServicesRequestImportMainMunicipalService
                   {
                       TransportGUID = transportGuid,
                       MunicipalServiceRef = new nsiRef
                                             {
                                                 Code = municipalService.MunicipalServiceRefCode,
                                                 GUID = municipalService.MunicipalServiceRefGuid
                                             },
                       GeneralNeeds = municipalService.GeneralNeeds,
                       MainMunicipalServiceName = municipalService.MainMunicipalServiceName,
                       MunicipalResourceRef = new nsiRef
                                              {
                                                  Code = municipalService.MunicipalResourceRefCode,
                                                  GUID = municipalService.MunicipalResourceRefGuid
                                              },
                       Item = item
                };
        }

        /// <summary>
        /// Метод получения списка записей для импорта
        /// </summary>
        /// <returns>Список объектов для импорта</returns>
        private List<RisMunicipalService> GetMainList()
        {
            var municipalServicesDomain = this.Container.ResolveDomain<RisMunicipalService>();

            try
            {
                return municipalServicesDomain.GetAll()
                    .WhereIf(this.Contragent != null, x => x.Contragent == this.Contragent)
                    .ToList();
            }
            finally
            {
                this.Container.Release(municipalServicesDomain);
            }
        }
    }
}
