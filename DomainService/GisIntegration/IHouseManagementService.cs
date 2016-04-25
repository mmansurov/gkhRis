namespace Bars.Gkh.Ris.DomainService.GisIntegration
{
    using Bars.B4;

    /// <summary>
    /// Интерфейс сервиса для получения объектов при выполнении импорта/экспорта данных через сервис HouseManagement
    /// </summary>
    public interface IHouseManagementService
    {
        /// <summary>
        /// Получить список домов
        /// </summary>
        /// <param name="baseParams">Параметры</param>
        /// <returns>Результат выполнения операции</returns>
        IDataResult GetHouseList(BaseParams baseParams);
    }
}
