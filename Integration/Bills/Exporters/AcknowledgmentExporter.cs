namespace Bars.Gkh.Ris.Integration.Bills.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using B4.Utils;
    using BillsAsync;
    using Entities.Payment;
    using Enums;
    using Tasks.Bills;

    /// <summary>
    /// Экспортер сведений о квитировании
    /// </summary>
    public class AcknowledgmentExporter: BaseDataExporter<importAcknowledgmentRequest, BillsPortsTypeAsyncClient>
    {
        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        private const int Portion = 1000;

        private List<RisAcknowledgment> acknowledgmentsToExport;

        /// <summary>
        /// Наименование метода
        /// </summary>
        public override string Name
        {
            get
            {
                return "Экспорт сведений о квитировании";
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
        /// Описание экспортера
        /// </summary>
        public override string Description
        {
            get
            {
                return "Операция позволяет экспортировать в ГИС ЖКХ информацию о квитировании. Сведения о квитировании включают в себя: идентификатор платежного документа (счета на оплату), идентификатор извещения, сведения об услугах и сумме квитирования.";
            }
        }

        /// <summary>
        /// Собрать данные
        /// </summary>
        /// <param name="parameters">Параметры экспорта</param>
        protected override void ExtractData(DynamicDictionary parameters)
        {
            var acknowledgmentDomain = this.Container.ResolveDomain<RisAcknowledgment>();

            //TODO выбрать по контрагенту

            try
            {
                this.acknowledgmentsToExport = acknowledgmentDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(acknowledgmentDomain);
            }
        }

        /// <summary>
        /// Валидация данных
        /// </summary>
        /// <returns>Результат валидации</returns>
        protected override List<ValidateObjectResult> ValidateData()
        {
            var result = new List<ValidateObjectResult>();

            var itemsToRemove = new List<RisAcknowledgment>();

            foreach (var item in this.acknowledgmentsToExport)
            {
                var validateResult = this.CheckAcknowledgmentsListItem(item);

                if (validateResult.State != ObjectValidateState.Success)
                {
                    result.Add(validateResult);
                    itemsToRemove.Add(item);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                this.acknowledgmentsToExport.Remove(itemToRemove);
            }

            return result;
        }

        /// <summary>
        /// Сформировать объекты запросов к асинхронному сервису ГИС
        /// </summary>
        /// <returns>Словарь Объект запроса - Словарь Транспортных идентификаторов: Тип обектов - Словарь: Транспортный идентификатор - Идентификатор объекта</returns>
        protected override Dictionary<importAcknowledgmentRequest, Dictionary<Type, Dictionary<string, long>>> GetRequestData()
        {
            var result = new Dictionary<importAcknowledgmentRequest, Dictionary<Type, Dictionary<string, long>>>();
                 
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
        protected override string ExecuteRequest(importAcknowledgmentRequest request)
        {
            AckRequest result;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient == null)
            {
                throw new Exception("Не удалось получить SOAP клиент");
            }

            soapClient.importAcknowledgment(this.GetNewRequestHeader(), request, out result);

            if (result == null || result.Ack == null)
            {
                throw new Exception("Пустой результат выполенния запроса");
            }

            return result.Ack.MessageGUID;

        }

        /// <summary>
        /// Получить тип задачи для получения результатов экспорта
        /// </summary>
        /// <returns>Тип задачи</returns>
        protected override Type GetTaskType()
        {
            return typeof(ExportAcknowledgmentTask);
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
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта</param>
        /// <param name="transportGuidDictionary">Словарь транспортных идентификаторов</param>
        /// <returns>Объект запроса</returns>
        private importAcknowledgmentRequest GetRequestObject(
            IEnumerable<RisAcknowledgment> listForImport,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var importRequestList = new List<importAcknowledgmentRequestAcknowledgmentRequestInfo>();

            var acknowledgmentTransportGuidDictionary = new Dictionary<string, long>();

            foreach (var item in listForImport)
            {
                importRequestList.Add(this.PrepareAcknowledgmentRequestInfo(item, acknowledgmentTransportGuidDictionary));
            }

            transportGuidDictionary.Add(typeof(RisAcknowledgment), acknowledgmentTransportGuidDictionary);

            return new importAcknowledgmentRequest { AcknowledgmentRequestInfo = importRequestList.ToArray() };
        }

        private importAcknowledgmentRequestAcknowledgmentRequestInfo PrepareAcknowledgmentRequestInfo(
            RisAcknowledgment acknowledgment,
            Dictionary<string, long> transportGuidDictionary)
        {
            var transportGuid = Guid.NewGuid().ToString();

            transportGuidDictionary.Add(transportGuid, acknowledgment.Id);

            string item;
            ItemChoiceType2 itemType;

            if (!string.IsNullOrEmpty(acknowledgment.HSType))
            {
                item = acknowledgment.HSType;
                itemType = ItemChoiceType2.HSType;
            }
            else if (!string.IsNullOrEmpty(acknowledgment.MSType))
            {
                item = acknowledgment.MSType;
                itemType = ItemChoiceType2.MSType;
            }
            else
            {
                item = acknowledgment.ASType;
                itemType = ItemChoiceType2.ASType;
            }

            return new importAcknowledgmentRequestAcknowledgmentRequestInfo
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
        /// Проверка данных о квитировании перед импортом
        /// </summary>
        /// <param name="item">Данные о квитировании</param>
        /// <returns>Результат проверки</returns>
        private ValidateObjectResult CheckAcknowledgmentsListItem(RisAcknowledgment item)
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

            return new ValidateObjectResult
                               {
                                   Id = item.Id,
                                   State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                                   Message = messages.ToString(),
                                   Description = "Сведения о квитировании"
                                };
        }

        /// <summary>
        /// Получает список порций объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список порций объектов ГИС</returns>
        private List<IEnumerable<RisAcknowledgment>> GetPortions()
        {
            List<IEnumerable<RisAcknowledgment>> result = new List<IEnumerable<RisAcknowledgment>>();

            if (this.acknowledgmentsToExport.Count > 0)
            {
                var startIndex = 0;
                do
                {
                    result.Add(this.acknowledgmentsToExport.Skip(startIndex).Take(AcknowledgmentExporter.Portion));
                    startIndex += AcknowledgmentExporter.Portion;
                }
                while (startIndex < this.acknowledgmentsToExport.Count);
            }

            return result;
        }
    }
}
