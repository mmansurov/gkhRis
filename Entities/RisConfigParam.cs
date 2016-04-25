namespace Bars.Gkh.Ris.Entities
{
    using Bars.B4.DataAccess;

    /// <summary>
    /// Параметр конфигурации
    /// </summary>
    public class RisConfigParam : BaseEntity
    {
        /// <summary>
        /// Имя параметра
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public virtual string Value { get; set; }
    }
}