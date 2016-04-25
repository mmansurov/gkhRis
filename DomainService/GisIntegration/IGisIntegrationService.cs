namespace Bars.Gkh.Ris.DomainService.GisIntegration
{
    using B4;

    using Bars.Gkh.Ris.Entities;
    using Bars.Gkh.Ris.NsiCommon;

    public interface IGisIntegrationService
    {
        /// <summary>
        /// Получить список методов
        /// </summary>
        //IDataResult GetDataExtractorList(BaseParams baseParams);

        /// <summary>
        /// Выполнение извлечения данных
        /// </summary>
        //IDataResult ExtractData(BaseParams baseParams);

        /// <summary>
        /// Получить список методов
        /// </summary>
        IDataResult GetMethodList(BaseParams baseParams);

        /// <summary>
        /// Метод проверки необходим ли для выполнения метода сертификат
        /// </summary>
        /// <param name="baseParams">Базовые параметры</param>
        /// <returns>Результат, содержащий true - сертификат нужен, false - в противном случае</returns>
        IDataResult NeedCertificate(BaseParams baseParams);

        /// <summary>
        /// Выполнение метода
        /// </summary>
        IDataResult ExecuteMethod(BaseParams baseParams);

        /// <summary>
        /// Метод получения справочников
        /// </summary>
        IDataResult GetDictList(BaseParams baseParams);

        /// <summary>
        /// Метод получения существующих методов интеграции для справочников
        /// </summary>
        IDataResult GetDictActions(BaseParams baseParams);

        /// <summary>
        /// Метод получения существующих методов интеграции для справочников
        /// </summary>
        IDataResult GetDictRecordList(BaseParams baseParams);

        /// <summary>
        /// Получение списка сертификатов для подписи сообщения
        /// </summary>
        IDataResult GetCertificates(BaseParams baseParams);

        /// <summary>
        /// Получить веб-клиент
        /// </summary>
        /// <returns></returns>
        NsiPortsTypeClient CreateNsiClient();

        /// <summary>
        /// Метод подготовки данных к экспорту, включая валидацию, формирование пакетов
        /// </summary>
        /// <param name="baseParams">Параметры экспорта</param>
        /// <returns>Результат подготовки данных</returns>
        IDataResult PrepareDataForExport(BaseParams baseParams);

        /// <summary>
        /// Получить неподписанные данные форматированные для просмотра
        /// либо неформатированные для подписи
        /// </summary>
        /// <param name="baseParams">Параметры, содержащие идентификатор пакета</param>
        /// <returns>Неподписанные данные форматированной xml строкой</returns>
        IDataResult GetNotSignedData(BaseParams baseParams);

        /// <summary>
        /// Сохранить подписанные xml
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        IDataResult SaveSignedData(BaseParams baseParams);

        /// <summary>
        /// Запустить экспорт
        /// </summary>
        /// <param name="baseParams">Параметры, содержащие идентификаторы пакетов</param>
        /// <returns>Идентификатор запланированной задачи получения результатов экспорта</returns>
        IDataResult ExecuteExporter(BaseParams baseParams);

        /// <summary>
        /// Удалить временные объекты
        /// </summary>
        /// <param name="baseParams">Параметры, содержащие идентификаторы всех созданных объектов в рамках работы одного мастера</param>
        /// <returns>Результат выполнения операции</returns>
        IDataResult DeleteTempObjects(BaseParams baseParams);

        /// <summary>
        /// Получить контрагента текущего пользователя
        /// </summary>
        /// <returns>Контрагент РИС текущего пользователя</returns>
        RisContragent GetCurrentContragent();
    }
}