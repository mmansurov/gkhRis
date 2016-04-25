namespace Bars.Gkh.Ris.Entities.Infrastructure
{
    /// <summary>
    /// Сведения об участках сети
    /// </summary>
    public class RisNetPieces : BaseRisEntity
    {
        /// <summary>
        /// Объект коммунальной инфраструктуры
        /// </summary>
        public virtual RisRkiItem RkiItem { get; set; }

        /// <summary>
        /// Наименование участка
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Диаметр(мм)
        /// </summary>
        public virtual decimal? Diameter { get; set; }

        /// <summary>
        /// Протяженность(км)
        /// </summary>
        public virtual decimal? Length { get; set; }

        /// <summary>
        /// Нуждается в замене(км)
        /// </summary>
        public virtual decimal? NeedReplaced { get; set; }

        /// <summary>
        /// Износ(%)
        /// </summary>
        public virtual decimal? Wearout { get; set; }

        /// <summary>
        /// НСИ "Уровень давления газопровода" - Код
        /// </summary>
        public virtual string PressureCode { get; set; }

        /// <summary>
        /// НСИ "Уровень давления газопровода" - Guid
        /// </summary>
        public virtual string PressureGuid { get; set; }

        /// <summary>
        /// НСИ "Уровень давления газопровода" - Наименование
        /// </summary>
        public virtual string PressureName { get; set; }

        /// <summary>
        /// НСИ "Уровень напряжения" - Код
        /// </summary>
        public virtual string VoltageCode { get; set; }

        /// <summary>
        /// НСИ "Уровень напряжения" - Guid
        /// </summary>
        public virtual string VoltageGuid { get; set; }

        /// <summary>
        /// НСИ "Уровень напряжения" - Наименование
        /// </summary>
        public virtual string VoltageName { get; set; }
    }
}
