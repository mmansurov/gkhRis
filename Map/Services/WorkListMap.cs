namespace Bars.Gkh.Ris.Map.Services
{
    using Entities.Services;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Services.RisHouseService"
    /// </summary>
    public class WorkListMap : BaseRisEntityMap<WorkList>
    {
        public WorkListMap() :
            base("Bars.Gkh.Ris.Entities.Services.WorkList", "RIS_WORKLIST")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.Contract, "Договор управления").Column("CONTRACT_ID").Fetch();
            this.Reference(x => x.House, "Дом").Column("HOUSE_ID").Fetch();
            this.Property(x => x.MonthFrom, "Период с (месяц)").Column("MONTH_FROM");
            this.Property(x => x.YearFrom, "Период с (год)").Column("YEAR_FROM");
            this.Property(x => x.MonthTo, "Период по (месяц)").Column("MONTH_TO");
            this.Property(x => x.YearTo, "Период по (год)").Column("YEAR_TO");
        }
    }
}