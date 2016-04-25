namespace Bars.Gkh.Ris.GisServiceProvider
{
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using B4.Utils;

    using Bars.Gkh.Ris.Integration.Signature;

    using ConfigSections;
    using Config;
    using Castle.Windsor;

    public abstract class BaseGisServiceProvider<T, K> : IGisServiceProvider<T>
        where T : ClientBase<K>
        where K : class
    {
        public abstract string ServiceAddress{get;}

        public IWindsorContainer Container { get; set; }

        public IGkhConfigProvider ConfigProvider { get; set; }

        public abstract T GetClient(BasicHttpBinding binding, EndpointAddress remoteAddress);

        /// <summary>
        /// Получить клиент для работы с сервисом
        /// </summary>
        public T GetSoapClient()
        {           
            var gisIntegrationConfig = this.ConfigProvider.Get<GisIntegrationConfig>();
            var isHttps = this.ServiceAddress.Split(":")[0] == "https";

            var binding = new BasicHttpBinding
            {
                Security =
                    {
                        Mode = isHttps
                            ? BasicHttpSecurityMode.Transport
                            : BasicHttpSecurityMode.TransportCredentialOnly,
                        Transport = new HttpTransportSecurity
                        {
                            ClientCredentialType = isHttps
                                ? HttpClientCredentialType.Certificate
                                : HttpClientCredentialType.Basic
                        }
                    }
            };

            var remoteAddress = new EndpointAddress(this.ServiceAddress);
            var client = this.GetClient(binding, remoteAddress);

            var defaultCredentials = client.Endpoint.Behaviors.Find<ClientCredentials>();
            client.Endpoint.Behaviors.Remove(defaultCredentials);

            // test
            // SDldfls4lz5@!82d
            if (gisIntegrationConfig.UseLoginCredentials)
            {
                var loginCredentials = new ClientCredentials();
                loginCredentials.UserName.UserName = gisIntegrationConfig.Login;
                loginCredentials.UserName.Password = gisIntegrationConfig.Password;
                client.Endpoint.Behaviors.Add(loginCredentials);
            }

            return client;
        }
    }
}