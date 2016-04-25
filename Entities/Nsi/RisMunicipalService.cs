namespace Bars.Gkh.Ris.Entities.Nsi
{
    /// <summary>
    /// Запись справочника «Коммунальные услуги»
    /// </summary>
    public class RisMunicipalService : BaseRisEntity
    {
        /// <summary>
        /// Ссылка на НСИ "Вид коммунальной услуги" - Код в ГИС ЖКХ
        /// </summary>
        public virtual string MunicipalServiceRefCode { get; set; }

        /// <summary>
        /// Ссылка на НСИ "Вид коммунальной услуги" - Guid в ГИС ЖКХ
        /// </summary>
        public virtual string MunicipalServiceRefGuid { get; set; }

        /// <summary>
        /// Признак "Услуга предоставляется на общедомовые нужды"
        /// </summary>
        public virtual bool GeneralNeeds { get; set; }

        /// <summary>
        /// Наименование главной коммунальной услуги
        /// </summary>
        public virtual string MainMunicipalServiceName { get; set; }

        /// <summary>
        /// Ссылка на НСИ "Вид коммунального ресурса" - Код в ГИС ЖКХ
        /// </summary>
        public virtual string MunicipalResourceRefCode { get; set; }

        /// <summary>
        /// Ссылка на НСИ "Вид коммунального ресурса" - Guid в ГИС ЖКХ
        /// </summary>
        public virtual string MunicipalResourceRefGuid { get; set; }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        public virtual string SortOrder { get; set; }

        /// <summary>
        /// Порядок сортировки не задан
        /// </summary>
        public virtual bool SortOrderNotDefined { get; set; }
    }
}
