namespace Bars.Gkh.Ris.Migrations.Version_2015121101
{
    using System.Data;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015121101")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015121100.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddRisEntityTable("RIS_PAYMENT_PERIOD",
                new Column("MONTH", DbType.Int32),
                new Column("YEAR", DbType.Int32),
                new Column("PAYMENT_PERIOD_TYPE", DbType.Int16, ColumnProperty.NotNull, "10"));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_PAYMENT_PERIOD");
        }
    }
}