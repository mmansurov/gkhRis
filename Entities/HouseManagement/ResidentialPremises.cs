namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Жилое помещение
    /// </summary>
    public class ResidentialPremises : BaseRisEntity
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
        /// Номер нежилого помещения
        /// </summary>
        public virtual string PremisesNum { get; set; }

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
        /// Дата прекращения существования объекта
        /// </summary>
        public virtual DateTime? TerminationDate { get; set; }

        /// <summary>
        /// Номер подъезда
        /// </summary>
        public virtual short? EntranceNum { get; set; }

        /// <summary>
        /// Код справочника "Характеристика помещения"
        /// </summary>
        public virtual string PremisesCharacteristicCode { get; set; }

        /// <summary>
        /// Гуид справочника "Характеристика помещения"
        /// </summary>
        public virtual string PremisesCharacteristicGuid { get; set; }

        /// <summary>
        /// Код справочника "Количество комнат"
        /// </summary>
        public virtual string RoomsNumCode { get; set; }

        /// <summary>
        /// Гуид справочника "Количество комнат"
        /// </summary>
        public virtual string RoomsNumGuid { get; set; }

        /// <summary>
        /// Код справочника "Тип жилого помещения"
        /// </summary>
        public virtual string ResidentialHouseTypeCode { get; set; }

        /// <summary>
        /// Гуид справочника "Тип жилого помещения"
        /// </summary>
        public virtual string ResidentialHouseTypeGuid { get; set; }

        /// <summary>
        /// Жилая площадь жилого помещения по паспорту помещения
        /// </summary>
        public virtual decimal? GrossArea { get; set; }

        /// <summary>
        /// Общая площадь жилого помещения по паспорту помещения
        /// </summary>
        public virtual decimal? TotalArea { get; set; }

        /// <summary>
        /// Этаж
        /// </summary>
        public virtual string Floor { get; set; }
    }
}