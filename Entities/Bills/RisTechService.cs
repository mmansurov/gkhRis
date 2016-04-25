namespace Bars.Gkh.Ris.Entities.Bills
{
    /// <summary>
    /// Объем потребления по дополнительной услуге
    /// </summary>
    public class RisTechService: BaseRisChargeInfo
    {
        /// <summary>
        /// Ссылка на начисление по дополнительной услуге
        /// </summary>
        public virtual RisAdditionalServiceExtChargeInfo AdditionalServiceExtChargeInfo { get; set; }
    }
}
