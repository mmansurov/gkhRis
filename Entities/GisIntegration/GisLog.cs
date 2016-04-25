namespace Bars.Gkh.Ris.Entities.GisIntegration
{
    using System;
    using B4.DataAccess;
    using B4.Modules.FileStorage;

    public class GisLog : BaseEntity, IUserEntity
    {
        /// <summary>
        /// ссылка на сервис
        /// </summary>
        public virtual string ServiceLink { get; set; }

        /// <summary>
        /// Имя юзера 
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Код метода 
        /// </summary>
        public virtual string MethodName { get; set; }

        /// <summary>
        /// Дата начала выполнения метода
        /// </summary>
        public virtual DateTime? DateStart { get; set; }

        /// <summary>
        /// Дата окончания выполнения метода
        /// </summary>
        public virtual DateTime? DateEnd { get; set; }

        /// <summary>
        /// Плановое количество объектов
        /// </summary>
        public virtual int CountObjects { get; set; }

        /// <summary>
        /// Количество выполненных объектов
        /// </summary>
        public virtual int ProcessedObjects { get; set; }

        /// <summary>
        /// Процент выполненных объектов
        /// </summary>
        public virtual decimal ProcessedPercent { get; set; }

        /// <summary>
        /// Файл лога 
        /// </summary>
        public virtual FileInfo FileLog { get; set; }
    }
}