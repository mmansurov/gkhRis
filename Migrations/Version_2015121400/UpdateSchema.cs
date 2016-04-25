namespace Bars.Gkh.Ris.Migrations.Version_2015121400
{
    using System.Data;
    using B4.Modules.NH.Migrations.DatabaseExtensions;
    using global::Bars.B4.Modules.Ecm7.Framework;

    [global::Bars.B4.Modules.Ecm7.Framework.Migration("2015121400")]
    [global::Bars.B4.Modules.Ecm7.Framework.MigrationDependsOn(typeof(global::Bars.Gkh.Ris.Migrations.Version_2015121200.UpdateSchema))]
    public class UpdateSchema : global::Bars.B4.Modules.Ecm7.Framework.Migration
    {
        public override void Up()
        {
            if (!this.Database.ColumnExists("RIS_HOUSE", "ADRESS"))
            {
                this.Database.AddColumn("RIS_HOUSE", new Column("ADRESS", DbType.String, 500));
            }

            this.Database.AddRisEntityTable("RIS_RKI_ITEM",
                new Column("NAME", DbType.String, 200),
                new Column("BASECODE", DbType.String, 200),
                new Column("BASEGUID", DbType.String, 200),
                new Column("ENDMANAGMENTDATE", DbType.DateTime),
                new Column("INDEFINITEMANAGEMENT", DbType.Boolean),
                new RefColumn("CONTRAGENT_ID", "RIS_RKI_ITEM_CONTRAGENT", "RIS_CONTRAGENT", "ID"),
                new Column("MUNICIPALITIES", DbType.Boolean),
                new Column("TYPECODE", DbType.String, 200),
                new Column("TYPEGUID", DbType.String, 200),
                new Column("WATERINTAKECODE", DbType.String, 200),
                new Column("WATERINTAKEGUID", DbType.String, 200),
                new Column("ESUBSTATIONCODE", DbType.String, 200),
                new Column("ESUBSTATIONGUID", DbType.String, 200),
                new Column("POWERPLANTCODE", DbType.String, 200),
                new Column("POWERPLANTGUID", DbType.String, 200),
                new Column("FUELCODE", DbType.String, 200),
                new Column("FUELGUID", DbType.String, 200),
                new Column("GASNETWORKCODE", DbType.String, 200),
                new Column("GASNETWORKGUID", DbType.String, 200),
                new RefColumn("HOUSE_ID", "RIS_RKI_ITEM_HOUSE", "RIS_HOUSE", "ID"),
                new Column("OKTMOCODE", DbType.String, 200),
                new Column("OKTMONAME", DbType.String, 200),
                new Column("INDEPENDENTSOURCE", DbType.Boolean),
                new Column("DETERIORATION", DbType.Decimal),
                new Column("COUNTACCIDENTS", DbType.Int32),
                new Column("ADDINFO", DbType.String, 2000));

           this.Database.AddEntityTable("RIS_ATTACHMENTS_ENERGYEFFICIENCY",
                new RefColumn("RKIITEM_ID", "RIS_ATCHMS_ENEFFIC_ITEM", "RIS_RKI_ITEM", "ID"),
                new RefColumn("ATTACHMENT_ID", "RIS_ATCHMS_ENEFFI_ATTACH", "RIS_ATTACHMENT", "ID"));

            this.Database.AddEntityTable("RIS_RKI_ATTACHMENT",
                new RefColumn("RKIITEM_ID", "RIS_RKI_ATTACHMENT_ITEM", "RIS_RKI_ITEM", "ID"),
                new RefColumn("ATTACHMENT_ID", "RIS_RKI_ATTACHMENT_ATTACH", "RIS_ATTACHMENT", "ID"));

            this.Database.AddRisEntityTable("RIS_RECEIVER_OKI",
                new RefColumn("RKIITEM_ID", "RIS_RCVR_OKI_ITEM", "RIS_RKI_ITEM", "ID"),
                new Column("RECEIVEROKI", DbType.String, 200));

            this.Database.AddRisEntityTable("RIS_SOURCE_OKI",
                new RefColumn("RKIITEM_ID", "RIS_SOURCE_OKI_ITEM", "RIS_RKI_ITEM", "ID"),
                new Column("SOURCEOKI", DbType.String, 200));

            this.Database.AddRisEntityTable("RIS_RESOURCE",
                new RefColumn("RKIITEM_ID", "RIS_RESOURCE_ITEM", "RIS_RKI_ITEM", "ID"),
                new Column("MUNICIPALRESOURCECODE", DbType.String, 200),
                new Column("MUNICIPALRESOURCEGUID", DbType.String, 200),
                new Column("MUNICIPALRESOURCENAME", DbType.String, 200),
                new Column("TOTALLOAD", DbType.Decimal),
                new Column("INDUSTRIALLOAD", DbType.Decimal),
                new Column("SOCIALLOAD", DbType.Decimal),
                new Column("POPULATIONLOAD", DbType.Decimal),
                new Column("SETPOWER", DbType.Decimal),
                new Column("SITINGPOWER", DbType.Decimal));

            this.Database.AddRisEntityTable("RIS_NET_PIECES",
                new RefColumn("RKIITEM_ID", "RIS_NET_PIECES_ITEM", "RIS_RKI_ITEM", "ID"),
                new Column("NAME", DbType.String, 200),
                new Column("DIAMETER", DbType.Decimal),
                new Column("LENGTH", DbType.Decimal),
                new Column("NEEDREPLACED", DbType.Decimal),
                new Column("WEAROUT", DbType.Decimal),
                new Column("PRESSURECODE", DbType.String, 200),
                new Column("PRESSUREGUID", DbType.String, 200),
                new Column("PRESSURENAME", DbType.String, 200),
                new Column("VOLTAGECODE", DbType.String, 200),
                new Column("VOLTAGEGUID", DbType.String, 200),
                new Column("VOLTAGENAME", DbType.String, 200));

            this.Database.AddRisEntityTable("RIS_TRANSPORTATION_RESOURCES",
                new RefColumn("RKIITEM_ID", "RIS_TRANSPORT_RESS_ITEM", "RIS_RKI_ITEM", "ID"),
                new Column("MUNICIPALRESOURCECODE", DbType.String, 200),
                new Column("MUNICIPALRESOURCEGUID", DbType.String, 200),
                new Column("MUNICIPALRESOURCENAME", DbType.String, 200),
                new Column("TOTALLOAD", DbType.Decimal),
                new Column("INDUSTRIALLOAD", DbType.Decimal),
                new Column("SOCIALLOAD", DbType.Decimal),
                new Column("POPULATIONLOAD", DbType.Decimal),
                new Column("VOLUMELOSSES", DbType.Decimal),
                new Column("COOLANTCODE", DbType.String, 200),
                new Column("COOLANTGUID", DbType.String, 200),
                new Column("COOLANTNAME", DbType.String, 200));
        }

        public override void Down()
        {
            if (this.Database.ColumnExists("RIS_HOUSE", "ADRESS"))
            {
                this.Database.RemoveColumn("RIS_HOUSE", "ADRESS");
            }

            this.Database.RemoveTable("RIS_TRANSPORTATION_RESOURCES");
            this.Database.RemoveTable("RIS_NET_PIECES");
            this.Database.RemoveTable("RIS_RESOURCE");
            this.Database.RemoveTable("RIS_SOURCE_OKI");
            this.Database.RemoveTable("RIS_RECEIVER_OKI");
            this.Database.RemoveTable("RIS_RKI_ATTACHMENT");
            this.Database.RemoveTable("RIS_ATTACHMENTS_ENERGYEFFICIENCY");
            this.Database.RemoveTable("RIS_RKI_ITEM");
        }
    }
}
