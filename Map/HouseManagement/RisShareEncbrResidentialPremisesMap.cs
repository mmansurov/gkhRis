namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisShareEncbrResidentialPremises"
    /// </summary>
    public class RisShareEncbrResidentialPremisesMap : BaseRisEntityMap<RisShareEncbrResidentialPremises>
    {
        public RisShareEncbrResidentialPremisesMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisShareEncbrResidentialPremises", "RIS_SHAREECNBRRESIDENTPREM")
        {
        }

        protected override void Map()
        {
            Property(x => x.IntPart, "IntPart").Column("INTPART").Length(50);
            Property(x => x.FracPart, "FracPart").Column("FRACPART").Length(50);
            Reference(x => x.Ecnbr, "Ecnbr").Column("ECNBR_ID").Fetch();
            Reference(x => x.Share, "Share").Column("SHARE_ID").Fetch();
            Reference(x => x.ResidentialPremises, "ResidentialPremises").Column("RESIDENTPREM_ID").Fetch();
        }
    }
}
