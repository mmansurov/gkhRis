namespace Bars.Gkh.Ris.ConfigSections
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Bars.Gkh.Config;
    using Bars.Gkh.Config.Attributes;
    using Bars.Gkh.Config.Attributes.UI;
    using Bars.Gkh.Gis.ConfigSections;
    using Bars.Gkh.Ris.Enums;

    /// <summary>
    /// Настройки интеграции с ГИС РФ
    /// </summary>
    [GkhConfigSection("GisIntegrationConfig", DisplayName = "Настройки интеграции с ГИС РФ", UIParent = typeof(GisConfig))]
    [Navigation]
    public class GisIntegrationConfig : IGkhConfigSection
    {
        /// <summary>
        /// Настройки сервисов
        /// </summary>
        [GkhConfigProperty(DisplayName = "Настройки сервисов", HideToolbar = true)]
        public virtual List<ServiceSettingConfig> ServiceSettingConfigs { get; set; }

        /// <summary>
        /// Подписывать сообщение
        /// </summary>
        [GkhConfigProperty(DisplayName = "Подписывать сообщение")]
        [DefaultValue(false)]
        public virtual bool SingXml { get; set; }

        /// <summary>
        /// Указывать логин/пароль
        /// </summary>
        [GkhConfigProperty(DisplayName = "Указывать логин/пароль")]
        [DefaultValue("false")]
        public virtual bool UseLoginCredentials { get; set; }

        /// <summary>
        /// Логин
        /// </summary>
        [GkhConfigProperty(DisplayName = "Логин")]
        [DefaultValue("test")]
        public virtual string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [GkhConfigProperty(DisplayName = "Пароль")]
        [DefaultValue("SDldfls4lz5@!82d")]
        public virtual string Password { get; set; }
    }

    /// <summary>
    /// Настройка сервиса
    /// </summary>
    [DisplayName(@"Настройки сервиса")]
    public class ServiceSettingConfig
    {
        /// <summary>
        /// Сервис интеграции
        /// </summary>
        [DisplayName(@"Сервис интеграции")]
        [ReadOnly(true)]
        public IntegrationService IntegrationService { get; set; }

        /// <summary>
        /// Отображаемое наименование
        /// </summary>
        [DisplayName(@"Отображаемое наименование")]
        public string Name { get; set; }

        /// <summary>
        /// Адрес сервиса
        /// </summary>
        [DisplayName(@"Адрес сервиса")]
        public string ServiceAddress { get; set; }

        /// <summary>
        /// Адрес асинхронного сервиса
        /// </summary>
        [DisplayName(@"Адрес асинхронного сервиса")]
        public string AsyncServiceAddress { get; set; }
    }
}