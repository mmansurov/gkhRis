namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;
    using Enums.HouseManagement;

    /// <summary>
    /// Счет
    /// </summary>
    public class RisAccount : BaseRisEntity
    {
        /// <summary>
        /// Счет для УО/РСО
        /// </summary>
        public virtual RisAccountType RisAccountType { get; set; }

        /// <summary>
        /// Собственник ФЛ
        /// </summary>
        public virtual RisInd OwnerInd { get; set; }

        /// <summary>
        /// Собственник ЮЛ/ИП/ОП
        /// </summary>
        public virtual RisContragent OwnerOrg { get; set; }

        /// <summary>
        /// ФЛ по договору социального наема
        /// </summary>
        public virtual RisInd RenterInd { get; set; }

        /// <summary>
        /// Арендатор ЮЛ/ИП/ОП
        /// </summary>
        public virtual RisContragent RenterOrg { get; set; }

        /// <summary>
        /// Количество проживающих
        /// </summary>
        public virtual int? LivingPersonsNumber { get; set; }

        /// <summary>
        /// Общая площадь для ЛС
        /// </summary>
        public virtual decimal? TotalSquare { get; set; }

        /// <summary>
        /// Жилая площадь
        /// </summary>
        public virtual decimal? ResidentialSquare { get; set; }

        /// <summary>
        /// Отапливаемая площадь
        /// </summary>
        public virtual decimal? HeatedArea { get; set; }

        /// <summary>
        /// Закрыт
        /// </summary>
        public virtual bool Closed { get; set; }

        /// <summary>
        /// Код справочника "Причина закрытия"
        /// </summary>
        public virtual string CloseReasonCode { get; set; }

        /// <summary>
        /// Гуид справочника "Причина закрытия"
        /// </summary>
        public virtual string CloseReasonGuid { get; set; }

        /// <summary>
        /// Дата закрытия
        /// </summary>
        public virtual DateTime? CloseDate { get; set; }

        /// <summary>
        /// Номер лицевого счета/Иной идентификтатор плательщика
        /// </summary>
        public virtual string AccountNumber { get; set; }
    }
}