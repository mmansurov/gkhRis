namespace Bars.Gkh.Ris
{
    using B4;

    public class RisPermissionMap : PermissionMap
    {
        public RisPermissionMap()
        {
            this.Namespace("Administration.OutsideSystemIntegrations", "Интеграция с внешними системами");

            this.Namespace("Administration.OutsideSystemIntegrations.Gis", "Интеграция с ГИС");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.View", "Просмотр");

            this.Namespace("Administration.OutsideSystemIntegrations.Gis.Dictions", "Справочники");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Dictions.View", "Просмотр");

            this.Namespace("Administration.OutsideSystemIntegrations.Gis.Logs", "Логи интеграции");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Logs.View", "Просмотр");

            this.Namespace("Administration.OutsideSystemIntegrations.Gis.Methods", "Методы");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.View", "Просмотр");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.ExecuteMethod", "Выполнить метод");           

            // методы
            this.Namespace("Administration.OutsideSystemIntegrations.Gis.Methods.List", "Список методов");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importOKI", "Управление ОКИ в РКИ");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importExaminations", "Импорт проверок ГЖИ");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importInspectionPlan", "Импорт планов и проверок ГЖИ");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importAdditionalServices", "Импорт записей справочника «Дополнительные услуги»");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importMunicipalService", "Импорт записей справочника «Коммунальные услуги»");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importDataProvider", "Импорт контрагента, получение senderId");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importNotificationsOfOrderExecution", "Импорт уведомлений о выполнении распоряжения");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importMSRSO", "Импорт КУ по прямым договорам с РСО");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importWorkingList", "Импорт перечня работ и услуг на период");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importVotingProtocol", "Импорт протоколов голосования");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importShareEncbrData", "Импорт данных о жилищном фонде о поставщиках информации");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importPublicPropertyContract", "Импорт договоров на пользование общим имуществом");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importNotificationData", "Импорт новостей для информирования граждан");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importMeteringDeviceData", "Импорт данных приборов учета");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importHouseUOData", "Импорт сведений о доме для полномочия УО");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importHouseRSOData", "Импорт сведений о доме для полномочия РСО");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importHouseOMSData", "Импорт данных дома для полномочия ОМС");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importContractData", "Импорт договора управления");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importAccountData", "Импорт счетов");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importMeteringDeviceValues", "Импорт проверочных показаний приборов учета, Импорт текущих показаний приборов учета, Импорт контрольных показаний приборов учета");
            // this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importMeteringDeviceValues", "Импорт текущих показаний приборов учета");
            // this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importMeteringDeviceValues", "Импорт контрольных показаний приборов учета");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.openOrgPaymentPeriod", "Открытие платежных периодов");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importPaymentDocument", "Импорт сведений о платежных документах");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.exportOrgRegistry", "Экспорт данных организации");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Methods.List.importAcknowledgment", "Импорт сведений  о квитировании");

            // экспортеры данных
            this.Namespace("Administration.OutsideSystemIntegrations.Gis.Exporters", "Экспортеры данных");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Exporters.AcknowledgmentExporter", "Экспортер сведений о квитировании");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Exporters.ContractDataExporter", "Экспорт договоров управления");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Exporters.CharterDataExporter", "Экспорт уставов");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Exporters.DataProviderExporter", "Экспортер сведений о поставщиках информации");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Exporters.HouseUODataExporter", "Экспорт сведений о доме для управляющих организаций");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Exporters.HouseOMSDataExporter", "Экспорт сведений о доме для органов местного самоуправления");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Exporters.HouseRSODataExporter", "Экспорт сведений о доме для ресурсоснабжающих организаций");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Exporters.AdditionalServicesExporter", "Экспорт данных справочника \"Дополнительные услуги\"");

            // задачи
            this.Namespace("Administration.OutsideSystemIntegrations.Gis.Tasks", "Задачи");
            this.Permission("Administration.OutsideSystemIntegrations.Gis.Tasks.View", "Просмотр");

            this.Namespace("Administration.OutsideSystemIntegrations.GisSettings", "Настройки интеграции с ГИС");
            this.Permission("Administration.OutsideSystemIntegrations.GisSettings.View", "Просмотр");
        }
    }
}