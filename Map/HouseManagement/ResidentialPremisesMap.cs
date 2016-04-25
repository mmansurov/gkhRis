namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.ResidentialPremises"
    /// </summary>
    public class ResidentialPremisesMap : BaseRisEntityMap<ResidentialPremises>
    {
        public ResidentialPremisesMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.ResidentialPremises", "RIS_RESIDENTIALPREMISES")
        {
        }

        protected override void Map()
        {
            Reference(x => x.ApartmentHouse, "ApartmentHouse").Column("HOUSE_ID").Fetch();
            Property(x => x.PremisesNum, "PremisesNum").Column("PREMISESNUM").Length(50);
            Property(x => x.CadastralNumber, "CadastralNumber").Column("CADASTRALNUMBER").Length(50);
            Property(x => x.PrevStateRegNumberCadastralNumber, "PrevStateRegNumberCadastralNumber")
                .Column("PREVSTATEREGNUMBER_CADASTRALNUMBER").Length(50);
            Property(x => x.PrevStateRegNumberInventoryNumber, "PrevStateRegNumberInventoryNumber")
                .Column("PREVSTATEREGNUMBER_INVENTORYNUMBER").Length(50);
            Property(x => x.PrevStateRegNumberConditionalNumber, "PrevStateRegNumberConditionalNumber")
                .Column("PREVSTATEREGNUMBER_CONDITIONALNUMBER").Length(50);
            Property(x => x.TerminationDate, "TerminationDate").Column("TERMINATIONDATE");
            Property(x => x.EntranceNum, "EntranceNum").Column("ENTRANCENUM");
            Property(x => x.PremisesCharacteristicCode, "PremisesCharacteristicCode").Column("PREMISESCHARACTERISTIC_CODE").Length(50);
            Property(x => x.PremisesCharacteristicGuid, "PremisesCharacteristicGuid").Column("PREMISESCHARACTERISTIC_GUID").Length(50);
            Property(x => x.RoomsNumCode, "RoomsNumCode").Column("ROOMSNUM_CODE").Length(50);
            Property(x => x.RoomsNumGuid, "RoomsNumGuid").Column("ROOMSNUM_GUID").Length(50);
            Property(x => x.ResidentialHouseTypeCode, "ResidentialHouseTypeCode").Column("RESIDENTIALHOUSETYPE_CODE").Length(50);
            Property(x => x.ResidentialHouseTypeGuid, "ResidentialHouseTypeGuid").Column("RESIDENTIALHOUSETYPE_GUID").Length(50);
            Property(x => x.GrossArea, "GrossArea").Column("GROSSAREA");
            Property(x => x.TotalArea, "TotalArea").Column("TOTALAREA");
            Property(x => x.Floor, "Floor").Column("FLOOR");
        }
    }
}