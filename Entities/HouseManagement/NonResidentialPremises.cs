namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Нежилое помещение
    /// </summary>
    public class NonResidentialPremises : BaseRisEntity
    {
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
        /// Этаж
        /// </summary>
        public virtual string Floor { get; set; }

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
        /// Код справочника "Назначение помещения"
        /// </summary>
        public virtual string PurposeCode { get; set; }

        /// <summary>
        /// Гуид справочника "Назначение помещения"
        /// </summary>
        public virtual string PurposeGuid { get; set; }

        /// <summary>
        /// Код справочника "Расположение"
        /// </summary>
        public virtual string PositionCode { get; set; }

        /// <summary>
        /// Гуид справочника "Расположение"
        /// </summary>
        public virtual string PositionGuid { get; set; }

        /// <summary>
        /// Жилая площадь нежилого помещения по паспорту помещения
        /// </summary>
        public virtual decimal? GrossArea { get; set; }

        /// <summary>
        /// Общая площадь нежилого помещения по паспорту помещения
        /// </summary>
        public virtual decimal? TotalArea { get; set; }

        /// <summary>
        /// Помещение, составляющее общее имущество в многоквартирном доме
        /// </summary>
        public virtual bool IsCommonProperty { get; set; }
    }
}