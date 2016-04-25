namespace Bars.Gkh.Ris.ViewModel.Task
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// Базовый узел дерева
    /// </summary>
    public class BaseNode
    {
        /// <summary>
        /// Узел является "листом"
        /// </summary>
        [JsonProperty("leaf")]
        public bool Leaf { get; set; }

        /// <summary>
        /// Дочерние узлы
        /// </summary>
        [JsonProperty("children")]
        public List<BaseNode> Children { get; set; }

        /// <summary>
        /// Cls иконки узла
        /// </summary>
        [JsonProperty("iconCls")]
        public string IconCls { get; set; }
    }
}
