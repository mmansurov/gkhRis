namespace Bars.Gkh.Ris.Entities
{
    using System;

    using Bars.B4.DataAccess;

    /// <summary>
    /// Задача
    /// </summary>
    public class RisTask : BaseEntity, IUserEntity
    {
        /// <summary>
        /// Имя класса, реализующего выполнение задачи
        /// </summary>
        public virtual string ClassName { get; set; }

        /// <summary>
        /// Описание задачи
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Ключ объекта Trigger в хранилище планировщика задач
        /// </summary>
        public virtual string QuartzTriggerKey { get; set; }

        /// <summary>
        /// Максимальное количество повторов
        /// </summary>
        public virtual int MaxRepeatCount { get; set; }

        /// <summary>
        /// Интервал в секундах
        /// </summary>
        public virtual int Interval { get; set; }

        /// <summary>
        /// Время начала выполнения
        /// </summary>
        public virtual DateTime StartTime { get; set; }

        /// <summary>
        /// Время окончания выполнения
        /// </summary>
        public virtual DateTime EndTime { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public virtual string UserName { get; set; }
    }
}
