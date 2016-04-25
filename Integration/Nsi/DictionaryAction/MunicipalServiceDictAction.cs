namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4.DataAccess;
    using Bars.Gkh.Gis.Entities.Dict;
    using Bars.Gkh.Gis.Enum;

    /// <summary>
    /// Сопоставление справочника ГИС "Вид коммунальной услуги" и справочника ЖКХ "Услуги"
    /// </summary>
    public class MunicipalServiceDictAction : BaseDictAction
    {
        /// <summary>
        /// Код.
        /// </summary>
        public override string Code
        {
            get { return "Коммунальные услуги"; }
        }

        /// <summary>
        /// Тип класса.
        /// </summary>
        public override Type ClassType
        {
            get { return typeof(ServiceDictionary); }
        }

        /// <summary>
        /// Получение записей справочника.
        /// </summary>
        /// <returns></returns>
        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var domain = this.Container.ResolveDomain<ServiceDictionary>();

            try
            {
                return domain.GetAll()
                    .Where( x => x.TypeService == TypeServiceGis.Communal)
                    .Select(x => new GkhDictProxyRecord
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
            }
            finally
            {
                this.Container.Release(domain);
            }
        }
    }
}
