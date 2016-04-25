namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction
{
    using B4.DataAccess;
    using GkhGji.Entities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Метод для справочника Вид проверки
    /// </summary>
    public class KindCheckDictAction : BaseDictAction
    {
        /// <summary>
        /// Код справочника 
        /// </summary>
        public override string Code
        {
            get
            {
                return "Вид проверки";
            }
        }

        /// <summary>
        /// Тип справочника 
        /// </summary>
        public override Type ClassType
        {
            get
            {
                return typeof(KindCheckGji);
            }
        }

        public override List<GkhDictProxyRecord> GetGkhRecords()
        {
            var domain = Container.ResolveDomain<KindCheckGji>();

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