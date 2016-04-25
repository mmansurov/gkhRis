namespace Bars.Gkh.Ris.Integration.DeviceMetering.Methods
{
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.DeviceMetering;
    using Entities.HouseManagement;
    using Enums.HouseManagement;
    using Ris.DeviceMetering;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Класс импорта проверочных показаний приборов учета
    /// </summary>
    public class ImportMeteringDeviceVerificationValueMethod : GisIntegrationDeviceMeteringMethod<RisMeteringDeviceVerificationValue, importMeteringDeviceValuesRequest>
    {
        private readonly List<RisMeteringDeviceVerificationValue> verificationValuesToSave = 
            new List<RisMeteringDeviceVerificationValue>();

        private readonly Dictionary<string, RisMeteringDeviceVerificationValue> verificationValuesByTransportGuidDict = 
            new Dictionary<string, RisMeteringDeviceVerificationValue>();

        /// <summary>
        /// Код метода
        /// </summary>
        public override string Code
        {
            get
            {
                return "importMeteringDeviceValues";
            }
        }

        /// <summary>
        /// Наименование метода
        /// </summary>
        public override string Name
        {
            get
            {
                return "Импорт проверочных показаний приборов учета";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 49;
            }
        }

        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        protected override int Portion
        {
            get
            {
                return 1000;
            }
        }

        /// <summary>
        /// Количество объектов для сохранения
        /// </summary>
        protected override int ProcessedObjects
        {
            get
            {
                return this.verificationValuesToSave.Count;
            }
        }

        /// <summary>
        /// Список объектов ГИС, по которым производится импорт.
        /// </summary>
        protected override IList<RisMeteringDeviceVerificationValue> MainList { get; set; }

        /// <summary>
        /// Подготовка кэша данных 
        /// </summary>
        protected override void Prepare()
        {
            var verificationValuesDomain = this.Container.ResolveDomain<RisMeteringDeviceVerificationValue>();

            try
            {
                this.MainList = verificationValuesDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(verificationValuesDomain);
            }
        }

        /// <summary>
        /// Проверка показаний прибора учета
        /// </summary>
        /// <param name="item">Прибор учета</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(RisMeteringDeviceVerificationValue item)
        {
            StringBuilder messages = new StringBuilder();

            if (item.MeteringDeviceData == null || string.IsNullOrEmpty(item.MeteringDeviceData.Guid))
            {
                messages.Append("MeteringDeviceData.Guid ");
            }
            else if (item.MeteringDeviceData.House == null || string.IsNullOrEmpty(item.MeteringDeviceData.House.FiasHouseGuid))
            {
                messages.Append("MeteringDeviceData.House.FIASHouseGuid ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта</param>
        /// <returns>Объект запроса</returns>
        protected override importMeteringDeviceValuesRequest GetRequestObject(
            IEnumerable<RisMeteringDeviceVerificationValue> listForImport)
        {
            var importRequestList = this.PrepareRequest(listForImport);

            this.CountObjects += importRequestList.Count;
            return new importMeteringDeviceValuesRequest { FIASHouseGuid = listForImport.First().MeteringDeviceData.House.FiasHouseGuid, MeteringDevicesValues = importRequestList.ToArray() };
        }

        private List<importMeteringDeviceValuesRequestMeteringDevicesValues> PrepareRequest(IEnumerable<RisMeteringDeviceVerificationValue> listForImport)
        {
            Dictionary<string, List<RisMeteringDeviceVerificationValue>> itemsByMeteringDeviceGuidDict = listForImport.GroupBy(x => x.MeteringDeviceData.Guid).ToDictionary(g => g.Key, g => g.ToList());

            List<importMeteringDeviceValuesRequestMeteringDevicesValues> result = new List<importMeteringDeviceValuesRequestMeteringDevicesValues>();

            foreach (var deviceItem in itemsByMeteringDeviceGuidDict)
            {
                List<RisMeteringDeviceVerificationValue> itemsWithEmptyAccount =
                    deviceItem.Value.Where(x => x.Account == null).ToList();

                foreach (var item in itemsWithEmptyAccount)
                {
                    deviceItem.Value.Remove(item);
                }

                Dictionary<RisAccount, List<RisMeteringDeviceVerificationValue>> itemsByAccountDict = deviceItem.Value.GroupBy(x => x.Account).ToDictionary(g => g.Key, g => g.ToList());

                switch (deviceItem.Value[0].MeteringDeviceData.MeteringDeviceType)
                {
                    case MeteringDeviceType.OneRateMeteringDevice:

                        foreach (var accountItem in itemsByAccountDict)
                        {
                            result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValues
                            {
                                MeteringDeviceGUID = deviceItem.Key,
                                AccountGUID = accountItem.Key.Guid,
                                Item = new importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValue
                                {
                                    VerificationValue = this.PrepareOneRateDeviceValueVerificationValue(accountItem.Value).ToArray()
                                }
                            });
                        }

                        result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValues
                        {
                            MeteringDeviceGUID = deviceItem.Key,
                            Item = new importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValue
                            {
                                VerificationValue = this.PrepareOneRateDeviceValueVerificationValue(itemsWithEmptyAccount).ToArray()
                            }
                        });

                        break;
                    case MeteringDeviceType.ElectricMeteringDevice:
                        foreach (var accountItem in itemsByAccountDict)
                        {
                            result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValues
                            {
                                MeteringDeviceGUID = deviceItem.Key,
                                AccountGUID = accountItem.Key.Guid,
                                Item = new importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValue
                                {
                                    VerificationValue = this.PrepareElectricDeviceValueVerificationValue(accountItem.Value).ToArray()
                                }
                            });
                        }

                        result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValues
                        {
                            MeteringDeviceGUID = deviceItem.Key,
                            Item = new importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValue
                            {
                                VerificationValue = this.PrepareElectricDeviceValueVerificationValue(itemsWithEmptyAccount).ToArray()
                            }
                        });

                        break;
                }
            }

            return result;
        }

        private List<importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValueVerificationValue> PrepareOneRateDeviceValueVerificationValue(List<RisMeteringDeviceVerificationValue> verificationValues)
        {
            var result = new List<importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValueVerificationValue>();

            foreach (var item in verificationValues)
            {
                var transportGUID = Guid.NewGuid().ToString();

                object verificationType;

                if (item.PlannedVerification)
                {
                    verificationType = true;
                }
                else
                {
                    verificationType = new nsiRef
                                       {
                                           GUID = item.VerificationReasonGuid,
                                           Code = item.VerificationReasonCode,
                                           Name = item.VerificationReasonName
                                       };
                }

                result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValueVerificationValue
                {
                    StartVerificationValue = new OneRateMeteringValueType
                    {
                        MeteringValue = item.StartVerificationValueT1,
                        ReadoutDate = item.StartVerificationReadoutDate,
                        ReadingsSource = item.StartVerificationReadingsSource,                       
                    },
                    EndVerificationValue = new OneRateMeteringValueType
                    {
                        MeteringValue = item.EndVerificationValueT1,
                        ReadoutDate = item.EndVerificationReadoutDate,
                        ReadingsSource = item.EndVerificationReadingsSource
                    },
                    Item = verificationType,
                    TransportGUID = transportGUID
                });

                this.verificationValuesByTransportGuidDict.Add(transportGUID, item);
            }

            return result;
        }

        private List<importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValueVerificationValue> PrepareElectricDeviceValueVerificationValue(List<RisMeteringDeviceVerificationValue> verificationValues)
        {
            var result =
                new List<importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValueVerificationValue>();

            foreach (var item in verificationValues)
            {
                var transportGUID = Guid.NewGuid().ToString();

                object verificationType;

                if (item.PlannedVerification)
                {
                    verificationType = true;
                }
                else
                {
                    verificationType = new nsiRef
                    {
                        GUID = item.VerificationReasonGuid,
                        Code = item.VerificationReasonCode,
                        Name = item.VerificationReasonName
                    };
                }

                result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValueVerificationValue
                {
                    StartVerificationValue = new ElectricMeteringValueType
                    {
                        MeteringValueT1 = item.StartVerificationValueT1,
                        MeteringValueT2 = item.StartVerificationValueT2.GetValueOrDefault(),
                        MeteringValueT3 = item.StartVerificationValueT3.GetValueOrDefault(),
                        ReadoutDate = item.StartVerificationReadoutDate,
                        ReadingsSource = item.StartVerificationReadingsSource
                    },
                    EndVerificationValue = new ElectricMeteringValueType
                    {
                        MeteringValueT1 = item.EndVerificationValueT1,
                        MeteringValueT2 = item.EndVerificationValueT2.GetValueOrDefault(),
                        MeteringValueT3 = item.EndVerificationValueT3.GetValueOrDefault(),
                        ReadoutDate = item.EndVerificationReadoutDate,
                        ReadingsSource = item.EndVerificationReadingsSource
                    },
                    Item = verificationType,
                    TransportGUID = transportGUID
                });

                this.verificationValuesByTransportGuidDict.Add(transportGUID, item);
            }

            return result;
        }

        /// <summary>
        /// Получить ответ от сервиса.
        /// </summary>
        /// <param name="request">Объект запроса</param>
        /// <returns>Ответ от сервиса</returns>
        protected override ImportResult GetRequestResult(importMeteringDeviceValuesRequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importMeteringDeviceValues(this.RequestHeader, request, out result);
            }

            return result;
        }

        /// <summary>
        /// Сохранение объектов ГИС после импорта.
        /// </summary>
        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.verificationValuesToSave, 1000, true, true);
        }

        /// <summary>
        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент response</param>
        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (!this.verificationValuesByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
                return;
            }

            var verificationValue = this.verificationValuesByTransportGuidDict[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("Проверочное показание прибора учета", verificationValue.Id, "Не загружено", errorNotation);

                return;
            }

            verificationValue.Guid = responseItem.GUID;
            this.verificationValuesToSave.Add(verificationValue);

            this.AddLineToLog("Проверочное показание прибора учета", verificationValue.Id, "Загружено", responseItem.GUID);
        }

        /// <summary>
        /// Получить список блоков объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список блоков объектов ГИС</returns>
        protected override List<IEnumerable<RisMeteringDeviceVerificationValue>> GetPortions()
        {
            List<IEnumerable<RisMeteringDeviceVerificationValue>> result = new List<IEnumerable<RisMeteringDeviceVerificationValue>>();
            Dictionary<string, List<RisMeteringDeviceVerificationValue>> verificationValuesByFiasHouseGuidDict = this.MainList.GroupBy(x => x.MeteringDeviceData.House.FiasHouseGuid).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var deviceList in verificationValuesByFiasHouseGuidDict.Values)
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
