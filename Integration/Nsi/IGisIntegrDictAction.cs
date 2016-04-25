namespace Bars.Gkh.Ris.Integration.Nsi
{
    using System;
    using System.Collections.Generic;

    using Bars.B4;
    using Bars.Gkh.Ris.NsiCommon;

    using DictionaryAction;
    using Entities.GisIntegration;
    using Entities.GisIntegration.Ref;

    public interface IGisIntegrDictAction
    {
        /// <summary>
        /// Код справочника 
        /// </summary>
        string Code { get; }

        /// <summary>
        /// Тип справочника 
        /// </summary>
        Type ClassType { get; }

        /// <summary>
        /// Клиент сервиса Nsi ГИС
        /// </summary>
        NsiPortsTypeClient SoapClient { get; set; }

        /// <summary>
        /// Справочник Барс
        /// </summary>
        GisDict Dict { get; set; }

        /// <summary>
        /// Метод обновления каталога 
        /// Добавляются отсутсвующие записи, актуализируются суммарные значения Количество сопоставленных и несопоставленных значений
        /// </summary>
        IDataResult Update();

        List<GisDictRef> GetRefRecords();

        List<GkhDictProxyRecord> GetGkhRecords();

        List<GisDictProxyRecord> GetGisRecords();
    }
}