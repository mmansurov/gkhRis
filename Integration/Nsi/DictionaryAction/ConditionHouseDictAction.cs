namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction
{
    using System;
    using System.Collections.Generic;
    using B4.Utils;
    using Enums;

    /// <summary>
    /// Метод для сопоставления состояния дома
    /// </summary>
    public class ConditionHouseDictAction : BaseDictAction
    {
        /// <summary>
        /// Код справочника 
        /// </summary>
        public override string Code
        {
            get
            {
                return "Состояние дома";
            }
        }

        /// <summary>
        /// Тип справочника 
        /// </summary>
        public override Type ClassType
        {
            get
            {
                return typeof(ConditionHouse);
            }
        }

        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var result = new List<GkhDictProxyRecord>();

            foreach (ConditionHouse type in Enum.GetValues(ClassType))
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