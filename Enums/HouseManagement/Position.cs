namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Расположение помещения
    /// </summary>
    public enum Position
    {
        /// <summary>
        /// Встроенное
        /// </summary>
        [Display("Встроенное")]
        BuiltIn = 10,

        /// <summary>
        /// Пристроенное
        /// </summary>
        [Display("Пристроенное")]
        Attached = 20
    }
}
