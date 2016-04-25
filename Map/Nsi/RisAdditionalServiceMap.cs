namespace Bars.Gkh.Ris.Map.Nsi
{
    using Bars.Gkh.Ris.Entities.Nsi;
    using Bars.Gkh.Ris.Map.GisIntegration;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.Nsi.RisAdditionalService
    /// </summary>
    public class RisAdditionalServiceMap : BaseRisEntityMap<RisAdditionalService>
    {
        /// <summary>
        /// Конструктор маппинга сущности Bars.Gkh.Ris.Entities.Nsi.RisAdditionalService
        /// </summary>
        public RisAdditionalServiceMap()
            : base("Bars.Gkh.Ris.Entities.Nsi.RisAdditionalService", "RIS_ADDITIONAL_SERVICE")
        {
        }

        /// <summary>
        /// Инициализация маппинга
        /// </summary>
        protected override void Map()
        {
            this.Property(x => x.AdditionalServiceTypeName, "AdditionalServiceTypeName").Column("ADDITIONAL_SERVICE_TYPE_NAME").Length(100);
            this.Property(x => x.Okei, "Okei").Column("OKEI").Length(3);
            this.Property(x => x.StringDimensionUnit, "StringDimensionUnit").Column("STRING_DIMENSION_UNIT").Length(100);
        }
    }
}
