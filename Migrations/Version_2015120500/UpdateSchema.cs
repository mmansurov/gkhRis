namespace Bars.Gkh.Ris.Migrations.Version_2015120500
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015120500")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015120400.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddEntityTable("RIS_NSICATALOG",
                new Column("CODE", DbType.String, 50),
                new Column("NAME", DbType.String, 50));

            this.Database.AddEntityTable("RIS_NSIREF",
                new RefColumn("RIS_NSICATALOG_ID", "RIS_NSIREFCATALOG", "RIS_NSICATALOG", "ID"),
                new Column("VALUE", DbType.String, 100),
                new Column("CODE", DbType.String, 50),
                new Column("GUID", DbType.String, 50));

            this.Database.AddRisEntityTable("RIS_PAYMENTDOCUMENT");

            this.Database.AddRisEntityTable("RIS_NOTIFORDEREXECUT",
                new Column("SUPPLIER_ID", DbType.String, 25),
                new Column("SUPPLIER_NAME", DbType.String, 160),
                new Column("RECIPIENT_INN", DbType.String, 12),
                new Column("RECIPIENT_KPP", DbType.String, 9),
                new Column("BANK_NAME", DbType.String, 160),
                new Column("RECIPIENT_BANK_BIK", DbType.String, 9),
                new Column("RECIPIENT_BANK_CORRACC", DbType.String, 120),
                new Column("RECIPIENT_ACCOUNT", DbType.String, 20),
                new Column("RECIPIENT_NAME", DbType.String, 160),
                new Column("ORDER_ID", DbType.String, 32),
                new RefColumn("PAYMDOC_ID", "RIS_NOTIF_PAYMDOC", "RIS_PAYMENTDOCUMENT", "ID"),
                new Column("ACCOUNT_NUMBER", DbType.String, 30),
                new Column("ORDER_NUM", DbType.String, 9),
                new Column("ORDER_DATE", DbType.DateTime, ColumnProperty.Null),
                new Column("AMOUNT", DbType.Decimal, ColumnProperty.Null),
                new Column("PAYMENT_PURPOSE", DbType.String, 210),
                new Column("COMMENT", DbType.String, 210));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_PAYMENTDOCUMENT");
            this.Database.RemoveTable("RIS_NSIREF");
            this.Database.RemoveTable("RIS_NSICATALOG");
        }
    }
}