namespace Bars.Gkh.Ris.Entities.Infrastructure
{
    using B4.DataAccess;

    /// <summary>
    /// Основание управления объектом коммунальной инфраструктуры
    /// </summary>
    public class RisRkiAttachment : BaseEntity
    {
        /// <summary>
        /// Объект коммунальной инфраструктуры
        /// </summary>
        public virtual RisRkiItem RkiItem { get; set; }

        /// <summary>
        /// Ссылка на вложение
        /// </summary>
        public virtual Attachment Attachment { get; set; }
    }
}
