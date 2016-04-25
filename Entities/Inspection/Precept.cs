namespace Bars.Gkh.Ris.Entities.Inspection
{
    using System;

    public class Precept : BaseRisEntity
    {
        public virtual Examination Examination { get; set; }

        public virtual string Number { get; set; }

        public virtual DateTime Date { get; set; }

        public virtual string CancelReason { get; set; }

        public virtual DateTime? CancelDate { get; set; }
    }
}