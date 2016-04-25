namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Тип прибора учета
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// ИПУ жилого дома
        /// </summary>
        [Display("ИПУ жилого дома")]
        ApartmentHouseDevice = 10,

        /// <summary>
        /// Общеквартирный ПУ
        /// </summary>
        [Display("Общеквартирный ПУ")]
        CollectiveApartmentDevice = 20,

        /// <summary>
        /// Комнатный ИПУ
        /// </summary>
        [Display("Комнатный ИПУ")]
        LivingRoomDevice = 30,

        /// <summary>
        /// ИПУ жилого помещения
        /// </summary>
        [Display("ИПУ жилого помещения")]
        ResidentialPremiseDevice = 40,

        /// <summary>
        /// ИПУ нежилого помещения
        /// </summary>
        [Display("ИПУ нежилого помещения")]
        NonResidentialPremiseDevice = 50,

        /// <summary>
        /// Общедомовой ПУ
        /// </summary>
        [Display("Общедомовой ПУ")]
        CollectiveDevice = 60
    }
}
