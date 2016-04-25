namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.HouseManagement;
    using Enums.HouseManagement;
    using Ris.HouseManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Класс метода импорта данных приборов учета
    /// </summary>
    public class ImportMeteringDeviceDataMethod :
        GisIntegrationHouseManagementMethod<RisMeteringDeviceData, importMeteringDeviceDataRequest>
    {
        private List<RisMeteringDeviceAccount> meteringDeviceAccount;

        private List<RisMeteringDeviceLivingRoom> meteringDeviceLivingRoom;

        private readonly Dictionary<string, RisMeteringDeviceData> meteringDeviceByTransportGuidDict = 
            new Dictionary<string, RisMeteringDeviceData>();

        private readonly List<RisMeteringDeviceData> meteringDevicesToSave = new List<RisMeteringDeviceData>();

        /// <summary>
        /// Код метода
        /// </summary>
        public override string Code
        {
            get
            {
                return "importMeteringDeviceData";
            }
        }

        /// <summary>
        /// Наименование метода
        /// </summary>
        public override string Name
        {
            get
            {
                return "Импорт данных приборов учета";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 47;
            }
        }

        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        protected override int Portion
        {
            get
            {
                return 100;
            }
        }

        /// <summary>
        /// Количество объектов для сохранения
        /// </summary>
        protected override int ProcessedObjects
        {
            get
            {
                return this.meteringDevicesToSave.Count;
            }
        }

        /// <summary>
        /// Список объектов ГИС, по которым производится импорт.
        /// </summary>
        protected override IList<RisMeteringDeviceData> MainList { get; set; }

        /// <summary>
        /// Подготовка кэша данных 
        /// </summary>
        protected override void Prepare()
        {
            var meteringDeviceDataDomain = this.Container.ResolveDomain<RisMeteringDeviceData>();
            var meteringDeviceAccountDomain = this.Container.ResolveDomain<RisMeteringDeviceAccount>();
            var meteringDeviceLivingRoomDomain = this.Container.ResolveDomain<RisMeteringDeviceLivingRoom>();

            try
            {
                this.MainList = meteringDeviceDataDomain.GetAll().ToList();
                this.meteringDeviceAccount = meteringDeviceAccountDomain.GetAll().ToList();
                this.meteringDeviceLivingRoom = meteringDeviceLivingRoomDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(meteringDeviceDataDomain);
                this.Container.Release(meteringDeviceAccountDomain);
                this.Container.Release(meteringDeviceLivingRoomDomain);
            }
        }

        /// <summary>
        /// Проверка прибора учета
        /// </summary>
        /// <param name="item">Прибор учета</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(RisMeteringDeviceData item)
        {
            StringBuilder messages = new StringBuilder();

            if (item.House == null || string.IsNullOrEmpty(item.House.Guid))
            {
                messages.Append("HOUSE_ID ");
            }

            if (string.IsNullOrEmpty(item.MunicipalResourceCode))
            {
                messages.Append("MUNICIPAL_RESOURCE_CODE ");
            }

            if (string.IsNullOrEmpty(item.MunicipalResourceGuid))
            {
                messages.Append("MUNICIPAL_RESOURCE_GUID ");
            }

            if (string.IsNullOrEmpty(item.MeteringDeviceNumber))
            {
                messages.Append("METERING_DEVICE_NUMBER ");
            }

            if (item.FirstVerificationDate == null && !string.IsNullOrEmpty(item.VerificationInterval))
            {
                messages.Append("FIRST_VERIFICATION_DATE ");
            }

            if (item.FirstVerificationDate != null && string.IsNullOrEmpty(item.VerificationInterval))
            {
                messages.Append("VERIFICATION_INTERVAL ");
            }

            if (item.DeviceType == DeviceType.ResidentialPremiseDevice
                || item.DeviceType == DeviceType.NonResidentialPremiseDevice
                || item.DeviceType == DeviceType.ApartmentHouseDevice
                || item.DeviceType == DeviceType.LivingRoomDevice
                || item.DeviceType == DeviceType.CollectiveApartmentDevice)
            {
                if (this.meteringDeviceAccount.Count(x => x.MeteringDeviceData.Id == item.Id) < 1)
                {
                    messages.Append("AccountGUID ");
                }
            }

            if (item.DeviceType == DeviceType.ResidentialPremiseDevice
                || item.DeviceType == DeviceType.CollectiveApartmentDevice)
            {
                if (item.ResidentialPremises == null || string.IsNullOrEmpty(item.ResidentialPremises.Guid))
                {
                    messages.Append("PREMISE_GUID ");
                }
            }

            if (item.DeviceType == DeviceType.NonResidentialPremiseDevice)
            {
                if (item.NonResidentialPremises == null || string.IsNullOrEmpty(item.NonResidentialPremises.Guid))
                {
                    messages.Append("PREMISE_GUID ");
                }
            }

            if (item.DeviceType == DeviceType.LivingRoomDevice
                && this.meteringDeviceLivingRoom.Count(x => x.MeteringDeviceData.Id == item.Id) < 1)
            {
                messages.Append("LivingRoomGUID ");
            }

            return new CheckingResult {Result = messages.Length == 0, Messages = messages };
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта</param>
        /// <returns>Объект запроса</returns>
        protected override importMeteringDeviceDataRequest GetRequestObject(
            IEnumerable<RisMeteringDeviceData> listForImport)
        {
            var importRequestList = new List<importMeteringDeviceDataRequestMeteringDevice>();

            foreach (var item in listForImport)
            {
                var prepareResult = this.PrepareRequest(item);
                importRequestList.Add(prepareResult);
            }

            this.CountObjects += importRequestList.Count;

            return new importMeteringDeviceDataRequest { FIASHouseGuid = listForImport.First().House.FiasHouseGuid, MeteringDevice = importRequestList.ToArray() };
        }

        private importMeteringDeviceDataRequestMeteringDevice PrepareRequest(RisMeteringDeviceData meteringDeviceData)
        {
            object meteringDevice = null;
            object meteringDeviceBasicCharacteristicsItem = null;

            var result = new importMeteringDeviceDataRequestMeteringDevice()
            {
                TransportGUID = Guid.NewGuid().ToString()
            };

            switch (meteringDeviceData.DeviceType)
            {
                case DeviceType.ApartmentHouseDevice:
                    meteringDeviceBasicCharacteristicsItem = new MeteringDeviceBasicCharacteristicsTypeApartmentHouseDevice
                                                             {
                                                                 AccountGUID = this.meteringDeviceAccount.Where(x => x.MeteringDeviceData.Id == meteringDeviceData.Id).Select(y => y.Account.Guid).ToArray()
                                                             };
                    break;
                case DeviceType.CollectiveApartmentDevice:
                    meteringDeviceBasicCharacteristicsItem = new MeteringDeviceBasicCharacteristicsTypeCollectiveApartmentDevice
                                                             {
                                                                    PremiseGUID = meteringDeviceData.ResidentialPremises.Guid,
                                                                    AccountGUID = this.meteringDeviceAccount.Where(x => x.MeteringDeviceData.Id == meteringDeviceData.Id).Select(y => y.Account.Guid).ToArray()
                    };
                    break;
                case DeviceType.LivingRoomDevice:
                    meteringDeviceBasicCharacteristicsItem = new MeteringDeviceBasicCharacteristicsTypeLivingRoomDevice
                                                             {
                                                                 LivingRoomGUID = this.meteringDeviceLivingRoom.Where(x => x.MeteringDeviceData.Id == meteringDeviceData.Id).Select(y => y.LivingRoom.Guid).ToArray(), 
                                                                 AccountGUID = this.meteringDeviceAccount.Where(x => x.MeteringDeviceData.Id == meteringDeviceData.Id).Select(y => y.Account.Guid).ToArray()
                    };
                    break;
                case DeviceType.ResidentialPremiseDevice:
                    meteringDeviceBasicCharacteristicsItem = new MeteringDeviceBasicCharacteristicsTypeResidentialPremiseDevice
                                                             {
                                                                    PremiseGUID = meteringDeviceData.ResidentialPremises.Guid,
                                                                    AccountGUID = this.meteringDeviceAccount.Where(x => x.MeteringDeviceData.Id == meteringDeviceData.Id).Select(y => y.Account.Guid).ToArray()
                    };
                    break;
                case DeviceType.NonResidentialPremiseDevice:
                    meteringDeviceBasicCharacteristicsItem = new MeteringDeviceBasicCharacteristicsTypeNonResidentialPremiseDevice
                                                             {
                                                                    PremiseGUID = meteringDeviceData.NonResidentialPremises.Guid,
                                                                    AccountGUID = this.meteringDeviceAccount.Where(x => x.MeteringDeviceData.Id == meteringDeviceData.Id).Select(y => y.Account.Guid).ToArray()
                    };
                    break;
                case DeviceType.CollectiveDevice:
                    meteringDeviceBasicCharacteristicsItem = true;
                    break;
            }

            switch (meteringDeviceData.MeteringDeviceType)
            {
                case MeteringDeviceType.OneRateMeteringDevice:
                    meteringDevice =
                        new importMeteringDeviceDataRequestMeteringDeviceDeviceDataToCreateOneRateMeteringDevice
                        {
                            MunicipalResource = new nsiRef
                                                {
                                                    Code = meteringDeviceData.MunicipalResourceCode,
                                                    GUID = meteringDeviceData.MunicipalResourceGuid
                                                },
                            ReadoutDate = meteringDeviceData.ReadoutDate,
                            ReadingsSource = meteringDeviceData.ReadingsSource,
                            BaseValue = meteringDeviceData.MeteringValueT1,
                            BasicChatacteristicts = new MeteringDeviceBasicCharacteristicsType
                                                    {
                                                        MeteringDeviceNumber = meteringDeviceData.MeteringDeviceNumber,
                                                        MeteringDeviceStamp = meteringDeviceData.MeteringDeviceStamp,
                                                         InstallationDate = meteringDeviceData.InstallationDate.GetValueOrDefault(),
                                                         CommissioningDate = meteringDeviceData.CommissioningDate,
                                                         ManualModeMetering = meteringDeviceData.ManualModeMetering,
                                                         VerificationCharacteristics = meteringDeviceData.FirstVerificationDate.HasValue || !string.IsNullOrEmpty(meteringDeviceData.VerificationInterval) ? new MeteringDeviceBasicCharacteristicsTypeVerificationCharacteristics
                                                                                       {
                                                                                           VerificationInterval = meteringDeviceData.VerificationInterval,
                                                                                           FirstVerificationDate = meteringDeviceData.FirstVerificationDate.GetValueOrDefault()
                                                                                        } : null,
                                                         Item = meteringDeviceBasicCharacteristicsItem

                            }
                        };
                    break;
                case MeteringDeviceType.ElectricMeteringDevice:
                    meteringDevice =
                        new importMeteringDeviceDataRequestMeteringDeviceDeviceDataToCreateElectricMeteringDevice
                        {
                            MunicipalResource = new nsiRef
                            {
                                Code = meteringDeviceData.MunicipalResourceCode, 
                                GUID = meteringDeviceData.MunicipalResourceGuid
                            },
                            ReadoutDate = meteringDeviceData.ReadoutDate,
                            ReadingsSource = meteringDeviceData.ReadingsSource,
                            BaseValueT1 = meteringDeviceData.MeteringValueT1,
                            BaseValueT2 = meteringDeviceData.MeteringValueT2.GetValueOrDefault(),
                            BaseValueT3 = meteringDeviceData.MeteringValueT3.GetValueOrDefault(),
                            BasicChatacteristicts = new MeteringDeviceBasicCharacteristicsType
                            {
                                MeteringDeviceNumber = meteringDeviceData.MeteringDeviceNumber,
                                MeteringDeviceStamp = meteringDeviceData.MeteringDeviceStamp,
                                InstallationDate = meteringDeviceData.InstallationDate.GetValueOrDefault(),
                                CommissioningDate = meteringDeviceData.CommissioningDate,
                                ManualModeMetering = meteringDeviceData.ManualModeMetering,
                                VerificationCharacteristics = meteringDeviceData.FirstVerificationDate.HasValue || !string.IsNullOrEmpty(meteringDeviceData.VerificationInterval) ? new MeteringDeviceBasicCharacteristicsTypeVerificationCharacteristics
                                {
                                    VerificationInterval = meteringDeviceData.VerificationInterval,
                                    FirstVerificationDate = meteringDeviceData.FirstVerificationDate.GetValueOrDefault()
                                } : null,
                                Item = meteringDeviceBasicCharacteristicsItem
                            }
                        };
                    break;
            }

            result.Item = new importMeteringDeviceDataRequestMeteringDeviceDeviceDataToCreate { Item = meteringDevice };

            this.meteringDeviceByTransportGuidDict.Add(result.TransportGUID, meteringDeviceData);

            return result;
        }

        /// <summary>
        /// Получить ответ от сервиса.
        /// </summary>
        /// <param name="request">Объект запроса</param>
        /// <returns>Ответ от сервиса</returns>
        protected override ImportResult GetRequestResult(importMeteringDeviceDataRequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importMeteringDeviceData(this.RequestHeader, request, out result);
            }

            return result;
        }

        /// <summary>
        /// Сохранение объектов ГИС после импорта.
        /// </summary>
        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.meteringDevicesToSave, 1000, true, true);
        }

        /// <summary>
        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент response</param>
        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (!this.meteringDeviceByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
                return;
            }

            var meteringDevice = this.meteringDeviceByTransportGuidDict[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("Прибор учета", meteringDevice.Id, "Не загружен", errorNotation);

                return;
            }

            meteringDevice.Guid = responseItem.GUID;
            this.meteringDevicesToSave.Add(meteringDevice);

            this.AddLineToLog("Прибор учета", meteringDevice.Id, "Загружен", responseItem.GUID);
        }

        /// <summary>
        /// Получить список блоков объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список блоков объектов ГИС</returns>
        protected override List<IEnumerable<RisMeteringDeviceData>> GetPortions()
        {
            List<IEnumerable<RisMeteringDeviceData>> result = new List<IEnumerable<RisMeteringDeviceData>>();
            Dictionary<string, List<RisMeteringDeviceData>> meteringDeviceByFiasHouseGuidDict = this.MainList.GroupBy(x => x.House.FiasHouseGuid).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var deviceList in meteringDeviceByFiasHouseGuidDict.Values)
            {
                var startIndex = 0;
                do
                {
                    result.Add(deviceList.Skip(startIndex).Take(this.Portion));
                    startIndex += this.Portion;
                }
                while (startIndex < deviceList.Count);
            }

            return result;
        }
    }
}
