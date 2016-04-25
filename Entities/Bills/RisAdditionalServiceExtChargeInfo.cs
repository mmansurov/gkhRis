namespace Bars.Gkh.Ris.Entities.Bills
{
    /// <summary>
    /// Начисление по услуге - Объемы потребления по дополнительным услугам
    /// </summary>
    public class RisAdditionalServiceExtChargeInfo : BaseRisEntity
    {
        /// <summary>
        /// Ссылка на платежный документ
        /// </summary>
        public virtual RisPaymentDocument PaymentDocument { get; set; }

        /// <summary>
        /// Перерасчеты, корректировки (руб)
        /// </summary>
        public virtual decimal? MoneyRecalculation { get; set; }

        /// <summary>
        /// Льготы, субсидии, скидки (руб)
        /// </summary>
        public virtual decimal? MoneyDiscount { get; set; }

        /// <summary>
        /// Код услуги - Код записи справочника
        /// </summary>
        public virtual string ServiceTypeCode { get; set; }

        /// <summary>
        /// Код услуги - Идентификатор в ГИС ЖКХ(ссылка на справочник в ГИС)
        /// </summary>
        public virtual string ServiceTypeGuid { get; set; }
    }
}
