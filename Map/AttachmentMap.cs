namespace Bars.Gkh.Ris.Map
{
    using B4.Modules.Mapping.Mappers;
    using Entities;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Attachment"
    /// </summary>
    public class AttachmentMap : BaseEntityMap<Attachment>
    {
        public AttachmentMap() :
            base("Bars.Gkh.Ris.Entities.Attachment", "RIS_ATTACHMENT")
        {
        }

        protected override void Map()
        {
            Reference(x => x.FileInfo, "FileInfo").Column("FILE_INFO_ID").NotNull().Fetch();
            Property(x => x.Guid, "Guid").Column("GUID").Length(50);
            Property(x => x.Hash, "Hash").Column("HASH").Length(200);
            Property(x => x.Name, "Name").Column("NAME").Length(50);
            Property(x => x.Description, "Description").Column("DESCRIPTION").Length(50);
        }
    }
}