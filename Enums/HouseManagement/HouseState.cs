namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Состояние дома
    /// </summary>
    public enum HouseState
    {
        /// <summary>
        /// Аварийный
        /// </summary>
        [Display("Аварийный")]
        Emergency = 10,

        /// <summary>
        /// Ветхий
        /// </summary>
        [Display("Ветхий")]
        Dilapidated = 20,

        /// <summary>
        /// Исправный
        /// </summary>
        [Display("Исправный")]
        Serviceable = 30,

        /// <summary>
        /// Снесен
        /// </summary>
        [Display("Снесен")]
        Razed = 40
    }
}