namespace Bars.Gkh.Ris.Migrations.Version_2015121100
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015121100")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015120800.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddRisEntityTable("RIS_PAYMENT_INFO",
                new Column("RECIPIENT", DbType.String, 160, ColumnProperty.NotNull),
                new Column("BANK_BIK", DbType.String, 9, ColumnProperty.NotNull),
                new Column("RECIPIENT_KPP", DbType.String, 9),
                new Column("OPERATING_ACCOUNT_NUMBER", DbType.String, 20, ColumnProperty.NotNull),
                new Column("CORRESPONDENT_BANK_ACCOUNT", DbType.String, 20, ColumnProperty.NotNull));

            Database.AddRisEntityTable("RIS_ADDRESS_INFO",
                new Column("LIVING_PERSON_NUMBER", DbType.Byte),
                new Column("RESIDENTIAL_SQUARE", DbType.Decimal),
                new Column("HEATED_AREA", DbType.Decimal),
                new Column("TOTAL_SQUARE", DbType.Decimal));

            Database.AddRisEntityTable("RIS_PAYMENT_DOC",
                new RefColumn("ACCOUNT_ID", "RIS_PAYMENT_DOC_ACCOUNT", "RIS_ACCOUNT", "ID"),
                new RefColumn("PAYMENT_INFO_ID", "RIS_PAYMENT_DOC_PAYMENT_INFO", "RIS_PAYMENT_INFO", "ID"),
                new RefColumn("ADDRESS_INFO_ID", "RIS_PAYMENT_DOC_ADDRESS_INFO", "RIS_ADDRESS_INFO", "ID"),
                new Column("STATE", DbType.Int16),
                new Column("TOTAL_PIECEMEAL_SUM", DbType.Decimal),
                new Column("DATE", DbType.DateTime),
                new Column("PERIODMONTH", DbType.Int16),
                new Column("PERIODYEAR", DbType.Int16)
                );

            Database.AddRisChargeInfoTable("RIS_HOUSING_SERVICE_CHARGE_INFO",                
                new RefColumn("PAYMENT_DOC_ID", "RIS_HOUSING_SERVICE_CHARGE_PAYMENT_DOC", "RIS_PAYMENT_DOC", "ID"),
                new Column("MONEY_RECALCULATION", DbType.Decimal),
                new Column("MONEY_DISCOUNT", DbType.Decimal)
                );

            Database.AddRisChargeInfoTable("RIS_ADDITIONAL_SERVICE_CHARGE_INFO",
                new RefColumn("PAYMENT_DOC_ID", "RIS_ADDITIONAL_SERVICE_CHARGE_PAYMENT_DOC", "RIS_PAYMENT_DOC", "ID"),
                new Column("MONEY_RECALCULATION", DbType.Decimal),
                new Column("MONEY_DISCOUNT", DbType.Decimal));

            Database.AddRisChargeInfoTable("RIS_MUNICIPAL_SERVICE_CHARGE_INFO",
                new RefColumn("PAYMENT_DOC_ID", "RIS_MUNICIPAL_SERVICE_CHARGE_PAYMENT_DOC", "RIS_PAYMENT_DOC", "ID"),
                new Column("MONEY_RECALCULATION", DbType.Decimal),
                new Column("MONEY_DISCOUNT", DbType.Decimal),
                new Column("PERIOD_PIECEMAL_SUM", DbType.Decimal),
                new Column("PAST_PERIOD_PIECEMAL_SUM", DbType.Decimal),
                new Column("PIECEMAL_PERCENT_RUB", DbType.Decimal),
                new Column("PIECEMAL_PERCENT", DbType.Decimal),
                new Column("PIECEMAL_PAYMENT_SUM", DbType.Decimal),
                new Column("PAYMENT_RECALCULATION_SUM", DbType.Decimal),
                new Column("PAYMENT_RECALCULATION_REASON", DbType.String, 1000));

            Database.AddRisEntityTable("RIS_ADDITIONAL_SERVICE_EXT_CHARGE_INFO",
                new RefColumn("PAYMENT_DOC_ID", "RIS_ADDITIONAL_SERVICE_EXT_CHARGE_PAYMENT_DOC", "RIS_PAYMENT_DOC", "ID"),
                new Column("MONEY_RECALCULATION", DbType.Decimal),
                new Column("MONEY_DISCOUNT", DbType.Decimal),
                new Column("SERVICE_TYPE_CODE", DbType.String, 20, ColumnProperty.NotNull),
                new Column("SERVICE_TYPE_GUID", DbType.String, 50, ColumnProperty.NotNull));

            Database.AddRisChargeInfoTable("RIS_TECH_SERVICE",
                new RefColumn("ADDITIONAL_SERVICE_EXT_CHARGE_INFO_ID", "RIS_TECH_SERVICE_ADDITIONAL_EXT_CHARGE", "RIS_ADDITIONAL_SERVICE_EXT_CHARGE_INFO", "ID"));

            Database.RemoveColumn("RIS_NOTIFORDEREXECUT", "PAYMDOC_ID");
            Database.AddColumn("RIS_NOTIFORDEREXECUT", new RefColumn("PAYMENT_DOC_ID", "RIS_NOTIF_PAYMENT_DOC", "RIS_PAYMENT_DOC", "ID"));

            Database.RemoveTable("RIS_PAYMENTDOCUMENT");
        }

        public override void Down()
        {
            this.Database.AddRisEntityTable("RIS_PAYMENTDOCUMENT");

            Database.RemoveColumn("RIS_NOTIFORDEREXECUT", "PAYMENT_DOC_ID");
            Database.AddColumn("RIS_NOTIFORDEREXECUT", new RefColumn("PAYMDOC_ID", "RIS_NOTIF_PAYMDOC", "RIS_PAYMENTDOCUMENT", "ID"));

            this.Database.RemoveTable("RIS_TECH_SERVICE");
            this.Database.RemoveTable("RIS_ADDITIONAL_SERVICE_EXT_CHARGE_INFO");
            this.Database.RemoveTable("RIS_HOUSING_SERVICE_CHARGE_INFO");
            this.Database.RemoveTable("RIS_ADDITIONAL_SERVICE_CHARGE_INFO");
            this.Database.RemoveTable("RIS_MUNICIPAL_SERVICE_CHARGE_INFO");
            this.Database.RemoveTable("RIS_PAYMENT_DOC");
            this.Database.RemoveTable("RIS_PAYMENT_INFO");
            this.Database.RemoveTable("RIS_ADDRESS_INFO");
        }
    }
}
