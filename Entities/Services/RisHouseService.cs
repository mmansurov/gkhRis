namespace Bars.Gkh.Ris.Entities.Services
{
    using System;
    using HouseManagement;

    /// <summary>
    /// Коммунальная услуга
    /// </summary>
    public class RisHouseService : BaseRisEntity
    {
        /// <summary>
        /// Дом
        /// </summary>
        public virtual RisHouse House { get; set; } 

        /// <summary>
        /// Код справочника "Услуги" (тип "Коммунальные")
        /// </summary>
        public virtual string MsTypeCode { get; set; }

        /// <summary>
        /// Гуид справочника "Услуги" (тип "Коммунальные")
        /// </summary>
        public virtual string MsTypeGuid { get; set; }

        /// <summary>
        /// Дата начала предоставления услуги
        /// </summary>
        public virtual DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания предоставления услуги
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Код типа учета
        /// </summary>
        public virtual string AccountingTypeCode { get; set; }

        /// <summary>
        /// Гуид типа учета
        /// </summary>
        public virtual string AccountingTypeGuid { get; set; }
    }
}