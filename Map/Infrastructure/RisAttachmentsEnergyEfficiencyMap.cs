namespace Bars.Gkh.Ris.Map.Infrastructure
{
    using B4.Modules.Mapping.Mappers;
    using Entities.Infrastructure;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Infrastructure.RisAttachmentsEnergyEfficiency"
    /// </summary>
    public class RisAttachmentsEnergyEfficiencyMap : BaseEntityMap<RisAttachmentsEnergyEfficiency>
    {
        public RisAttachmentsEnergyEfficiencyMap() :
            base("Bars.Gkh.Ris.Entities.Infrastructure.RisAttachmentsEnergyEfficiency", "RIS_ATTACHMENTS_ENERGYEFFICIENCY")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.RkiItem, "RkiItem").Column("RKIITEM_ID").Fetch();
            this.Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID").Fetch();
        }
    }
}
