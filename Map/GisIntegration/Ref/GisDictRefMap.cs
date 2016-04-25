/// <mapping-converter-backup>
/// namespace Bars.Gkh.Ris.Map.GisIntegration.Ref
/// {
///     using B4.DataAccess.ByCode;
///     using Entities.GisIntegration.Ref;
/// 
///     public class GisDictRefMap : BaseEntityMap<GisDictRef>
///     {
///         public GisDictRefMap()
///             : base("RIS_INTEGR_REF_DICT")
///         {
///             Map(x => x.ClassName, "CLASS_NAME", false, 1000);
///             Map(x => x.GisId, "GIS_REC_ID");
///             Map(x => x.GisName, "GIS_REC_NAME", false, 1000);
///             Map(x => x.GkhId, "GKH_REC_ID");
///             Map(x => x.GkhName, "GKH_REC_NAME", false, 1000);
/// 
///             References(x => x.Dict, "DICT_ID", ReferenceMapConfig.NotNullAndFetch);
///         }
///     }
/// }
/// </mapping-converter-backup>

namespace Bars.Gkh.Ris.Map.GisIntegration.Ref
{
    using Bars.B4.Modules.Mapping.Mappers;
    using Bars.Gkh.Ris.Entities.GisIntegration.Ref;

    /// <summary>Маппинг для "Bars.Gkh.Ris.Entities.GisIntegration.Ref.GisDictRef"</summary>
    public class GisDictRefMap : BaseEntityMap<GisDictRef>
    {
        /// <summary>
        /// Конструктор типа GisDictRefMap
        /// </summary>
        public GisDictRefMap() : base("Bars.Gkh.Ris.Entities.GisIntegration.Ref.GisDictRef", "RIS_INTEGR_REF_DICT")
        {
        }
        
        protected override void Map()
        {
            this.Property(x => x.ClassName, "ClassName").Column("CLASS_NAME").Length(1000);
            this.Property(x => x.GkhId, "GkhId").Column("GKH_REC_ID");
            this.Property(x => x.GkhName, "GkhName").Column("GKH_REC_NAME").Length(1000);
            this.Property(x => x.GisId, "GisId").Column("GIS_REC_ID").Length(10);
            this.Property(x => x.GisGuid, "GisGuid").Column("GIS_REC_GUID").Length(50);
            this.Property(x => x.GisName, "GisName").Column("GIS_REC_NAME").Length(1000);
            this.Reference(x => x.Dict, "Dict").Column("DICT_ID").NotNull().Fetch();
        }
    }
}