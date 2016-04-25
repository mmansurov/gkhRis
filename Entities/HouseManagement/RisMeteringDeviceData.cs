namespace Bars.Gkh.Ris.Entities.HouseManagement
{
    using System;
    using System.Collections.Generic;

    using Bars.Gkh.Ris.Enums.HouseManagement;


    /// <summary>
    /// Данные прибора учета
    /// </summary>
    public class RisMeteringDeviceData : BaseRisEntity
    {
        /// <summary>
        /// Тип ПУ
        /// </summary>
        public virtual MeteringDeviceType MeteringDeviceType { get; set; }

        /// <summary>
        /// Номер ПУ
        /// </summary>
        public virtual string MeteringDeviceNumber { get; set; }

        /// <summary>
        /// Марка ПУ
        /// </summary>
        public virtual string MeteringDeviceStamp { get; set; }

        /// <summary>
        /// Дата установки
        /// </summary>
        public virtual DateTime? InstallationDate { get; set; }

        /// <summary>
        /// Дата ввода в эксплуатацию
        /// </summary>
        public virtual DateTime CommissioningDate { get; set; }

        /// <summary>
        /// Внесение показаний осуществляется в ручном режиме
        /// </summary>
        public virtual bool ManualModeMetering { get; set; }

        /// <summary>
        /// Дата первичной поверки
        /// </summary>
        public virtual DateTime? FirstVerificationDate { get; set; }

        /// <summary>
        /// Межповерочный интервал (НСИ 16)
        /// </summary>
        public virtual string VerificationInterval { get; set; }

        /// <summary>
        /// Тип прибора учета
        /// </summary>
        public virtual DeviceType DeviceType { get; set; }

        /// <summary>
        /// Базовое показание T1 Характеристики ПУ 
        /// </summary>
        public virtual decimal MeteringValueT1 { get; set; }

        /// <summary>
        /// Базовое показание T2 Характеристики ПУ учета электрической энергии
        /// </summary>
        public virtual decimal? MeteringValueT2 { get; set; }

        /// <summary>
        /// Базовое показание T3. В зависимости от количества заданных при создании базовых значений ПУ определяется его тип по количеству тарифов.
        /// Характеристики ПУ учета электрической энергии
        /// </summary>
        public virtual decimal? MeteringValueT3 { get; set; }

        /// <summary>
        /// Время и дата снятия показания
        /// </summary>
        public virtual DateTime ReadoutDate { get; set; }

        /// <summary>
        /// Кем внесено
        /// </summary>
        public virtual string ReadingsSource { get; set; }

        /// <summary>
        /// Ссылка на дом
        /// </summary>
        public virtual RisHouse House { get; set; }

        /// <summary>
        /// Ссылка на жилое помещение
        /// </summary>
        public virtual ResidentialPremises ResidentialPremises { get; set; }

        /// <summary>
        /// Ссылка на нежилое помещение
        /// </summary>
        public virtual NonResidentialPremises NonResidentialPremises { get; set; }

        /// <summary>
        /// Коммунальный ресурс_Код записи справочника
        /// </summary>
        public virtual string MunicipalResourceCode { get; set; }

        /// <summary>
        /// Коммунальный ресурс_Идентификатор в ГИС ЖКХ
        /// </summary>
        public virtual string MunicipalResourceGuid { get; set; }
    }
}
