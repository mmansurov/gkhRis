namespace Bars.Gkh.Ris.Entities.OrgRegistry
{
    using System;

    /// <summary>
    /// Сведения об обособленном подразделении
    /// </summary>
    public class RisSubsidiary : BaseRisEntity
    {
        /// <summary>
        /// Полное наименование
        /// </summary>
        public virtual string FullName { get; set; }

        /// <summary>
        /// Сокращенное наименование
        /// </summary>
        public virtual string ShortName { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        public virtual string Ogrn { get; set; }

        /// <summary>
        /// ИНН
        /// </summary>
        public virtual string Inn { get; set; }

        /// <summary>
        /// КПП
        /// </summary>
        public virtual string Kpp { get; set; }

        /// <summary>
        /// ОКОПФ
        /// </summary>
        public virtual string Okopf { get; set; }

        /// <summary>
        /// Адрес регистрации
        /// </summary>
        public virtual string Address { get; set; }

        /// <summary>
        /// Адрес регистрации (Глобальный уникальный идентификатор дома по ФИАС)
        /// </summary>
        public virtual string FiasHouseGuid { get; set; }

        /// <summary>
        /// Дата прекращения деятельности
        /// </summary>
        public virtual DateTime? ActivityEndDate { get; set; }

        /// <summary>
        /// Источник информации - Наименование
        /// </summary>
        public virtual string SourceName { get; set; }

        /// <summary>
        /// Источник информации - дата - "от"
        /// </summary>
        public virtual string SourceDate { get; set; }
    }
}
