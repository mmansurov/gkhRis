namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisShareInd"
    /// </summary>
    public class RisShareIndMap : BaseRisEntityMap<RisShareInd>
    {
        public RisShareIndMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisShareInd", "RIS_SHAREIND")
        {
        }

        protected override void Map()
        {
            Reference(x => x.Share, "Share").Column("SHARE_ID").Fetch();
            Reference(x => x.Ind, "Ind").Column("IND_ID").Fetch();
        }
    }
}
