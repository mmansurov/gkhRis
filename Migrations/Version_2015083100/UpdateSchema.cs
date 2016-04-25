namespace Bars.Gkh.Ris.Migrations.Version_2015083100
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015083100")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_1.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddEntityTable("RIS_SETTINGS",
                new Column("CODE", DbType.String, 50),
                new Column("NAME", DbType.String, 100),
                new Column("VALUE", DbType.String, 100));
        }

        public override void Down()
        {
            Database.RemoveTable("RIS_SETTINGS");
        }
    }
}