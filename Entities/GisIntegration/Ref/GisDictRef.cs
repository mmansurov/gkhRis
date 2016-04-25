namespace Bars.Gkh.Ris.Entities.GisIntegration.Ref
{
    /// <summary>
    /// Запись справочника. Содержит свойства для связи записи справочника ГИС с записями соответствующих справочников систем источников данных.
    /// </summary>
    public class GisDictRef : BaseRisEntity
    {
        /// <summary>
        /// Полное имя класса справочника или енума
        /// </summary>
        public virtual string ClassName { get; set; }

        /// <summary>
        /// Идентификатор записи ЖКХ 
        /// </summary>
        public virtual long GkhId { get; set; }

        /// <summary>
        /// Наименование записи в  ЖКХ 
        /// </summary>
        public virtual string GkhName { get; set; }

        /// <summary>
        /// Идентификатор записи справочника в системе ГИС
        /// </summary>
        public virtual string GisId { get; set; }

        /// <summary>
        /// Гуид записи справочника в системе ГИС
        /// </summary>
        public virtual string GisGuid { get; set; }

        /// <summary>
        /// Наименование записи в ГИС 
        /// </summary>
        public virtual string GisName { get; set; }

        /// <summary>
        /// Каталог справочника  
        /// </summary>
        public virtual GisDict Dict { get; set; }
    }
}