namespace Bars.Gkh.Ris.Entities.Bills
{
    /// <summary>
    /// Базовый класс Начисление по услуге
    /// </summary>
    public class BaseRisChargeInfo : BaseRisEntity
    {
        /// <summary>
        /// Код услуги - Код записи справочника
        /// </summary>
        public virtual string ServiceTypeCode { get; set; }

        /// <summary>
        /// Код услуги - Идентификатор в ГИС ЖКХ(ссылка на справочник в ГИС)
        /// </summary>
        public virtual string ServiceTypeGuid { get; set; }

        /// <summary>
        /// Код ОКЕИ
        /// </summary>
        public virtual string Okei { get; set; }

        /// <summary>
        /// Тариф
        /// </summary>
        public virtual decimal Rate { get; set; }

        /// <summary>
        /// Текущие показания приборов учёта коммунальных услуг - индивидульное потребление
        /// </summary>
        public virtual decimal? IndividualConsumptionCurrentValue { get; set; }

        /// <summary>
        /// Текущие показания приборов учёта коммунальных услуг - общедомовые нужды
        /// </summary>
        public virtual decimal? HouseOverallNeedsCurrentValue { get; set; }

        /// <summary>
        /// Суммарный объём коммунальных услуг в доме - индивидульное потребление
        /// </summary>
        public virtual decimal? HouseTotalIndividualConsumption { get; set; }

        /// <summary>
        /// Суммарный объём коммунальных услуг в доме - общедомовые нужды
        /// </summary>
        public virtual decimal? HouseTotalHouseOverallNeeds { get; set; }

        /// <summary>
        /// Норматив потребления коммунальных услуг - общедомовые нужды
        /// </summary>
        public virtual decimal? HouseOverallNeedsNorm { get; set; }

        /// <summary>
        /// Норматив потребления коммунальных услуг - индивидульное потребление
        /// </summary>
        public virtual decimal? IndividualConsumptionNorm { get; set; }

        /// <summary>
        /// Объем услуг - индивидульное потребление
        /// </summary>
        public virtual decimal? IndividualConsumption { get; set; }

        /// <summary>
        /// Объем услуг - общедомовые нужды
        /// </summary>
        public virtual decimal? HouseOverallNeedsConsumption { get; set; }
    }
}
