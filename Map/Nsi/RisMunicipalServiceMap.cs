namespace Bars.Gkh.Ris.Map.Nsi
{
    using Bars.Gkh.Ris.Entities.Nsi;
    using Bars.Gkh.Ris.Map.GisIntegration;

    /// <summary>
    /// Маппинг сущности RisMunicipalService
    /// </summary>
    public class RisMunicipalServiceMap : BaseRisEntityMap<RisMunicipalService>
    {
        public RisMunicipalServiceMap()
            : base("Bars.Gkh.Ris.Entities.Nsi.RisMunicipalService", "RIS_MUNICIPAL_SERVICE")
        {
        }

        protected override void Map()
        {
            this.Property(x => x.MunicipalServiceRefCode, "MunicipalServiceRefCode").Column("MUNICIPAL_SERVICE_REF_CODE").Length(20);
            this.Property(x => x.MunicipalServiceRefGuid, "MunicipalServiceRefGuid").Column("MUNICIPAL_SERVICE_REF_GUID").Length(40);
            this.Property(x => x.GeneralNeeds, "GeneralNeeds").Column("GENERAL_NEEDS");
            this.Property(x => x.MainMunicipalServiceName, "MainMunicipalServiceName").Column("MAIN_MUNICIPAL_SERVICE_NAME").Length(100);
            this.Property(x => x.MunicipalResourceRefCode, "MunicipalResourceRefCode").Column("MUNICIPAL_RESOURCE_REF_CODE").Length(20);
            this.Property(x => x.MunicipalResourceRefGuid, "MunicipalResourceRefGuid").Column("MUNICIPAL_RESOURCE_REF_GUID").Length(40);
            this.Property(x => x.SortOrder, "SortOrder").Column("SORT_ORDER").Length(3);
            this.Property(x => x.SortOrderNotDefined, "SortOrderNotDefined").Column("SORT_ORDER_NOT_DEFINED");
        }
    }
}
