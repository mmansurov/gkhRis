namespace Bars.Gkh.Ris.Map.Inspection
{
    using Entities.Inspection;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Inspection.Examination"
    /// </summary>
    public class ExaminationMap : BaseRisEntityMap<Examination>
    {
        public ExaminationMap() :
            base("Bars.Gkh.Ris.Entities.Inspection.Examination", "RIS_INSPECTION_EXAMINATION")
        {
        }
        
        protected override void Map()
        {
            Reference(x => x.InspectionPlan, "InspectionPlan").Column("PLAN_ID").Fetch();
            Property(x => x.InspectionNumber, "InspectionNumber").Column("INSPECTIONNUMBER");
            Reference(x => x.RisContragent, "RisContragent").Column("CONTRAGENT_ID").Fetch();
            Property(x => x.BaseCode, "BaseCode").Column("BASE_CODE");
            Property(x => x.BaseGuid, "BaseGuid").Column("BASE_GUID");
            Property(x => x.CountDays, "CountDays").Column("COUNT_DAYS");
            Property(x => x.ExaminationFormCode, "ExaminationFormCode").Column("EXAMFORM_CODE");
            Property(x => x.ExaminationFormGuid, "ExaminationFormGuid").Column("EXAMFORM_GUID");
            Property(x => x.IsScheduled, "IsScheduled").Column("IS_SCHEDULED");
            Property(x => x.LastName, "LastName").Column("LASTNAME").Length(50);
            Property(x => x.FirstName, "FirstName").Column("FIRSTNAME").Length(50);
            Property(x => x.MiddleName, "MiddleName").Column("MIDDLENAME").Length(50);
            Property(x => x.OversightActivitiesCode, "OversightActivitiesCode").Column("OVERSIGHT_ACT_CODE").Length(50);
            Property(x => x.OversightActivitiesGuid, "OversightActivitiesGuid").Column("OVERSIGHT_ACT_GUID").Length(50);
            Property(x => x.Objective, "Objective").Column("OBJECTIVE").Length(200);
            Property(x => x.From, "From").Column("DATE_FROM");
            Property(x => x.To, "To").Column("DATE_TO");
            Property(x => x.Duration, "Duration").Column("DURATION");
            Property(x => x.ObjectCode, "ObjectCode").Column("OBJECT_CODE").Length(50);
            Property(x => x.ObjectGuid, "ObjectGuid").Column("OBJECT_GUID").Length(50);
            Property(x => x.Tasks, "Tasks").Column("TASKS").Length(200);
            Property(x => x.OrderGkhId, "OrderGkhId").Column("ORDER_GKH_ID");
            Property(x => x.OrderNumber, "OrderNumber").Column("ORDER_NUMBER").Length(50);
            Property(x => x.OrderDate, "OrderDate").Column("ORDER_DATE");
            Property(x => x.IsPhysicalPerson, "IsPhysicalPerson").Column("IS_PHYS_PERSON");
        }
    }
}