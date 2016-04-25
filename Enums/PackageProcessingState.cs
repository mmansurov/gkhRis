namespace Bars.Gkh.Ris.Enums
{
    using Bars.B4.Utils;

    /// <summary>
    /// Статус обработки пакета
    /// </summary>
    public enum PackageProcessingState
    {
        /// <summary>
        /// Ожидает обработки
        /// </summary>
        [Display("Ожидает обработки")]
        Waiting = 10,

        /// <summary>
        /// Успешно
        /// </summary>
        [Display("Успешно")]
        Success = 20,

        /// <summary>
        /// Ошибка получения состояния
        /// </summary>
        [Display("Ошибка получения состояния")]
        GettingStateError = 30,

        /// <summary>
        /// Ошибка обработки результата
        /// </summary>
        [Display("Ошибка обработки результата")]
        ProcessingResultError = 40,

        /// <summary>
        /// Ошибка превышения таймаута
        /// Превышено максимальное количество попыток получения результата асинхронного запроса
        /// </summary>
        [Display("Ошибка превышения таймаута")]
        TimeoutError = 50
    }
}
