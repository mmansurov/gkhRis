namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;
    using B4.DataAccess;

    /// <summary>
    /// Документы, подтверждающие полномочия заключать договор
    /// </summary>
    public class RisVotingProtocolAttachment : BaseEntity
    {
        /// <summary>
        /// Ссылка на протокол общего собрания 
        /// </summary>
        public virtual RisVotingProtocol VotingProtocol { get; set; }

        /// <summary>
        /// Ссылка на вложение
        /// </summary>
        public virtual Attachment Attachment { get; set; }
    }
}
