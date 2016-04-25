namespace Bars.Gkh.Ris.Integration
{
    using Enums;

    /// <summary>
    /// Результат валидации объекта
    /// </summary>
    public class ValidateObjectResult
    {
        /// <summary>
        /// Описание объекта
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Статус валидации
        /// </summary>
        public ObjectValidateState State { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }
    }
}
