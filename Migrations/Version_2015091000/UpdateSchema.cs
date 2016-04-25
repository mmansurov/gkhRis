namespace Bars.Gkh.Ris.Migrations.Version_2015091000
{
    using System.Data;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015091000")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015090200.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddColumn("RIS_INTEGR_REF_DICT", new Column("GIS_REC_GUID", DbType.String, 50));
        }

        public override void Down()
        {
            Database.RemoveColumn("RIS_INTEGR_REF_DICT", "GIS_REC_GUID");
        }
    }
}