namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction
{
    using System;
    using System.Collections.Generic;
    using B4.Utils;
    using Enums;

    /// <summary>
    /// Метод для сопоставления основания проверки
    /// </summary>
    public class TypeBaseCheckJurPersonDictAction : BaseDictAction
    {
        /// <summary>
        /// Код справочника 
        /// </summary>
        public override string Code
        {
            get
            {
                return "Основание проверки";
            }
        }

        /// <summary>
        /// Тип справочника 
        /// </summary>
        public override Type ClassType
        {
            get
            {
                return typeof(TypeBaseCheckJurPerson);
            }
        }

        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var result = new List<GkhDictProxyRecord>();

            foreach (TypeBaseCheckJurPerson type in Enum.GetValues(ClassType))
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