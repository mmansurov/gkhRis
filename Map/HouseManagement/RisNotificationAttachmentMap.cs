namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using B4.Modules.Mapping.Mappers;
    using Entities.HouseManagement;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisNotificationAttachment"
    /// </summary>
    public class RisNotificationAttachmentMap : BaseEntityMap<RisNotificationAttachment>
    {
        public RisNotificationAttachmentMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisNotificationAttachment", "RIS_NOTIFICATION_ATTACHMENT")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID");
            this.Reference(x => x.Notification, "Notification").Column("NOTIFICATION_ID");
        }
    }
}
