namespace Bars.Gkh.Ris.Map.Infrastructure
{
    using Entities.Infrastructure;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Infrastructure.RisRkiItem"
    /// </summary>
    public class RisRkiItemMap : BaseRisEntityMap<RisRkiItem>
    {
        public RisRkiItemMap() :
            base("Bars.Gkh.Ris.Entities.Infrastructure.RisRkiItem", "RIS_RKI_ITEM")
        {
        }

        protected override void Map()
        {
            this.Property(x => x.Name, "Name").Column("NAME").Length(200);
            this.Property(x => x.BaseCode, "BaseCode").Column("BASECODE").Length(200);
            this.Property(x => x.BaseGuid, "BaseGuid").Column("BASEGUID").Length(200);
            this.Property(x => x.EndManagmentDate, "EndManagmentDate").Column("ENDMANAGMENTDATE");
            this.Property(x => x.IndefiniteManagement, "IndefiniteManagement").Column("INDEFINITEMANAGEMENT");
            this.Reference(x => x.RisRkiContragent, "Contragent").Column("CONTRAGENT_ID").Fetch();
            this.Property(x => x.Municipalities, "Municipalities").Column("MUNICIPALITIES");
            this.Property(x => x.TypeCode, "TypeCode").Column("TYPECODE").Length(200);
            this.Property(x => x.TypeGuid, "TypeGuid").Column("TYPEGUID").Length(200);
            this.Property(x => x.WaterIntakeCode, "WaterIntakeCode").Column("WATERINTAKECODE").Length(200);
            this.Property(x => x.WaterIntakeGuid, "WaterIntakeGuid").Column("WATERINTAKEGUID").Length(200);
            this.Property(x => x.ESubstationCode, "ESubstationCode").Column("ESUBSTATIONCODE").Length(200);
            this.Property(x => x.ESubstationGuid, "ESubstationGuid").Column("ESUBSTATIONGUID").Length(200);
            this.Property(x => x.PowerPlantCode, "PowerPlantCode").Column("POWERPLANTCODE").Length(200);
            this.Property(x => x.PowerPlantGuid, "PowerPlantGuid").Column("POWERPLANTGUID").Length(200);
            this.Property(x => x.FuelCode, "FuelCode").Column("FUELCODE").Length(200);
            this.Property(x => x.FuelGuid, "FuelGuid").Column("FUELGUID").Length(200);
            this.Property(x => x.GasNetworkCode, "GasNetworkCode").Column("GASNETWORKCODE").Length(200);
            this.Property(x => x.GasNetworkGuid, "GasNetworkGuid").Column("GASNETWORKGUID").Length(200);
            this.Reference(x => x.House, "House").Column("HOUSE_ID").Fetch();
            this.Property(x => x.OktmoCode, "OktmoCode").Column("OKTMOCODE").Length(200);
            this.Property(x => x.OktmoName, "OktmoName").Column("OKTMONAME").Length(200);
            this.Property(x => x.IndependentSource, "IndependentSource").Column("INDEPENDENTSOURCE");
            this.Property(x => x.Deterioration, "Deterioration").Column("DETERIORATION");
            this.Property(x => x.CountAccidents, "CountAccidents").Column("COUNTACCIDENTS");
            this.Property(x => x.AddInfo, "AddInfo").Column("ADDINFO").Length(2000);
        }
    }
}
