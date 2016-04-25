//namespace Bars.Gkh.Ris.Integration.Bills.Methods
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Text;
//    using B4.DataAccess;
//    using Domain;
//    using Ris.Bills;
//    using Entities.Bills;
//    using Enums.Bills;
//    using B4.Utils;
//    /// <summary>
//    /// Класс - экспортер данных по платежным документам
//    /// </summary>
//    public class ImportPaymentDocumentMethod : GisIntegrationBillsMethod<RisPaymentDocument, importPaymentDocumentRequest>
//    {
//        private readonly List<RisPaymentDocument> paymentDocumentToSave;

//        private readonly Dictionary<string, RisPaymentDocument> paymentDocumentByTransportGuid;

//        private readonly Dictionary<RisPaymentDocument, List<RisHousingServiceChargeInfo>>
//            housingServiceChargesByPaymentDocument;

//        private readonly Dictionary<RisPaymentDocument, List<RisAdditionalServiceChargeInfo>>
//            additionalServiceChargesByPaymentDocument;

//        private readonly Dictionary<RisPaymentDocument, List<RisAdditionalServiceExtChargeInfo>>
//            additionalServiceExtChargesByPaymentDocument;

//        private readonly Dictionary<RisPaymentDocument, List<RisMunicipalServiceChargeInfo>>
//            municipalServiceChargesByPaymentDocument;

//        private readonly Dictionary<RisAdditionalServiceExtChargeInfo, List<RisTechService>>
//            techServicesByAdditionalServiceExtCharge;

//        public ImportPaymentDocumentMethod()
//        {
//            this.paymentDocumentToSave = new List<RisPaymentDocument>();
//            this.paymentDocumentByTransportGuid = new Dictionary<string, RisPaymentDocument>();
//            this.housingServiceChargesByPaymentDocument = new Dictionary<RisPaymentDocument, List<RisHousingServiceChargeInfo>>();
//            this.additionalServiceChargesByPaymentDocument = new Dictionary<RisPaymentDocument, List<RisAdditionalServiceChargeInfo>>();
//            this.additionalServiceExtChargesByPaymentDocument = new Dictionary<RisPaymentDocument, List<RisAdditionalServiceExtChargeInfo>>();
//            this.municipalServiceChargesByPaymentDocument = new Dictionary<RisPaymentDocument, List<RisMunicipalServiceChargeInfo>>();
//            this.techServicesByAdditionalServiceExtCharge = new Dictionary<RisAdditionalServiceExtChargeInfo, List<RisTechService>>();
//        }

//        /// <summary>
//        /// Код метода
//        /// </summary>
//        public override string Code
//        {
//            get
//            {
//                return "importPaymentDocument";
//            }
//        }

//        /// <summary>
//        /// Наименование метода
//        /// </summary>
//        public override string Name
//        {
//            get
//            {
//                return "Импорт сведений о платежных документах";
//            }
//        }

//        /// <summary>
//        /// Порядок импорта в списке
//        /// </summary>
//        public override int Order
//        {
//            get
//            {
//                return 54;
//            }
//        }

//        /// <summary>
//        /// Размер блока предаваемых данных (максимальное количество записей)
//        /// </summary>
//        protected override int Portion
//        {
//            get
//            {
//                return 1000;
//            }
//        }

//        /// <summary>
//        /// Количество объектов для сохранения
//        /// </summary>
//        protected override int ProcessedObjects
//        {
//            get
//            {
//                return this.paymentDocumentToSave.Count;
//            }
//        }

//        /// <summary>
//        /// Общее количество импортируемых объектов
//        /// </summary>
//        new protected int CountObjects
//        {
//            get
//            {
//                return this.paymentDocumentByTransportGuid.Count;
//            }
//        }

//        /// <summary>
//        /// Список платежных документов, по которым производится импорт.
//        /// </summary>
//        protected override IList<RisPaymentDocument> MainList { get; set; }

//        /// <summary>
//        /// Подготовка кэша данных 
//        /// </summary>
//        protected override void Prepare()
//        {
//            this.MainList = this.GetMainList();

//            foreach (var paymentDocument in this.MainList)
//            {
//                this.housingServiceChargesByPaymentDocument.Add(paymentDocument, this.GetHousingServiceCharges(paymentDocument));

//                this.additionalServiceChargesByPaymentDocument.Add(paymentDocument, this.GetAdditionalServiceCharges(paymentDocument));

//                var additionalServiceExtCharges = this.GetAdditionalServiceExtCharges(paymentDocument);

//                this.additionalServiceExtChargesByPaymentDocument.Add(paymentDocument, additionalServiceExtCharges);

//                foreach (var additionalServiceExtCharge in additionalServiceExtCharges)
//                {
//                    this.techServicesByAdditionalServiceExtCharge.Add(additionalServiceExtCharge, this.GetTechServices(additionalServiceExtCharge));
//                }

//                this.municipalServiceChargesByPaymentDocument.Add(paymentDocument, this.GetMunicipalServiceCharges(paymentDocument));
//            }
//        }

//        /// <summary>
//        /// Проверка платежных документов
//        /// </summary>
//        /// <param name="item">Платежный документ</param>
//        /// <returns>Результат проверки</returns>
//        protected override CheckingResult CheckMainListItem(RisPaymentDocument item)
//        {
//            StringBuilder messages = new StringBuilder();

//            if (item.Account == null || string.IsNullOrEmpty(item.Account.Guid))
//            {
//                messages.Append("Account.Guid ");
//            }

//            if (item.PaymentInformation == null)
//            {
//                messages.Append("PaymentInformation ");
//            }
//            else
//            {
//                if (string.IsNullOrEmpty(item.PaymentInformation.Recipient))
//                {
//                    messages.Append("PaymentInformation.Recipient ");
//                }

//                if (string.IsNullOrEmpty(item.PaymentInformation.BankBik))
//                {
//                    messages.Append("PaymentInformation.BankBik ");
//                }

//                if (string.IsNullOrEmpty(item.PaymentInformation.OperatingAccountNumber))
//                {
//                    messages.Append("PaymentInformation.OperatingAccountNumber ");
//                }

//                if (string.IsNullOrEmpty(item.PaymentInformation.CorrespondentBankAccount))
//                {
//                    messages.Append("PaymentInformation.CorrespondentBankAccount ");
//                }
//            }

//            if (item.AddressInfo == null)
//            {
//                messages.Append("AddressInfo ");
//            }
//            else
//            {
//                if (item.AddressInfo.TotalSquare <= 0m)
//                {
//                    messages.Append("AddressInfo.TotalSquare ");
//                }
//            }

//            if (item.TotalPiecemealPaymentSum < 0m)
//            {
//                messages.Append("TotalPiecemealPaymentSum ");
//            }

//            foreach (var housingServiceCharge in this.housingServiceChargesByPaymentDocument[item])
//            {
//                var houseServiceChargeCheckingResult = this.CheckHousingServiceCharge(housingServiceCharge);
//                messages.Append(houseServiceChargeCheckingResult.Messages);
//            }

//            foreach (var additionalServiceCharge in this.additionalServiceChargesByPaymentDocument[item])
//            {
//                var additionalServiceChargeCheckingResult = this.CheckAdditionalServiceCharge(additionalServiceCharge);
//                messages.Append(additionalServiceChargeCheckingResult.Messages);
//            }

//            foreach (var municipalServiceCharge in this.municipalServiceChargesByPaymentDocument[item])
//            {
//                var municipalServiceChargeCheckingResult = this.CheckMunicipalServiceCharge(municipalServiceCharge);
//                messages.Append(municipalServiceChargeCheckingResult.Messages);
//            }

//            foreach (var additionalServiceExtCharge in this.additionalServiceExtChargesByPaymentDocument[item])
//            {
//                var additionalServiceExtChargeCheckingResult = this.CheckAdditionalServiceExtCharge(additionalServiceExtCharge);
//                messages.Append(additionalServiceExtChargeCheckingResult.Messages);
//            }

//            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
//        }

//        /// <summary>
//        /// Получить объект запроса.
//        /// </summary>
//        /// <param name="listForImport">Список объектов для импорта</param>
//        /// <returns>Объект запроса</returns>
//        protected override importPaymentDocumentRequest GetRequestObject(
//            IEnumerable<RisPaymentDocument> listForImport)
//        {
//            var importRequestList = new List<importPaymentDocumentRequestPaymentDocument>();

//            foreach (var item in listForImport)
//            {
//                importRequestList.Add(this.PreparePaymentDocumentRequest(item));
//            }

//            var firstItem = listForImport.First();

//            return new importPaymentDocumentRequest { Month = firstItem.PeriodMonth, Year = firstItem.PeriodYear, PaymentDocument = importRequestList.ToArray() };
//        }

//        /// <summary>
//        /// Получить список блоков объектов ГИС для формирования объектов для запроса.
//        /// </summary>
//        /// <returns>Список блоков объектов ГИС</returns>
//        protected override List<IEnumerable<RisPaymentDocument>> GetPortions()
//        {
//            List<IEnumerable<RisPaymentDocument>> result = new List<IEnumerable<RisPaymentDocument>>();
//            Dictionary<string, List<RisPaymentDocument>> paymentDocumentsByMonthYear = this.MainList.GroupBy(x => string.Format("{0}/{1}", x.PeriodMonth, x.PeriodYear)).ToDictionary(g => g.Key, g => g.ToList());

//            foreach (var paymentDocuments in paymentDocumentsByMonthYear.Values)
//            {
//                var startIndex = 0;
//                do
//                {
//                    result.Add(paymentDocuments.Skip(startIndex).Take(this.Portion));
//                    startIndex += this.Portion;
//                }
//                while (startIndex < paymentDocuments.Count);
//            }

//            return result;
//        }

//        /// <summary>
//        /// Получить ответ от сервиса.
//        /// </summary>
//        /// <param name="request">Объект запроса</param>
//        /// <returns>Ответ от сервиса</returns>
//        protected override ImportResult GetRequestResult(importPaymentDocumentRequest request)
//        {
//            ImportResult result = null;
//            var soapClient = this.ServiceProvider.GetSoapClient();

//            if (soapClient != null)
//            {
//                soapClient.importPaymentDocumentData(this.RequestHeader, request, out result);
//            }

//            return result;
//        }

//        /// <summary>
//        /// Сохранение объектов ГИС после импорта.
//        /// </summary>
//        protected override void SaveObjects()
//        {
//            TransactionHelper.InsertInManyTransactions(this.Container, this.paymentDocumentToSave, 1000, true, true);
//        }

//        /// <summary>
//        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
//        /// </summary>
//        /// <param name="responseItem">Элемент response</param>
//        protected override void CheckResponseItem(CommonResultType responseItem)
//        {
//            if (!this.paymentDocumentByTransportGuid.ContainsKey(responseItem.TransportGUID))
//            {
//                return;
//            }

//            var paymentDocument = this.paymentDocumentByTransportGuid[responseItem.TransportGUID];

//            if (responseItem.GUID.IsEmpty())
//            {
//                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

//                var errorNotation = string.Empty;

//                if (error != null)
//                {
//                    errorNotation = error.Description;
//                }

//                this.AddLineToLog("Платежный документ", paymentDocument.Id, "Не загружен", errorNotation);

//                return;
//            }

//            paymentDocument.Guid = responseItem.GUID;
//            this.paymentDocumentToSave.Add(paymentDocument);

//            this.AddLineToLog("Платежный документ", paymentDocument.Id, "Загружен", responseItem.GUID);
//        }

//        private importPaymentDocumentRequestPaymentDocument PreparePaymentDocumentRequest(RisPaymentDocument paymentDocument)
//        {
//            var paymentInformation = paymentDocument.PaymentInformation;
//            var addressInfo = paymentDocument.AddressInfo;
//            var transportGuid = Guid.NewGuid().ToString();

//            var chargeInfo = new List<PaymentDocumentTypeChargeInfo>();

//            foreach (var housingServiceCharge in this.housingServiceChargesByPaymentDocument[paymentDocument])
//            {
//                chargeInfo.Add(new PaymentDocumentTypeChargeInfo
//                {
//                    Item = this.PrepareHousingServiceChargeRequest(housingServiceCharge)
//                });
//            }

//            foreach (var additionalServiceCharge in this.additionalServiceChargesByPaymentDocument[paymentDocument])
//            {
//                chargeInfo.Add(new PaymentDocumentTypeChargeInfo
//                {
//                    Item = this.PrepareAdditionalServiceChargeRequest(additionalServiceCharge)
//                });
//            }

//            foreach (var municipalServiceCharge in this.municipalServiceChargesByPaymentDocument[paymentDocument])
//            {
//                chargeInfo.Add(new PaymentDocumentTypeChargeInfo
//                {
//                    Item = this.PrepareMunicipalServiceChargeRequest(municipalServiceCharge)
//                });
//            }

//            foreach (var additionalServiceExtCharge in this.additionalServiceExtChargesByPaymentDocument[paymentDocument])
//            {
//                chargeInfo.Add(new PaymentDocumentTypeChargeInfo
//                {
//                    Item = this.PrepareAdditionalServiceExtChargeRequest(additionalServiceExtCharge)
//                });
//            }

//            this.paymentDocumentByTransportGuid.Add(transportGuid, paymentDocument);

//            return new importPaymentDocumentRequestPaymentDocument
//            {
//                AccountGuid = paymentDocument.Account.Guid,
//                PaymentInformation = new PaymentInformationType
//                {
//                    Recipient = paymentInformation.Recipient,
//                    BankBIK = paymentInformation.BankBik,
//                    RecipientKPP = paymentInformation.RecipientKpp,
//                    operatingAccountNumber = paymentInformation.OperatingAccountNumber,
//                    CorrespondentBankAccount = paymentInformation.CorrespondentBankAccount
//                },
//                AddressInfo = new PaymentDocumentTypeAddressInfo
//                {
//                    LivingPersonsNumber = (sbyte)addressInfo.LivingPersonsNumber.GetValueOrDefault(),
//                    ResidentialSquare = decimal.Round(addressInfo.ResidentialSquare.GetValueOrDefault(), 2),
//                    HeatedArea = decimal.Round(addressInfo.HeatedArea.GetValueOrDefault(), 2),
//                    TotalSquare = decimal.Round(addressInfo.TotalSquare, 2)

//                },
//                ChargeInfo = chargeInfo.ToArray(),
//                Item = paymentDocument.State == PaymentDocumentState.Expose,
//                totalPiecemealPaymentSum = decimal.Round(paymentDocument.TotalPiecemealPaymentSum, 2),
//                TransportGUID = transportGuid
//            };
//        }

//        private PDServiceChargeTypeHousingService PrepareHousingServiceChargeRequest(
//            RisHousingServiceChargeInfo housingServiceCharge)
//        {

//            ServiceInformation serviceInformation = null;

//            if (housingServiceCharge.IndividualConsumptionCurrentValue.GetValueOrDefault() != 0m
//                || housingServiceCharge.HouseOverallNeedsCurrentValue.GetValueOrDefault() != 0m
//                || housingServiceCharge.HouseTotalIndividualConsumption.GetValueOrDefault() != 0m
//                || housingServiceCharge.HouseTotalHouseOverallNeeds.GetValueOrDefault() != 0m
//                || housingServiceCharge.HouseOverallNeedsNorm.GetValueOrDefault() != 0m
//                || housingServiceCharge.IndividualConsumptionNorm.GetValueOrDefault() != 0m)
//            {
//                serviceInformation = new ServiceInformation
//                {
//                    individualConsumptionCurrentValue = decimal.Round(housingServiceCharge.IndividualConsumptionCurrentValue.GetValueOrDefault(), 2),
//                    houseOverallNeedsCurrentValue = decimal.Round(housingServiceCharge.HouseOverallNeedsCurrentValue.GetValueOrDefault(), 2),
//                    houseTotalIndividualConsumption = decimal.Round(housingServiceCharge.HouseTotalIndividualConsumption.GetValueOrDefault(), 2),
//                    houseTotalHouseOverallNeeds = decimal.Round(housingServiceCharge.HouseTotalHouseOverallNeeds.GetValueOrDefault(), 2),
//                    houseOverallNeedsNorm = decimal.Round(housingServiceCharge.HouseOverallNeedsNorm.GetValueOrDefault(), 2),
//                    individualConsumptionNorm = decimal.Round(housingServiceCharge.IndividualConsumptionNorm.GetValueOrDefault(), 2)
//                };
//            }

//            List<ServicePDTypeVolume> consumption = new List<ServicePDTypeVolume>();

//            if (housingServiceCharge.IndividualConsumption.GetValueOrDefault() != 0m)
//            {
//                consumption.Add(new ServicePDTypeVolume
//                {
//                    type = ServicePDTypeVolumeType.I,
//                    Value = housingServiceCharge.IndividualConsumption.GetValueOrDefault()
//                });
//            }

//            if (housingServiceCharge.HouseOverallNeedsConsumption.GetValueOrDefault() != 0m)
//            {
//                consumption.Add(new ServicePDTypeVolume
//                {
//                    type = ServicePDTypeVolumeType.O,
//                    Value = housingServiceCharge.HouseOverallNeedsConsumption.GetValueOrDefault()
//                });
//            }

//            return new PDServiceChargeTypeHousingService
//            {
//                ServiceType = new nsiRef
//                {
//                    Code = housingServiceCharge.ServiceTypeCode,
//                    GUID = housingServiceCharge.ServiceTypeGuid
//                },
//                OKEI = housingServiceCharge.Okei,
//                Rate = decimal.Round(housingServiceCharge.Rate, 2),
//                ServiceInformation = serviceInformation,
//                Consumption = consumption.Count > 0 ? consumption.ToArray() : null,
//                ServiceCharge = new ServiceChargeType
//                {
//                    MoneyRecalculation = decimal.Round(housingServiceCharge.MoneyRecalculation.GetValueOrDefault(), 2),
//                    MoneyDiscount = decimal.Round(housingServiceCharge.MoneyDiscount.GetValueOrDefault(), 2)
//                }
//            };
//        }

//        private PDServiceChargeTypeAdditionalService PrepareAdditionalServiceChargeRequest(
//            RisAdditionalServiceChargeInfo additionalServiceCharge)
//        {
//            ServiceInformation serviceInformation = null;

//            if (additionalServiceCharge.IndividualConsumptionCurrentValue.GetValueOrDefault() != 0m
//                || additionalServiceCharge.HouseOverallNeedsCurrentValue.GetValueOrDefault() != 0m
//                || additionalServiceCharge.HouseTotalIndividualConsumption.GetValueOrDefault() != 0m
//                || additionalServiceCharge.HouseTotalHouseOverallNeeds.GetValueOrDefault() != 0m
//                || additionalServiceCharge.HouseOverallNeedsNorm.GetValueOrDefault() != 0m
//                || additionalServiceCharge.IndividualConsumptionNorm.GetValueOrDefault() != 0m)
//            {
//                serviceInformation = new ServiceInformation
//                {
//                    individualConsumptionCurrentValue = decimal.Round(additionalServiceCharge.IndividualConsumptionCurrentValue.GetValueOrDefault(), 2),
//                    houseOverallNeedsCurrentValue = decimal.Round(additionalServiceCharge.HouseOverallNeedsCurrentValue.GetValueOrDefault(), 2),
//                    houseTotalIndividualConsumption = decimal.Round(additionalServiceCharge.HouseTotalIndividualConsumption.GetValueOrDefault(), 2),
//                    houseTotalHouseOverallNeeds = decimal.Round(additionalServiceCharge.HouseTotalHouseOverallNeeds.GetValueOrDefault(), 2),
//                    houseOverallNeedsNorm = decimal.Round(additionalServiceCharge.HouseOverallNeedsNorm.GetValueOrDefault(), 2),
//                    individualConsumptionNorm = decimal.Round(additionalServiceCharge.IndividualConsumptionNorm.GetValueOrDefault(), 2)
//                };
//            }

//            List<ServicePDTypeVolume> consumption = new List<ServicePDTypeVolume>();

//            if (additionalServiceCharge.IndividualConsumption.GetValueOrDefault() != 0m)
//            {
//                consumption.Add(new ServicePDTypeVolume
//                {
//                    type = ServicePDTypeVolumeType.I,
//                    Value = additionalServiceCharge.IndividualConsumption.GetValueOrDefault()
//                });
//            }

//            if (additionalServiceCharge.HouseOverallNeedsConsumption.GetValueOrDefault() != 0m)
//            {
//                consumption.Add(new ServicePDTypeVolume
//                {
//                    type = ServicePDTypeVolumeType.O,
//                    Value = additionalServiceCharge.HouseOverallNeedsConsumption.GetValueOrDefault()
//                });
//            }

//            return new PDServiceChargeTypeAdditionalService
//            {
//                ServiceType = new nsiRef
//                {
//                    Code = additionalServiceCharge.ServiceTypeCode,
//                    GUID = additionalServiceCharge.ServiceTypeGuid
//                },
//                OKEI = additionalServiceCharge.Okei,
//                Rate = decimal.Round(additionalServiceCharge.Rate, 2),
//                ServiceInformation = serviceInformation,
//                Consumption = consumption.Count > 0 ? consumption.ToArray() : null,
//                ServiceCharge = new ServiceChargeType
//                {
//                    MoneyRecalculation = decimal.Round(additionalServiceCharge.MoneyRecalculation.GetValueOrDefault(), 2),
//                    MoneyDiscount = decimal.Round(additionalServiceCharge.MoneyDiscount.GetValueOrDefault(), 2)
//                }
//            };
//        }

//        private PDServiceChargeTypeMunicipalService PrepareMunicipalServiceChargeRequest(
//            RisMunicipalServiceChargeInfo municipalServiceCharge)
//        {
//            ServiceInformation serviceInformation = null;

//            if (municipalServiceCharge.IndividualConsumptionCurrentValue.GetValueOrDefault() != 0m
//                || municipalServiceCharge.HouseOverallNeedsCurrentValue.GetValueOrDefault() != 0m
//                || municipalServiceCharge.HouseTotalIndividualConsumption.GetValueOrDefault() != 0m
//                || municipalServiceCharge.HouseTotalHouseOverallNeeds.GetValueOrDefault() != 0m
//                || municipalServiceCharge.HouseOverallNeedsNorm.GetValueOrDefault() != 0m
//                || municipalServiceCharge.IndividualConsumptionNorm.GetValueOrDefault() != 0m)
//            {
//                serviceInformation = new ServiceInformation
//                {
//                    individualConsumptionCurrentValue = decimal.Round(municipalServiceCharge.IndividualConsumptionCurrentValue.GetValueOrDefault(), 2),
//                    houseOverallNeedsCurrentValue = decimal.Round(municipalServiceCharge.HouseOverallNeedsCurrentValue.GetValueOrDefault(), 2),
//                    houseTotalIndividualConsumption = decimal.Round(municipalServiceCharge.HouseTotalIndividualConsumption.GetValueOrDefault(), 2),
//                    houseTotalHouseOverallNeeds = decimal.Round(municipalServiceCharge.HouseTotalHouseOverallNeeds.GetValueOrDefault(), 2),
//                    houseOverallNeedsNorm = decimal.Round(municipalServiceCharge.HouseOverallNeedsNorm.GetValueOrDefault(), 2),
//                    individualConsumptionNorm = decimal.Round(municipalServiceCharge.IndividualConsumptionNorm.GetValueOrDefault(), 2)
//                };
//            }

//            List<ServicePDTypeVolume> consumption = new List<ServicePDTypeVolume>();

//            if (municipalServiceCharge.IndividualConsumption.GetValueOrDefault() != 0m)
//            {
//                consumption.Add(new ServicePDTypeVolume
//                {
//                    type = ServicePDTypeVolumeType.I,
//                    Value = municipalServiceCharge.IndividualConsumption.GetValueOrDefault()
//                });
//            }

//            if (municipalServiceCharge.HouseOverallNeedsConsumption.GetValueOrDefault() != 0m)
//            {
//                consumption.Add(new ServicePDTypeVolume
//                {
//                    type = ServicePDTypeVolumeType.O,
//                    Value = municipalServiceCharge.HouseOverallNeedsConsumption.GetValueOrDefault()
//                });
//            }

//            return new PDServiceChargeTypeMunicipalService
//            {
//                ServiceType = new nsiRef
//                {
//                    Code = municipalServiceCharge.ServiceTypeCode,
//                    GUID = municipalServiceCharge.ServiceTypeGuid
//                },
//                OKEI = municipalServiceCharge.Okei,
//                Rate = decimal.Round(municipalServiceCharge.Rate, 2),
//                ServiceInformation = serviceInformation,
//                Consumption = consumption.Count > 0 ? consumption.ToArray() : null,
//                ServiceCharge = new ServiceChargeType
//                {
//                    MoneyRecalculation = decimal.Round(municipalServiceCharge.MoneyRecalculation.GetValueOrDefault(), 2),
//                    MoneyDiscount = decimal.Round(municipalServiceCharge.MoneyDiscount.GetValueOrDefault(), 2)
//                },
//                PiecemealPayment = new PiecemealPayment
//                {
//                    paymentPeriodPiecemealPaymentSum = decimal.Round(municipalServiceCharge.PaymentPeriodPiecemealPaymentSum.GetValueOrDefault(), 2),
//                    pastPaymentPeriodPiecemealPaymentSum = decimal.Round(municipalServiceCharge.PastPaymentPeriodPiecemealPaymentSum.GetValueOrDefault(), 2),
//                    piecemealPaymentPercentRub = decimal.Round(municipalServiceCharge.PiecemealPaymentPercentRub, 2),
//                    piecemealPaymentPercent = decimal.Round(municipalServiceCharge.PiecemealPaymentPercent, 2),
//                    piecemealPaymentSum = decimal.Round(municipalServiceCharge.PiecemealPaymentSum, 2)
//                },
//                PaymentRecalculation = new PDServiceChargeTypeMunicipalServicePaymentRecalculation
//                {
//                    recalculationReason = municipalServiceCharge.PaymentRecalculationReason,
//                    sum = decimal.Round(municipalServiceCharge.PaymentRecalculationSum, 2)
//                }
//            };
//        }

//        private PDServiceChargeTypeAdditionalServiceExt PrepareAdditionalServiceExtChargeRequest(
//            RisAdditionalServiceExtChargeInfo additionalServiceExtCharge)
//        {
//            var techServiceRequests = new List<ServicePDType>();

//            foreach (var techService in this.techServicesByAdditionalServiceExtCharge[additionalServiceExtCharge])
//            {
//                techServiceRequests.Add(this.PrepareTechServicesRequest(techService));
//            }

//            return new PDServiceChargeTypeAdditionalServiceExt
//            {
//                MoneyRecalculation = decimal.Round(additionalServiceExtCharge.MoneyRecalculation.GetValueOrDefault(), 2),
//                MoneyDiscount = decimal.Round(additionalServiceExtCharge.MoneyDiscount.GetValueOrDefault(), 2),
//                ServiceType = new nsiRef
//                {
//                    Code = additionalServiceExtCharge.ServiceTypeCode,
//                    GUID = additionalServiceExtCharge.ServiceTypeGuid
//                },
//                TechService = techServiceRequests.ToArray()
//            };
//        }

//        private ServicePDType PrepareTechServicesRequest(RisTechService techService)
//        {
//            ServiceInformation serviceInformation = null;

//            if (techService.IndividualConsumptionCurrentValue.GetValueOrDefault() != 0m
//                || techService.HouseOverallNeedsCurrentValue.GetValueOrDefault() != 0m
//                || techService.HouseTotalIndividualConsumption.GetValueOrDefault() != 0m
//                || techService.HouseTotalHouseOverallNeeds.GetValueOrDefault() != 0m
//                || techService.HouseOverallNeedsNorm.GetValueOrDefault() != 0m
//                || techService.IndividualConsumptionNorm.GetValueOrDefault() != 0m)
//            {
//                serviceInformation = new ServiceInformation
//                {
//                    individualConsumptionCurrentValue = decimal.Round(techService.IndividualConsumptionCurrentValue.GetValueOrDefault(), 2),
//                    houseOverallNeedsCurrentValue = decimal.Round(techService.HouseOverallNeedsCurrentValue.GetValueOrDefault(), 2),
//                    houseTotalIndividualConsumption = decimal.Round(techService.HouseTotalIndividualConsumption.GetValueOrDefault(), 2),
//                    houseTotalHouseOverallNeeds = decimal.Round(techService.HouseTotalHouseOverallNeeds.GetValueOrDefault(), 2),
//                    houseOverallNeedsNorm = decimal.Round(techService.HouseOverallNeedsNorm.GetValueOrDefault(), 2),
//                    individualConsumptionNorm = decimal.Round(techService.IndividualConsumptionNorm.GetValueOrDefault(), 2)
//                };
//            }

//            List<ServicePDTypeVolume> consumption = new List<ServicePDTypeVolume>();

//            if (techService.IndividualConsumption.GetValueOrDefault() != 0m)
//            {
//                consumption.Add(new ServicePDTypeVolume
//                {
//                    type = ServicePDTypeVolumeType.I,
//                    Value = techService.IndividualConsumption.GetValueOrDefault()
//                });
//            }

//            if (techService.HouseOverallNeedsConsumption.GetValueOrDefault() != 0m)
//            {
//                consumption.Add(new ServicePDTypeVolume
//                {
//                    type = ServicePDTypeVolumeType.O,
//                    Value = techService.HouseOverallNeedsConsumption.GetValueOrDefault()
//                });
//            }

//            return new ServicePDType
//            {
//                ServiceType = new nsiRef
//                {
//                    Code = techService.ServiceTypeCode,
//                    GUID = techService.ServiceTypeGuid
//                },
//                OKEI = techService.Okei,
//                Rate = decimal.Round(techService.Rate, 2),
//                ServiceInformation = serviceInformation,
//                Consumption = consumption.ToArray()
//            };
//        }


//        private List<RisHousingServiceChargeInfo> GetHousingServiceCharges(RisPaymentDocument paymentDocument)
//        {
//            var housingServiceChargeDomain = this.Container.ResolveDomain<RisHousingServiceChargeInfo>();

//            try
//            {
//                return housingServiceChargeDomain.GetAll().Where(x => x.PaymentDocument == paymentDocument).ToList();
//            }
//            finally
//            {
//                this.Container.Release(housingServiceChargeDomain);
//            }
//        }

//        private List<RisAdditionalServiceChargeInfo> GetAdditionalServiceCharges(RisPaymentDocument paymentDocument)
//        {
//            var additionalServiceChargeDomain = this.Container.ResolveDomain<RisAdditionalServiceChargeInfo>();

//            try
//            {
//                return additionalServiceChargeDomain.GetAll().Where(x => x.PaymentDocument == paymentDocument).ToList();
//            }
//            finally
//            {
//                this.Container.Release(additionalServiceChargeDomain);
//            }
//        }

//        private List<RisAdditionalServiceExtChargeInfo> GetAdditionalServiceExtCharges(RisPaymentDocument paymentDocument)
//        {
//            var additionalServiceExtChargeDomain = this.Container.ResolveDomain<RisAdditionalServiceExtChargeInfo>();

//            try
//            {
//                return additionalServiceExtChargeDomain.GetAll().Where(x => x.PaymentDocument == paymentDocument).ToList();
//            }
//            finally
//            {
//                this.Container.Release(additionalServiceExtChargeDomain);
//            }
//        }

//        private List<RisMunicipalServiceChargeInfo> GetMunicipalServiceCharges(RisPaymentDocument paymentDocument)
//        {
//            var municipalServiceChargeDomain = this.Container.ResolveDomain<RisMunicipalServiceChargeInfo>();

//            try
//            {
//                return municipalServiceChargeDomain.GetAll().Where(x => x.PaymentDocument == paymentDocument).ToList();
//            }
//            finally
//            {
//                this.Container.Release(municipalServiceChargeDomain);
//            }
//        }

//        private List<RisTechService> GetTechServices(RisAdditionalServiceExtChargeInfo additionalServiceExtChargeInfo)
//        {
//            var techServiceDomain = this.Container.ResolveDomain<RisTechService>();

//            try
//            {
//                return techServiceDomain.GetAll().Where(x => x.AdditionalServiceExtChargeInfo == additionalServiceExtChargeInfo).ToList();
//            }
//            finally
//            {
//                this.Container.Release(techServiceDomain);
//            }
//        }

//        /// <summary>
//        /// Метод получения списка документов для импорта
//        /// </summary>
//        /// <returns>Список объектов для импорта</returns>
//        private List<RisPaymentDocument> GetMainList()
//        {
//            var documentsDomain = this.Container.ResolveDomain<RisPaymentDocument>();

//            //Для реализации выборки платежных документов необходимо знать:
//            // контрагента (поставщика информации), за какой период выгружать
//            //соответственно, пока выгружаются все документы последовательно за все периоды


//            //TODO Сделать выборку по контрагенту, периоду

//            try
//            {
//                return documentsDomain.GetAll().ToList();
//            }
//            finally
//            {
//                this.Container.Release(documentsDomain);
//            }
//        }

//        private CheckingResult CheckHousingServiceCharge(RisHousingServiceChargeInfo housingServiceCharge)
//        {
//            StringBuilder messages = new StringBuilder();

//            var invalidProperties = new List<string>();

//            if (string.IsNullOrEmpty(housingServiceCharge.ServiceTypeCode)
//                || string.IsNullOrEmpty(housingServiceCharge.ServiceTypeGuid))
//            {
//                invalidProperties.Add("ServiceType");
//            }

//            if (housingServiceCharge.Rate < 0m)
//            {
//                invalidProperties.Add("Rate");
//            }

//            foreach (var invalidProperty in invalidProperties)
//            {
//                messages.Append(string.Format(
//                    "Начисление по услуге HousingService с id = {0}, свойство {1} ",
//                     housingServiceCharge.Id,
//                    invalidProperty));
//            }

//            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
//        }

//        private CheckingResult CheckAdditionalServiceCharge(RisAdditionalServiceChargeInfo additionalServiceCharge)
//        {
//            StringBuilder messages = new StringBuilder();

//            var invalidProperties = new List<string>();

//            if (string.IsNullOrEmpty(additionalServiceCharge.ServiceTypeCode)
//                || string.IsNullOrEmpty(additionalServiceCharge.ServiceTypeGuid))
//            {
//                invalidProperties.Add("ServiceType");
//            }

//            if (additionalServiceCharge.Rate < 0m)
//            {
//                invalidProperties.Add("Rate");
//            }

//            foreach (var invalidProperty in invalidProperties)
//            {
//                messages.Append(string.Format(
//                    "Начисление по услуге AdditionalService с id = {0}, свойство {1} ",
//                     additionalServiceCharge.Id,
//                    invalidProperty));
//            }

//            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
//        }

//        private CheckingResult CheckMunicipalServiceCharge(RisMunicipalServiceChargeInfo municipalServiceCharge)
//        {
//            StringBuilder messages = new StringBuilder();

//            var invalidProperties = new List<string>();

//            if (string.IsNullOrEmpty(municipalServiceCharge.ServiceTypeCode)
//                || string.IsNullOrEmpty(municipalServiceCharge.ServiceTypeGuid))
//            {
//                invalidProperties.Add("ServiceType");
//            }

//            if (municipalServiceCharge.Rate < 0m)
//            {
//                invalidProperties.Add("Rate");
//            }

//            if (municipalServiceCharge.PiecemealPaymentPercentRub < 0m)
//            {
//                invalidProperties.Add("PiecemealPaymentPercentRub");
//            }

//            if (municipalServiceCharge.PiecemealPaymentPercent < 0m)
//            {
//                invalidProperties.Add("PiecemealPaymentPercent");
//            }

//            if (municipalServiceCharge.PiecemealPaymentSum < 0m)
//            {
//                invalidProperties.Add("PiecemealPaymentSum");
//            }

//            if (string.IsNullOrEmpty(municipalServiceCharge.PaymentRecalculationReason))
//            {
//                invalidProperties.Add("PaymentRecalculationReason");
//            }

//            if (municipalServiceCharge.PaymentRecalculationSum < 0m)
//            {
//                invalidProperties.Add("PaymentRecalculationReason");
//            }

//            foreach (var invalidProperty in invalidProperties)
//            {
//                messages.Append(string.Format(
//                    "Начисление по услуге MunicipalService с id = {0}, свойство {1} ",
//                     municipalServiceCharge.Id,
//                    invalidProperty));
//            }

//            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
//        }

//        private CheckingResult CheckAdditionalServiceExtCharge(
//            RisAdditionalServiceExtChargeInfo additionalServiceExtCharge)
//        {
//            StringBuilder messages = new StringBuilder();

//            if (string.IsNullOrEmpty(additionalServiceExtCharge.ServiceTypeCode)
//               || string.IsNullOrEmpty(additionalServiceExtCharge.ServiceTypeGuid))
//            {
//                messages.Append(string.Format(
//                    "Начисление по услуге AdditionalServiceExt с id = {0}, свойство {1} ",
//                     additionalServiceExtCharge.Id,
//                    "ServiceType"));
//            }

//            foreach (var techService in this.techServicesByAdditionalServiceExtCharge[additionalServiceExtCharge])
//            {
//                var techServiceCheckingResult = this.CheckTechService(techService);
//                messages.Append(techServiceCheckingResult.Messages);
//            }

//            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
//        }

//        private CheckingResult CheckTechService(RisTechService techService)
//        {
//            StringBuilder messages = new StringBuilder();

//            var invalidProperties = new List<string>();

//            if (string.IsNullOrEmpty(techService.ServiceTypeCode)
//            || string.IsNullOrEmpty(techService.ServiceTypeGuid))
//            {
//                invalidProperties.Add("ServiceType");
//            }

//            if (techService.Rate < 0m)
//            {
//                invalidProperties.Add("Rate");
//            }

//            foreach (var invalidProperty in invalidProperties)
//            {
//                messages.Append(string.Format(
//                    "Начисление по услуге TechService с id = {0}, свойство {1} ",
//                     techService.Id,
//                    invalidProperty));
//            }

//            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
//        }
//    }
//}
