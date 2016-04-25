namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    /// <summary>
    /// Сущность для реализации отношения "многие-ко-многим" между домами и записями справочника "Тип внутренних стен"
    /// Пока вместо ссылки на запись справочника храним code и guid
    /// </summary>
    public class RisHouseInnerWallMaterial : BaseRisEntity
    {
        /// <summary>
        /// Ссылка на дом
        /// </summary>
        public virtual RisHouse House { get; set; }

        /// <summary>
        /// Код записи справочника "Тип внутренних стен"
        /// </summary>
        public virtual string InnerWallMaterialCode { get; set; }

        /// <summary>
        /// Guid записи справочника "Тип внутренних стен"
        /// </summary>
        public virtual string InnerWallMaterialGuid { get; set; }
    }
}
