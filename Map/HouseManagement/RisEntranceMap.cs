namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisEntrance"
    /// </summary>
    public class RisEntranceMap : BaseRisEntityMap<RisEntrance>
    {
        public RisEntranceMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisEntrance", "RIS_ENTRANCE")
        {
        }

        protected override void Map()
        {
            Reference(x => x.ApartmentHouse, "ApartmentHouse").Column("HOUSE_ID").Fetch();
            Property(x => x.EntranceNum, "EntranceNum").Column("ENTRANCENUM");
            Property(x => x.StoreysCount, "StoreysCount").Column("STOREYSCOUNT");
            Property(x => x.CreationDate, "CreationDate").Column("CREATIONDATE");
            Property(x => x.TerminationDate, "TerminationDate").Column("TERMINATIONDATE");
        }
    }
}