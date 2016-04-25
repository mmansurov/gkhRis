namespace Bars.Gkh.Ris.DomainService.GisIntegration
{
    using Bars.B4;

    /// <summary>
    /// Интерфейс сервиса для получения объектов при выполнении импорта/экспорта данных через сервис Nsi
    /// </summary>
    public interface INsiService
    {
        /// <summary>
        /// Получить список дополнительных услуг
        /// </summary>
        /// <param name="baseParams">Параметры</param>
        /// <returns>Результат выполнения операции</returns>
        IDataResult GetAdditionalServices(BaseParams baseParams);
    }
}