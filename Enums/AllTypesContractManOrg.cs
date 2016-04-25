namespace Bars.Gkh.Ris.Enums
{
    using Bars.B4.Utils;

    /// <summary>
    /// Тип договора управляющей организации
    /// </summary>
    public enum AllTypesContractManOrg
    {
        /// <summary>
        /// УК
        /// </summary>
        [Display("УК")]
        ManOrg = 10,

        /// <summary>
        /// ТСЖ
        /// </summary>
        [Display("ТСЖ")]
        Tsj = 20,

        /// <summary>
        /// ЖСК
        /// </summary>
        [Display("ЖСК")]
        Jsk = 30,

        /// <summary>
        /// Непосредственное управление
        /// </summary>
        [Display("Непосредственное управление")]
        DirectManag = 40,

        /// <summary>
        /// Прочее
        /// </summary>
        [Display("Прочее")]
        Other = 50,

        /// <summary>
        /// Не выбрано
        /// </summary>
        [Display("Не выбрано")]
        NotSelected = 60
    }
}