namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction.HouseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4.DataAccess;
    using Gkh.Entities;

    /// <summary>
    /// Сопоставление типов проекта здания
    /// </summary>
    public class ProjectTypeDictAction : BaseDictAction
    {
        /// <summary>
        /// Код справочника 
        /// </summary>
        public override string Code
        {
            get
            {
                return "Тип проекта здания";
            }
        }

        /// <summary>
        /// Тип справочника 
        /// </summary>
        public override Type ClassType
        {
            get
            {
                return typeof(WallMaterial);
            }
        }

        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var domain = Container.ResolveDomain<WallMaterial>();

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
                Container.Release(domain);
            }
        }
    }
}