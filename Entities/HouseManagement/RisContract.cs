namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;
    using Enums.HouseManagement;

    /// <summary>
    /// Договоры управления
    /// </summary>
    public class RisContract : BaseRisEntity
    {
        /// <summary>
        /// Номер
        /// </summary>
        public virtual string DocNum { get; set; }

        /// <summary>
        /// Дата заключения
        /// </summary>
        public virtual DateTime? SigningDate { get; set; }

        /// <summary>
        /// Дата вступления в силу
        /// </summary>
        public virtual DateTime? EffectiveDate { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public virtual DateTime? PlanDateComptetion { get; set; }

        /// <summary>
        /// Месяц
        /// </summary>
        public virtual int? ValidityMonth { get; set; }

        /// <summary>
        /// Год
        /// </summary>
        public virtual int? ValidityYear { get; set; }

        /// <summary>
        /// Тип владельца
        /// </summary>
        public virtual RisContractOwnersType? OwnersType { get; set; }

        /// <summary>
        /// Организация
        /// </summary>
        public virtual RisContragent Org { get; set; }

        /// <summary>
        /// Номер извещения
        /// </summary>
        public virtual string ProtocolPurchaseNumber { get; set; }

        /// <summary>
        /// Ссылка на НСИ "Основание заключения договора" (реестровый номер 58)_Код записи справочника
        /// </summary>
        public virtual string ContractBaseCode { get; set; }

        /// <summary>
        /// Ссылка на НСИ "Основание заключения договора" (реестровый номер 58)_Идентификатор в ГИС ЖКХ
        /// </summary>
        public virtual string ContractBaseGuid { get; set; }

        /// <summary>
        ///  День месяца начала ввода показаний по приборам учета
        /// </summary>
        public virtual  int? InputMeteringDeviceValuesBeginDate { get; set; }

        /// <summary>
        /// День месяца окончания ввода показаний по приборам учета
        /// </summary>
        public virtual int? InputMeteringDeviceValuesEndDate { get; set; }

        /// <summary>
        /// День выставления платежных документов
        /// </summary>
        public virtual int? DrawingPaymentDocumentDate { get; set; }

        /// <summary>
        /// День выставления платежных документов - этого месяца (если false - следующего месяца)
        /// </summary>
        public virtual bool ThisMonthPaymentDocDate { get; set; }

        /// <summary>
        /// Запрос лицензии
        /// </summary>
        public virtual bool LicenseRequest { get; set; }
    }
}
