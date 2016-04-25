namespace Bars.Gkh.Ris.Enums
{
    using Bars.B4.Utils;

    /// <summary>
    /// Тип основания проверки юр. лиц
    /// </summary>
    public enum TypeBaseCheckJurPerson
    {
        /// <summary>
        /// Не задано
        /// </summary>
        [Display("Не задано")]
        NotSet = 10,

        /// <summary>
        /// Истечение трех лет со дня государственной регистрации ЮЛ/ИП
        /// </summary>
        [Display("Истечение трех лет со дня государственной регистрации ЮЛ/ИП")]
        StateRegistrationAfter3Years = 20,

        /// <summary>
        /// Истечение трех лет со дня окончания проведения последней плановой проверки ЮЛ/ИП
        /// </summary>
        [Display("Истечение трех лет со дня окончания проведения последней плановой проверки ЮЛ/ИП")]
        LastWorkAfter3Years = 30,

        /// <summary>
        /// Истечение трех лет со дня начала осуществления ЮЛ/ИП предпринимательской деятельности
        /// </summary>
        [Display("Истечение трех лет со дня начала осуществления ЮЛ/ИП предпринимательской деятельности")]
        StartBusinessAfter3Years = 40
    }
}