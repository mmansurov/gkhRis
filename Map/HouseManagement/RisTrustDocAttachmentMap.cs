namespace Bars.Gkh.Ris.Map
{
    using B4.Modules.Mapping.Mappers;
    using Entities.HouseManagement;

    public class RisTrustDocAttachmentMap : BaseEntityMap<RisTrustDocAttachment>
    {
        public RisTrustDocAttachmentMap() :
            base("Bars.Gkh.Ris.Entities.RisTrustDocAttachment", "RIS_TRUSTDOCATTACHMENT")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.PublicPropertyContract, "PublicPropertyContract").Column("PUBLICPROPERTYCONTRACT_ID").Fetch();
            this.Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID").Fetch();
        }
    }
}
