namespace Bars.Gkh.Ris.DomainService.GisIntegration.Impl
{
    using System;
    using System.Linq;

    using Bars.B4;
    using Bars.B4.Utils;
    using Bars.Gkh.Entities;
    using Bars.Gkh.Ris.Entities.HouseManagement;
    using Bars.Gkh.Ris.Integration;
    using Bars.Gkh.Ris.Integration.HouseManagement.DataExtractors;

    using Castle.Windsor;

    /// <summary>
    /// Сервис для получения объектов при выполнении импорта/экспорта данных через сервис HouseManagement
    /// </summary>
    public class HouseManagementService: IHouseManagementService
    {
        /// <summary>
        /// Ioc контейнер
        /// </summary>
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Получить список домов
        /// </summary>
        /// <param name="baseParams">Параметры</param>
        /// <returns>Результат выполнения операции</returns>
        public IDataResult GetHouseList(BaseParams baseParams)
        {
            var extractor = (RisHouseDataExtractor)this.Container.Resolve<IDataExtractor<RisHouse, RealityObject>>("RisHouseDataExtractor");
            var gisIntegrService = this.Container.Resolve<IGisIntegrationService>();

            try
            {
                var currentContragent = gisIntegrService.GetCurrentContragent();

                if (currentContragent == null)
                {
                    throw new Exception("Не найден текущий контрагент");
                }

                if (baseParams.Params.GetAs<bool>("forUO", false))
                {
                    baseParams.Params.Add("uoId", currentContragent.GkhId);
                }

                if (baseParams.Params.GetAs<bool>("forOMS", false))
                {
                    baseParams.Params.Add("omsId", currentContragent.GkhId);
                }

                if (baseParams.Params.GetAs<bool>("forRSO", false))
                {
                    baseParams.Params.Add("rsoId", currentContragent.GkhId);
                }

                var houseList = extractor.GetExternalEntities(baseParams.Params);

                var loadParams = baseParams.GetLoadParam();

                var data = houseList.Select(x => 
                new
                {
                    x.Id,
                    x.Address,
                    HouseType = extractor.ConvertHouseType(x.TypeHouse).GetDisplayName()
                })
                .AsQueryable()               
                .Filter(loadParams, this.Container);

                return new ListDataResult(data.Paging(loadParams).ToList(), data.Count());
            }
            finally
            {
                this.Container.Release(extractor);
                this.Container.Release(gisIntegrService);
            }
        }
    }
}
