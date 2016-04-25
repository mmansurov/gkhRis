namespace Bars.Gkh.Ris.Migrations.Version_2015103100
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015103100")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015102800.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddColumn("RIS_ATTACHMENT", new Column("NAME", DbType.String, 50));
            Database.AddColumn("RIS_ATTACHMENT", new Column("DESCRIPTION", DbType.String, 50));

            Database.AddColumn("RIS_CHARTER", new RefColumn("ATTACHMENT_ID", "RIS_CHARTER_ATTACH", "RIS_ATTACHMENT", "ID"));
        }

        public override void Down()
        {
            Database.RemoveColumn("RIS_CHARTER", "ATTACHMENT_ID");

            Database.RemoveColumn("RIS_ATTACHMENT", "DESCRIPTION");
            Database.RemoveColumn("RIS_ATTACHMENT", "NAME");
        }
    }
}