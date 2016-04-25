namespace Bars.Gkh.Ris.Map.Infrastructure
{
    using Entities.Infrastructure;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Infrastructure.RisReceiverOki"
    /// </summary>
    public class RisReceiverOkiMap : BaseRisEntityMap<RisReceiverOki>
    {
        public RisReceiverOkiMap() :
            base("Bars.Gkh.Ris.Entities.Infrastructure.RisReceiverOki", "RIS_RECEIVER_OKI")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.RkiItem, "RkiItem").Column("RKIITEM_ID").Fetch();
            this.Property(x => x.ReceiverOki, "ReceiverOki").Column("RECEIVEROKI").Length(200);
        }
    }
}
