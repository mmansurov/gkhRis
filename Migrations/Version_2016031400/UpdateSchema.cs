namespace Bars.Gkh.Ris.Migrations.Version_2016031400
{
    using System.Data;

    using Bars.B4.Modules.Ecm7.Framework;

    /// <summary>
    /// Миграция модуля
    /// </summary>
    [Migration("2016031400")]
    [MigrationDependsOn(typeof(Version_2016022500.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Метод миграции на версию вперед
        /// </summary>
        public override void Up()
        {
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "REALITYOBJECT_ID"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", "REALITYOBJECT_ID", DbType.Int64);
            }

            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "FIASHOUSE_GUID"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", "FIASHOUSE_GUID", DbType.String, 200);
            }
        }

        /// <summary>
        /// Метод миграции на версию назад
        /// </summary>
        public override void Down()
        {
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "REALITYOBJECT_ID"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "REALITYOBJECT_ID");
            }

            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "FIASHOUSE_GUID"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "FIASHOUSE_GUID");
            }
        }
    }
}
