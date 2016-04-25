namespace Bars.Gkh.Ris.Migrations.Version_2015110200
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015110200")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015103100.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.RemoveColumn("RIS_INSPECTION_EXAMINATION", "INSPECTIONNUMBER");
            Database.AddColumn("RIS_INSPECTION_EXAMINATION", new Column("INSPECTIONNUMBER", DbType.Int32));

            if (!Database.ColumnExists("RIS_INSPECTION_EXAMINATION", "RIS_CONTAINER_ID"))
            {
                Database.AddRefColumn("RIS_INSPECTION_EXAMINATION", new RefColumn("RIS_CONTAINER_ID", "RIS_INSPECTION_EXAMINATION_CONTAINER", "RIS_CONTAINER", "ID"));
            }

            if (!Database.ColumnExists("RIS_INSPECTION_EXAMINATION", "GUID"))
            {
                Database.AddColumn("RIS_INSPECTION_EXAMINATION", new Column("GUID", DbType.String, 50));
            }

            if (!Database.ColumnExists("RIS_INSPECTION_PLAN", "RIS_CONTAINER_ID"))
            {
                Database.AddRefColumn("RIS_INSPECTION_PLAN", new RefColumn("RIS_CONTAINER_ID", "RIS_INSPECTION_PLAN_CONTAINER", "RIS_CONTAINER", "ID"));
            }

            if (!Database.ColumnExists("RIS_INSPECTION_PLAN", "GUID"))
            {
                Database.AddColumn("RIS_INSPECTION_PLAN", new Column("GUID", DbType.String, 50));
            }

            if (!Database.ColumnExists("RIS_INSPECTION_OFFENCE", "RIS_CONTAINER_ID"))
            {
                Database.AddRefColumn("RIS_INSPECTION_OFFENCE", new RefColumn("RIS_CONTAINER_ID", "RIS_INSPECTION_OFFENCE_CONTAINER", "RIS_CONTAINER", "ID"));
            }

            if (!Database.ColumnExists("RIS_INSPECTION_OFFENCE", "GUID"))
            {
                Database.AddColumn("RIS_INSPECTION_OFFENCE", new Column("GUID", DbType.String, 50));
            }

            if (!Database.ColumnExists("RIS_INSPECTION_PRECEPT", "RIS_CONTAINER_ID"))
            {
                Database.AddRefColumn("RIS_INSPECTION_PRECEPT", new RefColumn("RIS_CONTAINER_ID", "RIS_INSPECTION_PRECEPT_CONTAINER", "RIS_CONTAINER", "ID"));
            }

            if (!Database.ColumnExists("RIS_INSPECTION_PRECEPT", "GUID"))
            {
                Database.AddColumn("RIS_INSPECTION_PRECEPT", new Column("GUID", DbType.String, 50));
            }
        }

        public override void Down()
        {
            Database.RemoveColumn("RIS_INSPECTION_PRECEPT", "GUID");
            Database.RemoveColumn("RIS_INSPECTION_PRECEPT", "RIS_CONTAINER_ID");

            Database.RemoveColumn("RIS_INSPECTION_OFFENCE", "GUID");
            Database.RemoveColumn("RIS_INSPECTION_OFFENCE", "RIS_CONTAINER_ID");

            Database.RemoveColumn("RIS_INSPECTION_PLAN", "GUID");
            Database.RemoveColumn("RIS_INSPECTION_PLAN", "RIS_CONTAINER_ID");

            Database.RemoveColumn("RIS_INSPECTION_EXAMINATION", "GUID");
            Database.RemoveColumn("RIS_INSPECTION_EXAMINATION", "RIS_CONTAINER_ID");

            Database.RemoveColumn("RIS_INSPECTION_EXAMINATION", "INSPECTIONNUMBER");
            Database.AddColumn("RIS_INSPECTION_EXAMINATION", new Column("INSPECTIONNUMBER", DbType.String));
        }
    }
}