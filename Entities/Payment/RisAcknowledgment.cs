namespace Bars.Gkh.Ris.Entities.Payment
{
    /// <summary>
    /// Сведения о квитировании
    /// </summary>
    public class RisAcknowledgment : BaseRisEntity
    {
        /// <summary>
        /// Уникальный идентификатор распоряжения
        /// </summary>
        public virtual string OrderId { get; set; }

        /// <summary>
        /// Идентификатор платежного документа (счета на оплату)
        /// </summary>
        public virtual string PaymentDocumentNumber { get; set; }

        /// <summary>
        /// Вид КУ (справочник "Вид коммунальной услуги")
        /// </summary>
        public virtual string HSType { get; set; }

        /// <summary>
        /// Вид ЖУ (справочник "Классификатор видов работ (услуг)")
        /// </summary>
        public virtual string MSType { get; set; }

        /// <summary>
        /// Вид ДУ (справочник "Вид дополнительной услуги")
        /// </summary>
        public virtual string ASType { get; set; }

        /// <summary>
        /// Сумма квитирования (в копейках)
        /// </summary>
        public virtual decimal Amount { get; set; }
    }
}
