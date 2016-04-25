namespace Bars.Gkh.Ris.Map.Services
{
    using Entities.Services;
    using GisIntegration;

    /// <summary>
    /// Маппинг для "Bars.Gkh.Ris.Entities.Services.RisHouseService"
    /// </summary>
    public class WorkListItemMap : BaseRisEntityMap<WorkListItem>
    {
        public WorkListItemMap() :
            base("Bars.Gkh.Ris.Entities.Services.WorkListItem", "RIS_WORKLIST_ITEM")
        {
        }

        protected override void Map()
        {
            this.Reference(x => x.WorkList, "Перечень работ/услуг").Column("WORKLIST_ID").Fetch().NotNull();
            this.Property(x => x.TotalCost, "Общая стоимость").Column("TOTAL_COST");
            this.Reference(x => x.WorkItemNsi, "Ссылка на работу/услугу организации (НСИ 59)").Column("SERVICETYPE_ID").Fetch();
            this.Property(x => x.Index, "Номер строки в перечне работ и услуг").Column("INDEX");
        }
    }
}