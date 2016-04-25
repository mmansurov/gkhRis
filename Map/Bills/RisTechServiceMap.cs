namespace Bars.Gkh.Ris.Map.Bills
{
    using Bars.Gkh.Ris.Entities.Bills;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.Bills.RisTechService
    /// </summary>
    public class RisTechServiceMap : BaseRisChargeInfoMap<RisTechService>
    {
        /// <summary>
        /// Конструктор маппинга сущности Bars.Gkh.Ris.Entities.Bills.RisTechService
        /// </summary>
        public RisTechServiceMap()
            : base("Bars.Gkh.Ris.Entities.Bills.RisTechService", "RIS_TECH_SERVICE")
        {
        }

        /// <summary>
        /// Инициализация маппинга
        /// </summary>
        protected override void Map()
        {
            this.Reference(x => x.AdditionalServiceExtChargeInfo, "AdditionalServiceExtChargeInfo").Column("ADDITIONAL_SERVICE_EXT_CHARGE_INFO_ID");
        }
    }
}
