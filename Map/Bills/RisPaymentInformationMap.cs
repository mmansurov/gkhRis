namespace Bars.Gkh.Ris.Map.Bills
{
    using Bars.Gkh.Ris.Entities.Bills;
    using Bars.Gkh.Ris.Map.GisIntegration;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.Bills.RisPaymentInformation
    /// </summary>
    public class RisPaymentInformationMap : BaseRisEntityMap<RisPaymentInformation>
    {
        /// <summary>
        /// Конструктор маппинга сущности Bars.Gkh.Ris.Entities.Bills.RisPaymentInformation
        /// </summary>
        public RisPaymentInformationMap() :
            base("Bars.Gkh.Ris.Entities.Bills.RisPaymentInformation", "RIS_PAYMENT_INFO")
        {
        }

        /// <summary>
        /// Инициализация маппинга
        /// </summary>
        protected override void Map()
        {
            this.Property(x => x.Recipient, "Recipient").Column("RECIPIENT").Length(160);
            this.Property(x => x.BankBik, "BankBik").Column("BANK_BIK").Length(9);
            this.Property(x => x.RecipientKpp, "RecipientKpp").Column("RECIPIENT_KPP").Length(9);
            this.Property(x => x.OperatingAccountNumber, "OperatingAccountNumber").Column("OPERATING_ACCOUNT_NUMBER").Length(20);
            this.Property(x => x.CorrespondentBankAccount, "CorrespondentBankAccount").Column("CORRESPONDENT_BANK_ACCOUNT").Length(20);
        }
    }
}
