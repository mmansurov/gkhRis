namespace Bars.Gkh.Ris.Controllers
{
    using System.Web.Mvc;

    using Bars.B4;
    using Bars.Gkh.Ris.ViewModel;

    /// <summary>
    /// Контроллер дерева задач
    /// </summary>
    public class TaskTreeController: BaseController
    {
        /// <summary>
        /// Получить дочерние узлы
        /// </summary>
        /// <param name="baseParams">Параметры, в т.ч. параметры текущего узла</param>
        /// <returns>Дочерние узлы</returns>
        public ActionResult GetTaskTreeNodes(BaseParams baseParams)
        {
            var viewModel = this.Container.Resolve<ITreeViewModel>("TaskTreeViewModel");

            var result = viewModel.List(baseParams);

            return result.Success ? new JsonNetResult(result.Data) : this.JsFailure(result.Message);
        }
    }
}
