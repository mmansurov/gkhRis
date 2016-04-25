namespace Bars.Gkh.Ris.ExecutionAction
{
    using Bars.B4;
    using Bars.B4.IoC;
    using Bars.Gkh.Config;
    using Bars.Gkh.ExecutionAction;
    using Bars.Gkh.Ris.ConfigSections;
    using Bars.Gkh.Ris.Enums;
    using Castle.Windsor;
    using System;

    using Bars.B4.Utils;
    using Bars.Gkh.Config.Impl.Internal;

    /// <summary>
    /// Действие создания настроек интеграции с ГИС
    /// </summary>
    public class GisIntegrationConfigCreateAction : IExecutionAction
    {
        /// <summary>
        /// Контейнер IoC
        /// </summary>
        public IWindsorContainer Container { get; set; }
        
        /// <summary>
        /// Код
        /// </summary>
        public static string Code
        {
            get
            {
                return "GisIntegrationConfigCreateAction";
            }
        }

        /// <summary>
        /// Код
        /// </summary>
        string IExecutionAction.Code
        {
            get
            {
                return Code;
            }
        }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name
        {
            get
            {
                return "Сброс настроек сервисов интеграции с ГИС РФ";
            }
        }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description
        {
            get
            {
                return "Сброс настроек сервисов интеграции с ГИС РФ";
            }
        }

        /// <summary>
        /// Действие
        /// </summary>
        public Func<BaseDataResult> Action
        {
            get
            {
                return this.Migration;
            }
        }

        /// <summary>
        /// Метод миграции
        /// </summary>
        /// <returns>результат</returns>
        public BaseDataResult Migration()
        {
            var config = this.Container.Resolve<IGkhConfigProvider>();

            using (this.Container.Using(config))
            {
                var gisIntegrationConfig = config.Get<GisIntegrationConfig>();

                gisIntegrationConfig.ServiceSettingConfigs.Clear();

                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.Bills,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-bills-service/services/Bills",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-bills-service/services/BillsAsync",
                    Name = IntegrationService.Bills.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.DeviceMetering,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-device-metering-service/services/DeviceMetering",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-device-metering-service/services/DeviceMeteringAsync",
                    Name = IntegrationService.DeviceMetering.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.HouseManagement,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-home-management-service/services/HomeManagement",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-home-management-service/services/HomeManagementAsync",
                    Name = IntegrationService.HouseManagement.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.Nsi,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-nsi-service/services/Nsi",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-nsi-service/services/NsiAsync",
                    Name = IntegrationService.Nsi.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.NsiCommon,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-nsi-common-service/services/NsiCommon",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-nsi-common-service/services/NsiCommonAsync",
                    Name = IntegrationService.NsiCommon.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.Organization,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-organization-service/services/Organization",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-organization-service/services/OrganizationAsync",
                    Name = IntegrationService.Organization.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.OrgRegistry,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-org-registry-service/services/OrgRegistry",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-org-registry-service/services/OrgRegistryAsync",
                    Name = IntegrationService.OrgRegistry.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.OrgRegistryCommon,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-org-registry-common-service/services/OrgRegistryCommon",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-org-registry-common-service/services/OrgRegistryCommonAsync",
                    Name = IntegrationService.OrgRegistryCommon.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.Inspection,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-inspection-service/services/Inspection",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-inspection-service/services/InspectionAsync",
                    Name = IntegrationService.Inspection.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.Infrastructure,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-rki-service/services/Infrastructure",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-rki-service/services/InfrastructureAsync",
                    Name = IntegrationService.Infrastructure.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.Licenses,
                    ServiceAddress = "",
                    AsyncServiceAddress = "",
                    Name = IntegrationService.Licenses.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.Payment,
                    ServiceAddress = "http://127.0.0.1:8080/ext-bus-payment-service/services/Payment",
                    AsyncServiceAddress = "http://127.0.0.1:8080/ext-bus-payment-service/services/PaymentAsync",
                    Name = IntegrationService.Payment.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.Disclosure,
                    ServiceAddress = "",
                    AsyncServiceAddress = "",
                    Name = IntegrationService.Disclosure.GetDisplayName()
                });
                gisIntegrationConfig.ServiceSettingConfigs.Add(new ServiceSettingConfig
                {
                    IntegrationService = IntegrationService.Fas,
                    ServiceAddress = "",
                    AsyncServiceAddress = "",
                    Name = IntegrationService.Fas.GetDisplayName()
                });

                ValueHolder valueHolder;
                var serviceSettingConfigs = gisIntegrationConfig.ServiceSettingConfigs;
                if (config.ValueHolders.TryGetValue("GisIntegrationConfig.ServiceSettingConfigs", out valueHolder))
                {
                    valueHolder.SetValue(serviceSettingConfigs);
                    valueHolder.IsModified = true;
                    config.SaveChanges();
                }
            }

            return new BaseDataResult();
        }
    }
}
