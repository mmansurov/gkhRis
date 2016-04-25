namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction.HouseManagement
{
    using System;
    using System.Collections.Generic;
    using B4.Utils;
    using Enums.HouseManagement;

    /// <summary>
    /// Метод для сопоставления состояний дома
    /// </summary>
    public class HouseStateDictAction : BaseDictAction
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
                return typeof(HouseState);
            }
        }

        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var result = new List<GkhDictProxyRecord>();

            foreach (HouseState type in Enum.GetValues(ClassType))
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