namespace Bars.Gkh.Ris.Entities
{
    using System;
    using B4.DataAccess;

    public class RisContainer : PersistentObject
    {
        /// <summary>
        /// Для какого контрагента собран контейнер
        /// </summary>
        public virtual RisContragent RisContragent { get; set; }

        /// <summary>
        /// Дата создания контейнера
        /// </summary>
        public virtual DateTime Date { get; set; }

        /// <summary>
        /// Дата отправки контейнера
        /// </summary>
        public virtual DateTime? UploadDate { get; set; }

        /// <summary>
        /// Код метода
        /// </summary>
        public virtual string MethodCode { get; set; }
    }
}