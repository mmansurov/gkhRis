namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;
    using Enums;

    /// <summary>
    /// Дом
    /// </summary>
    public class RisHouse : BaseRisEntity
    {
        /// <summary>
        /// Тип дома
        /// </summary>
        public virtual HouseType HouseType { get; set; }

        /// <summary>
        /// Глобальный уникальный идентификатор дома по ФИАС
        /// </summary>
        public virtual string FiasHouseGuid { get; set; }

        /// <summary>
        /// Кадастровый номер
        /// </summary>
        public virtual string CadastralNumber { get; set; }

        /// <summary>
        /// Общая площадь
        /// </summary>
        public virtual decimal? TotalSquare { get; set; }

        /// <summary>
        /// Общая площадь жилых помещений
        /// </summary>
        public virtual decimal ResidentialSquare { get; set; }

        /// <summary>
        /// Общая площадь нежилых помещений
        /// </summary>
        public virtual decimal NonResidentialSquare { get; set; }

        /// <summary>
        /// Код справочника "Состояние"
        /// </summary>
        public virtual string StateCode { get; set; }

        /// <summary>
        /// Гуид справочника "Состояние"
        /// </summary>
        public virtual string StateGuid { get; set; }

        /// <summary>
        /// Серия проекта
        /// </summary>
        public virtual string ProjectSeries { get; set; }

        /// <summary>
        /// Код справочника "Тип проекта здания"
        /// </summary>
        public virtual string ProjectTypeCode { get; set; }

        /// <summary>
        /// Гуид справочника "Тип проекта здания"
        /// </summary>
        public virtual string ProjectTypeGuid { get; set; }

        /// <summary>
        /// Год постройки
        /// </summary>
        public virtual short? BuildingYear { get; set; }

        /// <summary>
        /// Год ввода в эксплуатацию
        /// </summary>
        public virtual short? UsedYear { get; set; }

        /// <summary>
        /// Общий износ здания
        /// </summary>
        public virtual decimal? TotalWear { get; set; }

        /// <summary>
        /// Дата проведения энергетического обследования
        /// </summary>
        public virtual DateTime? EnergyDate { get; set; }

        /// <summary>
        /// Код справочника "Класс энергетической эффективности"
        /// </summary>
        public virtual string EnergyCategoryCode { get; set; }

        /// <summary>
        /// Гуид справочника "Класс энергетической эффективности"
        /// </summary>
        public virtual string EnergyCategoryGuid { get; set; }

        /// <summary>
        /// Код по ОКТМО
        /// </summary>
        public virtual string OktmoCode { get; set; }

        /// <summary>
        /// Наименование по ОКТМО
        /// </summary>
        public virtual string OktmoName { get; set; }

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
        /// Код справочника "Часовая зона"
        /// </summary>
        public virtual string OlsonTZCode { get; set; }

        /// <summary>
        /// Гуид справочника "Часовая зона"
        /// </summary>
        public virtual string OlsonTZGuid { get; set; }

        /// <summary>
        /// Наличие у дома статуса объекта культурного наследия
        /// </summary>
        public virtual bool CulturalHeritage { get; set; }

        #region Поля многоквартирного дома

        /// <summary>
        /// Площадь застройки
        /// </summary>
        public virtual decimal? BuiltUpArea { get; set; }

        /// <summary>
        /// Количество этажей, наименьшее
        /// </summary>
        public virtual int? MinFloorCount { get; set; }

        /// <summary>
        /// Количество этажей
        /// </summary>
        public virtual string FloorCount { get; set; }

        /// <summary>
        /// Количество подземных этажей
        /// </summary>
        public virtual string UndergroundFloorCount { get; set; }

        /// <summary>
        /// Год последнего капитального ремонта
        /// </summary>
        public virtual short? OverhaulYear { get; set; }

        /// <summary>
        /// Код справочника "Способ формирования фонда капитального ремонта"
        /// </summary>
        public virtual string OverhaulFormingKindCode { get; set; }

        /// <summary>
        /// Гуид справочника "Способ формирования фонда капитального ремонта"
        /// </summary>
        public virtual string OverhaulFormingKindGuid { get; set; }

        /// <summary>
        /// Код справочника "Cпособ управления домом"
        /// </summary>
        public virtual string HouseManagementTypeCode { get; set; }

        /// <summary>
        /// Гуид справочника "Cпособ управления домом"
        /// </summary>
        public virtual string HouseManagementTypeGuid { get; set; }

        #endregion

        #region Поля жилого дома

        /// <summary>
        /// Код справочника "Тип жилого помещения"
        /// </summary>
        public virtual string ResidentialHouseTypeCode { get; set; }

        /// <summary>
        /// Гуид справочника "Тип жилого помещения"
        /// </summary>
        public virtual string ResidentialHouseTypeGuid { get; set; }

        #endregion

        /// <summary>
        /// Адрес
        /// </summary>
        public virtual string Adress { get; set; }
    }
}