namespace Bars.Gkh.Ris.GisServiceProvider.OrgRegistryCommon
{
    using System.ServiceModel;
    using ConfigSections;
    using Enums;
    using Extensions;
    using OrgRegistryCommonAsync;
    using Castle.Core.Internal;

    public class OrgRegistryCommonServiceProvider : BaseGisServiceProvider<RegOrgPortsTypeAsyncClient, RegOrgPortsTypeAsync>
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
                        IntegrationService.OrgRegistryCommon,
                        true,
                        "http://127.0.0.1:8080/ext-bus-org-registry-common-service/services/OrgRegistryCommonAsync");
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