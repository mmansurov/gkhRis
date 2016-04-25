namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using B4.DataAccess;

    /// <summary>
    /// Документ новости
    /// </summary>
    public class RisNotificationAttachment : BaseEntity
    {
        /// <summary>
        /// Новость
        /// </summary>
        public virtual RisNotification Notification { get; set; }

        /// <summary>
        /// Вложение
        /// </summary>
        public virtual Attachment Attachment { get; set; }
    }
}
