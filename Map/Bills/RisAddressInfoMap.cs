namespace Bars.Gkh.Ris.Map.Bills
{
    using Bars.Gkh.Ris.Entities.Bills;
    using GisIntegration;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.Bills.RisAddressInfo
    /// </summary>
    public class RisAddressInfoMap : BaseRisEntityMap<RisAddressInfo>
    {
        /// <summary>
        /// Конструктор маппинга сущности Bars.Gkh.Ris.Entities.Bills.RisAddressInfo
        /// </summary>
        public RisAddressInfoMap() :
            base("Bars.Gkh.Ris.Entities.Bills.RisAddressInfo", "RIS_ADDRESS_INFO")
        {
        }

        /// <summary>
        /// Инициализация маппинга
        /// </summary>
        protected override void Map()
        {
            this.Property(x => x.LivingPersonsNumber, "LivingPersonsNumber").Column("LIVING_PERSON_NUMBER");
            this.Property(x => x.ResidentialSquare, "ResidentialSquare").Column("RESIDENTIAL_SQUARE");
            this.Property(x => x.HeatedArea, "HeatedArea").Column("HEATED_AREA");
            this.Property(x => x.TotalSquare, "TotalSquare").Column("TOTAL_SQUARE");
        }
    }
}
