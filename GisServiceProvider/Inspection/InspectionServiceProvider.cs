namespace Bars.Gkh.Ris.GisServiceProvider.Inspection
{
    using System.ServiceModel;
    using ConfigSections;
    using Enums;
    using Extensions;
    using Ris.Inspection;

    using Castle.Core.Internal;

    public class InspectionServiceProvider : BaseGisServiceProvider<InspectionPortsTypeClient, InspectionPortsType>
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
                        IntegrationService.Inspection,
                        false,
                        "http://127.0.0.1:8080/ext-bus-inspection-service/services/Inspection");
                }

                return this.serviceAddress;
            }
        }

        public override InspectionPortsTypeClient GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress)
        {
            return new InspectionPortsTypeClient(binding, remoteAddress);
        }
    }
}
