namespace Bars.Gkh.Ris.Map.Inspection
{
    using Entities.Inspection;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Inspection.Offence"
    /// </summary>
    public class OffenceMap : BaseRisEntityMap<Offence>
    {
        public OffenceMap() :
            base("Bars.Gkh.Ris.Entities.Inspection.Offence", "RIS_INSPECTION_OFFENCE")
        {
        }

        protected override void Map()
        {
            Reference(x => x.Examination, "Examination").Column("EXAMINATION_ID").NotNull().Fetch();
            Property(x => x.Number, "Number").Column("NUMBER").Length(50);
            Property(x => x.Date, "Date").Column("DATE");
        }
    }
}