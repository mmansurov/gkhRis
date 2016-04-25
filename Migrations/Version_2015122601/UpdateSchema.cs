namespace Bars.Gkh.Ris.Migrations.Version_2015122601
{
    using System.Data;

    using Bars.B4.Modules.Ecm7.Framework;

    [Migration("2015122601")]
    [MigrationDependsOn(typeof(Version_2015122600.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        public override void Up()
        {
            this.Database.AddRisEntityTable("RIS_ACKNOWLEDGMENT",
                 new Column("ORDERID", DbType.String, 32, ColumnProperty.NotNull),
                 new Column("PAYMENTDOCUMENTNUMBER", DbType.String, 18, ColumnProperty.NotNull),
                 new Column("HSTYPE", DbType.String, 40),
                 new Column("MSTYPE", DbType.String, 40),
                 new Column("ASTYPE", DbType.String, 40),
                 new Column("AMOUNT", DbType.Decimal));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_ACKNOWLEDGMENT");
        }
    }
}
