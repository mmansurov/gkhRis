namespace Bars.Gkh.Ris.Migrations.Version_2015101200
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015101200")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015091000.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            Database.AddColumn("RIS_CONTRAGENT", "FACT_ADDRESS", DbType.String, 500);
            Database.AddColumn("RIS_CONTRAGENT", "JUR_ADDRESS", DbType.String, 500);
            Database.AddColumn("RIS_CONTRAGENT", new Column("IS_INDIVIDUAL", DbType.Boolean));

            Database.AddRisEntityTable("RIS_INSPECTION_PLAN",
                new Column("YEAR", DbType.Int16),
                new Column("APPROVAL_DATE", DbType.DateTime));

            Database.AddRisEntityTable("RIS_INSPECTION_ORDER",
                new Column("NUMBER", DbType.String, 50),
                new Column("DATE", DbType.DateTime));

            Database.AddRisEntityTable("RIS_INSPECTION_EXAMINATION",
                new RefColumn("PLAN_ID", "INSP_EXAM_PLAN", "RIS_INSPECTION_PLAN", "ID"),
                new Column("INSPECTIONNUMBER", DbType.String),
                new RefColumn("CONTRAGENT_ID", "EXAM_CONTRAGENT", "RIS_CONTRAGENT", "ID"),
                new Column("BASE_CODE", DbType.String, 10),
                new Column("BASE_GUID", DbType.String, 50),
                new Column("COUNT_DAYS", DbType.Double),
                new Column("EXAMFORM_CODE", DbType.String, 50),
                new Column("EXAMFORM_GUID", DbType.String, 50),
                new Column("IS_SCHEDULED", DbType.Boolean),
                new Column("LASTNAME", DbType.String, 50),
                new Column("FIRSTNAME", DbType.String, 50),
                new Column("MIDDLENAME", DbType.String, 50),
                new Column("OVERSIGHT_ACT_CODE", DbType.String, 50),
                new Column("OVERSIGHT_ACT_GUID", DbType.String, 50),
                new Column("OBJECTIVE", DbType.String, 200),
                new Column("DATE_FROM", DbType.DateTime),
                new Column("DATE_TO", DbType.DateTime),
                new Column("DURATION", DbType.Double),
                new Column("OBJECT_CODE", DbType.String, 50),
                new Column("OBJECT_GUID", DbType.String, 50),
                new Column("TASKS", DbType.String, 200),
                new Column("ORDER_GKH_ID", DbType.Int64),
                new Column("ORDER_NUMBER", DbType.String, 50),
                new Column("ORDER_DATE", DbType.DateTime, 50),
                new Column("IS_PHYS_PERSON", DbType.Boolean));

            Database.AddRisEntityTable("RIS_INSPECTION_PRECEPT",
                new RefColumn("EXAMINATION_ID", "RIS_INSP_PREC_EXAM", "RIS_INSPECTION_EXAMINATION", "ID"),
                new Column("NUMBER", DbType.String, 50),
                new Column("DATE", DbType.DateTime),
                new Column("CANCEL_REASON", DbType.String, 200),
                new Column("CANCEL_DATE", DbType.DateTime));

            Database.AddRisEntityTable("RIS_INSPECTION_OFFENCE",
                new RefColumn("EXAMINATION_ID", "RIS_INSP_OFFENCE_EXAM", "RIS_INSPECTION_EXAMINATION", "ID"),
                new Column("NUMBER", DbType.String, 50),
                new Column("DATE", DbType.DateTime));
        }

        public override void Down()
        {
            Database.RemoveTable("RIS_INSPECTION_OFFENCE");

            Database.RemoveTable("RIS_INSPECTION_PRECEPT");

            Database.RemoveTable("RIS_INSPECTION_EXAMINATION");

            Database.RemoveTable("RIS_INSPECTION_ORDER");

            Database.RemoveTable("RIS_INSPECTION_PLAN");

            Database.RemoveColumn("RIS_CONTRAGENT", "IS_INDIVIDUAL");
            Database.RemoveColumn("RIS_CONTRAGENT", "JUR_ADDRESS");
            Database.RemoveColumn("RIS_CONTRAGENT", "FACT_ADDRESS");
        }
    }
}