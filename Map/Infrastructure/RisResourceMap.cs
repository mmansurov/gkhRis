namespace Bars.Gkh.Ris.Map.Infrastructure
{
    using Entities.Infrastructure;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Infrastructure.RisResource"
    /// </summary>
    public class RisResourceMap : BaseRisEntityMap<RisResource>
    {
        public RisResourceMap() :
            base("Bars.Gkh.Ris.Entities.Infrastructure.RisResource", "RIS_RESOURCE")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.RkiItem, "RkiItem").Column("RKIITEM_ID").Fetch();
            this.Property(x => x.MunicipalResourceCode, "MunicipalResourceCode").Column("MUNICIPALRESOURCECODE").Length(200);
            this.Property(x => x.MunicipalResourceGuid, "MunicipalResourceGuid").Column("MUNICIPALRESOURCEGUID").Length(200);
            this.Property(x => x.MunicipalResourceName, "MunicipalResourceName").Column("MUNICIPALRESOURCENAME").Length(200);
            this.Property(x => x.TotalLoad, "TotalLoad").Column("TOTALLOAD");
            this.Property(x => x.IndustrialLoad, "IndustrialLoad").Column("INDUSTRIALLOAD");
            this.Property(x => x.SocialLoad, "SocialLoad").Column("SOCIALLOAD");
            this.Property(x => x.PopulationLoad, "PopulationLoad").Column("POPULATIONLOAD");
            this.Property(x => x.SetPower, "SetPower").Column("SETPOWER");
            this.Property(x => x.SitingPower, "SitingPower").Column("SITINGPOWER");
        }
    }
}
