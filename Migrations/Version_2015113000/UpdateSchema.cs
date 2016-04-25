namespace Bars.Gkh.Ris.Migrations.Version_2015113000
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015113000")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015112600.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddRisEntityTable("RIS_CONTRACT",
                new Column("DOCNUM", DbType.String, 200),
                new Column("SIGNINGDATE", DbType.DateTime),
                new Column("EFFECTIVEDATE", DbType.DateTime),
                new Column("PLANDATECOMPTETION", DbType.DateTime),
                new Column("VALIDITY_MONTH", DbType.Int32),
                new Column("VALIDITY_YEAR", DbType.Int32),
                new Column("OWNERS_TYPE", DbType.Int16),
                new RefColumn("ORG_ID", "RIS_CONTRACT_ORG", "RIS_CONTRAGENT", "ID"),
                new Column("PROTOCOL_PURCHASENUMBER", DbType.String, 200),
                new Column("CONTRACTBASE_CODE", DbType.String, 200),
                new Column("CONTRACTBASE_GUID", DbType.String, 200));

            this.Database.AddEntityTable("RIS_PROTOCOLOK",
               new RefColumn("ATTACHMENT_ID", "RIS_PROTOCOLOK_ATTACH", "RIS_ATTACHMENT", "ID"),
               new RefColumn("CONTRACT_ID", "RIS_PROTOCOLOK_CONTR", "RIS_CONTRACT", "ID"));


            this.Database.AddEntityTable("RIS_AGREEMENT",
                new RefColumn("ATTACHMENT_ID", "RIS_AGREEMENT_ATTACH", "RIS_ATTACHMENT", "ID"),
                new RefColumn("CONTRACT_ID", "RIS_AGREEMENT_CONTR", "RIS_CONTRACT", "ID"),
                new Column("AGREEMENTDATE", DbType.DateTime),
                new Column("AGREEMENTNUMBER", DbType.String, 200));

            if (!this.Database.ColumnExists("RIS_ADDSERVICE", "BASESERVISE_AGREEMENT_ID"))
            {
                this.Database.AddColumn("RIS_ADDSERVICE", new RefColumn("BASESERVISE_AGREEMENT_ID", "RIS_ADDSERV_AGR", "RIS_AGREEMENT", "ID"));
            }

            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "CONTRACT_ID"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new RefColumn("CONTRACT_ID", "RIS_CONTR_CONTR", "RIS_CONTRACT", "ID"));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "STATUSOBJECT"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new Column("STATUSOBJECT", DbType.Int16));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "BASEEXCLUSION_AGREEMENT_ID"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new RefColumn("BASEEXCLUSION_AGREEMENT_ID", "RIS_CONTR_EXCLAGR", "RIS_AGREEMENT", "ID"));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "BASEMSERVISE_AGREEMENT_ID"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new RefColumn("BASEMSERVISE_AGREEMENT_ID", "RIS_CONTR_MSERVAGR", "RIS_AGREEMENT", "ID"));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PERIODMETERING_STARTDATE"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new Column("PERIODMETERING_STARTDATE", DbType.Int32));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PERIODMETERING_ENDDATE"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new Column("PERIODMETERING_ENDDATE", DbType.Int32));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PERIODMETERING_LASTDAY"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new Column("PERIODMETERING_LASTDAY", DbType.Boolean));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PAYMENTDATE_STARTDATE"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new Column("PAYMENTDATE_STARTDATE", DbType.Int32));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PAYMENTDATE_LASTDAY"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new Column("PAYMENTDATE_LASTDAY", DbType.Boolean));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PAYMENTDATE_CURRENTMOUNTH"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new Column("PAYMENTDATE_CURRENTMOUNTH", DbType.Boolean));
            }
            if (!this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PAYMENTDATE_NEXTMOUNTH"))
            {
                this.Database.AddColumn("RIS_CONTRACTOBJECT", new Column("PAYMENTDATE_NEXTMOUNTH", DbType.Boolean));
            }

            if (!this.Database.ColumnExists("RIS_PROTOCOLMEETINGOWNER", "CONTRACT_ID"))
            {
                this.Database.AddColumn("RIS_PROTOCOLMEETINGOWNER", new RefColumn("CONTRACT_ID", "RIS_PROTOCOLMEET_CONTR", "RIS_CONTRACT", "ID"));
            }

            if (!this.Database.ColumnExists("RIS_CONTRACTATTACHMENT", "CONTRACT_ID"))
            {
                this.Database.AddColumn("RIS_CONTRACTATTACHMENT", new RefColumn("CONTRACT_ID", "RIS_CONTRACTATT_CONTR", "RIS_CONTRACT", "ID"));
            }

            if (!this.Database.ColumnExists("RIS_HOUSESERVICE", "BASESERVISE_AGREEMENT_ID"))
            {
                this.Database.AddColumn("RIS_HOUSESERVICE", new RefColumn("BASESERVISE_AGREEMENT_ID", "RIS_HOUSESERV_AGR", "RIS_AGREEMENT", "ID"));
            }
        }

        public override void Down()
        {
            if (this.Database.ColumnExists("RIS_ADDSERVICE", "BASESERVISE_AGREEMENT_ID"))
            {
                this.Database.RemoveColumn("RIS_ADDSERVICE", "BASESERVISE_AGREEMENT_ID");
            }

            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "CONTRACT_ID"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "CONTRACT_ID");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "STATUSOBJECT"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "STATUSOBJECT");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "BASEEXCLUSION_AGREEMENT_ID"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "BASEEXCLUSION_AGREEMENT_ID");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "BASEMSERVISE_AGREEMENT_ID"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "BASEMSERVISE_AGREEMENT_ID");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PERIODMETERING_STARTDATE"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "PERIODMETERING_STARTDATE");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PERIODMETERING_ENDDATE"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "PERIODMETERING_ENDDATE");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PERIODMETERING_LASTDAY"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "PERIODMETERING_LASTDAY");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PAYMENTDATE_STARTDATE"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "PAYMENTDATE_STARTDATE");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PAYMENTDATE_LASTDAY"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "PAYMENTDATE_LASTDAY");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PAYMENTDATE_CURRENTMOUNTH"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "PAYMENTDATE_CURRENTMOUNTH");
            }
            if (this.Database.ColumnExists("RIS_CONTRACTOBJECT", "PAYMENTDATE_NEXTMOUNTH"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTOBJECT", "PAYMENTDATE_NEXTMOUNTH");
            }

            if (this.Database.ColumnExists("RIS_PROTOCOLMEETINGOWNER", "CONTRACT_ID"))
            {
                this.Database.RemoveColumn("RIS_PROTOCOLMEETINGOWNER", "CONTRACT_ID");
            }

            if (this.Database.ColumnExists("RIS_CONTRACTATTACHMENT", "CONTRACT_ID"))
            {
                this.Database.RemoveColumn("RIS_CONTRACTATTACHMENT", "CONTRACT_ID");
            }

            if (this.Database.ColumnExists("RIS_HOUSESERVICE", "BASESERVISE_AGREEMENT_ID"))
            {
                this.Database.RemoveColumn("RIS_HOUSESERVICE", "BASESERVISE_AGREEMENT_ID");
            }

            this.Database.RemoveTable("RIS_AGREEMENT");
            this.Database.RemoveTable("RIS_PROTOCOLOK");
            this.Database.RemoveTable("RIS_CONTRACT");
        }
    }
}