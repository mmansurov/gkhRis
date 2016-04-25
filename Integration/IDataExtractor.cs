namespace Bars.Gkh.Ris.Integration
{
    using System.Collections.Generic;

    using Bars.B4.DataAccess;
    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Entities;

    /// <summary>
    /// Интерфейс компонента для извленения данных из сторонней системы в таблицы Ris
    /// </summary>
    /// <typeparam name="TRisEntity">Тип сущности Ris</typeparam>
    /// <typeparam name="TExternalEntity">Тип сущности внешней системы</typeparam>
    public interface IDataExtractor<TRisEntity, TExternalEntity>
        where TRisEntity : BaseRisEntity, new()
        where TExternalEntity : BaseEntity
    {
        /// <summary>
        /// Поставщик данных
        /// </summary>
        RisContragent Contragent { get; set; }

        /// <summary>
        /// Извлечь данные из сторонней системы в таблицы Ris
        /// </summary>
        /// <param name="parameters">Параметры извлечения данных</param>
        /// <returns>Ris сущности</returns>
        List<TRisEntity> Extract(DynamicDictionary parameters = null);

        /// <summary>
        /// Получить сущности сторонней системы
        /// </summary>
        /// <param name="parameters">Параметры сбора данных</param>
        /// <returns>Сущности сторонней системы</returns>
        List<TExternalEntity> GetExternalEntities(DynamicDictionary parameters);
    }
}
