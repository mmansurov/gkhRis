namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using Bars.Gkh.Ris.Enums.HouseManagement;
    using System;

    /// <summary>
    /// Протокол голосования жильцов
    /// </summary>
    public class RisVotingProtocol : BaseRisEntity
    {
        /// <summary>
        /// Дом
        /// </summary>
        public virtual RisHouse House { get; set; }

        /// <summary>
        /// Номер протокола
        /// </summary>
        public virtual string ProtocolNum { get; set; }

        /// <summary>
        /// Дата составления протокола
        /// </summary>
        public virtual DateTime? ProtocolDate { get; set; }

        /// <summary>
        /// Место проведения собрания
        /// </summary>
        public virtual string VotingPlace { get; set; }

        /// <summary>
        /// Дата начала проведения голосования
        /// </summary>
        public virtual DateTime? BeginDate { get; set; }

        /// <summary>
        /// Дата окончания голосования
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Порядок приема оформленных в письменной форме решений собственников
        /// </summary>
        public virtual string Discipline { get; set; }

        /// <summary>
        /// Правомочность собрания
        /// </summary>
        public virtual RisMeetingEligibility? MeetingEligibility { get; set; }

        /// <summary>
        /// Тип голосования
        /// </summary>
        public virtual RisVotingType? VotingType { get; set; }

        /// <summary>
        /// Тип проведения голосования
        /// </summary>
        public virtual RisVotingTimeType? VotingTimeType { get; set; }

        /// <summary>
        /// Разместить протокол
        /// </summary>
        public virtual bool? Placing { get; set; }

        /// <summary>
        /// Отменить последние изменения
        /// </summary>
        public virtual bool? Revert { get; set; }
    }
}
