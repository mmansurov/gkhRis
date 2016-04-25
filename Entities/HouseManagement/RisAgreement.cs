namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;
    using B4.DataAccess;

    /// <summary>
    /// Соглашение
    /// </summary>
    public class RisAgreement : BaseEntity
    {
        /// <summary>
        /// Вложение
        /// </summary>
        public virtual Attachment Attachment { get; set; }

        /// <summary>
        /// Договор управления
        /// </summary>
        public virtual RisContract Contract { get; set; }

        /// <summary>
        /// Номер соглашения
        /// </summary>
        public virtual string AgreementNumber { get; set; }

        /// <summary>
        /// Дата соглашения
        /// </summary>
        public virtual DateTime? AgreementDate { get; set; }
    }
}
