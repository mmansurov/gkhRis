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
    /// Класс импорта контрольных показаний приборов учета
    /// </summary>
    public class ImportMeteringDeviceControlValuesMethod : GisIntegrationDeviceMeteringMethod<RisMeteringDeviceControlValue, importMeteringDeviceValuesRequest>
    {
        private readonly List<RisMeteringDeviceControlValue> controlValuesToSave = 
            new List<RisMeteringDeviceControlValue>();

        private readonly Dictionary<string, RisMeteringDeviceControlValue> controlValuesByTransportGuidDict = 
            new Dictionary<string, RisMeteringDeviceControlValue>();

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
                return "Импорт контрольных показаний приборов учета";
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
                return this.controlValuesToSave.Count;
            }
        }

        /// <summary>
        /// Список объектов ГИС, по которым производится импорт.
        /// </summary>
        protected override IList<RisMeteringDeviceControlValue> MainList { get; set; }

        /// <summary>
        /// Подготовка кэша данных 
        /// </summary>
        protected override void Prepare()
        {
            var controlValuesDomain = this.Container.ResolveDomain<RisMeteringDeviceControlValue>();

            try
            {
                this.MainList = controlValuesDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(controlValuesDomain);
            }
        }

        /// <summary>
        /// Проверка показаний прибора учета
        /// </summary>
        /// <param name="item">Прибор учета</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(RisMeteringDeviceControlValue item)
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
            IEnumerable<RisMeteringDeviceControlValue> listForImport)
        {
            var importRequestList = this.PrepareRequest(listForImport);

            this.CountObjects += importRequestList.Count;
            return new importMeteringDeviceValuesRequest { FIASHouseGuid = listForImport.First().MeteringDeviceData.House.FiasHouseGuid, MeteringDevicesValues = importRequestList.ToArray() };
        }

        private List<importMeteringDeviceValuesRequestMeteringDevicesValues> PrepareRequest(IEnumerable<RisMeteringDeviceControlValue> listForImport)
        {
            Dictionary<string, List<RisMeteringDeviceControlValue>> itemsByMeteringDeviceGuidDict = listForImport.GroupBy(x => x.MeteringDeviceData.Guid).ToDictionary(g => g.Key, g => g.ToList());

            List < importMeteringDeviceValuesRequestMeteringDevicesValues > result = new List<importMeteringDeviceValuesRequestMeteringDevicesValues>();

            foreach (var deviceItem in itemsByMeteringDeviceGuidDict)
            {
                List<RisMeteringDeviceControlValue> itemsWithEmptyAccount =
                    deviceItem.Value.Where(x => x.Account == null).ToList();

                foreach (var item in itemsWithEmptyAccount)
                {
                    deviceItem.Value.Remove(item);
                }

                Dictionary<RisAccount, List<RisMeteringDeviceControlValue>> itemsByAccountDict = deviceItem.Value.GroupBy(x => x.Account).ToDictionary(g => g.Key, g => g.ToList());

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
                                                        ControlValue = this.PrepareOneRateDeviceValueControlValue(accountItem.Value).ToArray()
                                                  }
                            });
                        }

                        result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValues
                        {
                            MeteringDeviceGUID = deviceItem.Key,
                            Item = new importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValue
                            {
                                ControlValue = this.PrepareOneRateDeviceValueControlValue(itemsWithEmptyAccount).ToArray()
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
                                    ControlValue = this.PrepareElectricDeviceValueControlValue(accountItem.Value).ToArray()
                                }
                            });
                        }

                        result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValues
                        {
                            MeteringDeviceGUID = deviceItem.Key,
                            Item = new importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValue
                            {
                                ControlValue = this.PrepareElectricDeviceValueControlValue(itemsWithEmptyAccount).ToArray()
                            }
                        });

                        break;
                }      
            }

            return result;
        }

        private List<importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValueControlValue> PrepareOneRateDeviceValueControlValue(List<RisMeteringDeviceControlValue> controlValues)
        {
            var result = new List<importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValueControlValue>();

            foreach (var item in controlValues)
            {
                var transportGUID = Guid.NewGuid().ToString();

                result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValueControlValue
                    {
                        MeteringValue = item.ValueT1,
                        ReadoutDate = item.ReadoutDate,
                        ReadingsSource = item.ReadingsSource,
                        TransportGUID = transportGUID
                    });

                this.controlValuesByTransportGuidDict.Add(transportGUID, item);
            }

            return result;
        }

        private List<importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValueControlValue> PrepareElectricDeviceValueControlValue(List<RisMeteringDeviceControlValue> controlValues)
        {
            var result =
                new List<importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValueControlValue>();

            foreach (var item in controlValues)
            {
                var transportGUID = Guid.NewGuid().ToString();

                result.Add(new importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValueControlValue
                {
                    MeteringValueT1 = item.ValueT1,
                    MeteringValueT2 = item.ValueT2.GetValueOrDefault(),
                    MeteringValueT3 = item.ValueT3.GetValueOrDefault(),
                    ReadoutDate = item.ReadoutDate,
                    ReadingsSource = item.ReadingsSource,
                    TransportGUID = transportGUID
                });

                this.controlValuesByTransportGuidDict.Add(transportGUID, item);
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
            TransactionHelper.InsertInManyTransactions(this.Container, this.controlValuesToSave, 1000, true, true);
        }

        /// <summary>
        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент response</param>
        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (!this.controlValuesByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
                return;
            }

            var controlValue = this.controlValuesByTransportGuidDict[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("Контрольное показание прибора учета", controlValue.Id, "Не загружено", errorNotation);

                return;
            }

            controlValue.Guid = responseItem.GUID;
            this.controlValuesToSave.Add(controlValue);

            this.AddLineToLog("Контрольное показание прибора учета", controlValue.Id, "Загружено", responseItem.GUID);
        }

        /// <summary>
        /// Получить список блоков объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список блоков объектов ГИС</returns>
        protected override List<IEnumerable<RisMeteringDeviceControlValue>> GetPortions()
        {
            List<IEnumerable<RisMeteringDeviceControlValue>> result = new List<IEnumerable<RisMeteringDeviceControlValue>>();
            Dictionary<string, List<RisMeteringDeviceControlValue>> controlValuesByFiasHouseGuidDict = this.MainList.GroupBy(x => x.MeteringDeviceData.House.FiasHouseGuid).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var deviceList in controlValuesByFiasHouseGuidDict.Values)
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
