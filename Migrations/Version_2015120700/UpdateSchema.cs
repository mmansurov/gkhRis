namespace Bars.Gkh.Ris.Migrations.Version_2015120700
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015120700")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015120500.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            if (this.Database.ColumnExists("RIS_HOUSESERVICE", "CONTRACTOBJECT_ID"))
            {
                this.Database.RemoveColumn("RIS_HOUSESERVICE", "CONTRACTOBJECT_ID");
            }

            if (this.Database.ColumnExists("RIS_HOUSESERVICE", "BASESERVICECHARTER_PROTOCOLMEETINGOWNER_ID"))
            {
                this.Database.RemoveColumn("RIS_HOUSESERVICE", "BASESERVICECHARTER_PROTOCOLMEETINGOWNER_ID");
            }

            if (this.Database.ColumnExists("RIS_HOUSESERVICE", "BASESERVISECHARTER_CURRENTCHARTER"))
            {
                this.Database.RemoveColumn("RIS_HOUSESERVICE", "BASESERVISECHARTER_CURRENTCHARTER");
            }

            if (this.Database.ColumnExists("RIS_HOUSESERVICE", "SERVICETYPE_CODE"))
            {
                this.Database.ChangeColumn("RIS_HOUSESERVICE", new Column("SERVICETYPE_CODE", DbType.String, 200));
            }

            if (this.Database.ColumnExists("RIS_HOUSESERVICE", "SERVICETYPE_GUID"))
            {
                this.Database.ChangeColumn("RIS_HOUSESERVICE", new Column("SERVICETYPE_GUID", DbType.String, 200));
            }

            this.Database.AddColumn("RIS_HOUSESERVICE", new Column("ACCOUNTINGTYPE_CODE", DbType.String, 200));
            this.Database.AddColumn("RIS_HOUSESERVICE", new Column("ACCOUNTINGTYPE_GUID", DbType.String, 200));

            this.Database.AddColumn("RIS_HOUSESERVICE", new RefColumn("HOUSE_ID", "RIS_HOUSESRV_HOUSE", "RIS_HOUSE", "ID"));
        }

        public override void Down()
        {
            if (this.Database.ColumnExists("RIS_HOUSESERVICE", "ACCOUNTINGTYPE_CODE"))
            {
                this.Database.RemoveColumn("RIS_HOUSESERVICE", "ACCOUNTINGTYPE_CODE");
            }

            if (this.Database.ColumnExists("RIS_HOUSESERVICE", "ACCOUNTINGTYPE_GUID"))
            {
                this.Database.RemoveColumn("RIS_HOUSESERVICE", "ACCOUNTINGTYPE_GUID");
            }

            if (this.Database.ColumnExists("RIS_HOUSESERVICE", "HOUSE_ID"))
            {
                this.Database.RemoveColumn("RIS_HOUSESERVICE", "HOUSE_ID");
            }
        }
    }
}