namespace Bars.Gkh.Ris.Migrations.Version_2015122600
{
    using System.Data;

    using Bars.B4.Modules.Ecm7.Framework;

    [Migration("2015122600")]
    [MigrationDependsOn(typeof(Version_2015122300.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        public override void Up()
        {
            this.Database.AddRisEntityTable("RIS_SUBSIDARY",
                 new Column("FULLNAME", DbType.String, 255, ColumnProperty.NotNull),
                 new Column("SHORTNAME", DbType.String, 255),
                 new Column("OGRN", DbType.String, 13, ColumnProperty.NotNull),
                 new Column("INN", DbType.String, 12, ColumnProperty.NotNull),
                 new Column("KPP", DbType.String, 9, ColumnProperty.NotNull),
                 new Column("OKOPF", DbType.String, 5, ColumnProperty.NotNull),
                 new Column("ADDRESS", DbType.String, 255, ColumnProperty.NotNull),
                 new Column("FIASHOUSEGUID", DbType.String, 40),
                 new Column("ACTIVITYENDDATE", DbType.DateTime),
                 new Column("SOURCENAME", DbType.String, 255, ColumnProperty.NotNull),
                 new Column("SOURCEDATE", DbType.String, 50));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_SUBSIDARY");
        }
    }
}
