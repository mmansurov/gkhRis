namespace Bars.Gkh.Ris.Integration.Nsi.DataExtractors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4.DataAccess;
    using Bars.B4.Utils;
    using Bars.Gkh.Gis.Entities.Dict;
    using Bars.Gkh.Gis.Enum;
    using Bars.Gkh.Ris.Entities;
    using Bars.Gkh.Ris.Entities.GisIntegration.Ref;
    using Bars.Gkh.Ris.Entities.Nsi;

    /// <summary>
    /// Извлечение записей справочника «Коммунальные услуги»
    /// </summary>
    public class ImportMunicipalServiceDataExtractor : GisIntegrationDataExtractorBase
    {
        private Dictionary<long, GisDictRef> municipalServicesDictionary;
        private Dictionary<long, GisDictRef> municipalResourcesDictionary;

        /// <summary>
        /// Код метода - экспортера записей справочника «Коммунальные услуги»
        /// </summary>
        public override string Code
        {
            get
            {
                return "importMunicipalService";
            }
        }

        /// <summary>
        /// Подготовить словари с данными
        /// </summary>
        protected override void FillDictionaries()
        {
            var gisDictRefDomain = this.Container.ResolveDomain<GisDictRef>();

            try
            {
                this.municipalServicesDictionary = gisDictRefDomain.GetAll()
                    .Where(x => x.Dict.ActionCode == "Коммунальные услуги")
                    .GroupBy(x => x.GkhId)
                    .ToDictionary(x => x.Key, x => x.First());

                this.municipalResourcesDictionary = gisDictRefDomain.GetAll()
                    .Where(x => x.Dict.ActionCode == "Вид коммунального ресурса")
                    .GroupBy(x => x.GkhId)
                    .ToDictionary(x => x.Key, x => x.First());
            }
            finally
            {
                this.Container.Release(gisDictRefDomain);
            }
        }

        /// <summary>
        /// Сохранить новые записи РИС
        /// </summary>
        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var servicesDomain = this.Container.ResolveDomain<ServiceDictionary>();

            try
            {
                var municipalServices =
                    servicesDomain.GetAll()
                        .Where(x => x.TypeService == TypeServiceGis.Communal
                        && x.TypeCommunalResourse.HasValue).ToList();

                var municipalServicesToSave = new List<RisMunicipalService>();

                foreach (var municipalService in municipalServices)
                {
                    var resourceHashCode = municipalService.TypeCommunalResourse.GetValueOrDefault().GetHashCode();

                    if (this.municipalServicesDictionary.ContainsKey(municipalService.Id)
                        && this.municipalResourcesDictionary.ContainsKey(resourceHashCode))
                    {
                        var municipalServiceDictRef = this.municipalServicesDictionary[municipalService.Id];
                        var municipalResourceDictRef = this.municipalResourcesDictionary[resourceHashCode];

                        municipalServicesToSave.Add(new RisMunicipalService
                        {
                            ExternalSystemEntityId = municipalService.Id,
                            ExternalSystemName = "gkh",
                            GeneralNeeds = municipalService.IsProvidedForAllHouseNeeds,
                            MainMunicipalServiceName = municipalService.Name,
                            MunicipalServiceRefCode = municipalServiceDictRef.GisId,
                            MunicipalServiceRefGuid = municipalServiceDictRef.GisGuid,
                            MunicipalResourceRefCode = municipalResourceDictRef.GisId,
                            MunicipalResourceRefGuid = municipalResourceDictRef.GisGuid,
                            SortOrderNotDefined = true
                        });
                    }
                }

                this.SaveRisEntities<RisMunicipalService, ServiceDictionary>(municipalServicesToSave);

                return new Dictionary<Type, List<BaseRisEntity>>();
            }
            finally
            {
                this.Container.Release(servicesDomain);
            }
        }
    }
}