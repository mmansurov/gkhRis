namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Тип проведения голосования
    /// </summary>
    public enum RisVotingTimeType
    {
        /// <summary>
        /// Ежегодное собрание
        /// </summary>
        [Display("AnnualVoting")]
        AnnualVoting = 10,

        /// <summary>
        /// Внеочередное собрание
        /// </summary>
        [Display("ExtraVoting")]
        ExtraVoting = 20
    }
}