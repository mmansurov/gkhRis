namespace Bars.Gkh.Ris.Entities.Inspection
{
    using System;

    /// <summary>
    /// Проверка юридического лица
    /// </summary>
    public class Examination : BaseRisEntity
    {
        /// <summary>
        /// План проверок
        /// </summary>
        public virtual InspectionPlan InspectionPlan { get; set; }

        /// <summary>
        /// Порядковый номер
        /// </summary>
        public virtual int? InspectionNumber { get; set; }

        /// <summary>
        /// Субъект проверки
        /// </summary>
        public virtual RisContragent RisContragent { get; set; }

        /// <summary>
        /// Код основания проверки
        /// </summary>
        public virtual string BaseCode { get; set; }

        /// <summary>
        /// Guid основания проверки
        /// </summary>
        public virtual string BaseGuid { get; set; }

        /// <summary>
        /// Срок проверки (кол-во дней)
        /// </summary>
        public virtual double CountDays { get; set; }

        /// <summary>
        /// Код формы проведения проверки
        /// </summary>
        public virtual string ExaminationFormCode { get; set; }

        /// <summary>
        /// Guid формы проведения проверки
        /// </summary>
        public virtual string ExaminationFormGuid { get; set; }

        /// <summary>
        /// Является ли проверка плановой
        /// </summary>
        public virtual bool IsScheduled { get; set; }

        /// <summary>
        /// Фамилия
        /// </summary>
        public virtual string LastName { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public virtual string FirstName { get; set; }

        /// <summary>
        /// Отчество
        /// </summary>
        public virtual string MiddleName { get; set; }

        /// <summary>
        /// Вид осуществления контрольной деятельности (код)
        /// </summary>
        public virtual string OversightActivitiesCode { get; set; }

        /// <summary>
        /// Вид осуществления контрольной деятельности (гуид)
        /// </summary>
        public virtual string OversightActivitiesGuid { get; set; }

        /// <summary>
        /// Цель проведения проверки
        /// </summary>
        public virtual string Objective { get; set; }

        /// <summary>
        /// Дата начала проведения проверки
        /// </summary>
        public virtual DateTime? From { get; set; }

        /// <summary>
        /// Дата окончания проведения проверки
        /// </summary>
        public virtual DateTime? To { get; set; }

        /// <summary>
        /// Кол-во дней проверки
        /// </summary>
        public virtual double Duration { get; set; }

        /// <summary>
        /// Код предмета проверки
        /// </summary>
        public virtual string ObjectCode { get; set; }

        /// <summary>
        /// Гуид предмета проверки
        /// </summary>
        public virtual string ObjectGuid { get; set; }

        /// <summary>
        /// Задачи проведения проверки
        /// </summary>
        public virtual string Tasks { get; set; }

        /// <summary>
        /// Id приказа в МЖФ
        /// </summary>
        public virtual long OrderGkhId { get; set; }

        /// <summary>
        /// Номер приказа
        /// </summary>
        public virtual string OrderNumber { get; set; }

        /// <summary>
        /// Дата утверждения приказа
        /// </summary>
        public virtual DateTime? OrderDate { get; set; }

        /// <summary>
        /// Проверка в отношении физ. лица?
        /// </summary>
        public virtual bool IsPhysicalPerson { get; set; }
    }
}