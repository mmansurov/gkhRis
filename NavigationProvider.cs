namespace Bars.Gkh.Ris
{
    using B4;

    public class NavigationProvider: INavigationProvider
    {
        public void Init(MenuItem root)
        {              
            var administration = root.Add("Администрирование");

            administration
                .Add("Интеграция с внешними системами")
                .Add("Интеграция с ГИС", "gisintegration").AddRequiredPermission("Administration.OutsideSystemIntegrations.Gis.View");

            //administration
            //    .Add("Интеграция с внешними системами")
            //    .Add("Настройки интеграции с ГИС", "gisintegrationsettings")/*.AddRequiredPermission("Administration.OutsideSystemIntegrations.Gis")*/;
        }

        public string Key
        {
            get
            {
                return MainNavigationInfo.MenuName;
            }
        }
        public string Description
        {
            get
            {
                return MainNavigationInfo.MenuDescription;
            }
        }
    }
}