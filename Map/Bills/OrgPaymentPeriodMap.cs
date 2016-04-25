namespace Bars.Gkh.Ris.Map.Bills
{
    using Entities.Bills;
    using GisIntegration;

    public class OrgPaymentPeriodMap : BaseRisEntityMap<OrgPaymentPeriod>
    {
        public OrgPaymentPeriodMap()
            : base("Bars.Gkh.Ris.Entities.Bills.OrgPaymentPeriod", "RIS_PAYMENT_PERIOD")
        {
        }

        protected override void Map()
        {
            this.Property(x => x.Month, "Месяц").Column("MONTH");
            this.Property(x => x.Year, "Год").Column("YEAR");
            this.Property(x => x.RisPaymentPeriodType, "Тип платежного периода").Column("PAYMENT_PERIOD_TYPE");
        }
    }
}