namespace Bars.Gkh.Ris.GisServiceProvider.DeviceMetering
{
    using System.ServiceModel;
    using ConfigSections;
    using Enums;
    using Extensions;
    using Castle.Core.Internal;
    using Ris.DeviceMetering;

    public class DeviceMeteringServiceProvider : BaseGisServiceProvider<DeviceMeteringPortTypesClient, DeviceMeteringPortTypes>
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
                        IntegrationService.DeviceMetering,
                        false,
                        "http://127.0.0.1:8080/ext-bus-device-metering-service/services/DeviceMetering");
                }

                return this.serviceAddress;
            }
        }

        public override DeviceMeteringPortTypesClient GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress)
        {
            return new DeviceMeteringPortTypesClient(binding, remoteAddress);
        }
    }
}
