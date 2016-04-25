namespace Bars.Gkh.Ris.Map.Inspection
{
    using Entities.Inspection;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Inspection.Precept"
    /// </summary>
    public class PreceptMap : BaseRisEntityMap<Precept>
    {
        public PreceptMap() :
            base("Bars.Gkh.Ris.Entities.Inspection.Precept", "RIS_INSPECTION_PRECEPT")
        {
        }

        protected override void Map()
        {
            Reference(x => x.Examination, "Examination").Column("EXAMINATION_ID").NotNull().Fetch();
            Property(x => x.Number, "Number").Column("NUMBER").Length(50);
            Property(x => x.Date, "Date").Column("DATE");
            Property(x => x.CancelReason, "CancelReason").Column("CANCEL_REASON").Length(200);
            Property(x => x.CancelDate, "CancelDate").Column("CANCEL_DATE");
        }
    }
}