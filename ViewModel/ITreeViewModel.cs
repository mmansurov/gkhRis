namespace Bars.Gkh.Ris.ViewModel
{
    using Bars.B4;

    /// <summary>
    /// Интерфейс View - модели дерева задач
    /// </summary>
    public interface ITreeViewModel
    {
        /// <summary>
        /// Получить дочерние узлы
        /// </summary>
        /// <param name="baseParams">Параметры, в т.ч. параметры текущего узла</param>
        /// <returns>Дочерние узлы</returns>
        IDataResult List(BaseParams baseParams);
    }
}
