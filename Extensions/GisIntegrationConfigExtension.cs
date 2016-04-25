namespace Bars.Gkh.Ris.Extensions
{
    using Bars.Gkh.Ris.ConfigSections;
    using Bars.Gkh.Ris.Enums;
    using System.Linq;

    /// <summary>
    /// Расширения для получения значений из конфигурации
    /// </summary>
    public static class GisIntegrationConfigExtension
    {
        /// <summary>
        /// Получить адрес сервиса
        /// </summary>
        /// <param name="config">Конифгурация</param>
        /// <param name="integrationService">Тип сервиса</param>
        /// <param name="isAsync">Является ли сервис асинхронным</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Адрес сервиса</returns>
        public static string GetServiceAddress(this GisIntegrationConfig config, IntegrationService integrationService, bool isAsync, string defaultValue = null)
        {
            var result = defaultValue;

            var serviceConfig = config.ServiceSettingConfigs.FirstOrDefault(x => x.IntegrationService == integrationService);

            if (serviceConfig != null)
            {
                result = isAsync ? serviceConfig.AsyncServiceAddress : serviceConfig.ServiceAddress;
            }

            return result;
        }
    }
}
