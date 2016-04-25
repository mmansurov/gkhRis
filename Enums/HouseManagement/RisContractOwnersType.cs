namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Тип владельца договора
    /// </summary>
    public enum RisContractOwnersType
    {
        /// <summary>
        /// Собственник объекта жилищного фонда
        /// </summary>
        [Display("Собственник объекта жилищного фонда")]
        Owners = 10,

        /// <summary>
        /// Застройщик
        /// </summary>
        [Display("Застройщик")]
        BuildingOwner = 20,

        /// <summary>
        /// ТСЖ/Кооператив
        /// </summary>
        [Display("ТСЖ/Кооператив")]
        Cooperative = 30,

        /// <summary>
        /// Собственник муниципального жилья 
        /// </summary>
        [Display("Собственник муниципального жилья ")]
        MunicipalHousing = 40
    }
}
