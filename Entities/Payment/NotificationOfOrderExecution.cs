namespace Bars.Gkh.Ris.Entities.Payment
{
    using System;

    using Bars.Gkh.Ris.Entities.Bills;

    using Map.Payment;

    /// <summary>
    /// Уведомление о выполнении распоряжения
    /// </summary>
    public class NotificationOfOrderExecution : BaseRisEntity
    {
        /// <summary>
        /// Уникальный идентификатор плательщика
        /// </summary>
        public virtual string SupplierId { get; set; }

        /// <summary>
        /// Наименование плательщика
        /// </summary>
        public virtual string SupplierName { get; set; }

        /// <summary>
        /// ИНН получателя платежа
        /// </summary>
        public virtual string RecipientInn { get; set; }

        /// <summary>
        /// КПП получателя платежа
        /// </summary>
        public virtual string RecipientKpp { get; set; }

        /// <summary>
        /// Наименование банка получателя платежа
        /// </summary>
        public virtual string BankName { get; set; }

        /// <summary>
        /// БИК банка получателя платежа
        /// </summary>
        public virtual string RecipientBik { get; set; }

        /// <summary>
        /// Корр. счет банка получателя
        /// </summary>
        public virtual string CorrespondentBankAccount { get; set; }

        /// <summary>
        /// Счет получателя
        /// </summary>
        public virtual string RecipientAccount { get; set; }

        /// <summary>
        /// Наименование получателя платежа
        /// </summary>
        public virtual string RecipientName { get; set; }

        /// <summary>
        /// Уникальный идентификатор распоряжения
        /// </summary>
        public virtual string OrderId { get; set; }

        /// <summary>
        /// Платежный документ
        /// </summary>
        public virtual RisPaymentDocument RisPaymentDocument { get; set; }

        /// <summary>
        /// Номер лицевого счета/Иной идентификтатор плательщика
        /// </summary>
        public virtual string AccountNumber { get; set; }

        /// <summary>
        /// Номер распоряжения
        /// </summary>
        public virtual string OrderNum { get; set; }

        /// <summary>
        /// Дата распоряжения
        /// </summary>
        public virtual DateTime? OrderDate { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public virtual decimal? Amount { get; set; }

        /// <summary>
        /// Назначение платежа
        /// </summary>
        public virtual string PaymentPurpose { get; set; }

        /// <summary>
        /// Произвольный комментарий
        /// </summary>
        public virtual string Comment { get; set; }
    }
}