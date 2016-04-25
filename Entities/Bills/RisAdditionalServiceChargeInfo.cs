namespace Bars.Gkh.Ris.Entities.Bills
{
    /// <summary>
    /// Начисление по услуге - дополнительная услуга
    /// </summary>
    public class RisAdditionalServiceChargeInfo : BaseRisChargeInfo
    {
        /// <summary>
        /// Ссылка на платежный документ
        /// </summary>
        public virtual RisPaymentDocument PaymentDocument { get; set; }

        /// <summary>
        /// Перерасчеты, корректировки (руб)
        /// </summary>
        public virtual decimal? MoneyRecalculation { get; set; }

        /// <summary>
        /// Льготы, субсидии, скидки (руб)
        /// </summary>
        public virtual decimal? MoneyDiscount { get; set; }
    }
}
