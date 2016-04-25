namespace Bars.Gkh.Ris.Enums
{
    using Bars.B4.Utils;

    /// <summary>
    /// Статус задачи
    /// </summary>
    public enum TaskState
    {
        /// <summary>
        /// Ожидает выполнения
        /// </summary>
        [Display("Ожидает выполнения")]
        Waiting = 10,

        /// <summary>
        /// Выполняется
        /// </summary>
        [Display("Выполняется")]
        Processing = 20,

        /// <summary>
        /// Выполнение приостановлено
        /// </summary>
        [Display("Выполнение приостановлено")]
        Paused = 30,

        /// <summary>
        /// Выполнена успешно
        /// </summary>
        [Display("Выполнена успешно")]
        CompleteSuccess = 40,

        /// <summary>
        /// Выполнена c ошибками
        /// </summary>
        [Display("Выполнена c ошибками")]
        CompleteWithErrors = 50,

        /// <summary>
        /// Ошибка
        /// </summary>
        [Display("Ошибка")]
        Error = 60
    }
}
