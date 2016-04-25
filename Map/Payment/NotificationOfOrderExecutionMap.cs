namespace Bars.Gkh.Ris.Map.Payment
{
    using Entities.Payment;
    using GisIntegration;

    public class NotificationOfOrderExecutionMap : BaseRisEntityMap<NotificationOfOrderExecution>
    {
        public NotificationOfOrderExecutionMap() :
            base("Bars.Gkh.Ris.Entities.Payment.NotificationOfOrderExecution", "RIS_NOTIFORDEREXECUT")
        {
        }

        protected override void Map()
        {
            Property(x => x.SupplierId, "Уникальный идентификатор плательщика").Column("SUPPLIER_ID").Length(25);
            Property(x => x.SupplierName, "Наименование плательщика").Column("SUPPLIER_NAME").Length(160);
            Property(x => x.RecipientInn, "ИНН получателя платежа").Column("RECIPIENT_INN").Length(12);
            Property(x => x.RecipientKpp, "КПП получателя платежа").Column("RECIPIENT_KPP").Length(9);
            Property(x => x.BankName, "Наименование банка получателя платежа").Column("BANK_NAME").Length(160);
            Property(x => x.RecipientBik, "БИК банка получателя платежа").Column("RECIPIENT_BANK_BIK").Length(9);
            Property(x => x.CorrespondentBankAccount, "Корр. счет банка получателя").Column("RECIPIENT_BANK_CORRACC").Length(20);
            Property(x => x.RecipientAccount, "Счет получателя").Column("RECIPIENT_ACCOUNT").Length(20);
            Property(x => x.RecipientName, "Наименование получателя").Column("RECIPIENT_NAME").Length(160);
            Property(x => x.OrderId, "Уникальный идентификатор распоряжения").Column("ORDER_ID").Length(32);
            Reference(x => x.RisPaymentDocument, "Платежный документ").Column("RIS_PAYM_DOC_ID").Fetch().NotNull();
            Property(x => x.AccountNumber, "Номер лицевого счета/Иной идентификтатор плательщика").Column("ACCOUNT_NUMBER").Length(30);
            Property(x => x.OrderNum, "Номер распоряжения").Column("ORDER_NUM").Length(9);
            Property(x => x.OrderDate, "Дата распоряжения").Column("ORDER_DATE");
            Property(x => x.Amount, "Сумма").Column("AMOUNT");
            Property(x => x.PaymentPurpose, "Назначение платежа").Column("PAYMENT_PURPOSE").Length(210);
            Property(x => x.Comment, "Произвольный комментарий").Column("COMMENT").Length(210);
        }
    }
}