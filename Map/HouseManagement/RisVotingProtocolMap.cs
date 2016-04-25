namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisVotingProtocol"
    /// </summary>
    public class RisVotingProtocolMap : BaseRisEntityMap<RisVotingProtocol>
    {
        public RisVotingProtocolMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisVotingProtocol", "RIS_VOTINGPROTOCOL")
        {
        }

        protected override void Map()
        {
            Reference(x => x.House, "House").Column("HOUSE_ID").Fetch();
            Property(x => x.ProtocolNum, "ProtocolNum").Column("PROTOCOLNUM").Length(200);
            Property(x => x.ProtocolDate, "ProtocolDate").Column("PROTOCOLDATE");
            Property(x => x.VotingPlace, "VotingPlace").Column("VOTINGPLACE").Length(200);
            Property(x => x.BeginDate, "BeginDate").Column("BEGINDATE");
            Property(x => x.EndDate, "EndDate").Column("ENDDATE");
            Property(x => x.Discipline, "Discipline").Column("DISCIPLINE").Length(200);
            Property(x => x.MeetingEligibility, "MeetingEligibility").Column("MEETINGELIGIBILITY");
            Property(x => x.VotingType, "VotingType").Column("VOTINGTYPE");
            Property(x => x.VotingTimeType, "VotingTimeType").Column("VOTINGTIMETYPE");
            Property(x => x.Placing, "Placing").Column("PLACE");
            Property(x => x.Revert, "Revert").Column("REVERT");
        }
    }
}
