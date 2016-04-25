namespace Bars.Gkh.Ris.Migrations.Version_2015121700
{
    using System.Data;
    using B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015121700")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015121600.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.RemoveColumn("RIS_ADDITIONAL_SERVICE", "RIS_NSICATALOG_ID");
            this.Database.RemoveColumn("RIS_ADDITIONAL_SERVICE", "VALUE");
            this.Database.RemoveColumn("RIS_ADDITIONAL_SERVICE", "CODE");

            this.Database.RemoveColumn("RIS_MUNICIPAL_SERVICE", "RIS_NSICATALOG_ID");
            this.Database.RemoveColumn("RIS_MUNICIPAL_SERVICE", "VALUE");
            this.Database.RemoveColumn("RIS_MUNICIPAL_SERVICE", "CODE");

            this.Database.RemoveTable("RIS_NSIREF");
            this.Database.RemoveTable("RIS_NSICATALOG");
        }

        public override void Down()
        {
            this.Database.AddColumn("RIS_ADDITIONAL_SERVICE", new RefColumn("RIS_NSICATALOG_ID", "RIS_ADDITIONAL_SERVICE_NSIREFCATALOG", "RIS_NSICATALOG", "ID"));
            this.Database.AddColumn("RIS_ADDITIONAL_SERVICE", new Column("VALUE", DbType.String, 100));
            this.Database.AddColumn("RIS_ADDITIONAL_SERVICE", new Column("CODE", DbType.String, 50));

            this.Database.AddColumn("RIS_MUNICIPAL_SERVICE", new RefColumn("RIS_NSICATALOG_ID", "RIS_MUNICIPAL_SERVICE_NSIREFCATALOG", "RIS_NSICATALOG", "ID"));
            this.Database.AddColumn("RIS_MUNICIPAL_SERVICE", new Column("VALUE", DbType.String, 100));
            this.Database.AddColumn("RIS_MUNICIPAL_SERVICE", new Column("CODE", DbType.String, 50));

            this.Database.AddEntityTable("RIS_NSICATALOG",
                new Column("CODE", DbType.String, 50),
                new Column("NAME", DbType.String, 50));

            this.Database.AddRisEntityTable("RIS_NSIREF",
                new RefColumn("RIS_NSICATALOG_ID", "RIS_NSIREFCATALOG", "RIS_NSICATALOG", "ID"),
                new Column("VALUE", DbType.String, 100),
                new Column("CODE", DbType.String, 50));
        }
    }
}
