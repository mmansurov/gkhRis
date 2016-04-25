namespace Bars.Gkh.Ris.Enums
{
    using Bars.B4.Utils;

    /// <summary>
    /// Наименование хранилища данных ГИС
    /// </summary>
    public enum FileStorageName
    {
        /// <summary>
        /// Управление домами
        /// </summary>
        [Display("homemanagement")]
        HomeManagement = 10,

        /// <summary>
        /// Реестр коммунальной инфраструктуры
        /// </summary>
        [Display("rki")]
        Rki = 20,

        /// <summary>
        /// Голосования
        /// </summary>
        [Display("voting")]
        Voting = 30,

        /// <summary>
        /// Инспектирование жилищного фонда
        /// </summary>
        [Display("inspection")]
        Inspection = 40,

        /// <summary>
        /// Оповещения
        /// </summary>
        [Display("informing")]
        Informing = 50,

        /// <summary>
        /// Электронные счета
        /// </summary>
        [Display("bills")]
        Bills = 60,

        /// <summary>
        /// Лицензии
        /// </summary>
        [Display("licenses ")]
        Licenses = 70,

        /// <summary>
        /// Договора
        /// </summary>
        [Display("agreements")]
        Agreements = 80,

        /// <summary>
        /// Нормативно-справочная информации
        /// </summary>
        [Display("nsi")]
        Nsi = 90,

        /// <summary>
        /// Коммунальные услуги
        /// </summary>
        [Display("services")]
        Services = 100
    }
}