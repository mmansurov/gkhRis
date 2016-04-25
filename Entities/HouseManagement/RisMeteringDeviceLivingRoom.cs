namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Сущность для реализации отношения "многие-ко-многим" между приборами учета и комнатами
    /// </summary>
    public class RisMeteringDeviceLivingRoom : BaseRisEntity
    {
        /// <summary>
        /// Ссылка на прибор учета
        /// </summary>
        public virtual RisMeteringDeviceData MeteringDeviceData { get; set; }

        /// <summary>
        /// Ссылка на комнату
        /// </summary>
        public virtual LivingRoom LivingRoom { get; set; }
    }
}
