namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Список ДОИ
    /// </summary>
    public class RisPublicPropertyContract : BaseRisEntity
    {
        /// <summary>
        /// Номер протокола
        /// </summary>
        public virtual string ProtocolNumber { get; set; }

        /// <summary>
        /// Дата протокола
        /// </summary>
        public virtual DateTime? ProtocolDate { get; set; }

        /// <summary>
        /// Дата начала действия
        /// </summary>
        public virtual DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания действия
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Номер договора
        /// </summary>
        public virtual string ContractNumber { get; set; }

        /// <summary>
        /// Предмет договора
        /// </summary>
        public virtual string ContractObject { get; set; }

        /// <summary>
        /// Комментарий
        /// </summary>
        public virtual string Comments { get; set; }

        /// <summary>
        /// Дата подписания
        /// </summary>
        public virtual DateTime? DateSignature { get; set; }

        /// <summary>
        /// Документ подписан
        /// </summary>
        public virtual bool? IsSignatured { get; set; }

        /// <summary>
        /// Дом
        /// </summary>
        public virtual RisHouse House { get; set; }

        /// <summary>
        /// Физическое лицо
        /// </summary>
        public virtual RisInd Entrepreneur { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        public virtual RisContragent Organization { get; set; }
    }
}
