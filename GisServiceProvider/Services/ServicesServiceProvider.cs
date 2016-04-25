namespace Bars.Gkh.Ris.GisServiceProvider.Services
{
    using System.ServiceModel;
    using ConfigSections;
    using Enums;
    using Extensions;
    using Castle.Core.Internal;
    using Ris.Services;

    public class ServicesServiceProvider : BaseGisServiceProvider<ServicesPortsTypeClient, ServicesPortsType>
    {
        private string serviceAddress;
        public override string ServiceAddress {
            get
            {
                if (this.serviceAddress.IsNullOrEmpty())
                {
                    var gisIntegrationConfig = this.ConfigProvider.Get<GisIntegrationConfig>();

                    this.serviceAddress = gisIntegrationConfig.GetServiceAddress(
                        IntegrationService.Organization,
                        false,
                        "http://127.0.0.1:8080/ext-bus-organization-service/services/Organization");
                }

                return this.serviceAddress;
            }
        }

        public override ServicesPortsTypeClient GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress)
        {
            return new ServicesPortsTypeClient(binding, remoteAddress);
        }
    }
}
