namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction
{
    using System;
    using System.Collections.Generic;
    using B4.Utils;
    using Enums;

    /// <summary>
    /// Метод для сопоставления форм проверки
    /// </summary>
    public class CheckFormDictAction : BaseDictAction
    {
        /// <summary>
        /// Код справочника 
        /// </summary>
        public override string Code
        {
            get
            {
                return "Формы проверки";
            }
        }

        /// <summary>
        /// Тип справочника 
        /// </summary>
        public override Type ClassType
        {
            get
            {
                return typeof(CheckForm);
            }
        }

        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var result = new List<GkhDictProxyRecord>();

            foreach (CheckForm type in Enum.GetValues(ClassType))
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