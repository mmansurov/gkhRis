namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4.DataAccess;
    using GkhDi.Entities;

    /// <summary>
    /// Сопоставление типов услуг
    /// </summary>
    public class ServiceTypeDictAction : BaseDictAction
    {
        /// <summary>
        /// Код.
        /// </summary>
        public override string Code
        {
            get { return "Услуга"; }
        }

        /// <summary>
        /// Тип класса.
        /// </summary>
        public override Type ClassType
        {
            get { return typeof(TemplateService); }
        }

        /// <summary>
        /// Получение записей справочника.
        /// </summary>
        /// <returns></returns>
        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var domain = this.Container.ResolveDomain<TemplateService>();

            try
            {
                return domain.GetAll().Select(x => new GkhDictProxyRecord
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
