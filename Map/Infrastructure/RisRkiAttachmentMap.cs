namespace Bars.Gkh.Ris.Map.Infrastructure
{
    using B4.Modules.Mapping.Mappers;
    using Entities.Infrastructure;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Infrastructure.RisRkiAttachment"
    /// </summary>
    public class RisRkiAttachmentMap : BaseEntityMap<RisRkiAttachment>
    {
        public RisRkiAttachmentMap() :
            base("Bars.Gkh.Ris.Entities.Infrastructure.RisRkiAttachment", "RIS_RKI_ATTACHMENT")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.RkiItem, "RkiItem").Column("RKIITEM_ID").Fetch();
            this.Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID").Fetch();
        }
    }
}
