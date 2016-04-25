namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using B4.Modules.Mapping.Mappers;
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.ConfirmDoc"
    /// </summary>
    public class ConfirmDocMap : BaseEntityMap<ConfirmDoc>
    {
        public ConfirmDocMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.ConfirmDoc", "RIS_CONFIRMDOC")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.House, "House").Column("HOUSE_ID").Fetch();
            this.Property(x => x.Name, "Name").Column("NAME").Length(50);
            this.Property(x => x.Description, "Description").Column("DESCRIPTION").Length(50);
            this.Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID").NotNull().Fetch();
        }
    }
}