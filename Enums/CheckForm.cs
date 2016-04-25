namespace Bars.Gkh.Ris.Enums
{
    using Bars.B4.Utils;

    /// <summary>
    /// Форма проверки
    /// </summary>
    public enum CheckForm
    {
        /// <summary>
        /// Выездная
        /// </summary>
        [Display("Выездная")]
        Exit = 10,

        /// <summary>
        /// Документарная
        /// </summary>
        [Display("Документарная")]
        Documentation = 20,

        /// <summary>
        /// Выездная и документарная
        /// </summary>
        [Display("Выездная и документарная")]
        ExitAndDocumentation = 30,

        /// <summary>
        /// Визуальное обследование
        /// </summary>
        [Display("Визуальное обследование")]
        Visual = 40
    }
}