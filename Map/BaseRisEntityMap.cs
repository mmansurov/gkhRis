namespace Bars.Gkh.Ris.Map.GisIntegration
{
    using B4.Modules.Mapping.Mappers;
    using Entities;

    /// <summary>
    /// Базовый маппинг для BaseRisEntity-сущностей
    /// </summary>
    public abstract class BaseRisEntityMap<TBaseEntity> : BaseEntityMap<TBaseEntity> where TBaseEntity : BaseRisEntity
    {
        /// <summary>
        /// Конструктор типа BaseRisEntityMap
        /// </summary>
        /// <param name="entityName">Название сущности</param>
        /// <param name="tableName">Имя таблицы</param>
        protected BaseRisEntityMap(string entityName, string tableName)
            : base(entityName, tableName)
        {
        }

        public override void InitMap()
        {
            base.InitMap();

            this.Property(x => x.ExternalSystemEntityId, "Id объекта в системе, из которой он был перемещен").Column("EXTERNAL_ID").NotNull();
            this.Property(x => x.ExternalSystemName, "Наименование системы").Column("EXTERNAL_SYSTEM_NAME").Length(50);
            this.Reference(x => x.RisContainer, "Ссылка на контейнер данных").Column("RIS_CONTAINER_ID").Fetch();
            this.Reference(x => x.Contragent, "Поставщик данных").Column("RIS_CONTRAGENT_ID").Fetch();
            this.Property(x => x.Guid, "Гуид, присвоенный объекту при загрузке в ГИС").Column("GUID").Length(50);
            this.Property(x => x.Operation, "Операция, которую необходимо выполнить с записью в ГИС").Column("OPERATION");
        }
    }
}