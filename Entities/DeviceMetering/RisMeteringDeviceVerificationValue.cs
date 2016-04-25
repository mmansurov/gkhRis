namespace Bars.Gkh.Ris.Entities.DeviceMetering
{
    using System;

    using Bars.Gkh.Ris.Entities.HouseManagement;

    /// <summary>
    /// Показание поверки ПУ
    /// </summary>
    public class RisMeteringDeviceVerificationValue : BaseRisEntity
    {
        /// <summary>
        /// Ссылка на данные ПУ
        /// </summary>
        public virtual RisMeteringDeviceData MeteringDeviceData { get; set; }

        /// <summary>
        /// Ссылка на счет
        /// </summary>
        public virtual RisAccount Account { get; set; }

        /// <summary>
        /// Показание начала поверки по тарифу T1
        /// </summary>
        public virtual decimal StartVerificationValueT1 { get; set; }

        /// <summary>
        /// Показание начала поверки по тарифу T2
        /// </summary>
        public virtual decimal? StartVerificationValueT2 { get; set; }

        /// <summary>
        /// Показание начала поверки по тарифу T3
        /// </summary>
        public virtual decimal? StartVerificationValueT3 { get; set; }

        /// <summary>
        /// Дата снятия показания начала поверки
        /// </summary>
        public virtual DateTime StartVerificationReadoutDate { get; set; }

        /// <summary>
        /// Кем внесено показание начала поверки
        /// </summary>
        public virtual string StartVerificationReadingsSource { get; set; }

        /// <summary>
        /// Показание окончания поверки по тарифу T1
        /// </summary>
        public virtual decimal EndVerificationValueT1 { get; set; }

        /// <summary>
        /// Показание окончания поверки по тарифу T2
        /// </summary>
        public virtual decimal? EndVerificationValueT2 { get; set; }

        /// <summary>
        /// Показание окончания поверки по тарифу T3
        /// </summary>
        public virtual decimal? EndVerificationValueT3 { get; set; }

        /// <summary>
        /// Дата снятия показания окончания поверки
        /// </summary>
        public virtual DateTime EndVerificationReadoutDate { get; set; }

        /// <summary>
        /// Кем внесено показание окончания поверки
        /// </summary>
        public virtual string EndVerificationReadingsSource { get; set; }

        /// <summary>
        /// Признак плановой поверки
        /// </summary>
        public virtual bool PlannedVerification { get; set; }

        /// <summary>
        /// Причина выхода ПУ из строя (НСИ 78) - Код записи справочника
        /// </summary>
        public virtual string VerificationReasonCode { get; set; }

        /// <summary>
        /// Причина выхода ПУ из строя (НСИ 78) - Идентификатор в ГИС ЖКХ
        /// </summary>
        public virtual string VerificationReasonGuid { get; set; }

        /// <summary>
        /// Причина выхода ПУ из строя (НСИ 78) - Значение
        /// </summary>
        public virtual string VerificationReasonName { get; set; }
    }
}
