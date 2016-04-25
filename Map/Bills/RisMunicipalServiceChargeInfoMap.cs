namespace Bars.Gkh.Ris.Map.Bills
{
    using Bars.Gkh.Ris.Entities.Bills;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.Bills.RisMunicipalServiceChargeInfo
    /// </summary>
    public class RisMunicipalServiceChargeInfoMap: BaseRisChargeInfoMap<RisMunicipalServiceChargeInfo>
    {
        /// <summary>
        /// Конструктор маппинга Bars.Gkh.Ris.Entities.Bills.RisMunicipalServiceChargeInfo
        /// </summary>
        public RisMunicipalServiceChargeInfoMap()
            : base("Bars.Gkh.Ris.Entities.Bills.RisMunicipalServiceChargeInfo", "RIS_MUNICIPAL_SERVICE_CHARGE_INFO")
        {
        }

        /// <summary>
        /// Инициализация маппинга
        /// </summary>
        protected override void Map()
        {
            this.Reference(x => x.PaymentDocument, "PaymentDocument").Column("PAYMENT_DOC_ID");
            this.Property(x => x.MoneyRecalculation, "MoneyRecalculation").Column("MONEY_RECALCULATION");
            this.Property(x => x.MoneyDiscount, "MoneyDiscount").Column("MONEY_DISCOUNT");
            this.Property(x => x.PaymentPeriodPiecemealPaymentSum, "PaymentPeriodPiecemealPaymentSum").Column("PERIOD_PIECEMAL_SUM");
            this.Property(x => x.PastPaymentPeriodPiecemealPaymentSum, "PastPaymentPeriodPiecemealPaymentSum").Column("PAST_PERIOD_PIECEMAL_SUM");
            this.Property(x => x.PiecemealPaymentPercentRub, "PiecemealPaymentPercentRub").Column("PIECEMAL_PERCENT_RUB");
            this.Property(x => x.PiecemealPaymentPercent, "PiecemealPaymentPercent").Column("PIECEMAL_PERCENT");
            this.Property(x => x.PiecemealPaymentSum, "PiecemealPaymentSum").Column("PIECEMAL_PAYMENT_SUM");
            this.Property(x => x.PaymentRecalculationReason, "PaymentRecalculationReason").Column("PAYMENT_RECALCULATION_REASON");
            this.Property(x => x.PaymentRecalculationSum, "PaymentRecalculationSum").Column("PAYMENT_RECALCULATION_SUM");
        }
    }
}
