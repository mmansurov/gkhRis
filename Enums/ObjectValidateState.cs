namespace Bars.Gkh.Ris.Enums
{
    using Bars.B4.Utils;

    /// <summary>
    /// Статус валидации объекта
    /// </summary>
    public enum ObjectValidateState
    {
        /// <summary>
        /// Успешно
        /// </summary>
        [Display("Успешно")]
        Success = 10,

        /// <summary>
        /// Предупреждение
        /// </summary>
        [Display("Предупреждение")]
        Warning = 20,

        /// <summary>
        /// Ошибка
        /// </summary>
        [Display("Ошибка")]
        Error = 30
    }
}
