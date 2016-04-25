namespace Bars.Gkh.Ris.Integration
{
    using B4;
    using Entities;
    using Entities.GisIntegration;
    using System.Security.Cryptography.X509Certificates;

    public interface IGisIntegrationMethod
    {
        /// <summary>
        /// Код метода 
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Наименование метода 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Порядок выполнения
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Описание метода
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Необходимо подписывать данные
        /// </summary>
        bool NeedSign { get; }

        /// <summary>
        /// Сертификат для подписи xml
        /// </summary>
        X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Лог интеграции
        /// </summary>
        GisLog GisLog { get; set; }

        /// <summary>
        /// Идентификатор поставщика
        /// </summary>
        RisContragent Contragent { get; set; }

        /// <summary>
        /// Идентификатор поставщика
        /// </summary>
        string SenderId { get; }

        /// <summary>
        /// Выполнить 
        /// </summary>
        IDataResult Execute();
    }
}