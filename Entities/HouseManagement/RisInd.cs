using Bars.Gkh.Ris.Enums;

namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;

    /// <summary>
    /// Сведения о физлице
    /// </summary>
    public class RisInd : BaseRisEntity
    {
        /// <summary>
        /// Фамилия
        /// </summary>
        public virtual string Surname { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public virtual string Patronymic { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        public virtual RisGender Sex { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public virtual DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Документ, удостоверяющий личность (НСИ 95)
        /// </summary>
        public virtual string IdTypeGuid { get; set; }

        /// <summary>
        /// Документ, удостоверяющий личность (НСИ 95)
        /// </summary>
        public virtual string IdTypeCode { get; set; }

        /// <summary>
        /// Серия документа
        /// </summary>
        public virtual string IdSeries { get; set; }

        /// <summary>
        /// Номер документа
        /// </summary>
        public virtual string IdNumber { get; set; }

        /// <summary>
        /// Дата выдачи документа
        /// </summary>
        public virtual DateTime? IdIssueDate { get; set; }

        /// <summary>
        /// SNILS
        /// </summary>
        public virtual string Snils { get; set; }

        /// <summary>
        /// Место рождения
        /// </summary>
        public virtual string PlaceBirth { get; set; }

        /// <summary>
        /// Зарегистрирован
        /// </summary>
        public virtual bool? IsRegistered { get; set; }

        /// <summary>
        /// Проживает
        /// </summary>
        public virtual bool? IsResides { get; set; }
    }
}
