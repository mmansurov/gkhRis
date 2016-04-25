namespace Bars.Gkh.Ris.Map
{
    using Bars.B4.Modules.Mapping.Mappers;
    using Bars.Gkh.Ris.Entities;
    
    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.RisSettings"
    /// </summary>
    public class RisSettingsMap : BaseEntityMap<RisSettings>
    {
        public RisSettingsMap() : 
                base("Bars.Gkh.Ris.Entities.RisSettings", "RIS_SETTINGS")
        {
        }
        
        protected override void Map()
        {
            Property(x => x.Code, "Code").Column("CODE").Length(50);
            Property(x => x.Name, "Name").Column("NAME").Length(100);
            Property(x => x.Value, "Value").Column("VALUE").Length(100);
        }
    }
}