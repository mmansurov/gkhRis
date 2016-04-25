namespace Bars.Gkh.Ris.Entities.Infrastructure
{
    /// <summary>
    /// Приемник ОКИ
    /// </summary>
    public class RisReceiverOki : BaseRisEntity
    {
        /// <summary>
        /// Объект коммунальной инфраструктуры
        /// </summary>
        public virtual RisRkiItem RkiItem { get; set; }

        /// <summary>
        /// Идентификатор ОКИ в ГИС ЖКХ
        /// </summary>
        public virtual string ReceiverOki { get; set; }
    }
}
