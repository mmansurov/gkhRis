namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Сведения об инициаторах собрания
    /// </summary>
    public class RisVoteInitiators : BaseRisEntity
    {
        /// <summary>
        /// Физическое лицо
        /// </summary>
        public virtual RisInd Ind { get; set; }

        /// <summary>
        /// Организация инициатор собрания ИП или Юр. лицо
        /// </summary>
        public virtual RisContragent Org { get; set; }

        /// <summary>
        /// Ссылка на протокол общего собрания
        /// </summary>
        public virtual RisVotingProtocol VotingProtocol { get; set; }
    }
}
