namespace Bars.Gkh.Ris.Entities.Bills
{
    using Enums;

    /// <summary>
    /// Период
    /// </summary>
    public class OrgPaymentPeriod : BaseRisEntity
    {
        /// <summary>
        /// Месяц
        /// </summary>
        public virtual int Month { get; set; }

        /// <summary>
        /// Год
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// Тип платежного периода
        /// </summary>
        public virtual RisPaymentPeriodType RisPaymentPeriodType { get; set; }
    }
}