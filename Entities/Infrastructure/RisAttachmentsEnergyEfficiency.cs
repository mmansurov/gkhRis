namespace Bars.Gkh.Ris.Entities.Infrastructure
{
    using B4.DataAccess;

    /// <summary>
    /// Документы, подтверждающие соответствие требованиям энергетической эффективности
    /// </summary>
    public class RisAttachmentsEnergyEfficiency : BaseEntity
    {
        /// <summary>
        /// Объект коммунальной инфраструктуры
        /// </summary>
        public virtual RisRkiItem RkiItem { get; set; }

        /// <summary>
        /// Ссылка на вложение
        /// </summary>
        public virtual Attachment Attachment { get; set; }
    }
}
