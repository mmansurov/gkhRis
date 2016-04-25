namespace Bars.Gkh.Ris.Entities.Bills
{
    /// <summary>
    /// Сведения о платежных реквизитах организации
    /// </summary>
    public class RisPaymentInformation : BaseRisEntity
    {
        /// <summary>
        /// Наименование получателя
        /// </summary>
        public virtual string Recipient { get; set; }

        /// <summary>
        /// БИК банка получателя
        /// </summary>
        public virtual string BankBik { get; set; }

        /// <summary>
        /// КПП получателя платежа
        /// </summary>
        public virtual string RecipientKpp { get; set; }

        /// <summary>
        /// Номер расчетного счета
        /// </summary>
        public virtual string OperatingAccountNumber { get; set; }

        /// <summary>
        /// Корр. счет банка получателя
        /// </summary>
        public virtual string CorrespondentBankAccount { get; set; }
    }
}
