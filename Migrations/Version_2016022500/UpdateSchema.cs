namespace Bars.Gkh.Ris.Migrations.Version_2016022500
{
    using System.Data;
    using B4.Modules.Ecm7.Framework;

    /// <summary>
    /// Миграция модуля
    /// </summary>
    [Migration("2016022500")]
    [MigrationDependsOn(typeof(Version_2016022400.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Метод миграции на версию вперед
        /// </summary>
        public override void Up()
        {
            this.Database.AddColumn("RIS_PACKAGE", "USER_NAME", DbType.String, 200);
            this.Database.AddColumn("RIS_TASK", "USER_NAME", DbType.String, 200);
            this.Database.AddColumn("RIS_TASK_PACKAGE", "STATE", DbType.Int16);
            this.Database.AddColumn("RIS_TASK_PACKAGE", "MESSAGE", DbType.String, 500);
        }

        /// <summary>
        /// Метод миграции на версию назад
        /// </summary>
        public override void Down()
        {
            this.Database.RemoveColumn("RIS_PACKAGE", "USER_NAME");
            this.Database.RemoveColumn("RIS_TASK", "USER_NAME");
            this.Database.RemoveColumn("RIS_TASK_PACKAGE", "STATE");
            this.Database.RemoveColumn("RIS_TASK_PACKAGE", "MESSAGE");
        }
    }
}
