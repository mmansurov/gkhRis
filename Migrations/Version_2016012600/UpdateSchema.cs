namespace Bars.Gkh.Ris.Migrations.Version_2016012600
{
    using System.Data;
    using B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2016012600")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015122601.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {

        public override void Up()
        {
            if (!this.Database.ColumnExists("RIS_CONTRACT", "IMD_VALUES_BEGINDATE"))
            {
                this.Database.AddColumn("RIS_CONTRACT", new Column("IMD_VALUES_BEGINDATE", DbType.Int32));
            }

            if (!this.Database.ColumnExists("RIS_CONTRACT", "IMD_VALUES_ENDDATE"))
            {
                this.Database.AddColumn("RIS_CONTRACT", new Column("IMD_VALUES_ENDDATE", DbType.Int32));
            }

            if (!this.Database.ColumnExists("RIS_CONTRACT", "PAYMENT_DOC_DATE"))
            {
                this.Database.AddColumn("RIS_CONTRACT", new Column("PAYMENT_DOC_DATE", DbType.Int32));
            }
        }

        public override void Down()
        {
            if (this.Database.ColumnExists("RIS_CONTRACT", "IMD_VALUES_BEGINDATE"))
            {
                this.Database.RemoveColumn("RIS_CONTRACT", "IMD_VALUES_BEGINDATE");
            }

            if (this.Database.ColumnExists("RIS_CONTRACT", "IMD_VALUES_ENDDATE"))
            {
                this.Database.RemoveColumn("RIS_CONTRACT", "IMD_VALUES_ENDDATE");
            }

            if (this.Database.ColumnExists("RIS_CONTRACT", "PAYMENT_DOC_DATE"))
            {
                this.Database.RemoveColumn("RIS_CONTRACT", "PAYMENT_DOC_DATE");
            }
        }
    }
}
