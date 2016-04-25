namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Тип прибора учета
    /// </summary>
    public enum MeteringDeviceType
    {
        /// <summary>
        /// ПУ расхода электроэнергии
        /// </summary>
        [Display("ПУ расхода электроэнергии")]
        ElectricMeteringDevice = 10,

        /// <summary>
        /// Однотарифный ПУ
        /// </summary>
        [Display("Однотарифный ПУ")]
        OneRateMeteringDevice = 20
    }
}
