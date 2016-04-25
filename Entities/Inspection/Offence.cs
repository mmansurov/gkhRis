namespace Bars.Gkh.Ris.Entities.Inspection
{
    using System;

    public class Offence : BaseRisEntity
    {
        public virtual Examination Examination { get; set; }

        public virtual string Number { get; set; }

        public virtual DateTime Date { get; set; }
    }
}