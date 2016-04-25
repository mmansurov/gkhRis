namespace Bars.Gkh.Ris.Entities.GisIntegration
{
    using System;
    using B4.DataAccess;

    /// <summary>
    /// Справочник. Содержит свойства для связи справочников ГИС со справочниками систем источников данных (пока свойство ActionCode)
    /// </summary>
    public class GisDict : BaseEntity
    {
        /// <summary>
        /// Наименование
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Идентификатор IGisIntegrationDictAction
        /// </summary>
        public virtual string ActionCode { get; set; }

        /// <summary>
        /// Номер справочника в Nsi
        /// </summary>
        public virtual string NsiRegistryNumber { get; set; }

        /// <summary>
        /// Дата обновления
        /// </summary>
        public virtual DateTime? DateIntegration { get; set; }
    }
}