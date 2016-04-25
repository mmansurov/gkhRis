namespace Bars.Gkh.Ris.Entities.Infrastructure
{
    using System;
    using HouseManagement;

    /// <summary>
    /// Объект коммунальной инфраструктуры
    /// </summary>
    public class RisRkiItem : BaseRisEntity
    {
        /// <summary>
        /// Наименование объекта
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// НСИ "Основание эксплуатации объекта инфраструктуры" - Код
        /// </summary>
        public virtual string BaseCode { get; set; }

        /// <summary>
        /// НСИ "Основание эксплуатации объекта инфраструктуры" - Guid
        /// </summary>
        public virtual string BaseGuid { get; set; }

        /// <summary>
        /// Окончание управления
        /// </summary>
        public virtual DateTime? EndManagmentDate { get; set; }

        /// <summary>
        /// Бессрочное управление
        /// </summary>
        public virtual bool? IndefiniteManagement { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        public virtual RisContragent RisRkiContragent { get; set; }

        /// <summary>
        /// На балансе муниципального образования
        /// </summary>
        public virtual bool? Municipalities { get; set; }

        /// <summary>
        /// НСИ "Вид ОКИ" - Код
        /// </summary>
        public virtual string TypeCode { get; set; }

        /// <summary>
        /// НСИ "Вид ОКИ" - Guid
        /// </summary>
        public virtual string TypeGuid { get; set; }

        /// <summary>
        /// НСИ "Вид водозаборного сооружения" - Код
        /// </summary>
        public virtual string WaterIntakeCode { get; set; }

        /// <summary>
        /// НСИ "Вид водозаборного сооружения" - Guid
        /// </summary>
        public virtual string WaterIntakeGuid { get; set; }

        /// <summary>
        /// НСИ "Тип электрической подстанции" - Код
        /// </summary>
        public virtual string ESubstationCode { get; set; }

        /// <summary>
        /// НСИ "Тип электрической подстанции" - Guid
        /// </summary>
        public virtual string ESubstationGuid { get; set; }

        /// <summary>
        /// НСИ "Вид электростанции" - Код
        /// </summary>
        public virtual string PowerPlantCode { get; set; }

        /// <summary>
        /// НСИ "Вид электростанции" - Guid
        /// </summary>
        public virtual string PowerPlantGuid { get; set; }

        /// <summary>
        /// НСИ "Вид топлива" - Код
        /// </summary>
        public virtual string FuelCode { get; set; }

        /// <summary>
        /// НСИ "Вид топлива" - Guid
        /// </summary>
        public virtual string FuelGuid { get; set; }

        /// <summary>
        /// НСИ "Тип газораспределительной сети" - Код
        /// </summary>
        public virtual string GasNetworkCode { get; set; }

        /// <summary>
        /// НСИ "Тип газораспределительной сети" - Guid
        /// </summary>
        public virtual string GasNetworkGuid { get; set; }

        /// <summary>
        /// Дом
        /// </summary>
        public virtual RisHouse House { get; set; }

        /// <summary>
        /// Код по ОКТМО
        /// </summary>
        public virtual string OktmoCode { get; set; }

        /// <summary>
        /// Полное наименование по ОКТМО
        /// </summary>
        public virtual string OktmoName { get; set; }

        /// <summary>
        /// Автономный источник снабжения
        /// </summary>
        public virtual bool? IndependentSource { get; set; }

        /// <summary>
        /// Уровень износа (%)
        /// </summary>
        public virtual decimal? Deterioration { get; set; }

        /// <summary>
        /// Число аварий на 100 км сетей
        /// </summary>
        public virtual int? CountAccidents { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public virtual string AddInfo { get; set; }


    }
}
