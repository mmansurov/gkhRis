namespace Bars.Gkh.Ris.Map.DeviceMetering
{
    using Bars.Gkh.Ris.Entities.DeviceMetering;
    using Bars.Gkh.Ris.Map.GisIntegration;

    public class RisMeteringDeviceVerificationValueMap : BaseRisEntityMap<RisMeteringDeviceVerificationValue>
    {
        public RisMeteringDeviceVerificationValueMap()
            : base(
                "Bars.Gkh.Ris.Map.DeviceMetering.RisMeteringDeviceVerificationValue",
                "RIS_METERING_DEVICE_VERIFICATION_VALUE")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.MeteringDeviceData, "MeteringDeviceData").Column("METERING_DEVICE_DATA_ID");
            this.Reference(x => x.Account, "Account").Column("ACCOUNT_ID");

            this.Property(x => x.StartVerificationValueT1, "StartVerificationValueT1").Column("START_VERIFICATION_VALUE_T1");
            this.Property(x => x.StartVerificationValueT2, "StartVerificationValueT2").Column("START_VERIFICATION_VALUE_T2");
            this.Property(x => x.StartVerificationValueT3, "StartVerificationValueT3").Column("START_VERIFICATION_VALUE_T3");
            this.Property(x => x.StartVerificationReadoutDate, "StartVerificationReadoutDate").Column("START_VERIFICATION_READOUT_DATE");
            this.Property(x => x.StartVerificationReadingsSource, "StartVerificationReadingsSource").Column("START_VERIFICATION_READING_SOURCE");

            this.Property(x => x.EndVerificationValueT1, "EndVerificationValueT1").Column("END_VERIFICATION_VALUE_T1");
            this.Property(x => x.EndVerificationValueT2, "EndVerificationValueT2").Column("END_VERIFICATION_VALUE_T2");
            this.Property(x => x.EndVerificationValueT3, "EndVerificationValueT3").Column("END_VERIFICATION_VALUE_T3");
            this.Property(x => x.EndVerificationReadoutDate, "EndVerificationReadoutDate").Column("END_VERIFICATION_READOUT_DATE");
            this.Property(x => x.EndVerificationReadingsSource, "EndVerificationReadingsSource").Column("END_VERIFICATION_READING_SOURCE");

            this.Property(x => x.PlannedVerification, "PlannedVerification").Column("PLANNED_VERIFICATION");

            this.Property(x => x.VerificationReasonCode, "VerificationReasonCode").Column("VERIFICATION_REASON_CODE");
            this.Property(x => x.VerificationReasonGuid, "VerificationReasonGuid").Column("VERIFICATION_REASON_GUID");
            this.Property(x => x.VerificationReasonName, "VerificationReasonName").Column("VERIFICATION_REASON_NAME");
        }
    }
}
