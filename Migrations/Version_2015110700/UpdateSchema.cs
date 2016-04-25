namespace Bars.Gkh.Ris.Migrations.Version_2015110700
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015110700")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015110200.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddRisEntityTable("RIS_IND",
                new Column("SURNAME", DbType.String, 50),
                new Column("FIRSTNAME", DbType.String, 50),
                new Column("PATRONYMIC", DbType.String, 50),
                new Column("SEX", DbType.Int16, ColumnProperty.NotNull, "0"),
                new Column("DATEOFBIRTH", DbType.DateTime),
                new Column("IDTYPE_GUID", DbType.String, 50),
                new Column("IDTYPE_CODE", DbType.String, 50),
                new Column("IDSERIES", DbType.String, 50),
                new Column("IDNUMBER", DbType.String, 50),
                new Column("IDISSUEDATE", DbType.DateTime),
                new Column("SNILS", DbType.String, 50),
                new Column("PLACEBIRTH", DbType.String, 50));

            Database.AddRisEntityTable("RIS_ACCOUNT",
                new Column("TYPEACCOUNT", DbType.Int16, ColumnProperty.NotNull, "10"),
                new RefColumn("OWNERIND_ID", "RIS_ACC_OWNERIND", "RIS_IND", "ID"),
                new RefColumn("OWNERORG_ID", "RIS_ACC_OWNERORG", "RIS_CONTRAGENT", "ID"),
                new RefColumn("RENTERIND_ID", "RIS_ACC_RENTERIND", "RIS_IND", "ID"),
                new RefColumn("RENTERORG_ID", "RIS_ACC_RENTERORG", "RIS_CONTRAGENT", "ID"),
                new Column("LIVINGPERSONSNUMBER", DbType.Int32),
                new Column("TOTALSQUARE", DbType.Decimal),
                new Column("RESIDENTIALSQUARE", DbType.Decimal),
                new Column("HEATEDAREA", DbType.Decimal),
                new Column("CLOSED", DbType.Boolean),
                new Column("CLOSEREASON_CODE", DbType.String, 50),
                new Column("CLOSEREASON_GUID", DbType.String, 50),
                new Column("CLOSEDATE", DbType.DateTime),
                new Column("ACCOUNTNUMBER", DbType.String, 50));
        }

        public override void Down()
        {
            Database.RemoveTable("RIS_ACCOUNT");

            Database.RemoveTable("RIS_IND");
        }
    }
}