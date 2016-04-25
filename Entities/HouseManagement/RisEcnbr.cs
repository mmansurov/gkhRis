namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Доли обременения
    /// </summary>
    public class RisEcnbr : BaseRisEntity
    {
        /// <summary>
        /// Дата окончания
        /// </summary>
        public virtual DateTime? EndDate { get; set; }

        /// <summary>
        /// Код типа обременения
        /// </summary>
        public virtual string KindCode { get; set; }

        /// <summary>
        /// Идентификатор типа обременения
        /// </summary>
        public virtual string KindGuid { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        public virtual RisContragent RisEcnbrContragent { get; set; }

        /// <summary>
        /// Доля собственности
        /// </summary>
        public virtual RisShare Share { get; set; }
    }
}
