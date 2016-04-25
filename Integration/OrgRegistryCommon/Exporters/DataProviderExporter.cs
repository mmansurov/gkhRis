namespace Bars.Gkh.Ris.Integration.OrgRegistryCommon.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using B4.Utils;

    using OrgRegistryCommonAsync;

    using Gkh.Entities;
    using Entities;
    using Enums;
    using Tasks.OrgRegistryCommon;

    /// <summary>
    /// Экспорт сведений о поставщиках информации
    /// </summary>
    public class DataProviderExporter : BaseDataExporter<importDataProviderRequest, RegOrgPortsTypeAsyncClient>
    {
        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        private const int Portion = 100;

        private List<RisContragent> contragentsToExport;

        /// <summary>
        /// Наименование
        /// </summary>
        public override string Name => "Экспорт сведений о поставщиках информации";

        /// <summary>
        /// Порядок импорта в списке.
        /// </summary>
        public override int Order => 2;

        /// <summary>
        /// Описание экспортера
        /// </summary>
        public override string Description => "Экспорт сведений о поставщиках информации, получение SenderId";

        /// <summary>
        /// Необходимо подписывать данные
        /// </summary>
        public override bool NeedSign => true;

        /// <summary>
        /// Интервал запуска триггера получения результатов экспорта - в секундах
        /// </summary>
        public override int Interval => 60 * 7;

        /// <summary>
        /// Максимальное количество повторов триггера получения результатов экспорта
        /// </summary>
        public override int MaxRepeatCount => 30;

        /// <summary>
        /// Собрать данные
        /// </summary>
        /// <param name="parameters">Параметры экспорта</param>
        protected override void ExtractData(DynamicDictionary parameters)
        {
            var risContragentDomain = this.Container.ResolveDomain<RisContragent>();
            var contractContactDomain = this.Container.ResolveDomain<ContragentContact>();

            try
            {
                this.contragentsToExport = risContragentDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(risContragentDomain);
                this.Container.Release(contractContactDomain);
            }
        }

        /// <summary>
        /// Валидация данных
        /// </summary>
        /// <returns>Результат валидации</returns>
        protected override List<ValidateObjectResult> ValidateData()
        {
            var result = new List<ValidateObjectResult>();

            var itemsToRemove = new List<RisContragent>();

            foreach (var item in this.contragentsToExport)
            {
                var validateResult = this.CheckDataProviderListItem(item);

                if (validateResult.State != ObjectValidateState.Success)
                {
                    result.Add(validateResult);
                    itemsToRemove.Add(item);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                this.contragentsToExport.Remove(itemToRemove);
            }

            return result;
        }

        /// <summary>
        /// Сформировать объекты запросов к асинхронному сервису ГИС
        /// </summary>
        /// <returns>Словарь Объект запроса - Словарь Транспортных идентификаторов: Тип обектов - Словарь: Транспортный идентификатор - Идентификатор объекта</returns>
        protected override Dictionary<importDataProviderRequest, Dictionary<Type, Dictionary<string, long>>> GetRequestData()
        {
            var result = new Dictionary<importDataProviderRequest, Dictionary<Type, Dictionary<string, long>>>();

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
        protected override string ExecuteRequest(importDataProviderRequest request)
        {
            AckRequest result;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient == null)
            {
                throw new Exception("Не удалось получить SOAP клиент");
            }

            soapClient.importDataProvider(this.GetNewRequestHeader(), request, out result);

            return result != null ? result.Ack.MessageGUID : string.Empty;

        }

        /// <summary>
        /// Получить тип задачи для получения результатов экспорта
        /// </summary>
        /// <returns>Тип задачи</returns>
        protected override Type GetTaskType()
        {
            return typeof(ExportDataProviderTask);
        }

        /// <summary>
        /// Получить новый заголовок запроса
        /// </summary>
        /// <returns>Заголовок запроса</returns>
        private HeaderType GetNewRequestHeader()
        {
            return new HeaderType
            {
                Date = DateTime.Now,
                MessageGUID = Guid.NewGuid().ToStr()
            };
        }

        /// <summary>
        /// Проверка данных о квитировании перед импортом
        /// </summary>
        /// <param name="item">Данные о квитировании</param>
        /// <returns>Результат проверки</returns>
        private ValidateObjectResult CheckDataProviderListItem(RisContragent item)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(item.OrgRootEntityGuid))
            {
                messages.Append("OrgRootEntityGuid ");
            }

            return new ValidateObjectResult
            {
                Id = item.Id,
                State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                Message = messages.ToString(),
                Description = item.FullName
            };
        }

        /// <summary>
        /// Получает список порций объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список порций объектов ГИС</returns>
        private List<IEnumerable<RisContragent>> GetPortions()
        {
            List<IEnumerable<RisContragent>> result = new List<IEnumerable<RisContragent>>();

            if (this.contragentsToExport.Count > 0)
            {
                var startIndex = 0;
                do
                {
                    result.Add(this.contragentsToExport.Skip(startIndex).Take(DataProviderExporter.Portion));
                    startIndex += DataProviderExporter.Portion;
                }
                while (startIndex < this.contragentsToExport.Count);
            }

            return result;
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта</param>
        /// <param name="transportGuidDictionary">Словарь транспортных идентификаторов</param>
        /// <returns>Объект запроса</returns>
        private importDataProviderRequest GetRequestObject(
            IEnumerable<RisContragent> listForImport,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var importRequestList = new List<importDataProviderRequestDataProvider>();

            var dataProviderTransportGuidDictionary = new Dictionary<string, long>();

            foreach (var item in listForImport)
            {
                importRequestList.Add(this.PrepareDataProviderRequestInfo(item, dataProviderTransportGuidDictionary));
            }

            transportGuidDictionary.Add(typeof(RisContragent), dataProviderTransportGuidDictionary);

            return new importDataProviderRequest { DataProvider = importRequestList.ToArray() };
        }

        private importDataProviderRequestDataProvider PrepareDataProviderRequestInfo(
            RisContragent dataProvider,
            Dictionary<string, long> transportGuidDictionary)
        {
            var transportGuid = Guid.NewGuid().ToString();

            transportGuidDictionary.Add(transportGuid, dataProvider.Id);

            var item = new importDataProviderRequestDataProviderAllocateSenderID
            {
                RegOrg = new RegOrgType
                {
                    orgRootEntityGUID = dataProvider.OrgRootEntityGuid
                }
            };

            return new importDataProviderRequestDataProvider
            {
                Item = item,
                TransportGUID = transportGuid
            };
        }
    }
}