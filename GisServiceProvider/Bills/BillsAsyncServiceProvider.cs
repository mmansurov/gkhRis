namespace Bars.Gkh.Ris.GisServiceProvider.Bills
{
    using System.ServiceModel;

    using Bars.Gkh.Ris.BillsAsync;
    using Bars.Gkh.Ris.ConfigSections;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.Extensions;

    using Castle.Core.Internal;

    /// <summary>
    /// Поставщик информации о сервисе BillsAsync
    /// </summary>
    public class BillsAsyncServiceProvider : BaseGisServiceProvider<BillsPortsTypeAsyncClient, BillsPortsTypeAsync>
    {
        private string serviceAddress;

        /// <summary>
        /// Адрес сервиса
        /// </summary>
        public override string ServiceAddress
        {
            get
            {
                if (this.serviceAddress.IsNullOrEmpty())
                {
                    var gisIntegrationConfig = this.ConfigProvider.Get<GisIntegrationConfig>();

                    this.serviceAddress = gisIntegrationConfig.GetServiceAddress(
                        IntegrationService.Bills,
                        true,
                        "http://127.0.0.1:8080/ext-bus-bills-service/services/BillsAsync");
                }

                return this.serviceAddress;
            }
        }

        /// <summary>
        /// Получить soap клиент
        /// </summary>
        /// <param name="binding">Binding</param>
        /// <param name="remoteAddress">Адрес</param>
        /// <returns></returns>
        public override BillsPortsTypeAsyncClient GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress)
        {
            return new BillsPortsTypeAsyncClient(binding, remoteAddress);
        }
    }
}
