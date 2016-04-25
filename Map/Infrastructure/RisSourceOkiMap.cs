namespace Bars.Gkh.Ris.Map.Infrastructure
{
    using Entities.Infrastructure;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Infrastructure.RisSourceOki"
    /// </summary>
    public class RisSourceOkiMap : BaseRisEntityMap<RisSourceOki>
    {
        public RisSourceOkiMap() :
            base("Bars.Gkh.Ris.Entities.Infrastructure.RisSourceOki", "RIS_SOURCE_OKI")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.RkiItem, "RkiItem").Column("RKIITEM_ID").Fetch();
            this.Property(x => x.SourceOki, "SourceOki").Column("SOURCEOKI").Length(200);
        }
    }
}
