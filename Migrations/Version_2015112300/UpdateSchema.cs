namespace Bars.Gkh.Ris.Migrations.Version_2015112300
{
    using System.Data;

    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;

    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015112300")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015111300.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddRisEntityTable(
                "RIS_METERING_DEVICE_DATA",
                new Column("METERING_DEVICE_TYPE", DbType.Int16, ColumnProperty.NotNull, "10"),
                new Column("METERING_DEVICE_NUMBER", DbType.String, 50, ColumnProperty.NotNull),
                new Column("METERING_DEVICE_STAMP", DbType.String, 50),
                new Column("INSTALLATION_DATE", DbType.DateTime),
                new Column("COMMISSIONING_DATE", DbType.DateTime, ColumnProperty.NotNull),
                new Column("MANUAL_MODE_METERING", DbType.Boolean),
                new Column("FIRST_VERIFICATION_DATE", DbType.DateTime),
                new Column("VERIFICATION_INTERVAL", DbType.String, 50),
                new Column("DEVICE_TYPE", DbType.Int16, ColumnProperty.NotNull, "10"),              
                new Column("METERING_VALUE_T1", DbType.Decimal, ColumnProperty.NotNull),
                new Column("METERING_VALUE_T2", DbType.Decimal),
                new Column("METERING_VALUE_T3", DbType.Decimal),
                new Column("READOUT_DATE", DbType.DateTime, ColumnProperty.NotNull),
                new Column("READINGS_SOURCE", DbType.String, 50),
                new RefColumn("HOUSE_ID", "RIS_METERING_DEVICE_HOUSE", "RIS_HOUSE", "ID"),
                new RefColumn("RESIDENTIAL_PREMISES_ID", "RIS_METERING_DEVICE_RESIDENTIAL_PREMISES", "RIS_RESIDENTIALPREMISES", "ID"),
                new RefColumn("NONRESIDENTIAL_PREMISES_ID", "RIS_METERING_DEVICE_NONRESIDENTIAL_PREMISES", "RIS_NONRESIDENTIALPREMISES", "ID"),
                new Column("MUNICIPAL_RESOURCE_CODE", DbType.String, 50),
                new Column("MUNICIPAL_RESOURCE_GUID", DbType.String, 50));

            Database.AddRisEntityTable("RIS_METERING_DEVICE_ACCOUNT", 
                new RefColumn("ACCOUNT_ID", "RIS_METERING_DEVICE_ACCOUNT_ACCOUNT", "RIS_ACCOUNT", "ID"),
                new RefColumn("METERING_DEVICE_ID", "RIS_METERING_DEVICE_ACCOUNT_METERING_DEVICE", "RIS_METERING_DEVICE_DATA", "ID"));

            Database.AddRisEntityTable("RIS_METERING_DEVICE_LIVING_ROOM",
                new RefColumn("LIVING_ROOM_ID", "RIS_METERING_DEVICE_LIVING_ROOM_LIVING_ROOM", "RIS_LIVINGROOM", "ID"),
                new RefColumn("METERING_DEVICE_ID", "RIS_METERING_DEVICE_LIVING_ROOM_METERING_DEVICE", "RIS_METERING_DEVICE_DATA", "ID"));

            //исправление предыдущей миграции, маппингов нет
            if (Database.ColumnExists("RIS_PUBLICPROPERTYCONTRACT", "CONTRACT_GUID"))
            {
                Database.RemoveColumn("RIS_PUBLICPROPERTYCONTRACT", "CONTRACT_GUID");
            }
        }

        public override void Down()
        {
            Database.RemoveTable("RIS_METERING_DEVICE_ACCOUNT");
            Database.RemoveTable("RIS_METERING_DEVICE_LIVING_ROOM");
            Database.RemoveTable("RIS_METERING_DEVICE_DATA");
        }
    }
}
