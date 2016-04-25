namespace Bars.Gkh.Ris.Integration
{
    using Bars.Gkh.Ris.Entities;

    /// <summary>
    /// Результат отправки пакета
    /// </summary>
    public class PackageSendingResult
    {
        /// <summary>
        /// Пакет данных
        /// </summary>
        public RisPackage Package { get; set; }

        /// <summary>
        /// Результат отправки: успешно - true, в противном случае false
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Идентификатор сообщения для получения результата обработки пакета
        /// </summary>
        public string AckMessageGuid { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message { get; set; }
    }
}
