namespace Bars.Gkh.Ris.Enums.Bills
{
    using Bars.B4.Utils;

    /// <summary>
    /// Состояние платежного документа
    /// </summary>
    public enum PaymentDocumentState
    {
        /// <summary>
        /// Выставлен на оплату
        /// </summary>
        [Display("Выставлен на оплату")]
        Expose = 10,

        /// <summary>
        /// Отозван
        /// </summary>
        [Display("Отозван")]
        Withdraw = 20
    }
}
