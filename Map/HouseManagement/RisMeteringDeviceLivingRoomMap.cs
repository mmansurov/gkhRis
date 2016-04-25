namespace Bars.Gkh.Ris.Map.HouseManagement
{
    using Entities.HouseManagement;

    using GisIntegration;
    public class RisMeteringDeviceLivingRoomMap : BaseRisEntityMap<RisMeteringDeviceLivingRoom>
    {
        public RisMeteringDeviceLivingRoomMap()
            : base("Bars.Gkh.Ris.Entities.HouseManagement.RisMeteringDeviceLivingRoom", "RIS_METERING_DEVICE_LIVING_ROOM")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.LivingRoom, "LivingRoom").Column("LIVING_ROOM_ID").NotNull(); 
            this.Reference(x => x.MeteringDeviceData, "MeteringDeviceData").Column("METERING_DEVICE_ID").NotNull();
        }
    }
}
