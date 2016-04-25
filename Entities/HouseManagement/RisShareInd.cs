namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Физ лицо собстаенник
    /// </summary>
    public class RisShareInd: BaseRisEntity
    {
        /// <summary>
        /// Собственность
        /// </summary>
        public virtual RisShare Share { get; set; }

        /// <summary>
        /// Физ лицо
        /// </summary>
        public virtual RisInd Ind { get; set; }
    }
}
