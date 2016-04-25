namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Физ лицо обременитель
    /// </summary>
    public class RisEcnbrInd: BaseRisEntity
    {
        /// <summary>
        /// Обременение
        /// </summary>
        public virtual RisEcnbr Ecnbr { get; set; }

        /// <summary>
        /// Физ лицо
        /// </summary>
        public virtual RisInd Ind { get; set; }
    }
}
