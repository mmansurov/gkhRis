namespace Bars.Gkh.Ris.Controllers
{
    using System.Collections;
    using System.Web.Mvc;

    using Bars.B4;
    using Bars.Gkh.Ris.DomainService.GisIntegration;

    /// <summary>
    /// Контроллер для получения объектов при выполнении импорта/экспорта данных через сервис HouseManagement
    /// </summary>
    public class HouseManagementController : BaseController
    {
        /// <summary>
        /// Получить список домов
        /// </summary>
        /// <param name="baseParams">Параметры</param>
        /// <returns>Результат выполнения операции</returns>
        public ActionResult GetHouseList(BaseParams baseParams)
        {
            var houseManagementService = this.Container.Resolve<IHouseManagementService>();

            try
            {
                var result = (ListDataResult)houseManagementService.GetHouseList(baseParams);
                return new JsonListResult((IEnumerable)result.Data, result.TotalCount);
            }
            finally
            {
                this.Container.Release(houseManagementService);
            }
        }
    }
}
