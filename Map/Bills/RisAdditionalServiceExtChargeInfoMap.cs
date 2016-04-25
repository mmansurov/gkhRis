namespace Bars.Gkh.Ris.Map.Bills
{
    using Bars.Gkh.Ris.Entities.Bills;
    using Bars.Gkh.Ris.Map.GisIntegration;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.Bills.RisAdditionalServiceExtChargeInfo
    /// </summary>
    public class RisAdditionalServiceExtChargeInfoMap: BaseRisEntityMap<RisAdditionalServiceExtChargeInfo>
    {
        /// <summary>
        /// Конструктор маппинга сущности Bars.Gkh.Ris.Entities.Bills.RisAdditionalServiceExtChargeInfo
        /// </summary>
        public RisAdditionalServiceExtChargeInfoMap()
            : base(
                "Bars.Gkh.Ris.Entities.Bills.RisAdditionalServiceExtChargeInfo",
                "RIS_ADDITIONAL_SERVICE_EXT_CHARGE_INFO")
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
            this.Property(x => x.ServiceTypeCode, "ServiceTypeCode").Column("SERVICE_TYPE_CODE").Length(20);
            this.Property(x => x.ServiceTypeGuid, "ServiceTypeGuid").Column("SERVICE_TYPE_GUID").Length(50);
        }
    }
}
