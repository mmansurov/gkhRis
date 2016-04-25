namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Части долей и обремений по жилым помещениям
    /// </summary>
    public class RisShareEncbrResidentialPremises : BaseRisEntity
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
        /// Жилое помещение
        /// </summary>
        public virtual ResidentialPremises ResidentialPremises { get; set; }
    }
}
