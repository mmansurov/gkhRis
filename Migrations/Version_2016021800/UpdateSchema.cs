namespace Bars.Gkh.Ris.Migrations.Version_2016021800
{
    using B4.Modules.Ecm7.Framework;
    using System.Collections.Generic;
    using B4.Modules.NH.Migrations.DatabaseExtensions;

    /// <summary>
    /// Миграция модуля
    /// </summary>
    [Migration("2016021800")]
    [MigrationDependsOn(typeof(Version_2016021600.UpdateSchema))]
    public class UpdateSchema : Migration
    {
        private readonly List<string> tableNames = new List<string>
        {
            "RIS_WORKLIST", "RIS_WORKLIST_ITEM", "RIS_HOUSESERVICE", "RIS_NOTIFORDEREXECUT", "RIS_MUNICIPAL_SERVICE",
            "RIS_ADDITIONAL_SERVICE", "RIS_INSPECTION_PRECEPT", "RIS_INSPECTION_OFFENCE", "RIS_INSPECTION_PLAN",
            "RIS_INSPECTION_EXAMINATION", "RIS_TRANSPORTATION_RESOURCES","RIS_SOURCE_OKI", "RIS_RKI_ITEM","RIS_RKI_ATTACHMENT",
            "RIS_RESOURCE", "RIS_RECEIVER_OKI", "RIS_NET_PIECES","RIS_ATTACHMENTS_ENERGYEFFICIENCY",
            "RIS_VOTINGPROTOCOL", "RIS_VOTEINITIATORS","RIS_SHARE", "RIS_SHAREIND", "RIS_SHAREECNBRRESIDENTPREM",
            "RIS_SHAREECNBRNONRESPREM", "RIS_SHAREECNBRLIVINGROOM","RIS_SHAREECNBRLIVINGHOUSE", "RIS_PUBLICPROPERTYCONTRACT",
            "RIS_PROTOCOLOK", "RIS_NOTIFICATION","RIS_NOTIFICATION_ATTACHMENT", "RIS_NOTIFICATION_ADDRESSEE",
            "RIS_METERING_DEVICE_LIVING_ROOM", "RIS_METERING_DEVICE_DATA","RIS_METERING_DEVICE_ACCOUNT", "RIS_IND", "RIS_HOUSE",
            "RIS_HOUSE_INNER_WALL_MATERIAL",  "RIS_ENTRANCE", "RIS_ECNBR","RIS_ECNBRIND", "RIS_DECISIONLIST", "RIS_CONTRACT",
            "RIS_CONTRACTATTACHMENT", "RIS_AGREEMENT", "RIS_ACCOUNT","RIS_RESIDENTIALPREMISES", "RIS_PROTOCOLMEETINGOWNER",
            "RIS_NONRESIDENTIALPREMISES", "RIS_LIVINGROOM","RIS_CONTRACTOBJECT", "RIS_CONFIRMDOC", "RIS_CHARTER", "RIS_ADDSERVICE",
            "RIS_METERING_DEVICE_VERIFICATION_VALUE", "RIS_METERING_DEVICE_CURRENT_VALUE","RIS_METERING_DEVICE_CONTROL_VALUE",
            "RIS_TECH_SERVICE","RIS_PAYMENT_INFO", "RIS_PAYMENT_DOC", "RIS_MUNICIPAL_SERVICE_CHARGE_INFO","RIS_HOUSING_SERVICE_CHARGE_INFO",
            "RIS_ADDRESS_INFO","RIS_ADDITIONAL_SERVICE_EXT_CHARGE_INFO", "RIS_ADDITIONAL_SERVICE_CHARGE_INFO", "RIS_PAYMENT_PERIOD",
            "RIS_SUBSIDARY","RIS_ACKNOWLEDGMENT", "HOUSE_SERVICE"};

        /// <summary>
        /// Метод миграции на версию вперед
        /// </summary>
        public override void Up()
        {
            foreach (var tableName in this.tableNames)
            {
                this.AddContragentColumn(tableName);
            }
        }

        /// <summary>
        /// Метод миграции на версию назад
        /// </summary>
        public override void Down()
        {
            foreach (var tableName in this.tableNames)
            {
                this.RemoveContragentColumn(tableName);
            }
        }

        private void AddContragentColumn(string tableName)
        {
            if (!this.Database.ColumnExists(tableName, "RIS_CONTRAGENT_ID"))
            {
                this.Database.AddColumn(tableName, new RefColumn("RIS_CONTRAGENT_ID", string.Format("{0}_CONTRAGENT", tableName), "RIS_CONTRAGENT", "ID"));
            }
        }

        private void RemoveContragentColumn(string tableName)
        {
            if (this.Database.ColumnExists(tableName, "RIS_CONTRAGENT_ID"))
            {
                this.Database.RemoveColumn(tableName, string.Format("{0}_CONTRAGENT", tableName));
            }
        }
    }
}
