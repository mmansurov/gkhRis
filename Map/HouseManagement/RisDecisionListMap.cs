namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisDecisionList"
    /// </summary>
    public class RisDecisionListMap : BaseRisEntityMap<RisDecisionList>
    {
        public RisDecisionListMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisDecisionList", "RIS_DECISIONLIST")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.VotingProtocol, "VotingProtocol").Column("VOTINGPROTOСOL_ID").Fetch();
            this.Property(x => x.QuestionNumber, "QuestionNumber").Column("QUESTIONNUMBER");
            this.Property(x => x.QuestionName, "QuestionName").Column("QUESTIONNAME").Length(200);
            this.Property(x => x.DecisionsTypeCode, "DecisionsTypeCode").Column("DECISIONSTYPE_CODE");
            this.Property(x => x.DecisionsTypeGuid, "DecisionsTypeGuid").Column("DECISIONSTYPE_GUID");
            this.Property(x => x.Agree, "Agree").Column("AGREE");
            this.Property(x => x.Against, "Against").Column("AGAINST");
            this.Property(x => x.Abstent, "Abstent").Column("ABSTENT");
            this.Property(x => x.VotingResume, "VotingResume").Column("VOTINGRESUME");
        }
    }
}
