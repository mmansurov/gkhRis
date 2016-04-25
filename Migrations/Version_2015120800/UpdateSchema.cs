namespace Bars.Gkh.Ris.Migrations.Version_2015120800
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015120800")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015120700.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.RemoveTable("RIS_INTEGR_METHOD");
        }

        public override void Down()
        {
            Database.AddEntityTable("RIS_INTEGR_METHOD",
                new Column("NAME", DbType.String, 1000, ColumnProperty.NotNull),
                new Column("DESCRIPTION", DbType.String, 1000),
                new Column("DATE_INTEG", DbType.DateTime),
                new Column("METHODCODE", DbType.String, 1000, ColumnProperty.NotNull),
                new Column("SERVICE_ADDRESS", DbType.String, 1000, ColumnProperty.NotNull));
        }
    }
}
