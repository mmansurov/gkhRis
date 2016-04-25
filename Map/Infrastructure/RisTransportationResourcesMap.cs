namespace Bars.Gkh.Ris.Map.Infrastructure
{
    using Entities.Infrastructure;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Infrastructure.RisTransportationResources"
    /// </summary>
    public class RisTransportationResourcesMap : BaseRisEntityMap<RisTransportationResources>
    {
        public RisTransportationResourcesMap() :
            base("Bars.Gkh.Ris.Entities.Infrastructure.RisTransportationResources", "RIS_TRANSPORTATION_RESOURCES")
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
            this.Property(x => x.VolumeLosses, "VolumeLosses").Column("VOLUMELOSSES");
            this.Property(x => x.CoolantCode, "CoolantCode").Column("COOLANTCODE").Length(200);
            this.Property(x => x.CoolantGuid, "CoolantGuid").Column("COOLANTGUID").Length(200);
            this.Property(x => x.CoolantName, "CoolantName").Column("COOLANTNAME").Length(200);
        }
    }
}
