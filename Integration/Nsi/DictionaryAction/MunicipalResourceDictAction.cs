namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction
{
    using System;
    using System.Collections.Generic;

    using Bars.B4.Utils;
    using Bars.Gkh.Gis.Enum;

    /// <summary>
    /// Сопоставление справочника ГИС "Вид коммунального ресурса" и элементов енума ЖКХ TypeCommunalResourse
    /// </summary>
    class MunicipalResourceDictAction : BaseDictAction
    {
        /// <summary>
        /// Код.
        /// </summary>
        public override string Code
        {
            get { return "Вид коммунального ресурса"; }
        }

        /// <summary>
        /// Тип класса.
        /// </summary>
        public override Type ClassType
        {
            get { return typeof(TypeCommunalResourse); }
        }

        /// <summary>
        /// Получение записей справочника.
        /// </summary>
        /// <returns></returns>
        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var result = new List<GkhDictProxyRecord>();

            foreach (TypeCommunalResourse type in Enum.GetValues(this.ClassType))
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
