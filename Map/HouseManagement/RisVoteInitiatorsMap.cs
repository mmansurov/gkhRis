namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisVoteInitiators"
    /// </summary>
    public class RisVoteInitiatorsMap : BaseRisEntityMap<RisVoteInitiators>
    {
        public RisVoteInitiatorsMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisVoteInitiators", "RIS_VOTEINITIATORS")
        {
        }

        protected override void Map()
        {
            Reference(x => x.Ind, "Ind").Column("IND_ID").Fetch();
            Reference(x => x.Org, "Org").Column("ORG_ID").Fetch();
            Reference(x => x.VotingProtocol, "VotingProtocol").Column("VOTINGPROTOCOL_ID").Fetch();
        }
    }
}
