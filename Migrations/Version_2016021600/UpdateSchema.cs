namespace Bars.Gkh.Ris.Migrations.Version_2016021600
{
    using System.Data;
    using B4.Modules.Ecm7.Framework;

    using Bars.B4.Modules.NH.Migrations.DatabaseExtensions;

    /// <summary>
    /// Миграция модуля
    /// </summary>
    [Migration("2016021600")]
    [MigrationDependsOn(typeof(Version_2016021001.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Метод миграции на версию вперед
        /// </summary>
        public override void Up()
        {
            this.Database.AddColumn("RIS_TASK_PACKAGE", "ACK_MESSAGE_GUID", DbType.String, 100);
            this.Database.AddColumn("RIS_TASK_PACKAGE", new RefColumn("RESULTLOG_ID", "RIS_FILE_RESULTLOG", "B4_FILE_INFO", "ID"));            
        }

        /// <summary>
        /// Метод миграции на версию назад
        /// </summary>
        public override void Down()
        {
            this.Database.RemoveColumn("RIS_TASK_PACKAGE", "ACK_MESSAGE_GUID");
            this.Database.RemoveColumn("RIS_TASK_PACKAGE", "RESULTLOG_ID");
        }
    }
}
