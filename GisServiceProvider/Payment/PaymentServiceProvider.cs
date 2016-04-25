namespace Bars.Gkh.Ris.GisServiceProvider.Payment
{
    using System.ServiceModel;
    using ConfigSections;
    using Enums;
    using Extensions;
    using Castle.Core.Internal;
    using PaymentAsync;

    public class PaymentServiceProvider : BaseGisServiceProvider<PaymentPortsTypeAsyncClient, PaymentPortsTypeAsync>
    {
        private string serviceAddress;
        public override string ServiceAddress { get
            {
                if (this.serviceAddress.IsNullOrEmpty())
                {
                    var gisIntegrationConfig = this.ConfigProvider.Get<GisIntegrationConfig>();

                    this.serviceAddress = gisIntegrationConfig.GetServiceAddress(
                        IntegrationService.Payment,
                         true,
                    "http://127.0.0.1:8080/ext-bus-payment-service/services/PaymentAsync");
                }

                return this.serviceAddress;
            }
        }

        public override PaymentPortsTypeAsyncClient GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress)
        {
            return new PaymentPortsTypeAsyncClient(binding, remoteAddress);
        }
    }
}
