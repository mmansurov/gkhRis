namespace Bars.Gkh.Ris.Migrations.Version_2016020100
{
    using System.Data;
    using B4.Modules.Ecm7.Framework;
    using B4.Modules.NH.Migrations.DatabaseExtensions;

    /// <summary>
    /// Migration("2016020100")
    /// </summary>
    [Migration("2016020100")]
    [MigrationDependsOn(typeof(Version_2016012600.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        /// <summary>
        /// Накатить миграцию
        /// </summary>
        public override void Up()
        {
            if (!this.Database.ColumnExists("RIS_CONTRACT", "DRAWING_PD_DATE_THIS_MONTH"))
            {
                this.Database.AddColumn("RIS_CONTRACT", new Column("DRAWING_PD_DATE_THIS_MONTH", DbType.Boolean, ColumnProperty.NotNull, false));
            }

            this.Database.AddRisEntityTable("HOUSE_SERVICE",
                new RefColumn("CONTRACTOBJECT_ID", "HOUSSERV_CONTROBJ", "RIS_CONTRACTOBJECT", "ID"),
                new Column("SERVICETYPE_CODE", DbType.String, 50),
                new Column("SERVICETYPE_GUID", DbType.String, 50),
                new Column("STARTDATE", DbType.DateTime),
                new Column("ENDDATE", DbType.DateTime),
                new Column("BASESERVICE_CURRENTDOC", DbType.Boolean),
                new RefColumn("BASESERVICE_AGREEMENT_ID", "HOUSSERV_AGREEMENT_ID", "RIS_AGREEMENT", "ID"));

            if (!this.Database.ColumnExists("RIS_PUBLICPROPERTYCONTRACT", "PROTOCOLNUMBER"))
            {
                this.Database.AddColumn("RIS_PUBLICPROPERTYCONTRACT", new Column("PROTOCOLNUMBER", DbType.String, 200));
            }

            if (!this.Database.ColumnExists("RIS_PUBLICPROPERTYCONTRACT", "PROTOCOLDATE"))
            {
                this.Database.AddColumn("RIS_PUBLICPROPERTYCONTRACT", new Column("PROTOCOLDATE", DbType.DateTime));
            }
        }

        /// <summary>
        /// Откатить миграцию
        /// </summary>
        public override void Down()
        {
            if (this.Database.ColumnExists("RIS_CONTRACT", "DRAWING_PD_DATE_THIS_MONTH"))
            {
                this.Database.RemoveColumn("RIS_CONTRACT", "DRAWING_PD_DATE_THIS_MONTH");
            }

            this.Database.RemoveTable("HOUSE_SERVICE");

            if (this.Database.ColumnExists("RIS_PUBLICPROPERTYCONTRACT", "PROTOCOLNUMBER"))
            {
                this.Database.RemoveColumn("RIS_PUBLICPROPERTYCONTRACT", "PROTOCOLNUMBER");
            }

            if (this.Database.ColumnExists("RIS_PUBLICPROPERTYCONTRACT", "PROTOCOLDATE"))
            {
                this.Database.RemoveColumn("RIS_PUBLICPROPERTYCONTRACT", "PROTOCOLDATE");
            }
        }
    }
}
