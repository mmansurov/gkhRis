namespace Bars.Gkh.Ris.Migrations.Version_2015101600
{
    using System.Data;
    using global::Bars.B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015101600")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015101200.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            this.Database.AddPersistentObjectTable("RIS_CONTAINER",
                new RefColumn("CONTRAGENT_ID", "RIS_EXAM_CONTRAGENT", "RIS_CONTRAGENT", "ID"),
                new Column("CONTAINER_DATE", DbType.DateTime, ColumnProperty.NotNull));

            this.Database.AddRisEntityTable("RIS_HOUSE",
                new Column("HOUSE_TYPE", DbType.Int16, ColumnProperty.NotNull, "10"),
                new Column("FIASHOUSEGUID", DbType.String, 50),
                new Column("CADASTRALNUMBER", DbType.String, 50),
                new Column("TOTALSQUARE", DbType.Decimal),
                new Column("STATE_CODE", DbType.String, 50),
                new Column("STATE_GUID", DbType.String, 50),
                new Column("PROJECTSERIES", DbType.String, 50),
                new Column("PROJECTTYPE_CODE", DbType.String, 50),
                new Column("PROJECTTYPE_GUID", DbType.String, 50),
                new Column("BUILDINGYEAR", DbType.Int16),
                new Column("TOTALWEAR", DbType.Decimal),
                new Column("ENERGYDATE", DbType.DateTime),
                new Column("ENERGYCATEGORY_CODE", DbType.String, 50),
                new Column("ENERGYCATEGORY_GUID", DbType.String, 50),
                new Column("OKTMO_CODE", DbType.String, 50),
                new Column("OKTMO_NAME", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_CADASTRALNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_INVENTORYNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_CONDITIONALNUMBER", DbType.String, 50),
                new Column("OLSONTZ_CODE", DbType.String, 50),
                new Column("OLSONTZ_GUID", DbType.String, 50),
                new Column("BUILTUPAREA", DbType.Decimal),
                new Column("MINFLOORCOUNT", DbType.Int32),
                new Column("OVERHAULYEAR", DbType.Int16),
                new Column("OVERHAULFORMING_CODE", DbType.String, 50),
                new Column("OVERHAULFORMING_GUID", DbType.String, 50),
                new Column("HOUSEMANAGEMENTTYPE_CODE", DbType.String, 50),
                new Column("HOUSEMANAGEMENTTYPE_GUID", DbType.String, 50),
                new Column("RESIDENTIALHOUSETYPE_CODE", DbType.String, 50),
                new Column("RESIDENTIALHOUSETYPE_GUID", DbType.String, 50));

            this.Database.AddEntityTable("RIS_ATTACHMENT",
                new RefColumn("FILE_INFO_ID", "RIS_ATTACH_FILE_INFO", "B4_FILE_INFO", "ID"),
                new Column("GUID", DbType.String, 50),
                new Column("HASH", DbType.String, 200));

            this.Database.AddEntityTable("RIS_CONFIRMDOC",
                new RefColumn("HOUSE_ID", "RIS_CONFDOC_HOUSE", "RIS_HOUSE", "ID"),
                new Column("NAME", DbType.String, 50),
                new Column("DESCRIPTION", DbType.String, 50),
                new RefColumn("ATTACHMENT_ID", "RIS_CONFDOC_ATTACH", "RIS_ATTACHMENT", "ID"));

            this.Database.AddRisEntityTable("RIS_ENTRANCE",
                new RefColumn("HOUSE_ID", "RIS_ENTRANCE_HOUSE", "RIS_HOUSE", "ID"),
                new Column("ENTRANCENUM", DbType.Int16),
                new Column("STOREYSCOUNT", DbType.Int16),
                new Column("CREATIONDATE", DbType.DateTime),
                new Column("TERMINATIONDATE", DbType.DateTime));

            this.Database.AddRisEntityTable("RIS_NONRESIDENTIALPREMISES",
                new RefColumn("HOUSE_ID", "RIS_NRESPREM_HOUSE", "RIS_HOUSE", "ID"),
                new Column("PREMISESNUM", DbType.String, 50),
                new Column("CADASTRALNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_CADASTRALNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_INVENTORYNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_CONDITIONALNUMBER", DbType.String, 50),
                new Column("TERMINATIONDATE", DbType.DateTime),
                new Column("PURPOSE_CODE", DbType.String, 50),
                new Column("PURPOSE_GUID", DbType.String, 50),
                new Column("POSITION_CODE", DbType.String, 50),
                new Column("POSITION_GUID", DbType.String, 50),
                new Column("GROSSAREA", DbType.Decimal));

            this.Database.AddRisEntityTable("RIS_RESIDENTIALPREMISES",
                new RefColumn("HOUSE_ID", "RIS_RESPREM_HOUSE", "RIS_HOUSE", "ID"),
                new Column("PREMISESNUM", DbType.String, 50),
                new Column("CADASTRALNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_CADASTRALNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_INVENTORYNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_CONDITIONALNUMBER", DbType.String, 50),
                new Column("TERMINATIONDATE", DbType.DateTime),
                new Column("EntranceNum", DbType.Int16),
                new Column("PREMISESCHARACTERISTIC_CODE", DbType.String, 50),
                new Column("PREMISESCHARACTERISTIC_GUID", DbType.String, 50),
                new Column("ROOMSNUM_CODE", DbType.String, 50),
                new Column("ROOMSNUM_GUID", DbType.String, 50),
                new Column("RESIDENTIALHOUSETYPE_CODE", DbType.String, 50),
                new Column("RESIDENTIALHOUSETYPE_GUID", DbType.String, 50),
                new Column("GROSSAREA", DbType.Decimal));

            this.Database.AddRisEntityTable("RIS_LIVINGROOM",
                new RefColumn("RES_PREMISES_ID", "RIS_LIVROOM_PREMISES", "RIS_RESIDENTIALPREMISES", "ID"),
                new RefColumn("HOUSE_ID", "RIS_LIVROOM_HOUSE", "RIS_HOUSE", "ID"),
                new Column("ROOMNUMBER", DbType.String, 50),
                new Column("SQUARE", DbType.Decimal),
                new Column("TERMINATIONDATE", DbType.DateTime),
                new Column("CADASTRALNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_CADASTRALNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_INVENTORYNUMBER", DbType.String, 50),
                new Column("PREVSTATEREGNUMBER_CONDITIONALNUMBER", DbType.String, 50));
        }

        public override void Down()
        {
            this.Database.RemoveTable("RIS_LIVINGROOM");
            this.Database.RemoveTable("RIS_RESIDENTIALPREMISES");
            this.Database.RemoveTable("RIS_NONRESIDENTIALPREMISES");
            this.Database.RemoveTable("RIS_ENTRANCE");
            this.Database.RemoveTable("RIS_CONFIRMDOC");
            this.Database.RemoveTable("RIS_ATTACHMENT");
            this.Database.RemoveTable("RIS_HOUSE");
            this.Database.RemoveTable("RIS_CONTAINER");
        }
    }
}