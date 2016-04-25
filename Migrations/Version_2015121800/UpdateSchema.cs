namespace Bars.Gkh.Ris.Migrations.Version_2015121800
{
    using System.Data;
    using B4.Modules.NH.Migrations.DatabaseExtensions;
    using B4.Modules.Ecm7.Framework;

    [Migration("2015121800")]
    [MigrationDependsOn(typeof(Version_2015121700.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        public override void Up()
        {
            this.Database.AddRisEntityTable("RIS_SERVICE_TYPE",
                 new RefColumn("GISDICTREF_ID", "RIS_ST_DICTREF", "RIS_INTEGR_REF_DICT", "ID"));

            this.Database.AddRisEntityTable("RIS_WORKLIST",
                new RefColumn("CONTRACT_ID", "RIS_WORKLIST_CONTRACT", "RIS_CONTRACT", "ID"),
                new RefColumn("HOUSE_ID", "RIS_WORKLIST_HOUSE", "RIS_HOUSE", "ID"),
                new Column("MONTH_FROM", DbType.Int32),
                new Column("YEAR_FROM", DbType.Int16),
                new Column("MONTH_TO", DbType.Int32),
                new Column("YEAR_TO", DbType.Int16));

            this.Database.AddRisEntityTable("RIS_WORKLIST_ITEM",
                new RefColumn("WORKLIST_ID", "RIS_ITEM_WORKLIST", "RIS_WORKLIST", "ID"),
                new Column("TOTAL_COST", DbType.Decimal),
                new RefColumn("SERVICETYPE_ID", "RIS_WORKITEM_SRVT", "RIS_SERVICE_TYPE", "ID"),
                new Column("INDEX", DbType.Int16));

            this.Database.AddEntityTable("RIS_WORKLIST_ATTACHMENT",
                new RefColumn("WORKLIST_ID", "RIS_ATTACH_WORKLIST", "RIS_WORKLIST", "ID"),
                new RefColumn("ATTACHMENT_ID", "RIS_WORKITEM_ATTACH", "RIS_ATTACHMENT", "ID"));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_WORKLIST_ATTACHMENT");
            this.Database.RemoveTable("RIS_WORKLIST_ITEM");
            this.Database.RemoveTable("RIS_WORKLIST");
            this.Database.RemoveTable("RIS_SERVICE_TYPE");
        }
    }
}