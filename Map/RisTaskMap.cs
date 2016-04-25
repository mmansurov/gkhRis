namespace Bars.Gkh.Ris.Map
{
    using Bars.B4.Modules.Mapping.Mappers;
    using Bars.Gkh.Ris.Entities;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.RisTaskMap
    /// </summary>
    public class RisTaskMap: BaseEntityMap<RisTask>
    {
        /// <summary>
        /// Конструктор маппинга 
        /// </summary>
        public RisTaskMap() :
            base("Bars.Gkh.Ris.Entities.RisTaskMap", "RIS_TASK")
        {
        }

        /// <summary>
        /// Инициализация маппинга
        /// </summary>
        protected override void Map()
        {
            this.Property(x => x.ClassName, "ClassName").Column("CLASS_NAME").Length(200);
            this.Property(x => x.Description, "Description").Column("DESCRIPTION").Length(500);
            this.Property(x => x.QuartzTriggerKey, "QuartzTriggerKey").Column("QRTZ_TRIGGER_KEY").Length(200);
            this.Property(x => x.MaxRepeatCount, "MaxRepeatCount").Column("MAX_REPEAT_COUNT");
            this.Property(x => x.Interval, "Interval").Column("INTERVAL");
            this.Property(x => x.StartTime, "StartTime").Column("START_TIME");
            this.Property(x => x.EndTime, "EndTime").Column("END_TIME");
            this.Property(x => x.UserName, "UserName").Column("USER_NAME").Length(200);
        }
    }
}
