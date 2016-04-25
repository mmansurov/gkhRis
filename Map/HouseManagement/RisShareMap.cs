namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisShare"
    /// </summary>
    public class RisShareMap : BaseRisEntityMap<RisShare>
    {
        public RisShareMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisShare", "RIS_SHARE")
        {
        }

        protected override void Map()
        {
            Property(x => x.IsPrivatized, "IsPrivatized").Column("ISPRIVATIZED");
            Property(x => x.TermDate, "TermDate").Column("TERMDATE");
            Reference(x => x.RisShareContragent, "Contragent").Column("CONTRAGENT_ID").Fetch();
            Reference(x => x.Account, "Account").Column("ACCOUNT_ID").Fetch();
        }
    }
}
