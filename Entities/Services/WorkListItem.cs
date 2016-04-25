namespace Bars.Gkh.Ris.Entities.Services
{
    using GisIntegration.Ref;
    using Nsi;

    /// <summary>
    /// Работа/услуга перечня
    /// </summary>
    public class WorkListItem : BaseRisEntity
    {
        /// <summary>
        /// Перечень работ/услуг
        /// </summary>
        public virtual WorkList WorkList { get; set; }

        /// <summary>
        /// Общая стоимость
        /// </summary>
        public virtual decimal TotalCost { get; set; }

        /// <summary>
        /// Ссылка на работу/услугу организации (НСИ 59)
        /// </summary>
        public virtual ServiceType WorkItemNsi { get; set; }

        /// <summary>
        /// Номер строки в перечне работ и услуг
        /// </summary>
        public virtual int Index { get; set; }
    }
}