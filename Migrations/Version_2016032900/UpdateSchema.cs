namespace Bars.Gkh.Ris.Migrations.Version_2016032900
{
    using System.Data;
    using B4.Modules.Ecm7.Framework;
    using B4.Modules.NH.Migrations.DatabaseExtensions;

    /// <summary>
    /// Миграция модуля
    /// </summary>
    [Migration("2016032900")]
    [MigrationDependsOn(typeof(Version_2016031500.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Метод миграции на версию вперед
        /// </summary>
        public override void Up()
        {
            this.AddRisColumns("RIS_PROTOCOLOK");
            this.AddRisColumns("RIS_CONTRACTATTACHMENT");
        }

        /// <summary>
        /// Метод миграции на версию назад
        /// </summary>
        public override void Down()
        {
            this.RemoveRisColumns("RIS_PROTOCOLOK");
            this.RemoveRisColumns("RIS_CONTRACTATTACHMENT");
        }

        /// <summary>
        /// Добавить колонки RIS
        /// </summary>
        /// <param name="tableName">Наименование таблицы</param>
        private void AddRisColumns(string tableName)
        {
            if (!this.Database.ColumnExists(tableName, "OPERATION"))
            {
                this.Database.AddColumn(tableName, "OPERATION", DbType.Int16, ColumnProperty.NotNull, "0");
            }

            if (!this.Database.ColumnExists(tableName, "EXTERNAL_ID"))
            {
                this.Database.AddColumn(tableName, "EXTERNAL_ID", DbType.Int64, ColumnProperty.NotNull);
            }

            if (!this.Database.ColumnExists(tableName, "EXTERNAL_SYSTEM_NAME"))
            {
                this.Database.AddColumn(tableName, "EXTERNAL_SYSTEM_NAME", DbType.String, 50, ColumnProperty.NotNull, "'gkh'");
            }

            if (!this.Database.ColumnExists(tableName, "RIS_CONTAINER_ID"))
            {
                this.Database.AddRefColumn(tableName, new RefColumn("RIS_CONTAINER_ID", tableName + "_CONTAINER", "RIS_CONTAINER", "ID"));
            }

            if (!this.Database.ColumnExists(tableName, "RIS_CONTRAGENT_ID"))
            {
                this.Database.AddRefColumn(tableName, new RefColumn("RIS_CONTRAGENT_ID", tableName + "_CONTRAGENT", "RIS_CONTRAGENT", "ID"));
            }

            if (!this.Database.ColumnExists(tableName, "GUID"))
            {
                this.Database.AddColumn(tableName, "GUID", DbType.String, 50);
            }
        }

        /// <summary>
        /// Удалить колонки RIS
        /// </summary>
        /// <param name="tableName">Наименование таблицы</param>
        private void RemoveRisColumns(string tableName)
        {
            this.Database.RemoveColumn(tableName, "GUID");
            this.Database.RemoveColumn(tableName, "RIS_CONTRAGENT_ID");
            this.Database.RemoveColumn(tableName, "RIS_CONTAINER_ID");
            this.Database.RemoveColumn(tableName, "EXTERNAL_SYSTEM_NAME");
            this.Database.RemoveColumn(tableName, "EXTERNAL_ID");
            this.Database.RemoveColumn(tableName, "OPERATION");
        }
    }
}