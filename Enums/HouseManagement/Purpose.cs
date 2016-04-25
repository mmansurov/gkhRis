namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Назначение помещения
    /// </summary>
    public enum Purpose
    {
        /// <summary>
        /// Ветеринарная аптека
        /// </summary>
        [Display("Ветеринарная аптека")]
        VeterinaryDrugstore = 10,

        /// <summary>
        /// Ветеринарная компания
        /// </summary>
        [Display("Ветеринарная компания")]
        VeterinaryCompany = 20,

        /// <summary>
        /// Фармация
        /// </summary>
        [Display("Фармация")]
        Pharmacy = 30
    }
}
