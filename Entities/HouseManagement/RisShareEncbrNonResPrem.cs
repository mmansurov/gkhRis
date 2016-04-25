namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Части долей и обремений по нежилым помещениям
    /// </summary>
    public class RisShareEncbrNonResPrem : BaseRisEntity
    {
        /// <summary>
        /// Целая часть доли
        /// </summary>
        public virtual string IntPart { get; set; }

        /// <summary>
        /// Дробная часть доли
        /// </summary>
        public virtual string FracPart { get; set; }

        /// <summary>
        /// Обременение
        /// </summary>
        public virtual RisEcnbr Ecnbr { get; set; }

        /// <summary>
        /// Собственность
        /// </summary>
        public virtual RisShare Share { get; set; }

        /// <summary>
        /// Нежилое помещение
        /// </summary>
        public virtual NonResidentialPremises NonResidentialPremises { get; set; }
    }
}
