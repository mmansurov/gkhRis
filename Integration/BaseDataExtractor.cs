namespace Bars.Gkh.Ris.Integration
{
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Entities;
    using B4.DataAccess;

    using Bars.Gkh.Domain;
    using Bars.Gkh.Ris.DomainService.GisIntegration;
    using Bars.Gkh.Ris.Enums;

    using Castle.Windsor;

    /// <summary>
    /// Базовый класс для извленения данных из сторонней системы в таблицы Ris
    /// </summary>
    /// <typeparam name="TRisEntity">Тип сущности Ris</typeparam>
    /// <typeparam name="TExternalEntity">Тип сущности внешней системы</typeparam>
    public abstract class BaseDataExtractor<TRisEntity, TExternalEntity> : IDataExtractor<TRisEntity, TExternalEntity>
        where TRisEntity: BaseRisEntity, new() 
        where TExternalEntity : BaseEntity
    {
        private RisContragent contragent;

        /// <summary>
        /// Ioc контейнер
        /// </summary>
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Поставщик данных
        /// </summary>
        public RisContragent Contragent {
            get
            {
                if (this.contragent == null && this.ContragentIsRequired)
                {
                    this.contragent = this.GetCurrentContragent();
                }

                return this.contragent;
            }

            set
            {
                this.contragent = value;
            }
        }

        /// <summary>
        /// Контрагент обязателен
        /// </summary>
        protected virtual bool ContragentIsRequired => true;

        /// <summary>
        /// Извлечь данные из сторонней системы в таблицы Ris
        /// </summary>
        /// <param name="parameters">Параметры извлечения данных</param>
        /// <returns>Ris сущности</returns>
        public List<TRisEntity> Extract(DynamicDictionary parameters = null)
        {
            this.BeforeExtractHandle(parameters);

            var result = new List<TRisEntity>();

            var externalEntities = this.GetExternalEntities(parameters);

            this.BeforePrepareRisEntitiesHandle(parameters, externalEntities);

            var searchDeletedEntities = this.SearchDeletedEntities(parameters);

            var existingRisEntities = this.GetExistingRisEntities(externalEntities.Select(x => x.Id).ToArray(), searchDeletedEntities);

            var preparedRisEntities = this.PrepareRisEntities(externalEntities, existingRisEntities);
            result.AddRange(preparedRisEntities);

            if (searchDeletedEntities)
            {
                var searchArea = existingRisEntities
                    .Where(x => x.ExternalSystemEntityId > 0 
                        && !string.IsNullOrEmpty(x.Guid)
                        && preparedRisEntities.All(y => y.Id != x.Id))
                    .ToList();

                var deletedEntities = this.GetDeletedRisEntities(searchArea);
                result.AddRange(deletedEntities);
            }

            this.SaveRisEntities(result);

            return result;
        }

        /// <summary>
        /// Получить сущности сторонней системы
        /// </summary>
        /// <param name="parameters">Параметры сбора данных</param>
        /// <returns>Сущности сторонней системы</returns>
        public abstract List<TExternalEntity> GetExternalEntities(DynamicDictionary parameters);

        /// <summary>
        /// Обновить значения атрибутов Ris сущности
        /// </summary>
        /// <param name="externalEntity">Сущность внешней системы</param>
        /// <param name="risEntity">Ris сущность</param>
        protected abstract void UpdateRisEntity(TExternalEntity externalEntity, TRisEntity risEntity);

        /// <summary>
        /// Определить необходимость искать удаленные сущности
        /// 1. Не для всех типов сущностей есть операция Delete (может быть поле Дата прекащения существования и операция Update)
        /// 2. В зависимости от входных параметров:
        /// 1) если извлекаем данные по заданным объектам, искать удаленные не надо
        /// 2) если делаем массовое извлечение без фильтрации, искать в том числе и удаленные
        /// </summary>
        /// <param name="parameters">Входные параметры извлечения данных</param>
        /// <returns>true - искать, false - в противном случае</returns>
        protected virtual bool SearchDeletedEntities(DynamicDictionary parameters)
        {
            return false;
        }

        /// <summary>
        /// Выполнить обработку перед подготовкой Ris сущностей
        /// Например, подготовить словари с данными
        /// </summary>
        /// <param name="parameters">Входные параметры</param>
        /// <param name="externalEntities">Выбранные сущности внешней системы</param>
        protected virtual void BeforePrepareRisEntitiesHandle(DynamicDictionary parameters, List<TExternalEntity> externalEntities)
        {
        }

        /// <summary>
        /// Выполнить обработку перед извлечением данных
        /// Например, подготовить словари с данными
        /// </summary>
        /// <param name="parameters">Входные параметры</param>
        protected virtual void BeforeExtractHandle(DynamicDictionary parameters)
        {
        }

        private List<TRisEntity> GetExistingRisEntities(long[] externalEntitiesIds, bool searchDeletedEntities)
        {
            var risEntitiesDomain = this.Container.ResolveDomain<TRisEntity>();

            try
            {
                return
                    risEntitiesDomain.GetAll()
                        .WhereIf(this.ContragentIsRequired, x => x.Contragent.Id == this.Contragent.Id)
                        .WhereIf(!searchDeletedEntities, x => externalEntitiesIds.Contains(x.ExternalSystemEntityId))
                        .Where(x => x.Operation != RisEntityOperation.Delete)
                        .ToList();
            }
            finally
            {
                this.Container.Release(risEntitiesDomain);
            }
        }

        private List<TRisEntity> PrepareRisEntities(List<TExternalEntity> externalEntities, List<TRisEntity> existingRisEntities)
        {
            var result = new List<TRisEntity>();

            foreach (var externalEntity in externalEntities)
            {
                var risEntity = existingRisEntities.FirstOrDefault(x => x.ExternalSystemEntityId == externalEntity.Id);

                if (risEntity == null)
                {
                    risEntity = new TRisEntity();
                }

                this.UpdateRisEntity(externalEntity, risEntity);

                risEntity.Contragent = this.Contragent;
                risEntity.Operation = string.IsNullOrEmpty(risEntity.Guid) 
                    ? RisEntityOperation.Create 
                    : RisEntityOperation.Update;

                result.Add(risEntity);
            }

            return result;
        }

        private List<TRisEntity> GetDeletedRisEntities(List<TRisEntity> existingRisEntities)
        {
            var result = new List<TRisEntity>();

            var externalEntityDomain = this.Container.ResolveDomain<TExternalEntity>();

            try
            {
                var deletedEntities = existingRisEntities
                    .Where(x => !externalEntityDomain.GetAll()
                        .Any(y => y.Id == x.ExternalSystemEntityId))
                    .ToList();

                foreach (var deletedEntity in deletedEntities)
                {
                    deletedEntity.Operation = RisEntityOperation.Delete;
                    result.Add(deletedEntity);
                }
            }
            finally 
            {
                this.Container.Release(externalEntityDomain);
            }

            return result;
        }

        private void SaveRisEntities(List<TRisEntity> risEntities)
        {
            TransactionHelper.InsertInManyTransactions(this.Container, risEntities);
        }

        private RisContragent GetCurrentContragent()
        {           
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                return gisIntegrService.GetCurrentContragent();          
            }
            finally
            {
                this.Container.Release(gisIntegrService);
            }
        }
    }
}
