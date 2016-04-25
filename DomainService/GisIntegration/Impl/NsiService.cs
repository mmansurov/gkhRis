namespace Bars.Gkh.Ris.DomainService.GisIntegration.Impl
{
    using System.Linq;

    using Bars.B4;

    using Bars.Gkh.Ris.Integration;
    using Bars.Gkh.Ris.Integration.Nsi.DataExtractors;
    using Castle.Windsor;

    /// <summary>
    /// Сервис для получения объектов при выполнении импорта/экспорта данных через сервис Nsi
    /// </summary>
    public class NsiService : INsiService
    {
        /// <summary>
        /// IoC контейнер
        /// </summary>
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Получить список дополнительных услуг
        /// </summary>
        /// <param name="baseParams">Параметры</param>
        /// <returns>Результат выполнения операции</returns>
        public IDataResult GetAdditionalServices(BaseParams baseParams)
        {
            var extractor = (AdditionalServicesDataExtractor)this.Container.Resolve<IGisIntegrationDataExtractor>("AdditionalServicesDataExtractor");

            try
            {
                var addServList = extractor.GetAdditionalServices(baseParams.Params);

                var loadParams = baseParams.GetLoadParam();

                var data = addServList.Select(x => new
                {
                    x.Id,
                    x.Name,
                    UnitMeasure = x.UnitMeasure.Name
                })
                .AsQueryable()
                .Filter(loadParams, this.Container);

                return new ListDataResult(data.Paging(loadParams).ToList(), data.Count());
            }
            finally
            {
                this.Container.Release(extractor);
            }
        }
    }
}