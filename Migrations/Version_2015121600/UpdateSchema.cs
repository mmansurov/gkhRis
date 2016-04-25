namespace Bars.Gkh.Ris.Migrations.Version_2015121600
{
    using System.Data;
    using B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015121600")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015121400.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddNsiRefTable("RIS_ADDITIONAL_SERVICE",
                new Column("ADDITIONAL_SERVICE_TYPE_NAME", DbType.String, 100, ColumnProperty.NotNull),
                new Column("OKEI", DbType.String, 3),
                new Column("STRING_DIMENSION_UNIT", DbType.String, 100));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_ADDITIONAL_SERVICE");
        }
    }
}
