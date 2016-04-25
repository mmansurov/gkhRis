namespace Bars.Gkh.Ris.Entities.Inspection
{
    using System;

    /// <summary>
    /// План проверок юридических лиц
    /// </summary>
    public class InspectionPlan : BaseRisEntity
    {
        /// <summary>
        /// Год плана
        /// </summary>
        public virtual int Year { get; set; }

        /// <summary>
        /// Дата утверждения плана
        /// </summary>
        public virtual DateTime? ApprovalDate { get; set; }
    }
}