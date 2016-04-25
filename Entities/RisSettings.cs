namespace Bars.Gkh.Ris.Entities
{
    using B4.DataAccess;

    /// <summary>
    /// Сущность для хранения настроек РИС
    /// </summary>
    public class RisSettings : BaseEntity
    {
        /// <summary>
        /// Код раздела настроек
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// Наименование настройки
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public virtual string Value { get; set; }
    }
}