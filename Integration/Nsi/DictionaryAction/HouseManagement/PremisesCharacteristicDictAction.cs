namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction.HouseManagement
{
    using System;
    using System.Collections.Generic;

    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Enums.HouseManagement;

    /// <summary>
    /// Класс для сопоставления записей справочника  "Характеристика помещения"
    /// </summary>
    public class PremisesCharacteristicDictAction : BaseDictAction
    {
        /// <summary>
        /// Код справочника 
        /// </summary>
        public override string Code
        {
            get
            {
                return "Характеристика помещения";
            }
        }

        /// <summary>
        /// Тип справочника 
        /// </summary>
        public override Type ClassType
        {
            get
            {
                return typeof(PremisesCharacteristic);
            }
        }

        /// <summary>
        /// Получить записи справочника жкх
        /// </summary>
        /// <returns></returns>
        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var result = new List<GkhDictProxyRecord>();

            foreach (PremisesCharacteristic type in Enum.GetValues(this.ClassType))
            {
                result.Add(new GkhDictProxyRecord
                {
                    Id = type.GetHashCode(),
                    Name = type.GetEnumMeta().Display
                });

            }

            return result;
        }
    }
}
