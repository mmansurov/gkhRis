namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisAccount"
    /// </summary>
    public class RisAccountMap : BaseRisEntityMap<RisAccount>
    {
        public RisAccountMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisAccount", "RIS_ACCOUNT")
        {
        }

        protected override void Map()
        {
            Property(x => x.RisAccountType, "RisAccountType").Column("TYPEACCOUNT");
            Reference(x => x.OwnerInd, "OwnerInd").Column("OWNERIND_ID").Fetch();
            Reference(x => x.OwnerOrg, "OwnerOrg").Column("OWNERORG_ID").Fetch();
            Reference(x => x.RenterInd, "RenterInd").Column("RENTERIND_ID").Fetch();
            Reference(x => x.RenterOrg, "RenterOrg").Column("RENTERORG_ID").Fetch();
            Property(x => x.LivingPersonsNumber, "LivingPersonsNumber").Column("LIVINGPERSONSNUMBER");
            Property(x => x.TotalSquare, "TotalSquare").Column("TOTALSQUARE");
            Property(x => x.ResidentialSquare, "ResidentialSquare").Column("RESIDENTIALSQUARE");
            Property(x => x.HeatedArea, "HeatedArea").Column("HEATEDAREA");
            Property(x => x.Closed, "Closed").Column("CLOSED");
            Property(x => x.CloseReasonCode, "CloseReasonCode").Column("CLOSEREASON_CODE").Length(50);
            Property(x => x.CloseReasonGuid, "CloseReasonGuid").Column("CLOSEREASON_GUID").Length(50);
            Property(x => x.CloseDate, "CloseDate").Column("CLOSEDATE");
            Property(x => x.AccountNumber, "AccountNumber").Column("ACCOUNTNUMBER").Length(50);
        }
    }
}