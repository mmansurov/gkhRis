namespace Bars.Gkh.Ris.Migrations.Version_2016031500
{
    using System.Data;

    using B4.Modules.Ecm7.Framework;

    using Bars.B4.Modules.NH.Migrations.DatabaseExtensions;

    /// <summary>
    /// Миграция модуля
    /// </summary>
    [Migration("2016031500")]
    [MigrationDependsOn(typeof(Version_2016021800.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Метод миграции на версию вперед
        /// </summary>
        public override void Up()
        {
            this.Database.AddColumn("RIS_PROTOCOLMEETINGOWNER", "OPERATION", DbType.Int16, ColumnProperty.NotNull, "0");
            this.Database.AddColumn("RIS_PROTOCOLMEETINGOWNER", "EXTERNAL_ID", DbType.Int64, ColumnProperty.NotNull);
            this.Database.AddColumn("RIS_PROTOCOLMEETINGOWNER", "EXTERNAL_SYSTEM_NAME", DbType.String, 50, ColumnProperty.NotNull, "'gkh'");
            this.Database.AddRefColumn("RIS_PROTOCOLMEETINGOWNER", 
                new RefColumn("RIS_CONTAINER_ID", "RIS_PROTMEETOWN_CONTAINER", "RIS_CONTAINER", "ID"));
            this.Database.AddRefColumn("RIS_PROTOCOLMEETINGOWNER", 
                new RefColumn("RIS_CONTRAGENT_ID", "RIS_PROTMEETOWN_CONTRAGENT", "RIS_CONTRAGENT", "ID"));
            this.Database.AddColumn("RIS_PROTOCOLMEETINGOWNER", "GUID", DbType.String, 50);
        }

        /// <summary>
        /// Метод миграции на версию назад
        /// </summary>
        public override void Down()
        {
            this.Database.RemoveColumn("RIS_PROTOCOLMEETINGOWNER", "GUID");
            this.Database.RemoveColumn("RIS_PROTOCOLMEETINGOWNER", "RIS_CONTRAGENT_ID");
            this.Database.RemoveColumn("RIS_PROTOCOLMEETINGOWNER", "RIS_CONTAINER_ID");
            this.Database.RemoveColumn("RIS_PROTOCOLMEETINGOWNER", "EXTERNAL_SYSTEM_NAME");
            this.Database.RemoveColumn("RIS_PROTOCOLMEETINGOWNER", "EXTERNAL_ID");
            this.Database.RemoveColumn("RIS_PROTOCOLMEETINGOWNER", "OPERATION");
        }
    }
}