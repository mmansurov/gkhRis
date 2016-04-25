namespace Bars.Gkh.Ris.Map.Bills
{
    using Bars.Gkh.Ris.Entities.Bills;
    using Bars.Gkh.Ris.Map.GisIntegration;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.Bills.RisPaymentDocument
    /// </summary>
    public class RisPaymentDocumentMap : BaseRisEntityMap<RisPaymentDocument>
    {
        /// <summary>
        /// Конструктор маппинга сущности Bars.Gkh.Ris.Entities.Bills.RisPaymentDocument
        /// </summary>
        public RisPaymentDocumentMap()
            : base("Bars.Gkh.Ris.Entities.Bills.RisPaymentDocument", "RIS_PAYMENT_DOC")
        {
        }

        /// <summary>
        /// Инициализация маппинга
        /// </summary>
        protected override void Map()
        {
            this.Reference(x => x.Account, "Account").Column("ACCOUNT_ID");
            this.Reference(x => x.PaymentInformation, "PaymentInformation").Column("PAYMENT_INFO_ID");
            this.Reference(x => x.AddressInfo, "AddressInfo").Column("ADDRESS_INFO_ID");
            this.Property(x => x.State, "State").Column("STATE");
            this.Property(x => x.TotalPiecemealPaymentSum, "TotalPiecemealPaymentSum").Column("TOTAL_PIECEMEAL_SUM");
            this.Property(x => x.Date, "Date").Column("DATE");
            this.Property(x => x.PeriodMonth, "PeriodMonth").Column("PERIODMONTH");
            this.Property(x => x.PeriodYear, "PeriodYear").Column("PERIODYEAR");
        }
    }
}
