namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisNotification"
    /// </summary>
    public class RisNotificationMap : BaseRisEntityMap<RisNotification>
    {
        public RisNotificationMap() : base("Bars.Gkh.Ris.Entities.HouseManagement.RisNotification", "RIS_NOTIFICATION")
        {
        }

        protected override void Map()
        {
            this.Property(x => x.Topic, "Topic").Column("TOPIC").Length(200);
            this.Property(x => x.IsImportant, "IsImportant").Column("ISIMPORTANT");
            this.Property(x => x.Content, "Content").Column("CONTENT").Length(200);
            this.Property(x => x.IsAll, "IsAll").Column("ISALL");
            this.Property(x => x.IsNotLimit, "IsNotLimit").Column("ISNOTLIMIT");
            this.Property(x => x.StartDate, "StartDate").Column("STARTDATE");
            this.Property(x => x.EndDate, "EndDate").Column("ENDDATE");
            this.Property(x => x.IsShipOff, "IsShipOff").Column("ISSHIPOFF");
            this.Property(x => x.Deleted, "Deleted").Column("DELETED");
        }
    }
}