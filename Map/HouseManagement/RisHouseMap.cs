namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.HouseManagement.RisHouse"
    /// </summary>
    public class RisHouseMap : BaseRisEntityMap<RisHouse>
    {
        public RisHouseMap() :
            base("Bars.Gkh.Ris.Entities.HouseManagement.RisHouse", "RIS_HOUSE")
        {
        }

        protected override void Map()
        {
            this.Property(x => x.HouseType, "HouseType").Column("HOUSE_TYPE");
            this.Property(x => x.FiasHouseGuid, "FiasHouseGuid").Column("FIASHOUSEGUID").Length(50);
            this.Property(x => x.CadastralNumber, "CadastralNumber").Column("CADASTRALNUMBER").Length(300);
            this.Property(x => x.TotalSquare, "TotalSquare").Column("TOTALSQUARE");
            this.Property(x => x.ResidentialSquare, "ResidentialSquare").Column("RESIDENTIALSQUARE").NotNull();
            this.Property(x => x.NonResidentialSquare, "NonResidentialSquare").Column("NONRESIDENTIALSQUARE").NotNull();
            this.Property(x => x.StateCode, "StateCode").Column("STATE_CODE").Length(50);
            this.Property(x => x.StateGuid, "StateGuid").Column("STATE_GUID").Length(50);
            this.Property(x => x.ProjectSeries, "ProjectSeries").Column("PROJECTSERIES").Length(300);
            this.Property(x => x.ProjectTypeCode, "ProjectTypeCode").Column("PROJECTTYPE_CODE").Length(50);
            this.Property(x => x.ProjectTypeGuid, "ProjectTypeGuid").Column("PROJECTTYPE_GUID").Length(50);
            this.Property(x => x.BuildingYear, "BuildingYear").Column("BUILDINGYEAR");
            this.Property(x => x.UsedYear, "UsedYear").Column("USEDYEAR");
            this.Property(x => x.TotalWear, "TotalWear").Column("TOTALWEAR");
            this.Property(x => x.EnergyDate, "EnergyDate").Column("ENERGYDATE");
            this.Property(x => x.EnergyCategoryCode, "EnergyCategoryCode").Column("ENERGYCATEGORY_CODE").Length(50);
            this.Property(x => x.EnergyCategoryGuid, "EnergyCategoryGuid").Column("ENERGYCATEGORY_GUID").Length(50);
            this.Property(x => x.OktmoCode, "OktmoCode").Column("OKTMO_CODE").Length(50);
            this.Property(x => x.OktmoName, "OktmoName").Column("OKTMO_NAME").Length(50);
            this.Property(x => x.PrevStateRegNumberCadastralNumber, "PrevStateRegNumberCadastralNumber").Column("PREVSTATEREGNUMBER_CADASTRALNUMBER").Length(50);
            this.Property(x => x.PrevStateRegNumberInventoryNumber, "PrevStateRegNumberInventoryNumber").Column("PREVSTATEREGNUMBER_INVENTORYNUMBER").Length(50);
            this.Property(x => x.PrevStateRegNumberConditionalNumber, "PrevStateRegNumberConditionalNumber").Column("PREVSTATEREGNUMBER_CONDITIONALNUMBER").Length(50);
            this.Property(x => x.OlsonTZCode, "OlsonTZCode").Column("OLSONTZ_CODE").Length(50);
            this.Property(x => x.OlsonTZGuid, "OlsonTZGuid").Column("OLSONTZ_GUID").Length(50);
            this.Property(x => x.CulturalHeritage, "CulturalHeritage").Column("CULTURALHERITAGE");
            this.Property(x => x.BuiltUpArea, "BuiltUpArea").Column("BUILTUPAREA");
            this.Property(x => x.MinFloorCount, "MinFloorCount").Column("MINFLOORCOUNT");
            this.Property(x => x.FloorCount, "FloorCount").Column("FLOORCOUNT").Length(50);
            this.Property(x => x.UndergroundFloorCount, "UndergroundFloorCount").Column("UNDERGROUNDFLOORCOUNT").Length(50);
            this.Property(x => x.OverhaulYear, "OverhaulYear").Column("OVERHAULYEAR");
            this.Property(x => x.OverhaulFormingKindCode, "OverhaulFormingKindCode").Column("OVERHAULFORMING_CODE").Length(50);
            this.Property(x => x.OverhaulFormingKindGuid, "OverhaulFormingKindGuid").Column("OVERHAULFORMING_GUID").Length(50);
            this.Property(x => x.HouseManagementTypeCode, "HouseManagementTypeCode").Column("HOUSEMANAGEMENTTYPE_CODE").Length(50);
            this.Property(x => x.HouseManagementTypeGuid, "HouseManagementTypeGuid").Column("HOUSEMANAGEMENTTYPE_GUID").Length(50);
            this.Property(x => x.ResidentialHouseTypeCode, "ResidentialHouseTypeCode").Column("RESIDENTIALHOUSETYPE_CODE").Length(50);
            this.Property(x => x.ResidentialHouseTypeGuid, "ResidentialHouseTypeGuid").Column("RESIDENTIALHOUSETYPE_GUID").Length(50);
            this.Property(x => x.Adress, "Adress").Column("ADRESS").Length(500);
        }
    }
}