namespace Bars.Gkh.Ris.Map.Bills
{
    using Bars.Gkh.Ris.Entities.Bills;
    using Bars.Gkh.Ris.Map.GisIntegration;

    /// <summary>
    /// Базовый маппинг для BaseRisChargeInfo сущностей
    /// </summary>
    /// <typeparam name="TBaseEntity">Типсущности</typeparam>
    public abstract class BaseRisChargeInfoMap<TBaseEntity> : BaseRisEntityMap<TBaseEntity> where TBaseEntity : BaseRisChargeInfo
    {
        /// <summary>
        /// Конструктор маппинга сущности BaseRisChargeInfo
        /// </summary>
        /// <param name="entityName">Полное имя типа</param>
        /// <param name="tableName">Название таблицы</param>
        protected BaseRisChargeInfoMap(string entityName, string tableName)
            : base(entityName, tableName)
        {
        }

        /// <summary>
        /// Инициализация маппинга
        /// </summary>
        public override void InitMap()
        {
            base.InitMap();

            this.Property(x => x.ServiceTypeCode, "Код услуги - Код записи справочника").Column("SERVICE_TYPE_CODE").Length(20);
            this.Property(x => x.ServiceTypeGuid, "Код услуги - Идентификатор в ГИС ЖКХ(ссылка на справочник в ГИС)").Column("SERVICE_TYPE_GUID").Length(50);
            this.Property(x => x.Okei, "Код ОКЕИ").Column("OKEI").Length(3);
            this.Property(x => x.Rate, "Тариф").Column("RATE");
            this.Property(x => x.IndividualConsumptionCurrentValue, "Текущие показания приборов учёта коммунальных услуг - индивидульное потребление").Column("INDIVIDUAL_CONSUMPTION_CURRENT_VALUE");
            this.Property(x => x.HouseOverallNeedsCurrentValue, "Текущие показания приборов учёта коммунальных услуг - общедомовые нужды").Column("HOUSE_OVERALL_NEEDS_CURRENT_VALUE");
            this.Property(x => x.HouseTotalIndividualConsumption, "Суммарный объём коммунальных услуг в доме - индивидульное потребление").Column("HOUSE_TOTAL_INDIVIDUAL_CONSUMPTION");
            this.Property(x => x.HouseTotalHouseOverallNeeds, "Суммарный объём коммунальных услуг в доме - общедомовые нужды").Column("HOUSE_TOTAL_OVERALL_NEEDS");
            this.Property(x => x.HouseOverallNeedsNorm, "Норматив потребления коммунальных услуг - общедомовые нужды").Column("HOUSE_OVERALL_NEEDS_NORM");
            this.Property(x => x.IndividualConsumptionNorm, "Норматив потребления коммунальных услуг - индивидульное потребление").Column("INDIVIDUAL_CONSUMPTION_NORM");
            this.Property(x => x.IndividualConsumption, "Объем услуг - индивидульное потребление").Column("INDIVIDUAL_CONSUMPTION");
            this.Property(x => x.HouseOverallNeedsConsumption, "Объем услуг - общедомовые нужды").Column("HOUSE_OVERALL_NEEDS_CONSUMPTION");
        }
    }
}
