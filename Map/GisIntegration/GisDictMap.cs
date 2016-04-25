/// <mapping-converter-backup>
/// namespace Bars.Gkh.Ris.Map.GisIntegration
/// {
///     using B4.DataAccess.ByCode;
///     using Entities.GisIntegration;
/// 
///     public class GisDictMap : BaseEntityMap<GisDict>
///     {
///         public GisDictMap()
///             : base("RIS_INTEGR_DICT")
///         {
///             Map(x => x.Name, "NAME", false, 500);
///             Map(x => x.ActionCode, "ACTION_CODE", true, 500);
///             Map(x => x.NsiRegistryNumber, "REGISTRY_NUMBER", true, 50);
///             Map(x => x.DateIntegration, "DATE_INTEG");
///         }
///     }
/// }
/// </mapping-converter-backup>

namespace Bars.Gkh.Ris.Map.GisIntegration
{
    using Bars.B4.Modules.Mapping.Mappers;
    using Bars.Gkh.Ris.Entities.GisIntegration;
    
    
    /// <summary>Маппинг для "Bars.Gkh.Ris.Entities.GisIntegration.GisDict"</summary>
    public class GisDictMap : BaseEntityMap<GisDict>
    {
        
        public GisDictMap() : 
                base("Bars.Gkh.Ris.Entities.GisIntegration.GisDict", "RIS_INTEGR_DICT")
        {
        }
        
        protected override void Map()
        {
            Property(x => x.Name, "Name").Column("NAME").Length(500);
            Property(x => x.ActionCode, "ActionCode").Column("ACTION_CODE").Length(500).NotNull();
            Property(x => x.NsiRegistryNumber, "NsiRegistryNumber").Column("REGISTRY_NUMBER").Length(50).NotNull();
            Property(x => x.DateIntegration, "DateIntegration").Column("DATE_INTEG");
        }
    }
}
