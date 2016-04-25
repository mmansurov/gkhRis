namespace Bars.Gkh.Ris.Migrations.Version_2015112400
{
    using System.Data;

    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;

    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015112400")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015112300.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddRisEntityTable(
                "RIS_METERING_DEVICE_CURRENT_VALUE",
                new RefColumn("METERING_DEVICE_DATA_ID", "RIS_METERING_DEVICE_CURRENT_VALUE_DATA", "RIS_METERING_DEVICE_DATA", "ID"),
                new RefColumn("ACCOUNT_ID", "RIS_METERING_DEVICE_CURRENT_VALUE_ACCOUNT", "RIS_ACCOUNT", "ID"),
                new Column("VALUE_T1", DbType.Decimal, ColumnProperty.NotNull),
                new Column("VALUE_T2", DbType.Decimal),
                new Column("VALUE_T3", DbType.Decimal),
                new Column("READOUT_DATE", DbType.DateTime, ColumnProperty.NotNull),
                new Column("READING_SOURCE", DbType.String, 50));

            Database.AddRisEntityTable("RIS_METERING_DEVICE_CONTROL_VALUE",
                new RefColumn("METERING_DEVICE_DATA_ID", "RIS_METERING_DEVICE_CONTROL_VALUE_DATA", "RIS_METERING_DEVICE_DATA", "ID"),
                new RefColumn("ACCOUNT_ID", "RIS_METERING_DEVICE_CONTROL_VALUE_ACCOUNT", "RIS_ACCOUNT", "ID"),
                new Column("VALUE_T1", DbType.Decimal, ColumnProperty.NotNull),
                new Column("VALUE_T2", DbType.Decimal),
                new Column("VALUE_T3", DbType.Decimal),
                new Column("READOUT_DATE", DbType.DateTime, ColumnProperty.NotNull),
                new Column("READING_SOURCE", DbType.String, 50));

            Database.AddRisEntityTable("RIS_METERING_DEVICE_VERIFICATION_VALUE",
                new RefColumn("METERING_DEVICE_DATA_ID", "RIS_METERING_DEVICE_VERIFICATION_VALUE_DATA", "RIS_METERING_DEVICE_DATA", "ID"),
                new RefColumn("ACCOUNT_ID", "RIS_METERING_DEVICE_VERIFICATION_VALUE_ACCOUNT", "RIS_ACCOUNT", "ID"),
                new Column("START_VERIFICATION_VALUE_T1", DbType.Decimal, ColumnProperty.NotNull),
                new Column("START_VERIFICATION_VALUE_T2", DbType.Decimal),
                new Column("START_VERIFICATION_VALUE_T3", DbType.Decimal),
                new Column("START_VERIFICATION_READOUT_DATE", DbType.DateTime, ColumnProperty.NotNull),
                new Column("START_VERIFICATION_READING_SOURCE", DbType.String, 50),
                new Column("END_VERIFICATION_VALUE_T1", DbType.Decimal, ColumnProperty.NotNull),
                new Column("END_VERIFICATION_VALUE_T2", DbType.Decimal),
                new Column("END_VERIFICATION_VALUE_T3", DbType.Decimal),
                new Column("END_VERIFICATION_READOUT_DATE", DbType.DateTime, ColumnProperty.NotNull),
                new Column("END_VERIFICATION_READING_SOURCE", DbType.String, 50),
                new Column("PLANNED_VERIFICATION", DbType.Boolean),
                new Column("VERIFICATION_REASON_CODE", DbType.String, 50),
                new Column("VERIFICATION_REASON_GUID", DbType.String, 50),
                new Column("VERIFICATION_REASON_NAME", DbType.String, 50));
        }

        public override void Down()
        {
            Database.RemoveTable("RIS_METERING_DEVICE_CURRENT_VALUE");
            Database.RemoveTable("RIS_METERING_DEVICE_CONTROL_VALUE");
            Database.RemoveTable("RIS_METERING_DEVICE_VERIFICATION_VALUE");
        }
    }
}
