namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Вложения договоров управления
    /// </summary>
    public class RisProtocolOk : BaseRisEntity
    {
        /// <summary>
        /// Договор управления
        /// </summary>
        public virtual RisContract Contract { get; set; }

        /// <summary>
        /// Вложение
        /// </summary>
        public virtual Attachment Attachment { get; set; }
    }
}
