namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Bars.Gkh.Ris.Map.GisIntegration;

    using Entities.HouseManagement;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.ProtocolMeetingOwner"
    /// </summary>
    public class ProtocolMeetingOwnerMap : BaseRisEntityMap<ProtocolMeetingOwner>
    {
        public ProtocolMeetingOwnerMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.ProtocolMeetingOwner", "RIS_PROTOCOLMEETINGOWNER")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.Charter, "Charter").Column("CHARTER_ID").Fetch();
            this.Reference(x => x.Attachment, "Attachment").Column("ATTACHMENT_ID").NotNull().Fetch();
            this.Reference(x => x.Contract, "Contract").Column("CONTRACT_ID").NotNull().Fetch();
        }
    }
}