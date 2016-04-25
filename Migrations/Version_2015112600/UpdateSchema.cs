namespace Bars.Gkh.Ris.Migrations.Version_2015112600
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015112600")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015112500.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddRisEntityTable("RIS_NOTIFICATION",
                new Column("TOPIC", DbType.String, 200),
                new Column("ISIMPORTANT", DbType.Boolean),
                new Column("CONTENT", DbType.String, 5000),
                new Column("ISALL", DbType.Boolean),
                new Column("ISNOTLIMIT", DbType.Boolean),
                new Column("STARTDATE", DbType.DateTime),
                new Column("ENDDATE", DbType.DateTime),
                new Column("ISSHIPOFF", DbType.Boolean),
                new Column("DELETED", DbType.Boolean));

            this.Database.AddRisEntityTable("RIS_NOTIFICATION_ADDRESSEE",
                new RefColumn("HOUSE_ID", "RIS_NOTIF_ADDR_HOUSE", "RIS_HOUSE", "ID"),
                new RefColumn("NOTIFICATION_ID", "RIS_NOTIF_ADDR_NOTIF", "RIS_NOTIFICATION", "ID"));

            this.Database.AddEntityTable("RIS_NOTIFICATION_ATTACHMENT",
                new RefColumn("ATTACHMENT_ID", "RIS_NOTIF_ATT_ATTACH", "RIS_ATTACHMENT", "ID"),
                new RefColumn("NOTIFICATION_ID", "RIS_NOTIF_ATT_NOTIF", "RIS_NOTIFICATION", "ID"));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_NOTIFICATION_ATTACHMENT");
            this.Database.RemoveTable("RIS_NOTIFICATION_ADDRESSEE");
            this.Database.RemoveTable("RIS_NOTIFICATION");
        }
    }
}
