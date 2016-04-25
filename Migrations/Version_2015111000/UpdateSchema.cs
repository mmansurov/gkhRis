namespace Bars.Gkh.Ris.Migrations.Version_2015111000
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015111000")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015110701.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.RemoveColumn("RIS_PROTOCOLMEETINGOWNER", "NAME");
            Database.RemoveColumn("RIS_PROTOCOLMEETINGOWNER", "DESCRIPTION");

            Database.RemoveColumn("RIS_CHARTER", "HEAD_SURNAME");
            Database.RemoveColumn("RIS_CHARTER", "HEAD_FIRSTNAME");
            Database.RemoveColumn("RIS_CHARTER", "HEAD_PATRONYMIC");
            Database.RemoveColumn("RIS_CHARTER", "HEAD_GENDER");
            Database.RemoveColumn("RIS_CHARTER", "HEAD_DATEOFBIRTH");
            Database.AddRefColumn("RIS_CHARTER", new RefColumn("HEAD_ID", "RIS_CHARTER_HEAD_IND", "RIS_IND", "ID"));

            Database.ChangeColumn("RIS_INSPECTION_EXAMINATION", new RefColumn("PLAN_ID", ColumnProperty.Null, "INSP_EXAM_PLAN", "RIS_INSPECTION_PLAN", "ID"));
        }

        public override void Down()
        {
            Database.RemoveColumn("RIS_CHARTER", "HEAD_ID");
            Database.AddColumn("RIS_CHARTER", new Column("HEAD_DATEOFBIRTH", DbType.DateTime));
            Database.AddColumn("RIS_CHARTER", new Column("HEAD_GENDER", DbType.Int16, ColumnProperty.NotNull, "0"));
            Database.AddColumn("RIS_CHARTER", new Column("HEAD_PATRONYMIC", DbType.String, 50));
            Database.AddColumn("RIS_CHARTER", new Column("HEAD_FIRSTNAME", DbType.String, 50));
            Database.AddColumn("RIS_CHARTER", new Column("HEAD_SURNAME", DbType.String, 50));

            Database.AddColumn("RIS_PROTOCOLMEETINGOWNER", new Column("DESCRIPTION", DbType.String, 50));
            Database.AddColumn("RIS_PROTOCOLMEETINGOWNER", new Column("NAME", DbType.String, 50));
        }
    }
}