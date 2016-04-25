namespace Bars.Gkh.Ris.Integration.Payment.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4;
    using B4.DataAccess;
    using B4.Utils;
    using Tasks.Bills;
    using Domain;
    using Quartz.Scheduler;
    using Entities.Payment;
    using PaymentAsync;
    using global::Quartz;

    /// <summary>
    /// Класс экспортер сведений о квитировании
    /// </summary>
    public class ImportAcknowledgmentMethod :
        GisIntegrationPaymentAsyncMethod<RisAcknowledgment, importAcknowledgmentRequest>
    {
        private readonly List<RisAcknowledgment> acknowledgmentsToSave;

        private readonly Dictionary<string, RisAcknowledgment> acknowledgmentsByTransportGuid;

        /// <summary>
        /// Конструктор класса экспортера
        /// </summary>
        public ImportAcknowledgmentMethod()
        {
            this.acknowledgmentsToSave = new List<RisAcknowledgment>();
            this.acknowledgmentsByTransportGuid = new Dictionary<string, RisAcknowledgment>();
        }

        /// <summary>
        /// Код метода
        /// </summary>
        public override string Code
        {
            get
            {
                return "importAcknowledgment";
            }
        }

        /// <summary>
        /// Наименование метода
        /// </summary>
        public override string Name
        {
            get
            {
                return "Импорт сведений  о квитировании";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 58;
            }
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
                return this.acknowledgmentsToSave.Count;
            }
        }

        /// <summary>
        /// Общее количество импортируемых объектов
        /// </summary>
        new protected int CountObjects
        {
            get
            {
                return this.acknowledgmentsByTransportGuid.Count;
            }
        }

        /// <summary>
        /// Список записей, по которым производится импорт.
        /// </summary>
        protected override IList<RisAcknowledgment> MainList { get; set; }

        /// <summary>
        /// Подготовка кэша данных 
        /// </summary>
        protected override void Prepare()
        {
            this.MainList = this.GetMainList();
        }

        /// <summary>
        /// Проверка данных о квитировании перед импортом
        /// </summary>
        /// <param name="item">Данные о квитировании</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(RisAcknowledgment item)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(item.OrderId))
            {
                messages.Append("OrderId ");
            }

            if (string.IsNullOrEmpty(item.PaymentDocumentNumber))
            {
                messages.Append("PaymentDocumentNumber ");
            }

            var typeCount = 0;

            if (!string.IsNullOrEmpty(item.HSType))
            {
                typeCount++;
            }

            if (!string.IsNullOrEmpty(item.MSType))
            {
                typeCount++;
            }

            if (!string.IsNullOrEmpty(item.ASType))
            {
                typeCount++;
            }

            if (typeCount != 1)
            {
                messages.Append("HSType MSType ASType ");
            }

            if (decimal.Round(item.Amount, 2) < 1m)
            {
                messages.Append("Amount ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта</param>
        /// <returns>Объект запроса</returns>
        protected override importAcknowledgmentRequest GetRequestObject(
            IEnumerable<RisAcknowledgment> listForImport)
        {
            var importRequestList = new List<importAcknowledgmentRequestAcknowledgmentRequestInfo>();

            foreach (var item in listForImport)
            {
                importRequestList.Add(this.PrepareAcknowledgmentRequestInfo(item));
            }

            return new importAcknowledgmentRequest { AcknowledgmentRequestInfo = importRequestList.ToArray() };
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
                var errorItem = item as CommonResultTypeError;
                var errorMessageTypeItem = item as ErrorMessageType;
                var responseItem = item as CommonResultType;

                if (errorItem != null)
                {
                    this.AddLineToLog(string.Empty, 0, string.Empty, errorItem.Description);
                }
                else if (errorMessageTypeItem != null)
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
        protected void CheckResponseItem(CommonResultType responseItem)
        {
            if (!this.acknowledgmentsByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                return;
            }

            var acknowledgment = this.acknowledgmentsByTransportGuid[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("Сведения о квитировании", acknowledgment.Id, "Не загружены", errorNotation);

                return;
            }

            acknowledgment.Guid = responseItem.GUID;
            this.acknowledgmentsToSave.Add(acknowledgment);

            this.AddLineToLog("Сведения о квитировании", acknowledgment.Id, "Загружены", responseItem.GUID);
        }

        /// <summary>
        /// Отправить запрос на сервер
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns>Ответ</returns>
        protected override AckRequest GetRequestAckRequest(importAcknowledgmentRequest request)
        {
            AckRequest result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importAcknowledgment(this.RequestHeader, request, out result);
            }

            return result;
        }

        /// <summary>
        /// Сохранение объектов ГИС после импорта.
        /// </summary>
        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.acknowledgmentsToSave, 1000, true, true);
        }

        /// <summary>
        /// Получить входные данные для выполнения задачи получения результатов экспорта
        /// </summary>
        /// <returns>Входные данных</returns>
        protected override JobDataMap GetJobDataMap()
        {
            var result = new JobDataMap();

            result.Put(
                "AcknowledgmentsByTransportGuid",
                this.acknowledgmentsByTransportGuid.ToDictionary(k => k.Key, v => v.Value.Id));           

            result.Put("ServiceAddress", this.ServiceProvider.ServiceAddress);

            result.Put("Name", this.Name);

            result.Put("SenderId", this.SenderId);

            result.Put("ExecutionOwner", this.GetExecutionOwner());

            return result;
        }

        /// <summary>
        /// Получить тип задачи для получения результатов экспорта
        /// </summary>
        /// <returns>Тип задачи</returns>
        protected override Type GetTaskType()
        {
            return typeof(ExportAcknowledgmentTask);
        }

        private importAcknowledgmentRequestAcknowledgmentRequestInfo PrepareAcknowledgmentRequestInfo(
            RisAcknowledgment acknowledgment)
        {
            var transportGuid = Guid.NewGuid().ToString();

            this.acknowledgmentsByTransportGuid.Add(transportGuid, acknowledgment);

            string item;
            ItemChoiceType itemType;

            if (!string.IsNullOrEmpty(acknowledgment.HSType))
            {
                item = acknowledgment.HSType;
                itemType = ItemChoiceType.HSType;
            }
            else if (!string.IsNullOrEmpty(acknowledgment.MSType))
            {
                item = acknowledgment.MSType;
                itemType = ItemChoiceType.MSType;
            }
            else 
            {
                item = acknowledgment.ASType;
                itemType = ItemChoiceType.ASType;
            }

            return  new importAcknowledgmentRequestAcknowledgmentRequestInfo
                    {
                        OrderID = acknowledgment.OrderId,
                        PaymentDocumentNumber = acknowledgment.PaymentDocumentNumber,
                        Item = item,
                        ItemElementName = itemType,
                        Amount = decimal.Round(acknowledgment.Amount, 2),
                        TransportGUID = transportGuid
                    };
        }

        /// <summary>
        /// Метод получения списка записей для импорта
        /// </summary>
        /// <returns>Список объектов для импорта</returns>
        private List<RisAcknowledgment> GetMainList()
        {
            var acknowledgmentDomain = this.Container.ResolveDomain<RisAcknowledgment>();

            //TODO выбрать по контрагенту

            try
            {
                return acknowledgmentDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(acknowledgmentDomain);
            }
        }

        private IExecutionOwner GetExecutionOwner()
        {
            try
            {
                var userInfo = this.Container.Resolve<RequestingUserInformation>();
                return new UserExecutionOwner
                {
                    UserId = userInfo.UserIdentity.UserId,
                    TrackId = userInfo.UserIdentity.TrackId,
                    RequestIpAddress = userInfo.RequestIpAddress,
                    Name = userInfo.UserIdentity.Name
                };
            }
            catch
            {
                return new SystemExecutionOwner();
            }
        }
    }
}
