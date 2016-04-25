namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisShareEncbrNonResPrem"
    /// </summary>
    public class RisShareEncbrNonResPremMap : BaseRisEntityMap<RisShareEncbrNonResPrem>
    {
        public RisShareEncbrNonResPremMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisShareEncbrNonResPrem", "RIS_SHAREECNBRNONRESPREM")
        {
        }

        protected override void Map()
        {
            Property(x => x.IntPart, "IntPart").Column("INTPART").Length(50);
            Property(x => x.FracPart, "FracPart").Column("FRACPART").Length(50);
            Reference(x => x.Ecnbr, "Ecnbr").Column("ECNBR_ID").Fetch();
            Reference(x => x.Share, "Share").Column("SHARE_ID").Fetch();
            Reference(x => x.NonResidentialPremises, "NonResidentialPremises").Column("NONRESIDENTIALPREMISES_ID").Fetch();
        }
    }
}
