namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;
    using Enums.HouseManagement;

    /// <summary>
    /// Объект управления
    /// </summary>
    public class ContractObject : BaseRisEntity
    {
        /// <summary>
        /// Устав
        /// </summary>
        public virtual Charter Charter { get; set; }

        /// <summary>
        /// Дом
        /// </summary>
        [Obsolete("В системе на момент создания ContractObject ещё не заполнена таблица RisHouse")]
        public virtual RisHouse House { get; set; }

        /// <summary>
        /// Идентификатор дома по FIAS
        /// </summary>
        public virtual string FiasHouseGuid { get; set; }

        /// <summary>
        /// Идентификатор дома
        /// </summary>
        public virtual long RealityObjectId { get; set; }

        /// <summary>
        /// Дата начала предоставления услуг
        /// </summary>
        public virtual DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания предоставления услуг
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Основание (Текущий устав)
        /// </summary>
        public virtual bool BaseMServiseCurrentCharter { get; set; }

        /// <summary>
        /// Основание (Протокол собрания собственников)
        /// </summary>
        public virtual ProtocolMeetingOwner BaseMServiseProtocolMeetingOwner { get; set; }
        
        /// <summary>
        /// Дата исключения
        /// </summary>
        public virtual DateTime? DateExclusion { get; set; }

        /// <summary>
        /// Основание исключения (Текущий устав)
        /// </summary>
        public virtual bool BaseExclusionCurrentCharter { get; set; }

        /// <summary>
        /// Основание исключения (Протокол собрания собственников)
        /// </summary>
        public virtual ProtocolMeetingOwner BaseExclusionProtocolMeetingOwner { get; set; }

        /// <summary>
        /// Договор управления
        /// </summary>
        public virtual RisContract Contract { get; set; }

        /// <summary>
        /// Статус в ГИС ЖКХ
        /// </summary>
        public virtual RisContractObjectStatus? StatusObject { get; set; }

        /// <summary>
        /// Соглашение
        /// </summary>
        public virtual RisAgreement BaseExclusionAgreement { get; set; }

        /// <summary>
        /// Соглашение
        /// </summary>
        public virtual RisAgreement BaseMServiseAgreement { get; set; }

        /// <summary>
        /// День месяца начала ввода показаний по ПУ
        /// </summary>
        public virtual int? PeriodMeteringStartDate { get; set; }

        /// <summary>
        /// День месяца окончания ввода показаний по ПУ
        /// </summary>
        public virtual int? PeriodMeteringEndDate { get; set; }

        /// <summary>
        /// Период ввода показаний по ПУ. Последний день месяца
        /// </summary>
        public virtual bool? PeriodMeteringLastDay { get; set; }

        /// <summary>
        /// Дата выставления платежных документов. День месяца
        /// </summary>
        public virtual int? PaymentDateStartDate { get; set; }

        /// <summary>
        /// Дата выставления платежных документов. Последний день месяца
        /// </summary>
        public virtual bool? PaymentDateLastDay { get; set; }

        /// <summary>
        /// Дата выставления платежных документов. Текущего месяца
        /// </summary>
        public virtual bool? PaymentDateCurrentMounth { get; set; }

        /// <summary>
        /// Дата выставления платежных документов. Следующего месяца
        /// </summary>
        public virtual bool? PaymentDateNextMounth { get; set; }
    }
}