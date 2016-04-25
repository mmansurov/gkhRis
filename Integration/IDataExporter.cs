namespace Bars.Gkh.Ris.Integration
{
    using B4.Utils;
    using Entities;
    using System;
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// Интерфейс экспортера данных
    /// </summary>
    public interface IDataExporter
    {
        /// <summary>
        /// Наименование экспортера 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Порядок выполнения
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Описание экспортера
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Поставщик данных
        /// </summary>
        string SenderId { get;}

        /// <summary>
        /// Необходимо подписывать данные
        /// </summary>
        bool NeedSign { get; }

        /// <summary>
        /// Запустить экспорт
        /// </summary>
        /// <param name="packageIds">Идентификаторы пакетов</param>
        /// <returns>Результат отправки данных на обработку</returns>
        DataSendingResult Execute(long[] packageIds);

        /// <summary>
        /// Метод подготовки данных к экспорту, включая валидацию, формирование пакетов
        /// </summary>
        /// <param name="parameters">Параметры экспорта</param>
        /// <returns>Результат подготовки данных</returns>
        PrepareDataResult PrepareData(DynamicDictionary parameters);

        /// <summary>
        /// Сохранить подписанные данные
        /// </summary>
        /// <param name="signedData">Словарь подписанных данных: Идентификатор пакета - Подписанный XML</param>
        void SaveSignedData(Dictionary<long, string> signedData);

        /// <summary>
        /// Поставщик данных
        /// </summary>
        RisContragent Contragent { get; set; }
    }
}