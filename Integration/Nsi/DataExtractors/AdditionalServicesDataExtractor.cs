namespace Bars.Gkh.Ris.Integration.Nsi.DataExtractors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4.DataAccess;
    using Bars.B4.Utils;

    using Bars.Gkh.Domain;
    using Bars.Gkh.Gis.Entities.Dict;
    using Bars.Gkh.Gis.Enum;
    using Bars.Gkh.Ris.Entities;
    using Bars.Gkh.Ris.Entities.Nsi;
    using Bars.Gkh.Ris.Enums;

    /// <summary>
    /// Извлечение записей справочника «Дополнительные услуги»
    /// </summary>
    public class AdditionalServicesDataExtractor : GisIntegrationDataExtractorBase
    {
        /// <summary>
        /// Подготовить словари с данными
        /// </summary>
        protected override void FillDictionaries()
        {
        }

        /// <summary>
        /// Сохранить новые записи РИС
        /// </summary>
        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var risAdditServiceDomain = this.Container.ResolveDomain<RisAdditionalService>();

            try
            {
                var uploadedEntitiesDict = risAdditServiceDomain.GetAll()
                    .WhereIf(this.Contragent != null, x => x.Contragent != null && x.Contragent == this.Contragent)
                    .Where(x => x.Operation != RisEntityOperation.Delete)
                    .Where(x => x.Guid != null && x.Guid != "")
                    .Select(x => new
                    {
                        x.Id,
                        x.ExternalSystemEntityId
                    })
                    .ToList()
                    .GroupBy(x => x.ExternalSystemEntityId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Id).First());

                var additionalServicesToSave = this.GetAdditionalServices(parameters)
                    .Select(x => new RisAdditionalService
                    {
                        Id = uploadedEntitiesDict.ContainsKey(x.Id)
                            ? uploadedEntitiesDict[x.Id]
                            : 0,
                        Operation = uploadedEntitiesDict.ContainsKey(x.Id)
                            ? RisEntityOperation.Update
                            : RisEntityOperation.Create,
                        ExternalSystemEntityId = x.Id,
                        ExternalSystemName = "gkh",
                        AdditionalServiceTypeName = x.Name,
                        StringDimensionUnit = x.UnitMeasure.Name
                    });

                TransactionHelper.InsertInManyTransactions(this.Container, additionalServicesToSave);

                return new Dictionary<Type, List<BaseRisEntity>>
                {
                    { typeof(RisAdditionalService), additionalServicesToSave.Cast<BaseRisEntity>().ToList() }
                };
            }
            finally
            {
                this.Container.Release(risAdditServiceDomain);
            }
        }

        public List<ServiceDictionary> GetAdditionalServices(DynamicDictionary parameters)
        {
            var servicesDomain = this.Container.ResolveDomain<ServiceDictionary>();

            try
            {
                var selectedIds = parameters.GetAs("selectedList", string.Empty).ToLongArray();

                return servicesDomain.GetAll()
                    .WhereIf(selectedIds.Length > 0, x => selectedIds.Contains(x.Id))
                    .Where(x => x.TypeService == TypeServiceGis.Housing || x.TypeService == TypeServiceGis.Other)
                    .ToList();
            }
            finally
            {
                Container.Release(servicesDomain);
            }
        }
    }
}