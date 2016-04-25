namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;
    using B4.DataAccess;

    /// <summary>
    /// Документы, подтверждающие полномочия заключать договор
    /// </summary>
    public class RisTrustDocAttachment : BaseEntity
    {
        /// <summary>
        /// Ссылка на ДОИ
        /// </summary>
        public virtual RisPublicPropertyContract PublicPropertyContract { get; set; }

        /// <summary>
        /// Ссылка на вложение
        /// </summary>
        public virtual Attachment Attachment { get; set; }
    }
}
