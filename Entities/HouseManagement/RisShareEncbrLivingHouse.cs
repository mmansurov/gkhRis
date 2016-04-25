namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Части долей и обремений по жилым домам
    /// </summary>
    public class RisShareEncbrLivingHouse : BaseRisEntity
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
        /// Жилой дом
        /// </summary>
        public virtual RisHouse House { get; set; }
    }
}
