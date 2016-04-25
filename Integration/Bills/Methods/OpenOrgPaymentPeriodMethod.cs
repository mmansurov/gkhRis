//namespace Bars.Gkh.Ris.Integration.Bills.Methods
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;

//    using B4.DataAccess;
//    using B4.Utils;

//    using Domain;
//    using Entities.Bills;
//    using Enums;
//    using Ris.Bills;

//    public class OpenOrgPaymentPeriodMethod : GisIntegrationBillsMethod<OrgPaymentPeriod, openOrgPaymentPeriodRequest>
//    {
//        private readonly Dictionary<string, OrgPaymentPeriod> periodByTransportGuidDict = new Dictionary<string, OrgPaymentPeriod>();
//        private readonly List<OrgPaymentPeriod> periodsToSave = new List<OrgPaymentPeriod>();

//        protected override int ProcessedObjects { get { return this.periodsToSave.Count; } }

//        public override string Code
//        {
//            get { return "openOrgPaymentPeriod"; }
//        }

//        public override string Name
//        {
//            get { return "Открытие платежных периодов"; }
//        }

//        /// <summary>
//        /// Порядок импорта в списке
//        /// </summary>
//        public override int Order
//        {
//            get
//            {
//                return 52;
//            }
//        }

//        protected override int Portion { get { return 1; } }

//        protected override IList<OrgPaymentPeriod> MainList { get; set; }

//        protected override void Prepare()
//        {
//            var paymentPeriodDomain = this.Container.ResolveDomain<OrgPaymentPeriod>();

//            try
//            {
//                this.MainList = paymentPeriodDomain.GetAll().ToList();
//            }
//            finally
//            {
//                this.Container.Release(paymentPeriodDomain);
//            }
//        }

//        /// <summary>
//        /// Проверка платежного периода
//        /// </summary>
//        /// <param name="item">Прибор учета</param>
//        /// <returns>Результат проверки</returns>
//        protected override CheckingResult CheckMainListItem(OrgPaymentPeriod item)
//        {
//            var messages = new StringBuilder();

//            if (item.Month == 0)
//            {
//                messages.Append("month ");
//            }
//            if (item.Year == 0)
//            {
//                messages.Append("year ");
//            }

//            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
//        }

//        /// <summary>
//        /// Получить объект запроса.
//        /// </summary>
//        /// <param name="listForImport">Список объектов для импорта</param>
//        /// <returns>Объект запроса</returns>
//        protected override openOrgPaymentPeriodRequest GetRequestObject(
//            IEnumerable<OrgPaymentPeriod> listForImport)
//        {
//            var risPaymentPeriod = listForImport.First();

//            var paymentPeriod = new PaymentPeriodType
//            {
//                Month = risPaymentPeriod.Month,
//                Year = (short)risPaymentPeriod.Year,
//                Item = true,
//                ItemElementName = risPaymentPeriod.RisPaymentPeriodType == RisPaymentPeriodType.UO
//                    ? ItemChoiceType.isUO
//                    : ItemChoiceType.isRSO
//            };

//            var request = new openOrgPaymentPeriodRequest
//            {
//                TransportGUID = Guid.NewGuid().ToString(),
//                Item = paymentPeriod,
//                ItemElementName = ItemChoiceType1.createFirstPeriod
//            };

//            this.CountObjects++;

//            this.periodByTransportGuidDict.Add(request.TransportGUID, risPaymentPeriod);

//            return request;
//        }

//        protected override void SaveObjects()
//        {
//            TransactionHelper.InsertInManyTransactions(this.Container, this.periodsToSave, 1000, true, true);
//        }

//        protected override ImportResult GetRequestResult(openOrgPaymentPeriodRequest request)
//        {
//            ImportResult result = null;
//            var soapClient = this.ServiceProvider.GetSoapClient();

//            if (soapClient != null)
//            {
//                soapClient.openOrgPaymentPeriod(this.RequestHeader, request, out result);
//            }

//            return result;
//        }

//        /// <summary>
//        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
//        /// </summary>
//        /// <param name="responseItem">Элемент response</param>
//        protected override void CheckResponseItem(CommonResultType responseItem)
//        {
//            if (!this.periodByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
//            {
//                return;
//            }

//            var controlValue = this.periodByTransportGuidDict[responseItem.TransportGUID];

//            if (responseItem.GUID.IsEmpty())
//            {
//                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

//                var errorNotation = string.Empty;

//                if (error != null)
//                {
//                    errorNotation = error.Description;
//                }

//                this.AddLineToLog("Платежный период", controlValue.Id, "Не загружен", errorNotation);

//                return;
//            }

//            controlValue.Guid = responseItem.GUID;
//            this.periodsToSave.Add(controlValue);

//            this.AddLineToLog("Платежный период", controlValue.Id, "Загружен", responseItem.GUID);
//        }
//    }
//}