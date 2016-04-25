namespace Bars.Gkh.Ris.Enums
{
    using Bars.B4.Utils;

    /// <summary>
    /// Статус отправки данных
    /// </summary>
    public enum DataSendingState
    {
        /// <summary>
        /// Успешно
        /// </summary>
        [Display("Успешно")]
        Success = 10,

        /// <summary>
        /// С ошибками
        /// </summary>
        [Display("С ошибками")]
        WithErrors = 20,

        /// <summary>
        /// Ошибка
        /// </summary>
        [Display("Ошибка")]
        Error = 30
    }
}
