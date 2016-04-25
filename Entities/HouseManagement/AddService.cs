namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Дополнительная услуга
    /// </summary>
    public class AddService : BaseRisEntity
    {
        /// <summary>
        /// Объект управления
        /// </summary>
        public virtual ContractObject ContractObject { get; set; }

        /// <summary>
        /// Код из справочника "Дополнительные услуги"
        /// </summary>
        public virtual string ServiceTypeCode { get; set; }

        /// <summary>
        /// Гуид из справочника "Дополнительные услуги"
        /// </summary>
        public virtual string ServiceTypeGuid { get; set; }

        /// <summary>
        /// Дата начала предоставления услуги
        /// </summary>
        public virtual DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания предоставления услуги
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Основание (Текущий устав)
        /// </summary>
        public virtual bool BaseServiceCurrentDoc { get; set; }

        /// <summary>
        /// Основание (Протокол собрания собственников)
        /// </summary>
        public virtual ProtocolMeetingOwner BaseServiceCharterProtocolMeetingOwner { get; set; }

        /// <summary>
        /// Соглашение
        /// </summary>
        public virtual RisAgreement BaseServiceAgreement { get; set; }
    }
}