namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.Charter"
    /// </summary>
    public class CharterMap : BaseRisEntityMap<Charter>
    {
        public CharterMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.Charter", "RIS_CHARTER")
        {
        }

        protected override void Map()
        {
            Property(x => x.DocNum, "DocNum").Column("DOCNUM").Length(50);
            Property(x => x.DocDate, "DocDate").Column("DOCDATE");
            Property(x => x.PeriodMeteringStartDate, "PeriodMeteringStartDate").Column("PERIODMETERING_STARTDATE");
            Property(x => x.PeriodMeteringEndDate, "PeriodMeteringEndDate").Column("PERIODMETERING_ENDDATE");
            Property(x => x.PeriodMeteringLastDay, "PeriodMeteringLastDay").Column("PERIODMETERING_LASTDAY");
            Property(x => x.PaymentDateStartDate, "PaymentDateStartDate").Column("PAYMENTDATE_STARTDATE");
            Property(x => x.PaymentDateLastDay, "PaymentDateLastDay").Column("PAYMENTDATE_LASTDAY");
            Property(x => x.Managers, "Managers").Column("MANAGERS").Length(50);
            Reference(x => x.Head, "Head").Column("HEAD_ID").Fetch().NotNull();
            Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID").Fetch();
            Property(x => x.ApprovalCharter, "ApprovalCharter").Column("APPROVALCHARTER");
            Property(x => x.RollOverCharter, "RollOverCharter").Column("ROLLOVERCHARTER");
            Property(x => x.TerminateCharterDate, "TerminateCharterDate").Column("TERMINATECHARTER_DATE");
            Property(x => x.TerminateCharterReason, "TerminateCharterReason").Column("TERMINATECHARTER_REASON");
        }
    }
}