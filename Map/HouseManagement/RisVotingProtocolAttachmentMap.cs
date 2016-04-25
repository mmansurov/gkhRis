namespace Bars.Gkh.Ris.Map
{
    using B4.Modules.Mapping.Mappers;
    using Entities.HouseManagement;

    public class RisVotingProtocolAttachmentMap : BaseEntityMap<RisVotingProtocolAttachment>
    {
        public RisVotingProtocolAttachmentMap() :
            base("Bars.Gkh.Ris.Entities.RisVotingProtocolAttachment", "RIS_VOTINGPROTOCOL_ATTACHMENT")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.VotingProtocol, "VotingProtocol").Column("VOTINGPROTOCOL_ID").Fetch();
            this.Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID").Fetch();
        }
    }
}
