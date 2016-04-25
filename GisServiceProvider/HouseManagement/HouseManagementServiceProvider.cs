namespace Bars.Gkh.Ris.GisServiceProvider.HouseManagement
{
    using System.ServiceModel;
    using HouseManagementAsync;
    using ConfigSections;
    using Enums;
    using Extensions;
    using Castle.Core.Internal;

    public class HouseManagementServiceProvider : BaseGisServiceProvider<HouseManagementPortsTypeAsyncClient, HouseManagementPortsTypeAsync>
    {
        private string serviceAddress;

        public override string ServiceAddress
        {
            get
            {
                if (this.serviceAddress.IsNullOrEmpty())
                {
                    var gisIntegrationConfig = this.ConfigProvider.Get<GisIntegrationConfig>();

                    this.serviceAddress = gisIntegrationConfig.GetServiceAddress(
                        IntegrationService.HouseManagement,
                        true,
                        "http://127.0.0.1:8080/ext-bus-home-management-service/services/HomeManagementAsync");
                }

                return this.serviceAddress;
            }
        }

        public override HouseManagementPortsTypeAsyncClient GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress)
        {
            return new HouseManagementPortsTypeAsyncClient(binding, remoteAddress);
        }
    }
}
