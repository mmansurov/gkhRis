namespace Bars.Gkh.Ris.GisServiceProvider.OrgRegistry
{
    using System.ServiceModel;
    using ConfigSections;
    using Enums;
    using Extensions;
    using OrgRegistryAsync;
    using Castle.Core.Internal;

    public class OrgRegistryServiceProvider : BaseGisServiceProvider<RegOrgPortsTypeAsyncClient, RegOrgPortsTypeAsync>
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
                        IntegrationService.OrgRegistry,
                        true,
                        "http://127.0.0.1:8080/ext-bus-org-registry-service/services/OrgRegistryAsync");
                }

                return this.serviceAddress;
            }
        }

        public override RegOrgPortsTypeAsyncClient GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress)
        {
            return new RegOrgPortsTypeAsyncClient(binding, remoteAddress);
        }
    }
}