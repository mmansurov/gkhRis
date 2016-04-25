namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Коммунальная услуга
    /// </summary>
    public class HouseManService : BaseRisEntity
    {
        /// <summary>
        /// Объект управления
        /// </summary>
        public virtual ContractObject ContractObject { get; set; }

        /// <summary>
        /// Код из справочника "Вид коммунальной услуги"
        /// </summary>
        public virtual string ServiceTypeCode { get; set; }

        /// <summary>
        /// Гуид из справочника "Вид коммунальной услуги"
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
        /// Основание (Текущий документ)
        /// </summary>
        public virtual bool BaseServiceCurrentDoc { get; set; }

        /// <summary>
        /// Соглашение
        /// </summary>
        public virtual RisAgreement BaseServiceAgreement { get; set; }
    }
}