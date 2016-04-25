namespace Bars.Gkh.Ris.Map.Bills
{
    using Bars.Gkh.Ris.Entities.Bills;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.Bills.RisHousingServiceChargeInfo
    /// </summary>
    public class RisHousingServiceChargeInfoMap: BaseRisChargeInfoMap<RisHousingServiceChargeInfo>
    {
        /// <summary>
        /// Конструктор маппинга сущности Bars.Gkh.Ris.Entities.Bills.RisHousingServiceChargeInfo
        /// </summary>
        public RisHousingServiceChargeInfoMap()
            : base("Bars.Gkh.Ris.Entities.Bills.RisHousingServiceChargeInfo", "RIS_HOUSING_SERVICE_CHARGE_INFO")
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
        }
    }
}
