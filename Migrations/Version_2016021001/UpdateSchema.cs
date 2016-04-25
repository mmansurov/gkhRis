namespace Bars.Gkh.Ris.Migrations.Version_2016021001
{
    using System.Data;
    using B4.Modules.Ecm7.Framework;

    using Bars.B4.Modules.NH.Migrations.DatabaseExtensions;

    /// <summary>
    /// Миграция модуля
    /// </summary>
    [Migration("2016021001")]
    [MigrationDependsOn(typeof(Version_2016021000.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Метод миграции на версию вперед
        /// </summary>
        public override void Up()
        {
            this.Database.AddEntityTable("RIS_TASK",
                 new Column("CLASS_NAME", DbType.String, 200),
                 new Column("DESCRIPTION", DbType.String, 500),
                 new Column("QRTZ_TRIGGER_KEY", DbType.String, 200),
                 new Column("MAX_REPEAT_COUNT", DbType.Int32),
                 new Column("INTERVAL", DbType.Int32),
                 new Column("START_TIME", DbType.DateTime),
                 new Column("END_TIME", DbType.DateTime));

            this.Database.AddEntityTable("RIS_PACKAGE",
                new Column("NOT_SIGNED_DATA", DbType.Binary, ColumnProperty.Null),
                new Column("SIGNED_DATA", DbType.Binary, ColumnProperty.Null),
                new Column("TRANSPORT_GUID_DICTIONARY", DbType.Binary, ColumnProperty.Null));

            this.Database.AddEntityTable("RIS_TASK_PACKAGE",
                new RefColumn("TASK_ID", "RIS_TASK_PACKAGE_TASK", "RIS_TASK", "ID"),
                new RefColumn("PACKAGE_ID", "RIS_TASK_PACKAGE_PACKAGE", "RIS_PACKAGE", "ID"));
        }

        /// <summary>
        /// Метод миграции на версию назад
        /// </summary>
        public override void Down()
        {
            this.Database.RemoveTable("RIS_TASK_PACKAGE");
            this.Database.RemoveTable("RIS_TASK");
            this.Database.RemoveTable("RIS_PACKAGE");
        }
    }
}
