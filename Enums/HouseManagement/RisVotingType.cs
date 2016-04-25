namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Тип голосования
    /// </summary>
    public enum RisVotingType
    {
        /// <summary>
        /// Заочное голосование
        /// </summary>
        [Display("AVoting")]
        AVoting = 10,

        /// <summary>
        /// Электронное голосование
        /// </summary>
        [Display("EVoting")]
        EVoting = 20,

        /// <summary>
        /// Собрание
        /// </summary>
        [Display("Meeting")]
        Meeting = 30
    }
}