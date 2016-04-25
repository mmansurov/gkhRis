namespace Bars.Gkh.Ris.Entities.DeviceMetering
{
    using System;

    using Bars.Gkh.Ris.Entities.HouseManagement;

    /// <summary>
    /// Текущие показания ПУ
    /// </summary>
    public class RisMeteringDeviceCurrentValue : BaseRisEntity
    {
        /// <summary>
        /// Ссылка на данные ПУ
        /// </summary>
        public virtual RisMeteringDeviceData MeteringDeviceData { get; set; }

        /// <summary>
        /// Ссылка на счет
        /// </summary>
        public virtual RisAccount Account { get; set; }

        /// <summary>
        /// Показание по тарифу T1
        /// </summary>
        public virtual decimal ValueT1 { get; set; }

        /// <summary>
        /// Показание по тарифу T2
        /// </summary>
        public virtual decimal? ValueT2 { get; set; }

        /// <summary>
        /// Показание по тарифу T3
        /// </summary>
        public virtual decimal? ValueT3 { get; set; }

        /// <summary>
        /// Время и дата снятия показания
        /// </summary>
        public virtual DateTime ReadoutDate { get; set; }

        /// <summary>
        /// Кем внесено
        /// </summary>
        public virtual string ReadingsSource { get; set; }
    }
}
