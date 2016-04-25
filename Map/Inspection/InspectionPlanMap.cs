namespace Bars.Gkh.Ris.Map.Inspection
{
    using Entities.Inspection;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Inspection.InspectionPlan"
    /// </summary>
    public class InspectionPlanMap : BaseRisEntityMap<InspectionPlan>
    {
        public InspectionPlanMap() : 
                base("Bars.Gkh.Ris.Entities.Inspection.InspectionPlan", "RIS_INSPECTION_PLAN")
        {
        }
        
        protected override void Map()
        {
            Property(x => x.Year, "Year").Column("YEAR");
            Property(x => x.ApprovalDate, "ApprovalDate").Column("APPROVAL_DATE");
        }
    }
}