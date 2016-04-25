namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Новости
    /// </summary>
    public class RisNotification : BaseRisEntity
    {
        /// <summary>
        /// Тема
        /// </summary>
        public virtual string Topic { get; set; }

        /// <summary>
        /// Высокая важность новости
        /// </summary>
        public virtual bool? IsImportant { get; set; }

        /// <summary>
        /// Текст новости
        /// </summary>
        public virtual string Content { get; set; }

        /// <summary>
        /// Все дома (в адресатах)
        /// </summary>
        public virtual bool? IsAll { get; set; }

        /// <summary>
        /// Не ограничено
        /// </summary>
        public virtual bool? IsNotLimit { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public virtual DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Направить новость адресатам
        /// </summary>
        public virtual bool? IsShipOff { get; set; }

        /// <summary>
        /// Новость удалена
        /// </summary>
        public virtual bool? Deleted { get; set; }
    }
}
