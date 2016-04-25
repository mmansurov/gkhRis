namespace Bars.Gkh.Ris.Tasks
{
    using System.Collections.Generic;

    using Bars.B4.DataAccess;
    using Bars.Gkh.Ris.Enums;

    /// <summary>
    /// Результат обработки объекта
    /// </summary>
    public class ObjectProcessingResult
    {
        /// <summary>
        /// Идентификатор сущности Ris
        /// </summary>
        public long RisId { get; set; }

        /// <summary>
        /// Идентификатор сущности Gkh
        /// </summary>
        public long GkhId { get; set; }

        /// <summary>
        /// Идентификатор сущности Gis
        /// </summary>
        public string GisId { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public ObjectProcessingState State { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Объекты для сохранения
        /// </summary>
        public List<PersistentObject> ObjectsToSave { get; set; }
    }
}
