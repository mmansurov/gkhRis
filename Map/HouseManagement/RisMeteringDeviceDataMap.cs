namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;
    using GisIntegration;

    public class RisMeteringDeviceDataMap : BaseRisEntityMap<RisMeteringDeviceData>
    {
        public RisMeteringDeviceDataMap()
            : base("Bars.Gkh.Ris.Entities.HouseManagement.RisMeteringDeviceData", "RIS_METERING_DEVICE_DATA")
        {
        }

        protected override void Map()
        {
            this.Property(x => x.MeteringDeviceType, "MeteringDeviceType").Column("METERING_DEVICE_TYPE");
            this.Property(x => x.MeteringDeviceNumber, "MeteringDeviceNumber").Column("METERING_DEVICE_NUMBER").Length(50);
            this.Property(x => x.MeteringDeviceStamp, "MeteringDeviceStamp").Column("METERING_DEVICE_STAMP").Length(50);
            this.Property(x => x.InstallationDate, "InstallationDate").Column("INSTALLATION_DATE");
            this.Property(x => x.CommissioningDate, "CommissioningDate").Column("COMMISSIONING_DATE");
            this.Property(x => x.ManualModeMetering, "ManualModeMetering").Column("MANUAL_MODE_METERING");
            this.Property(x => x.FirstVerificationDate, "FirstVerificationDate").Column("FIRST_VERIFICATION_DATE");
            this.Property(x => x.VerificationInterval, "VerificationInterval").Column("VERIFICATION_INTERVAL").Length(50);
            this.Property(x => x.DeviceType, "DeviceType").Column("DEVICE_TYPE");                      
            this.Property(x => x.MeteringValueT1, "MeteringValueT1")
                .Column("METERING_VALUE_T1");
            this.Property(x => x.MeteringValueT2, "MeteringValueT2")
                .Column("METERING_VALUE_T2");
            this.Property(x => x.MeteringValueT3, "MeteringValueT3")
                .Column("METERING_VALUE_T3");
            this.Property(x => x.ReadoutDate, "ReadoutDate").Column("READOUT_DATE");
            this.Property(x => x.ReadingsSource, "ReadingsSource").Column("READINGS_SOURCE").Length(50);        
            this.Reference(x => x.House, "House").Column("HOUSE_ID");
            this.Reference(x => x.ResidentialPremises, "ResidentialPremises").Column("RESIDENTIAL_PREMISES_ID");
            this.Reference(x => x.NonResidentialPremises, "NonResidentialPremises").Column("NONRESIDENTIAL_PREMISES_ID");
            this.Property(x => x.MunicipalResourceCode, "MunicipalResourceCode").Column("MUNICIPAL_RESOURCE_CODE").Length(50);
            this.Property(x => x.MunicipalResourceGuid, "MunicipalResourceGuid").Column("MUNICIPAL_RESOURCE_GUID").Length(50);
        }
    }
}

