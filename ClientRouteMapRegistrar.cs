namespace Bars.Gkh.Ris
{
    using B4;

    public class ClientRouteMapRegistrar : IClientRouteMapRegistrar
    {
        public void RegisterRoutes(ClientRouteMap map)
        {
            map.RegisterRoute("gisintegration", "B4.controller.integrations.GisIntegration", requiredPermission: "Administration.OutsideSystemIntegrations.Gis.View");
            map.RegisterRoute("gisintegrationsettings", "B4.controller.integrations.GisIntegrationSettings", requiredPermission: "Administration.OutsideSystemIntegrations.Gis");
        }
    }
}