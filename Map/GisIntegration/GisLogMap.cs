namespace Bars.Gkh.Ris.Map.GisIntegration
{
    using Bars.B4.Modules.Mapping.Mappers;
    using Bars.Gkh.Ris.Entities.GisIntegration;
    
    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.GisIntegration.GisLog"
    /// </summary>
    public class GisLogMap : BaseEntityMap<GisLog>
    {
        public GisLogMap() : 
                base("Bars.Gkh.Ris.Entities.GisIntegration.GisLog", "RIS_INTEGR_LOG")
        {
        }
        
        protected override void Map()
        {
            Property(x => x.ServiceLink, "ServiceLink").Column("LINK").Length(2000).NotNull();
            Property(x => x.UserName, "UserName").Column("USER_NAME").Length(300).NotNull();
            Property(x => x.MethodName, "MethodName").Column("METHOD_NAME").Length(300).NotNull();
            Property(x => x.DateStart, "DateStart").Column("DATE_START");
            Property(x => x.DateEnd, "DateEnd").Column("DATE_END");
            Property(x => x.CountObjects, "CountObjects").Column("COUNT_OBJECTS");
            Property(x => x.ProcessedObjects, "ProcessedObjects").Column("PROCESSED_OBJECTS");
            Property(x => x.ProcessedPercent, "ProcessedPercent").Column("PROCESSED_PERCENT");
            Reference(x => x.FileLog, "FileLog").Column("FILELOG_ID").Fetch();
        }
    }
}