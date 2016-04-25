//namespace Bars.Gkh.Ris.Integration.OrgRegistry.Methods
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using B4.DataAccess;
//    using B4.Utils;
//    using Domain;
//    using Entities.OrgRegistry;
//    using Ris.OrgRegistry;

//    /// <summary>
//    /// Класс - экспортер сведений об обособленных подразделениях
//    /// </summary>
//    public class ImportSubsidiaryMethod : GisIntegrationOrgRegistryMethod<RisSubsidiary, importSubsidiaryRequest>
//    {
//        private readonly List<RisSubsidiary> subsidiariesToSave;

//        private readonly Dictionary<string, RisSubsidiary> subsidiariesByTransportGuid;

//        /// <summary>
//        /// Конструктор класса экспортера
//        /// </summary>
//        public ImportSubsidiaryMethod()
//        {
//            this.subsidiariesToSave = new List<RisSubsidiary>();
//            this.subsidiariesByTransportGuid = new Dictionary<string, RisSubsidiary>();
//        }

//        /// <summary>
//        /// Код метода
//        /// </summary>
//        public override string Code
//        {
//            get
//            {
//                return "importSubsidiary";
//            }
//        }

//        /// <summary>
//        /// Наименование метода
//        /// </summary>
//        public override string Name
//        {
//            get
//            {
//                return "Импорт сведений об обособленных подразделениях";
//            }
//        }

///// <summary>
///// Порядок импорта в списке
///// </summary>
//public override int Order
//{
//    get
//    {
//        return 8;
//    }
//}

//        /// <summary>
//        /// Размер блока предаваемых данных (максимальное количество записей)
//        /// </summary>
//        protected override int Portion
//        {
//            get
//            {
//                return 100;
//            }
//        }

//        /// <summary>
//        /// Количество объектов для сохранения
//        /// </summary>
//        protected override int ProcessedObjects
//        {
//            get
//            {
//                return this.subsidiariesToSave.Count;
//            }
//        }

//        /// <summary>
//        /// Общее количество импортируемых объектов
//        /// </summary>
//        new protected int CountObjects
//        {
//            get
//            {
//                return this.subsidiariesByTransportGuid.Count;
//            }
//        }

//        /// <summary>
//        /// Список записей, по которым производится импорт.
//        /// </summary>
//        protected override IList<RisSubsidiary> MainList { get; set; }

//        /// <summary>
//        /// Подготовка кэша данных 
//        /// </summary>
//        protected override void Prepare()
//        {
//            this.MainList = this.GetMainList();
//        }

//        /// <summary>
//        /// Проверка данных об обособленном подразделении перед импортом
//        /// </summary>
//        /// <param name="item">Данные об обособленном подразделении</param>
//        /// <returns>Результат проверки</returns>
//        protected override CheckingResult CheckMainListItem(RisSubsidiary item)
//        {
//            StringBuilder messages = new StringBuilder();

//            if (string.IsNullOrEmpty(item.FullName))
//            {
//                messages.Append("FullName ");
//            }

//            if (string.IsNullOrEmpty(item.Ogrn))
//            {
//                messages.Append("Ogrn ");
//            }

//            if (string.IsNullOrEmpty(item.Inn))
//            {
//                messages.Append("Inn ");
//            }

//            if (string.IsNullOrEmpty(item.Kpp))
//            {
//                messages.Append("Kpp ");
//            }

//            if (string.IsNullOrEmpty(item.Okopf))
//            {
//                messages.Append("Okopf ");
//            }

//            if (string.IsNullOrEmpty(item.Address))
//            {
//                messages.Append("Address ");
//            }

//            if (string.IsNullOrEmpty(item.SourceName))
//            {
//                messages.Append("SourceName ");
//            }

//            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
//        }

//        /// <summary>
//        /// Получить объект запроса.
//        /// </summary>
//        /// <param name="listForImport">Список объектов для импорта</param>
//        /// <returns>Объект запроса</returns>
//        protected override importSubsidiaryRequest GetRequestObject(
//            IEnumerable<RisSubsidiary> listForImport)
//        {
//            var importRequestList = new List<importSubsidiaryRequestSubsidiary>();

//            foreach (var item in listForImport)
//            {
//                importRequestList.Add(this.PrepareSubsidiaryRequest(item));
//            }

//            return new importSubsidiaryRequest { Subsidiary = importRequestList.ToArray() };
//        }

//        /// <summary>
//        /// Получить ответ от сервиса.
//        /// </summary>
//        /// <param name="request">Объект запроса</param>
//        /// <returns>Ответ от сервиса</returns>
//        protected override ImportResult GetRequestResult(importSubsidiaryRequest request)
//        {
//            ImportResult result = null;
//            var soapClient = this.ServiceProvider.GetSoapClient();

//            if (soapClient != null)
//            {
//                soapClient.importSubsidiary(this.RequestHeader, request, out result);
//            }

//            return result;
//        }

//        /// <summary>
//        /// Сохранение объектов ГИС после импорта.
//        /// </summary>
//        protected override void SaveObjects()
//        {
//            TransactionHelper.InsertInManyTransactions(this.Container, this.subsidiariesToSave, 1000, true, true);
//        }

//        /// <summary>
//        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
//        /// </summary>
//        /// <param name="responseItem">Элемент response</param>
//        protected override void CheckResponseItem(CommonResultType responseItem)
//        {
//            if (!this.subsidiariesByTransportGuid.ContainsKey(responseItem.TransportGUID))
//            {
//                return;
//            }

//            var subsidiary = this.subsidiariesByTransportGuid[responseItem.TransportGUID];

//            if (responseItem.GUID.IsEmpty())
//            {
//                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

//                var errorNotation = string.Empty;

//                if (error != null)
//                {
//                    errorNotation = error.Description;
//                }

//                this.AddLineToLog("Сведения об обособленном подразделении", subsidiary.Id, "Не загружены", errorNotation);

//                return;
//            }

//            subsidiary.Guid = responseItem.GUID;
//            this.subsidiariesToSave.Add(subsidiary);

//            this.AddLineToLog("Сведения об обособленном подразделении", subsidiary.Id, "Загружены", responseItem.GUID);
//        }

//        private importSubsidiaryRequestSubsidiary PrepareSubsidiaryRequest(RisSubsidiary subsidiary)
//        {
//            var transportGuid = Guid.NewGuid().ToString();

//            this.subsidiariesByTransportGuid.Add(transportGuid, subsidiary);

//            return new importSubsidiaryRequestSubsidiary
//                   {
//                       FullName = subsidiary.FullName,
//                       ShortName = subsidiary.ShortName,
//                       OGRN = subsidiary.Ogrn,
//                       INN = subsidiary.Inn,
//                       KPP = subsidiary.Kpp,
//                       OKOPF = subsidiary.Okopf,
//                       Address = subsidiary.Address,
//                       FIASHouseGuid = subsidiary.FiasHouseGuid,
//                       ActivityEndDate = subsidiary.ActivityEndDate.GetValueOrDefault(),
//                       SourceName = new SubsidiaryTypeSourceName
//                                    {
//                                        Date = subsidiary.SourceDate,
//                                        Value = subsidiary.ShortName
//                                    },
//                       TransportGUID = transportGuid
//                   };
//        }

//        /// <summary>
//        /// Метод получения списка записей для импорта
//        /// </summary>
//        /// <returns>Список объектов для импорта</returns>
//        private List<RisSubsidiary> GetMainList()
//        {
//            var subsidiariesDomain = this.Container.ResolveDomain<RisSubsidiary>();

//            try
//            {
//                return subsidiariesDomain.GetAll().ToList();
//            }
//            finally
//            {
//                this.Container.Release(subsidiariesDomain);
//            }
//        }
//    }
//}
