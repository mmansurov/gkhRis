namespace Bars.Gkh.Ris.Integration.Payment.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.Payment;
    using PaymentAsync;

    /// <summary>
    /// Класс импорта уведомлений о выполнении распоряжения
    /// </summary>
    public class ImportNotificationsOfOrderExecutionMethod :
        GisIntegrationPaymentAsyncMethod<NotificationOfOrderExecution, importNotificationsOfOrderExecutionRequest>
    {
        private readonly List<NotificationOfOrderExecution> notificationsToSave = new List<NotificationOfOrderExecution>();
        private readonly Dictionary<string, NotificationOfOrderExecution> notificationsByTransportGuid = 
            new Dictionary<string, NotificationOfOrderExecution>();

        protected override int ProcessedObjects
        {
            get { return this.notificationsToSave.Count; }
        }

        public override string Code
        {
            get { return "importNotificationsOfOrderExecution"; }
        }

        public override string Name
        {
            get { return "Импорт уведомлений о выполнении распоряжения"; }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 16;
            }
        }

        protected override int Portion
        {
            get { return 1000; }
        }

        protected override IList<NotificationOfOrderExecution> MainList { get; set; }

        protected override void Prepare()
        {
            var notificationDomain = this.Container.ResolveDomain<NotificationOfOrderExecution>();

            try
            {
                this.MainList = notificationDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(notificationDomain);
            }
        }

        protected override CheckingResult CheckMainListItem(NotificationOfOrderExecution item)
        {
            var messages = new StringBuilder();

            if (item.SupplierId.IsEmpty())
            {
                messages.Append("SUPPLIER_ID/DOCNUM ");
            }

            if (item.SupplierName.IsEmpty())
            {
                messages.Append("SUPPLIER_NAME ");
            }

            if (item.RecipientInn.IsEmpty())
            {
                messages.Append("RECIPIENT_INN ");
            }

            if (item.BankName.IsEmpty())
            {
                messages.Append("BANK_NAME ");
            }

            if (item.RecipientBik.IsEmpty())
            {
                messages.Append("RECIPIENT_BANK_BIK ");
            }

            if (item.CorrespondentBankAccount.IsEmpty())
            {
                messages.Append("RECIPIENT_BANK_CORRACC ");
            }

            if (item.RecipientAccount.IsEmpty())
            {
                messages.Append("RECIPIENT_ACCOUNT ");
            }

            if (item.RecipientName.IsEmpty())
            {
                messages.Append("RECIPIENT_NAME ");
            }

            if (item.OrderId.IsEmpty())
            {
                messages.Append("ORDER_ID ");
            }

            if (item.RisPaymentDocument == null)
            {
                messages.Append("RIS_PAYM_DOC_ID ");
            }

            if (item.OrderNum.IsEmpty())
            {
                messages.Append("ORDER_NUM ");
            }

            if (!item.OrderDate.HasValue)
            {
                messages.Append("ORDER_DATE ");
            }

            if (!item.Amount.HasValue)
            {
                messages.Append("AMOUNT ");
            }

            if (item.PaymentPurpose.IsEmpty())
            {
                messages.Append("PAYMENT_PURPOSE ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

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

        protected override importNotificationsOfOrderExecutionRequest GetRequestObject(IEnumerable<NotificationOfOrderExecution> listForImport)
        {
            var importNotifOfOrderExecTypeList = new List<importNotificationsOfOrderExecutionRequestNotificationOfOrderExecutionType>();

            foreach (var item in listForImport)
            {
                var prepareResult = this.PrepareRequest(item);
                importNotifOfOrderExecTypeList.Add(prepareResult);
            }

            this.CountObjects += importNotifOfOrderExecTypeList.Count;

            return new importNotificationsOfOrderExecutionRequest
            {
                NotificationOfOrderExecutionType = importNotifOfOrderExecTypeList.ToArray()
            };
        }

        protected override AckRequest GetRequestAckRequest(importNotificationsOfOrderExecutionRequest request)
        {
            AckRequest result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importNotificationsOfOrderExecution(this.RequestHeader, request, out result);
            }

            return result;
        }

        protected void CheckResponseItem(CommonResultType responseItem)
        {
            if (this.notificationsByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                var notification = this.notificationsByTransportGuid[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;
                    var errorNotation = string.Empty;

                    if (error != null)
                    {
                        errorNotation = error.Description;
                    }

                    this.AddLineToLog("Договора управления", notification.Id, "Не загружен", errorNotation);
                    return;
                }

                notification.Guid = responseItem.GUID;
                this.notificationsToSave.Add(notification);

                this.AddLineToLog("Договора управления", notification.Id, "Загружен", responseItem.GUID);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.notificationsToSave, 1000, true, true);
        }

        private importNotificationsOfOrderExecutionRequestNotificationOfOrderExecutionType PrepareRequest(
            NotificationOfOrderExecution notification)
        {
            var notificationRequest = new importNotificationsOfOrderExecutionRequestNotificationOfOrderExecutionType
            {
                TransportGUID = Guid.NewGuid().ToString(),
                SupplierInfo = new NotificationOfOrderExecutionTypeSupplierInfo
                {
                    SupplierID = notification.SupplierId,
                    SupplierName = notification.SupplierName
                },
                RecipientInfo = new NotificationOfOrderExecutionTypeRecipientInfo
                {
                    RecipientINN = notification.RecipientInn,
                    RecipientKPP = notification.RecipientKpp,
                    BankName = notification.BankName,
                    RecipientBIK = notification.RecipientBik,
                    CorrespondentBankAccount = notification.CorrespondentBankAccount,
                    RecipientAccount = notification.RecipientAccount,
                    RecipientName = notification.RecipientName
                },
                OrderInfo = new NotificationOfOrderExecutionTypeOrderInfo
                {
                    OrderID = notification.OrderId,
                    PaymentDocumentGuid = notification.RisPaymentDocument.Guid,
                    AccountNumber = notification.AccountNumber,
                    OrderNum = notification.OrderNum,
                    OrderDate = notification.OrderDate.Value,
                    Amount = notification.Amount.Value,
                    PaymentPurpose = notification.PaymentPurpose,
                    Comment = notification.Comment
                }
            };

            notificationsByTransportGuid.Add(notificationRequest.TransportGUID, notification);

            return notificationRequest;
        }
    }
}