namespace Bars.Gkh.Ris.GisServiceProvider.Nsi
{
    using System.ServiceModel;
    
    using Castle.Core.Internal;
    using ConfigSections;
    using Enums;
    using Extensions;
    using NsiAsync;

    public class NsiServiceProvider : BaseGisServiceProvider<NsiPortsTypeAsyncClient, NsiPortsTypeAsync>
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
                        IntegrationService.Nsi,
                        true,
                        "http://127.0.0.1:8080/ext-bus-nsi-service/services/NsiAsync");
                }

                return this.serviceAddress;
            }
        }

        public override NsiPortsTypeAsyncClient GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress)
        {
            return new NsiPortsTypeAsyncClient(binding, remoteAddress);
        }
    }
}