namespace Bars.Gkh.Ris.Integration.Nsi.DictionaryAction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4;
    using Bars.B4.DataAccess;
    using B4.Utils;

    using Bars.Gkh.Ris.NsiCommon;

    using Castle.Windsor;
    using Entities.GisIntegration;
    using Entities.GisIntegration.Ref;
    using Nsi;

    public abstract class BaseDictAction : IGisIntegrDictAction
    {
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Код справочника 
        /// </summary>
        public abstract string Code { get; }

        /// <summary>
        /// Тип справочника 
        /// </summary>
        public abstract Type ClassType { get; }

        /// <summary>
        /// Клиент для сервисов ГИС
        /// </summary>
        public NsiPortsTypeClient SoapClient { get; set; }

        /// <summary>
        /// Справочник ГИС
        /// </summary>
        public GisDict Dict { get; set; }

        public IDataResult Update()
        {
            if (this.Dict == null)
            {
                return new BaseDataResult(false, "Не удалось опредлить справочник ГИС");
            }

            if (this.Dict.NsiRegistryNumber.IsEmpty())
            {
                return new BaseDataResult(false, "Не задан идентификатор справочника ГИС");
            }

            var refs = this.GetRefRecords();
            var gkhRecs = this.GetGkhRecords();
            var gisRecs = this.GetGisRecords();

            var curRefDict = refs.Where(x => !string.IsNullOrEmpty(x.GkhName))
                .GroupBy(x => x.GkhName.ToLower())
                .ToDictionary(x => x.Key, y => y.FirstOrDefault());
            var gisDict = gisRecs.Where(x => !string.IsNullOrEmpty(x.Name))
                .GroupBy(x => x.Name.ToLower())
                .ToDictionary(x => x.Key, y => y.FirstOrDefault());

            var refToSave = new List<GisDictRef>();

            foreach (var gkhRec in gkhRecs)
            {
                var name = gkhRec.Name.ToLower();

                var gkhName = gkhRec.Name;
                var gkhId = gkhRec.Id;
                var gisId = string.Empty;
                var gisName = string.Empty;
                var gisGuid = string.Empty;

                var rec = curRefDict.Get(name);

                if (rec == null)
                {
                    rec = new GisDictRef { Dict = this.Dict };
                }
                else
                {
                    // берем снова те сопоставления котоыре были уже отобраны для текущей стрки
                    gisId = rec.GisId;
                    gisName = rec.GisName;
                    gisGuid = rec.GisGuid;
                    curRefDict.Remove(name); // удаляем, чтобы потом не удалить
                }

                var gisRec = gisDict.Get(name);

                // Если найдено четкое соответсвие по имени в справочнике ГИС, то эти значения считаем приоритетными 
                if (gisRec != null)
                {
                    gisId = gisRec.Id.ToString();
                    gisName = gisRec.Name;
                    gisGuid = gisRec.Guid;
                }

                if (rec.Id == 0L || rec.GkhId != gkhId || rec.GkhName != gkhName
                    || rec.GisId != gisId || rec.GisName != gisName || rec.GisGuid != gisGuid)
                {
                    rec.GkhId = gkhId;
                    rec.GkhName = gkhName;
                    rec.GisId = gisId;
                    rec.GisName = gisName;
                    rec.GisGuid = gisGuid;
                    rec.ClassName = this.ClassType.FullName;
                    refToSave.Add(rec);
                }

            }

            using (var tr = this.Container.Resolve<IDataTransaction>())
            {
                var gisDictRefDomain = this.Container.ResolveDomain<GisDictRef>();

                try
                {
                    foreach (var item in refToSave)
                    {
                        if (item.Id > 0)
                        {
                            gisDictRefDomain.Update(item);
                        }
                        else
                        {
                            gisDictRefDomain.Save(item);
                        }
                    }

                    foreach (var kvp in curRefDict)
                    {
                        gisDictRefDomain.Delete(kvp.Value.Id);
                    }

                    tr.Commit();
                }
                catch (System.Exception)
                {
                    tr.Rollback();
                    throw;
                }
                finally
                {
                    this.Container.Release(gisDictRefDomain);
                }
            }

            return new BaseDataResult();

        }

        /// <summary>
        /// ПОЛУЧАЕМ сохраненые связи между справочниками
        /// </summary>
        /// <returns></returns>
        public List<GisDictRef> GetRefRecords()
        {
            var gisDictRefDomain = this.Container.ResolveDomain<GisDictRef>();

            try
            {
                return gisDictRefDomain.GetAll()
                    .Where(x => x.Dict != null)
                    .Where(x => x.Dict.Id == this.Dict.Id)
                    .ToList();
            }
            finally
            {
                this.Container.Release(gisDictRefDomain);
            }
        }

        /// <summary>
        /// Получаем данные сохраненные для данного спраовчника
        /// </summary>
        /// <returns></returns>
        public abstract List<GkhDictProxyRecord> GetGkhRecords();

        /// <summary>
        /// Получаем данные из ГИС
        /// </summary>
        /// <returns></returns>
        public List<GisDictProxyRecord> GetGisRecords()
        {
            var requestHeader = new HeaderType
            {
                Date = DateTime.Now,
                MessageGUID = Guid.NewGuid().ToStr()//,

                //SenderID = "7cca76f6-175f-44d7-bdc0-3700801f64ca"
            };

            var itemRequest = new exportNsiItemRequest
            {
                Id = "block-to-sign",
                RegistryNumber = this.Dict.NsiRegistryNumber
            };

            exportNsiItemResult itemResult;

            this.SoapClient.exportNsiItem(requestHeader, itemRequest, out itemResult);

            var error = itemResult.Item as ErrorMessageType;

            if (error != null)
            {
                var messageText = string.Format("Ошибка при загрузке списка записей справочника: ErrorCode: {0}, Description: {1}", error.ErrorCode, error.Description);

                throw new Exception(messageText);
            }

            var items = (NsiItemType)itemResult.Item;

            if (items.NsiElement.Any())
            {
                return items.NsiElement.Select(x =>
                {
                    var nsiElementField = x.NsiElementField.FirstOrDefault(y => y.GetType() == typeof(NsiElementStringFieldType));

                    return new GisDictProxyRecord
                    {
                        Id = x.Code.ToLong(),
                        Name = nsiElementField is NsiElementStringFieldType
                            ? ((NsiElementStringFieldType)nsiElementField).Value
                            : string.Empty,
                        Guid = x.GUID
                    };
                })
                .Where(x => x.Name.IsNotEmpty())
                .ToList();
            }


            return new List<GisDictProxyRecord>();
        }
    }

    public class GkhDictProxyRecord
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
    public class GisDictProxyRecord
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Guid { get; set; }
    }
}