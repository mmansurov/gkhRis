namespace Bars.Gkh.Ris.Entities
{
    using Bars.B4.DataAccess;
    using Bars.B4.Modules.FileStorage;
    using Bars.Gkh.Ris.Enums;

    /// <summary>
    /// Сущность, связывающая задачу и пакет данных
    /// </summary>
    public class RisTaskPackage : BaseEntity
    {
        /// <summary>
        /// Ссылка на задачу
        /// </summary>
        public virtual RisTask Task { get; set; }

        /// <summary>
        /// Ссылка на пакет
        /// </summary>
        public virtual RisPackage Package { get; set; }

        /// <summary>
        /// Идентификатор сообщения для получения результата от асинхронного сервиса
        /// </summary>
        public virtual string AckMessageGuid { get; set; }

        /// <summary>
        /// Файл лога с результатом обработки пакета в рамках задачи
        /// </summary>
        public virtual FileInfo ResultLog { get; set; }

        /// <summary>
        /// Статус обработки пакета
        /// </summary>
        public virtual PackageProcessingState State { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public virtual string Message { get; set; }
    }
}
