namespace Bars.Gkh.Ris.Migrations.Version_2015112500
{
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;
    using System.Data;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015112500")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015112400.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddRisEntityTable("RIS_VOTINGPROTOCOL",
                new RefColumn("HOUSE_ID", "RIS_VOTINGPROT_HOUSE", "RIS_HOUSE", "ID"),
                new Column("PROTOCOLNUM", DbType.String, 200),
                new Column("PROTOCOLDATE", DbType.DateTime),
                new Column("VOTINGPLACE", DbType.String, 200),
                new Column("ENDDATE", DbType.DateTime),
                new Column("BEGINDATE", DbType.DateTime),
                new Column("DISCIPLINE", DbType.String, 200),
                new Column("MEETINGELIGIBILITY", DbType.Int16),
                new Column("VOTINGTYPE", DbType.Int16),
                new Column("VOTINGTIMETYPE", DbType.Int16),
                new Column("REVERT", DbType.Boolean),
                new Column("PLACE", DbType.Boolean));

            this.Database.AddEntityTable("RIS_VOTINGPROTOCOL_ATTACHMENT",
                new RefColumn("ATTACHMENT_ID", "RIS_VOTINGPROT_ATTACH", "RIS_ATTACHMENT", "ID"),
                new RefColumn("VOTINGPROTOCOL_ID", "RIS_VOTINGPROT_VOTINGPROT", "RIS_VOTINGPROTOCOL", "ID"));

            this.Database.AddRisEntityTable("RIS_DECISIONLIST",
               new RefColumn("VOTINGPROTOCOL_ID", "RIS_DECLIST_VOTINGPROT", "RIS_VOTINGPROTOCOL", "ID"),
               new Column("QUESTIONNUMBER", DbType.Int32),
               new Column("QUESTIONNAME", DbType.String, 200),
               new Column("DECISIONSTYPE_CODE", DbType.String, 50),
               new Column("DECISIONSTYPE_GUID", DbType.String, 50),
               new Column("AGREE", DbType.Decimal),
               new Column("AGAINST", DbType.Decimal),
               new Column("ABSTENT", DbType.Decimal),
               new Column("VOTINGRESUME", DbType.Int16));

            this.Database.AddRisEntityTable("RIS_VOTEINITIATORS",
                new RefColumn("IND_ID", "RIS_VOTEINITS_IND", "RIS_IND", "ID"),
                new RefColumn("ORG_ID", "RIS_VOTEINITS_ORG", "RIS_CONTRAGENT", "ID"),
                new RefColumn("VOTINGPROTOCOL_ID", "RIS_VOTEINITS_VOTINGPROT", "RIS_VOTINGPROTOCOL", "ID"));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_VOTEINITIATORS");
            this.Database.RemoveTable("RIS_DECISIONLIST");
            this.Database.RemoveTable("RIS_VOTINGPROTOCOL_ATTACHMENT");
            this.Database.RemoveTable("RIS_VOTINGPROTOCOL");
        }
    }
}
