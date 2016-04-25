namespace Bars.Gkh.Ris.GisServiceProvider.Infrastructure
{
    using System.ServiceModel;
    using ConfigSections;
    using Enums;
    using Extensions;
    using Castle.Core.Internal;
    using Ris.Infrastructure;

    public class InfrastructureServiceProvider : BaseGisServiceProvider<InfrastructurePortsTypeClient, InfrastructurePortsType>
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
                        IntegrationService.Infrastructure,
                        false,
                        "http://127.0.0.1:8080/ext-bus-rki-service/services/Infrastructure");
                }

                return this.serviceAddress;
            }
        }

        public override InfrastructurePortsTypeClient GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress)
        {
            return new InfrastructurePortsTypeClient(binding, remoteAddress);
        }
    }
}
