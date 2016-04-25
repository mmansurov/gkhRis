namespace Bars.Gkh.Ris.Entities.Infrastructure
{
    /// <summary>
    /// Источник ОКИ
    /// </summary>
    public class RisSourceOki : BaseRisEntity
    {
        /// <summary>
        /// Объект коммунальной инфраструктуры
        /// </summary>
        public virtual RisRkiItem RkiItem { get; set; }

        /// <summary>
        /// Идентификатор ОКИ в ГИС ЖКХ
        /// </summary>
        public virtual string SourceOki { get; set; }
    }
}
