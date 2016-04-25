namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Документы договора
    /// </summary>
    public class RisContractAttachment : BaseRisEntity
    {
        /// <summary>
        /// ДОИ
        /// </summary>
        public virtual RisPublicPropertyContract PublicPropertyContract { get; set; }

        /// <summary>
        /// Вложение
        /// </summary>
        public virtual Attachment Attachment { get; set; }

        /// <summary>
        /// Договор управления
        /// </summary>
        public virtual RisContract Contract { get; set; }
    }
}
