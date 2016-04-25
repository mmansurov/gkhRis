namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Сущность для реализации отношения "многие-ко-многим" между приборами учета и счетами
    /// </summary>
    public class RisMeteringDeviceAccount : BaseRisEntity
    {
        /// <summary>
        /// Ссылка на прибор учета
        /// </summary>
        public virtual RisMeteringDeviceData MeteringDeviceData { get; set; }

        /// <summary>
        /// Ссылка на счет
        /// </summary>
        public virtual RisAccount Account { get; set; }
    }
}
