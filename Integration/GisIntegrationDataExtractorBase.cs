namespace Bars.Gkh.Ris.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4.DataAccess;
    using B4.Utils;
    using Castle.Windsor;
    using Domain;
    using Entities;
    using Enums;

    public abstract class GisIntegrationDataExtractorBase : IGisIntegrationDataExtractor
    {
        public virtual string Code => string.Empty;

        /// <summary>
        /// Ioc контейнер
        /// </summary>
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Поставщик данных
        /// </summary>
        public RisContragent Contragent { get; set; }

        /// <summary>
        /// Подготовить словари с данными
        /// </summary>
        protected abstract void FillDictionaries();

        /// <summary>
        /// Создать и сохранить новые записи РИС
        /// </summary>
        protected abstract Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters);

        /// <summary>
        /// Выполнить получение записей
        /// </summary>
        public Dictionary<Type, List<BaseRisEntity>> Extract(DynamicDictionary parameters)
        {
            this.FillDictionaries();
            return this.ExtractInternal(parameters);
        }

        protected void SetContragentAndOperation<T>(ref List<T> entitiesToSave) where T : BaseRisEntity
        {
            var domain = this.Container.ResolveDomain<T>();

            try
            {
                var previosEntities = domain.GetAll()
                    .WhereIf(this.Contragent != null, x => x.Contragent != null && x.Contragent == this.Contragent)
                    .Where(x => x.Operation != RisEntityOperation.Delete)
                    //.Select(x => (BaseRisEntity)x)
                    .ToList();

                foreach (var entityToSave in entitiesToSave)
                {
                    entityToSave.Contragent = this.Contragent;
                    entityToSave.Operation = previosEntities.Any(x => x.ExternalSystemEntityId == entityToSave.ExternalSystemEntityId && !string.IsNullOrEmpty(x.Guid))
                        ? RisEntityOperation.Update
                        : RisEntityOperation.Create;
                }
            }
            finally
            {
                this.Container.Release(domain);
            }
        }

        /// <summary>
        /// Добавить записи РИС на удаление
        /// </summary>
        /// <typeparam name="T">Тип объекта РИС</typeparam>
        /// <typeparam name="K">Тип исходного объекта ЖКХ</typeparam>
        /// <param name="entitiesToSave">Список объектов РИС для отправки в ГИС</param>
        private void CheckDeletedEntities<T,K>(ref List<T> entitiesToSave) 
            where T : BaseRisEntity, new()
            where K: PersistentObject
        {
            var risDomain = this.Container.ResolveDomain<T>();
            var gkhDomain = this.Container.ResolveDomain<K>();

            try
            {
                var risEntities = risDomain.GetAll()
                    .WhereIf(this.Contragent != null, x => x.Contragent != null && x.Contragent == this.Contragent)
                    .Where(x => x.Operation != RisEntityOperation.Delete);

                var gkhEntities = gkhDomain.GetAll();
                
                // удалим такие РИС сущности, соответствия которым больше нет в таблицах ЖКХ
                var toDelete = risEntities.Where(x => !gkhEntities.Any(y => y.Id == x.ExternalSystemEntityId)).ToList();

                foreach (var entityToAddAsDeleted in toDelete)
                {
                    entitiesToSave.Add(new T
                    {
                        ExternalSystemEntityId = entityToAddAsDeleted.ExternalSystemEntityId,
                        ExternalSystemName = entityToAddAsDeleted.ExternalSystemName,
                        Guid = entityToAddAsDeleted.Guid,
                        Contragent = this.Contragent,
                        Operation = RisEntityOperation.Delete
                    });
                }
            }
            finally
            {
                this.Container.Release(risDomain);
                this.Container.Release(gkhDomain);
            }
        }

        /// <summary>
        /// Сохранить новые записи РИС
        /// </summary>
        /// <typeparam name="T">Тип созданных записей РИС</typeparam>
        /// <typeparam name="K">Тип исходных данных ЖКХ</typeparam>
        /// <param name="entitiesToSave">Список созданных записей РИС</param>
        protected void SaveRisEntities<T,K>(List<T> entitiesToSave) 
            where T : BaseRisEntity, new()
            where K : PersistentObject
        {
            this.SetContragentAndOperation(ref entitiesToSave);
            this.CheckDeletedEntities<T,K>(ref entitiesToSave);
            TransactionHelper.InsertInManyTransactions(this.Container, entitiesToSave);
        }
    }
}
