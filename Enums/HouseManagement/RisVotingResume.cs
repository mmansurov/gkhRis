namespace Bars.Gkh.Ris.Enums.HouseManagement
{
    using Bars.B4.Utils;

    /// <summary>
    /// Тип голосования
    /// </summary>
    public enum RisVotingResume
    {
        /// <summary>
        /// Решение принято 
        /// </summary>
        [Display("DECISION_IS_MADE")]
        M = 10,

        /// <summary>
        /// Решение не принято
        /// </summary>
        [Display("DECISION_IS_NOT_MADE")]
        N = 20
    }
}