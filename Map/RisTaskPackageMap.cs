namespace Bars.Gkh.Ris.Map
{
    using Bars.B4.Modules.Mapping.Mappers;
    using Bars.Gkh.Ris.Entities;

    /// <summary>
    /// Маппинг сущности Bars.Gkh.Ris.Entities.RisTaskPackage
    /// </summary>
    public class RisTaskPackageMap : BaseEntityMap<RisTaskPackage>
    {
        /// <summary>
        /// Конструктор маппинга
        /// </summary>
        public RisTaskPackageMap() :
            base("Bars.Gkh.Ris.Entities.RisTaskPackage", "RIS_TASK_PACKAGE")
        {
        }

        /// <summary>
        /// Инициализация маппинга
        /// </summary>
        protected override void Map()
        {
            this.Reference(x => x.Task, "Task").Column("TASK_ID").Fetch();
            this.Reference(x => x.Package, "Package").Column("PACKAGE_ID").Fetch();
            this.Property(x => x.AckMessageGuid, "AckMessageGuid").Column("ACK_MESSAGE_GUID").Length(100);
            this.Reference(x => x.ResultLog, "ResultLog").Column("RESULTLOG_ID").Fetch();
            this.Property(x => x.State, "State").Column("STATE");
            this.Property(x => x.Message, "Message").Column("MESSAGE").Length(500);
        }
    }
}
