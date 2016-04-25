namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Адресат новости
    /// </summary>
    public class RisNotificationAddressee : BaseRisEntity
    {
        /// <summary>
        /// Новость
        /// </summary>
        public virtual RisNotification Notification { get; set; }

        /// <summary>
        /// Адресат (дом)
        /// </summary>
        public virtual RisHouse House { get; set; }
    }
}
