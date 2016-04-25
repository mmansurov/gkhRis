namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using Bars.Gkh.Ris.Enums.HouseManagement;
    using System;

    /// <summary>
    /// Принятые решения
    /// </summary>
    public class RisDecisionList : BaseRisEntity
    {
        /// <summary>
        /// Номер вопроса
        /// </summary>
        public virtual int? QuestionNumber { get; set; }

        /// <summary>
        /// Вопрос
        /// </summary>
        public virtual string QuestionName { get; set; }

        /// <summary>
        /// Код записи справочника
        /// </summary>
        public virtual string DecisionsTypeCode { get; set; }

        /// <summary>
        /// Идентификатор в ГИС ЖКХ
        /// </summary>
        public virtual string DecisionsTypeGuid { get; set; }

        /// <summary>
        /// Результаты голосования «За»
        /// </summary>
        public virtual decimal? Agree { get; set; }

        /// <summary>
        /// Результаты голосования «Против»
        /// </summary>
        public virtual decimal? Against { get; set; }

        /// <summary>
        /// Результаты голосования «Воздержался»
        /// </summary>
        public virtual decimal? Abstent { get; set; }

        /// <summary>
        /// Итог голосования
        /// </summary>
        public virtual RisVotingResume? VotingResume { get; set; }

        /// <summary>
        /// Ссылка на Протокол общего собрания собственников
        /// </summary>
        public virtual RisVotingProtocol VotingProtocol { get; set; }
    }
}
