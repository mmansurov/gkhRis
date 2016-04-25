namespace Bars.Gkh.Ris.Migrations.Version_2015111300
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015111300")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015111100.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddRisEntityTable("RIS_PUBLICPROPERTYCONTRACT",
                new Column("STARTDATE", DbType.DateTime),
                new Column("ENDDATE", DbType.DateTime),
                new Column("CONTRACTNUMBER", DbType.String, 50),
                new Column("CONTRACTOBJECT", DbType.String, 50),
                new Column("COMMENTS", DbType.String, 1000),
                new Column("DATESIGNATURE", DbType.DateTime),
                new Column("ISSIGNATURED", DbType.Boolean),
                new RefColumn("HOUSE_ID", "RIS_PUBPROPCONTR_HOUSE", "RIS_HOUSE", "ID"),
                new RefColumn("ENTREPRENEUR_ID", "RIS_PUBPROPCONTR_IND", "RIS_IND", "ID"),
                new RefColumn("ORGANIZATION_ID", "RIS_PUBPROPCONTR_CONT", "RIS_CONTRAGENT", "ID"));

            this.Database.AddEntityTable("RIS_TRUSTDOCATTACHMENT",
                new RefColumn("PUBLICPROPERTYCONTRACT_ID", "RIS_TRUSTDOCATT_PUBCONTRACT", "RIS_PUBLICPROPERTYCONTRACT", "ID"),
                new RefColumn("ATTACHMENT_ID", "RIS_TRUSTDOCATT_ATTACH", "RIS_ATTACHMENT", "ID"));

            this.Database.AddEntityTable("RIS_CONTRACTATTACHMENT",
                new RefColumn("PUBLICPROPERTYCONTRACT_ID", "RIS_CONTRATT_PUBCONTRACT", "RIS_PUBLICPROPERTYCONTRACT", "ID"),
                new RefColumn("ATTACHMENT_ID", "RIS_CONTRATT_ATTACH", "RIS_ATTACHMENT", "ID"));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_CONTRACTATTACHMENT");
            this.Database.RemoveTable("RIS_TRUSTDOCATTACHMENT");
            this.Database.RemoveTable("RIS_PUBLICPROPERTYCONTRACT");
        }
    }
}