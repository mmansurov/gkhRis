namespace Bars.Gkh.Ris.GisServiceProvider
{
    using Config;
    using Castle.Windsor;

    public interface IGisServiceProvider<T> 
    {
        IWindsorContainer Container { get; set; }

        IGkhConfigProvider ConfigProvider { get; set; }

        string ServiceAddress { get; }

        /// <summary>
        /// Получить клиент для работы с сервисом
        /// </summary>
        T GetSoapClient();
    }
}