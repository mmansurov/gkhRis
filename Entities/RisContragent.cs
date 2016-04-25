namespace Bars.Gkh.Ris.Entities
{
    using B4.DataAccess;

    /// <summary>
    /// Контрагент
    /// </summary>
    public class RisContragent : BaseEntity
    {
        /// <summary>
        /// Id в системе, из которой загружен
        /// </summary>
        public virtual long GkhId { get; set; }

        /// <summary>
        /// Полное наименование
        /// </summary>
        public virtual string FullName { get; set; }

        /// <summary>
        /// ОГРН
        /// </summary>
        public virtual string Ogrn { get; set; }

        /// <summary>
        /// OrgRootEntityGuid
        /// </summary>
        public virtual string OrgRootEntityGuid { get; set; }

        /// <summary>
        /// OrgVersionGuid
        /// </summary>
        public virtual string OrgVersionGuid { get; set; }

        /// <summary>
        /// Guid-идентификатор контрагента
        /// </summary>
        public virtual string SenderId { get; set; }

        /// <summary>
        /// Фактический адрес
        /// </summary>
        public virtual string FactAddress { get; set; }

        /// <summary>
        /// Юридический адрес
        /// </summary>
        public virtual string JuridicalAddress { get; set; }

        /// <summary>
        /// Является индивидуальным предпринимателем
        /// </summary>
        public virtual bool IsIndividual { get; set; }
    }
}