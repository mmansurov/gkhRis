namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Подъезд
    /// </summary>
    public class RisEntrance : BaseRisEntity
    {
        /// <summary>
        /// Ссылка на контейнер данных
        /// </summary>
        public virtual RisContainer RisContainer { get; set; }

        /// <summary>
        /// Многоквартирный дом
        /// </summary>
        public virtual RisHouse ApartmentHouse { get; set; }

        /// <summary>
        /// Номер подъезда
        /// </summary>
        public virtual short? EntranceNum { get; set; }

        /// <summary>
        /// Этажность
        /// </summary>
        public virtual short? StoreysCount { get; set; }

        /// <summary>
        /// Дата постройки
        /// </summary>
        public virtual DateTime? CreationDate { get; set; }

        /// <summary>
        /// Дата прекращения существования объекта
        /// </summary>
        public virtual DateTime? TerminationDate { get; set; }
    }
}