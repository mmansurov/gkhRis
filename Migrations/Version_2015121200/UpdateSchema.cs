namespace Bars.Gkh.Ris.Migrations.Version_2015121200
{
    using System.Data;

    using Bars.B4.Modules.NH.Migrations.DatabaseExtensions;

    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015121200")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015121101.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddColumn("RIS_NSIREF", new Column("OBJECT_VERSION", DbType.Int64, ColumnProperty.NotNull));
            this.Database.AddColumn("RIS_NSIREF", new Column("OBJECT_CREATE_DATE", DbType.DateTime, ColumnProperty.NotNull));
            this.Database.AddColumn("RIS_NSIREF", new Column("OBJECT_EDIT_DATE", DbType.DateTime, ColumnProperty.NotNull));
            this.Database.AddColumn("RIS_NSIREF", new Column("EXTERNAL_ID", DbType.Int64, ColumnProperty.NotNull));
            this.Database.AddColumn("RIS_NSIREF", new Column("EXTERNAL_SYSTEM_NAME", DbType.String, 50, ColumnProperty.NotNull, "'gkh'"));
            this.Database.AddColumn("RIS_NSIREF", new RefColumn("RIS_CONTAINER_ID", "RIS_NSIREF_CONTAINER", "RIS_CONTAINER", "ID"));

            this.Database.AddNsiRefTable("RIS_MUNICIPAL_SERVICE",
                new Column("MUNICIPAL_SERVICE_REF_CODE", DbType.String, 20, ColumnProperty.NotNull),
                new Column("MUNICIPAL_SERVICE_REF_GUID", DbType.String, 40, ColumnProperty.NotNull),
                new Column("GENERAL_NEEDS", DbType.Boolean),
                new Column("MAIN_MUNICIPAL_SERVICE_NAME", DbType.String, 100, ColumnProperty.NotNull),
                new Column("MUNICIPAL_RESOURCE_REF_CODE", DbType.String, 20, ColumnProperty.NotNull),
                new Column("MUNICIPAL_RESOURCE_REF_GUID", DbType.String, 40, ColumnProperty.NotNull),
                new Column("SORT_ORDER", DbType.String, 3),
                new Column("SORT_ORDER_NOT_DEFINED", DbType.Boolean));
        }

        public override void Down()
        {
            this.Database.RemoveColumn("RIS_NSIREF", "OBJECT_VERSION");
            this.Database.RemoveColumn("RIS_NSIREF", "OBJECT_CREATE_DATE");
            this.Database.RemoveColumn("RIS_NSIREF", "OBJECT_EDIT_DATE");
            this.Database.RemoveColumn("RIS_NSIREF", "EXTERNAL_ID");
            this.Database.RemoveColumn("RIS_NSIREF", "EXTERNAL_SYSTEM_NAME");
            this.Database.RemoveColumn("RIS_NSIREF", "RIS_CONTAINER_ID");

            this.Database.RemoveTable("RIS_MUNICIPAL_SERVICE");
        }
    }
}