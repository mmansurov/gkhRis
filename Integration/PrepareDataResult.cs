namespace Bars.Gkh.Ris.Integration
{
    using System.Collections.Generic;
    using Entities;

    /// <summary>
    /// Результат подготовки данных к экспорту
    /// </summary>
    public class PrepareDataResult
    {
        /// <summary>
        /// Результат валидации данных перед формированием пакетов
        /// </summary>
        public List<ValidateObjectResult> ValidateResult { get; set; }

        /// <summary>
        /// Сформированные пакеты
        /// </summary>
        public List<RisPackage> Packages { get; set; }
    }
}
