namespace Bars.Gkh.Ris.Migrations.Version_2015102800
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015102800")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015101600.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddRisEntityTable("RIS_CHARTER",
                new Column("DOCNUM", DbType.String, 50),
                new Column("DOCDATE", DbType.DateTime),
                new Column("PERIODMETERING_STARTDATE", DbType.DateTime),
                new Column("PERIODMETERING_ENDDATE", DbType.DateTime),
                new Column("PERIODMETERING_LASTDAY", DbType.Int16),
                new Column("PAYMENTDATE_STARTDATE", DbType.DateTime),
                new Column("PAYMENTDATE_LASTDAY", DbType.Int16),
                new Column("MANAGERS", DbType.String, 50),
                new Column("HEAD_SURNAME", DbType.String, 50),
                new Column("HEAD_FIRSTNAME", DbType.String, 50),
                new Column("HEAD_PATRONYMIC", DbType.String, 50),
                new Column("HEAD_GENDER", DbType.Int16, ColumnProperty.NotNull, "0"),
                new Column("HEAD_DATEOFBIRTH", DbType.DateTime),
                new Column("APPROVALCHARTER", DbType.Boolean),
                new Column("ROLLOVERCHARTER", DbType.Boolean),
                new Column("TERMINATECHARTER_DATE", DbType.DateTime),
                new Column("TERMINATECHARTER_REASON", DbType.String, 50));

            this.Database.AddEntityTable("RIS_PROTOCOLMEETINGOWNER",
                new RefColumn("CHARTER_ID", "RIS_PROTMEETOWN_CHARTER", "RIS_CHARTER", "ID"),
                new Column("NAME", DbType.String, 50),
                new Column("DESCRIPTION", DbType.String, 50),
                new RefColumn("ATTACHMENT_ID", "RIS_PROTMEETOWN_ATTACH", "RIS_ATTACHMENT", "ID"));

            this.Database.AddRisEntityTable("RIS_CONTRACTOBJECT",
                new RefColumn("CHARTER_ID", "RIS_CONTROBJ_CHARTER", "RIS_CHARTER", "ID"),
                new RefColumn("HOUSE_ID", "RIS_CONTROBJ_HOUSE", "RIS_HOUSE", "ID"),
                new Column("STARTDATE", DbType.DateTime),
                new Column("ENDDATE", DbType.DateTime),
                new Column("BASEMSERVISE_CURRENTCHARTER", DbType.Boolean),
                new RefColumn("BASEMSERVISE_PROTOCOLMEETINGOWNER_ID", "RIS_BMS_CONTROBJ_PMO", "RIS_PROTOCOLMEETINGOWNER", "ID"),
                new Column("DATEEXCLUSION", DbType.DateTime),
                new Column("BASEEXCLUSION_CURRENTCHARTER", DbType.Boolean),
                new RefColumn("BASEEXCLUSION_PROTOCOLMEETINGOWNER_ID", "RIS_BEXCL_CONTROBJ_PMO", "RIS_PROTOCOLMEETINGOWNER", "ID"));

            this.Database.AddRisEntityTable("RIS_HOUSESERVICE",
                new RefColumn("CONTRACTOBJECT_ID", "RIS_HOUSSERV_CONTROBJ", "RIS_CONTRACTOBJECT", "ID"),
                new Column("SERVICETYPE_CODE", DbType.String, 50),
                new Column("SERVICETYPE_GUID", DbType.String, 50),
                new Column("STARTDATE", DbType.DateTime),
                new Column("ENDDATE", DbType.DateTime),
                new Column("BASESERVISECHARTER_CURRENTCHARTER", DbType.Boolean),
                new RefColumn("BASESERVICECHARTER_PROTOCOLMEETINGOWNER_ID", "RIS_BSC_HOUSSERV_PMO", "RIS_PROTOCOLMEETINGOWNER", "ID"));

            this.Database.AddRisEntityTable("RIS_ADDSERVICE",
                new RefColumn("CONTRACTOBJECT_ID", "RIS_ADDSERV_CONTROBJ", "RIS_CONTRACTOBJECT", "ID"),
                new Column("SERVICETYPE_CODE", DbType.String, 50),
                new Column("SERVICETYPE_GUID", DbType.String, 50),
                new Column("STARTDATE", DbType.DateTime),
                new Column("ENDDATE", DbType.DateTime),
                new Column("BASESERVISECHARTER_CURRENTCHARTER", DbType.Boolean),
                new RefColumn("BASESERVICECHARTER_PROTOCOLMEETINGOWNER_ID", "RIS_BSC_ADDSERV_PMO", "RIS_PROTOCOLMEETINGOWNER", "ID"));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_ADDSERVICE");
            this.Database.RemoveTable("RIS_HOUSESERVICE");
            this.Database.RemoveTable("RIS_CONTRACTOBJECT");
            this.Database.RemoveTable("RIS_PROTOCOLMEETINGOWNER");
            this.Database.RemoveTable("RIS_CHARTER");
        }
    }
}