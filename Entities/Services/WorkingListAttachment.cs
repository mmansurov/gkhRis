namespace Bars.Gkh.Ris.Entities.Services
{
    using B4.DataAccess;

    /// <summary>
    /// Основание перечня работ/услуг
    /// </summary>
    public class WorkingListAttachment : BaseEntity
    {
        /// <summary>
        /// Перечень работ/услуг
        /// </summary>
        public virtual WorkList WorkList { get; set; }

        /// <summary>
        /// Файл-вложение
        /// </summary>
        public virtual Attachment Attachment { get; set; }
    }
}