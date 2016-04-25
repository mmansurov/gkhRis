namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Комната в жилом доме
    /// </summary>
    public class LivingRoom : BaseRisEntity
    {
        /// <summary>
        /// Жилое Помещение
        /// </summary>
        public virtual ResidentialPremises ResidentialPremises { get; set; }

        /// <summary>
        /// Жилой дом
        /// </summary>
        public virtual RisHouse House { get; set; }

        /// <summary>
        /// Номер комнаты
        /// </summary>
        public virtual string RoomNumber { get; set; }

        /// <summary>
        /// Площадь
        /// </summary>
        public virtual decimal? Square { get; set; }

        /// <summary>
        /// Дата прекращения существования объекта
        /// </summary>
        public virtual DateTime? TerminationDate { get; set; }

        /// <summary>
        /// Кадастровый номер
        /// </summary>
        public virtual string CadastralNumber { get; set; }

        /// <summary>
        /// Ранее присвоенный государственный учетный номер (Кадастровый номер)
        /// </summary>
        public virtual string PrevStateRegNumberCadastralNumber { get; set; }

        /// <summary>
        /// Ранее присвоенный государственный учетный номер (Инвентарный номер)
        /// </summary>
        public virtual string PrevStateRegNumberInventoryNumber { get; set; }

        /// <summary>
        /// Ранее присвоенный государственный учетный номер (Условный номер)
        /// </summary>
        public virtual string PrevStateRegNumberConditionalNumber { get; set; }

        /// <summary>
        /// Этаж
        /// </summary>
        public virtual string Floor { get; set; }
    }
}