namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Характеристика помещения
    /// </summary>
    public enum PremisesCharacteristic
    {
        /// <summary>
        /// Отдельная квартира
        /// </summary>
        [Display("Отдельная квартира")]
        CertainApartment = 10,

        /// <summary>
        /// Квартира коммунального заселения
        /// </summary>
        [Display("Квартира коммунального заселения")]
        ApartmentOfMunicipalSettling = 20,

        /// <summary>
        /// Общежитие
        /// </summary>
        [Display("Общежитие")]
        Hostel = 30
    }
}
