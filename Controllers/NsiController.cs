namespace Bars.Gkh.Ris.Controllers
{
    using System.Collections;
    using System.Web.Mvc;

    using Bars.B4;
    using Bars.Gkh.Ris.DomainService.GisIntegration;

    /// <summary>
    /// Контроллер для получения объектов при выполнении импорта/экспорта данных через сервис Nsi
    /// </summary>
    public class NsiController : BaseController
    {
        /// <summary>
        /// Получить список дополнительных услуг
        /// </summary>
        /// <param name="baseParams">Параметры</param>
        /// <returns>Результат выполнения операции</returns>
        public ActionResult GetAdditionalServices(BaseParams baseParams)
        {
            var nsiService = this.Container.Resolve<INsiService>();

            try
            {
                var result = (ListDataResult)nsiService.GetAdditionalServices(baseParams);
                return new JsonListResult((IEnumerable)result.Data, result.TotalCount);
            }
            finally
            {
                this.Container.Release(nsiService);
            }
        }
    }
}