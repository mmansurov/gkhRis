namespace Bars.Gkh.Ris.Entities.Infrastructure
{
    /// <summary>
    /// Характеристика сетевого объекта
    /// </summary>
    public class RisResource : BaseRisEntity
    {
        /// <summary>
        /// Объект коммунальной инфраструктуры
        /// </summary>
        public virtual RisRkiItem RkiItem { get; set; }

        /// <summary>
        /// НСИ "Вид коммунального ресурса" - Код
        /// </summary>
        public virtual string MunicipalResourceCode { get; set; }

        /// <summary>
        /// НСИ "Вид коммунального ресурса" - Guid
        /// </summary>
        public virtual string MunicipalResourceGuid { get; set; }

        /// <summary>
        /// НСИ "Вид коммунального ресурса" - Наименование
        /// </summary>
        public virtual string MunicipalResourceName { get; set; }

        /// <summary>
        /// Присоединенная нагрузка
        /// </summary>
        public virtual decimal? TotalLoad { get; set; }

        /// <summary>
        /// Промышленность
        /// </summary>
        public virtual decimal? IndustrialLoad { get; set; }

        /// <summary>
        /// Социальная сфера
        /// </summary>
        public virtual decimal? SocialLoad { get; set; }

        /// <summary>
        /// Население
        /// </summary>
        public virtual decimal? PopulationLoad { get; set; }

        /// <summary>
        /// Установленная мощность
        /// </summary>
        public virtual decimal? SetPower { get; set; }

        /// <summary>
        /// Распологаемая мощность
        /// </summary>
        public virtual decimal? SitingPower { get; set; }
    }
}
