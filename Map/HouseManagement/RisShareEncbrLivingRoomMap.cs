namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisShareEncbrLivingRoom"
    /// </summary>
    public class RisShareEncbrLivingRoomMap : BaseRisEntityMap<RisShareEncbrLivingRoom>
    {
        public RisShareEncbrLivingRoomMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisShareEncbrLivingRoom", "RIS_SHAREECNBRLIVINGROOM")
        {
        }

        protected override void Map()
        {
            Property(x => x.IntPart, "IntPart").Column("INTPART").Length(50);
            Property(x => x.FracPart, "FracPart").Column("FRACPART").Length(50);
            Reference(x => x.Ecnbr, "Ecnbr").Column("ECNBR_ID").Fetch();
            Reference(x => x.Share, "Share").Column("SHARE_ID").Fetch();
            Reference(x => x.LivingRoom, "LivingRoom").Column("LIVINGROOM_ID").Fetch();
        }
    }
}
