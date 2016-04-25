namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.LivingRoom"
    /// </summary>
    public class LivingRoomMap : BaseRisEntityMap<LivingRoom>
    {
        public LivingRoomMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.LivingRoom", "RIS_LIVINGROOM")
        {
        }

        protected override void Map()
        {
            Reference(x => x.ResidentialPremises, "ResidentialPremises").Column("RES_PREMISES_ID").Fetch();
            Reference(x => x.House, "House").Column("HOUSE_ID").Fetch();
            Property(x => x.RoomNumber, "RoomNumber").Column("ROOMNUMBER").Length(50);
            Property(x => x.Square, "Square").Column("SQUARE");
            Property(x => x.TerminationDate, "TerminationDate").Column("TERMINATIONDATE");
            Property(x => x.CadastralNumber, "CadastralNumber").Column("CADASTRALNUMBER").Length(50);
            Property(x => x.PrevStateRegNumberCadastralNumber, "PrevStateRegNumberCadastralNumber")
                .Column("PREVSTATEREGNUMBER_CADASTRALNUMBER").Length(50);
            Property(x => x.PrevStateRegNumberInventoryNumber, "PrevStateRegNumberInventoryNumber")
                .Column("PREVSTATEREGNUMBER_INVENTORYNUMBER").Length(50);
            Property(x => x.PrevStateRegNumberConditionalNumber, "PrevStateRegNumberConditionalNumber")
                .Column("PREVSTATEREGNUMBER_CONDITIONALNUMBER").Length(50);
            Property(x => x.Floor, "Floor").Column("FLOOR").Length(50);
        }
    }
}