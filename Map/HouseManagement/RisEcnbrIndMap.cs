namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisEcnbrInd"
    /// </summary>
    public class RisEcnbrIndMap : BaseRisEntityMap<RisEcnbrInd>
    {
        public RisEcnbrIndMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisEcnbrInd", "RIS_ECNBRIND")
        {
        }

        protected override void Map()
        {
            Reference(x => x.Ecnbr, "Ecnbr").Column("ECNBR_ID").Fetch();
            Reference(x => x.Ind, "Ind").Column("IND_ID").Fetch();
        }
    }
}
