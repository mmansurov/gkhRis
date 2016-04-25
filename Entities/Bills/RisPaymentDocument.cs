namespace Bars.Gkh.Ris.Entities.Bills
{
    using System;

    using Bars.Gkh.Ris.Entities.HouseManagement;
    using Bars.Gkh.Ris.Enums.Bills;

    /// <summary>
    /// Платежный документ
    /// </summary>
    public class RisPaymentDocument : BaseRisEntity
    {
        /// <summary>
        /// Ссылка на лицевой счет
        /// </summary>
        public virtual RisAccount Account { get; set; }

        /// <summary>
        /// Ссылка на сведения о платежных реквизитах организации
        /// </summary>
        public virtual RisPaymentInformation PaymentInformation { get; set; }

        /// <summary>
        /// Ссылка на адресные сведения
        /// </summary>
        public virtual RisAddressInfo AddressInfo { get; set; }

        /// <summary>
        /// Состояние платежного документа
        /// </summary>
        public virtual PaymentDocumentState State { get; set; }

        /// <summary>
        /// Сумма к оплате с учетом рассрочки платежа и процентов за рассрочку, руб.
        /// </summary>
        public virtual decimal TotalPiecemealPaymentSum { get; set; }

        /// <summary>
        /// Дата выставления документа - счета
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// Месяц рассчетного периода, за который выставлен счет
        /// </summary>
        public virtual short PeriodMonth { get; set; }

        /// <summary>
        /// Год рассчетного периода, за который выставлен счет
        /// </summary>
        public virtual short PeriodYear { get; set; }
    }
}
