namespace Bars.Gkh.Ris.Map.DeviceMetering
{
    using Bars.Gkh.Ris.Entities.DeviceMetering;
    using Bars.Gkh.Ris.Map.GisIntegration;

    public class RisMeteringDeviceCurrentValueMap : BaseRisEntityMap<RisMeteringDeviceCurrentValue>
    {
        public RisMeteringDeviceCurrentValueMap()
            : base("Bars.Gkh.Ris.Entities.DeviceMetering.RisMeteringDeviceCurrentValue", "RIS_METERING_DEVICE_CURRENT_VALUE")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.MeteringDeviceData, "MeteringDeviceData").Column("METERING_DEVICE_DATA_ID");
            this.Reference(x => x.Account, "Account").Column("ACCOUNT_ID");
            this.Property(x => x.ValueT1, "ValueT1").Column("VALUE_T1").NotNull();
            this.Property(x => x.ValueT2, "ValueT2").Column("VALUE_T2");
            this.Property(x => x.ValueT3, "ValueT3").Column("VALUE_T3");
            this.Property(x => x.ReadoutDate, "ReadoutDate").Column("READOUT_DATE").NotNull();
            this.Property(x => x.ReadingsSource, "ReadingsSource").Column("READING_SOURCE").NotNull();
        }
    }
}
