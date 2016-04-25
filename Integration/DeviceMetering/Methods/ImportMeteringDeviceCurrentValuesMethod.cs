namespace Bars.Gkh.Ris.Integration.DeviceMetering.Methods
{
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.DeviceMetering;
    using Enums.HouseManagement;
    using Ris.DeviceMetering;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ImportMeteringDeviceCurrentValuesMethod :
        GisIntegrationDeviceMeteringMethod<RisMeteringDeviceCurrentValue, importMeteringDeviceValuesRequest>
    {
        private List<RisMeteringDeviceCurrentValue>  currentValuesToSave = new List<RisMeteringDeviceCurrentValue>();

        private Dictionary<string, RisMeteringDeviceCurrentValue> currentValuesByTransportGuidDict = new Dictionary<string, RisMeteringDeviceCurrentValue>();

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
                return "Импорт текущих показаний приборов учета";
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
                return this.currentValuesToSave.Count;
            }
        }

        /// <summary>
        /// Список объектов ГИС, по которым производится импорт.
        /// </summary>
        protected override IList<RisMeteringDeviceCurrentValue> MainList { get; set; }

        /// <summary>
        /// Подготовка кэша данных 
        /// </summary>
        protected override void Prepare()
        {
            var currentValuesDomain = this.Container.ResolveDomain<RisMeteringDeviceCurrentValue>();
           
            try
            {
                this.MainList = currentValuesDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(currentValuesDomain);
            }
        }

        /// <summary>
        /// Проверка показаний прибора учета
        /// </summary>
        /// <param name="item">Прибор учета</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(RisMeteringDeviceCurrentValue item)
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
            IEnumerable<RisMeteringDeviceCurrentValue> listForImport)
        {
            var importRequestList = new List<importMeteringDeviceValuesRequestMeteringDevicesValues>();

            foreach (var item in listForImport)
            {
                var prepareResult = this.PrepareRequest(item);
                importRequestList.Add(prepareResult);
            }

            this.CountObjects += importRequestList.Count;
            return new importMeteringDeviceValuesRequest { FIASHouseGuid = listForImport.First().MeteringDeviceData.House.FiasHouseGuid, MeteringDevicesValues = importRequestList.ToArray() };
        }

        private importMeteringDeviceValuesRequestMeteringDevicesValues PrepareRequest(RisMeteringDeviceCurrentValue meteringDeviceCurrentValue)
        {
            object value = null;
            var transportGUID = Guid.NewGuid().ToString();

            switch (meteringDeviceCurrentValue.MeteringDeviceData.MeteringDeviceType)
            {
                case MeteringDeviceType.OneRateMeteringDevice:
                    value = new importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValue
                            {
                                CurrentValue = new importMeteringDeviceValuesRequestMeteringDevicesValuesOneRateDeviceValueCurrentValue
                                {
                                    MeteringValue = meteringDeviceCurrentValue.ValueT1,
                                    ReadoutDate = meteringDeviceCurrentValue.ReadoutDate,
                                    ReadingsSource = meteringDeviceCurrentValue.ReadingsSource,
                                    TransportGUID = transportGUID
                                }
                            };
                    break;
                case MeteringDeviceType.ElectricMeteringDevice:
                    value = new importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValue
                            {
                                CurrentValue = new importMeteringDeviceValuesRequestMeteringDevicesValuesElectricDeviceValueCurrentValue
                                {
                                    MeteringValueT1 = meteringDeviceCurrentValue.ValueT1,
                                    MeteringValueT2 = meteringDeviceCurrentValue.ValueT2.GetValueOrDefault(),
                                    MeteringValueT3 = meteringDeviceCurrentValue.ValueT3.GetValueOrDefault(),
                                    ReadoutDate = meteringDeviceCurrentValue.ReadoutDate,
                                    ReadingsSource = meteringDeviceCurrentValue.ReadingsSource,
                                    TransportGUID = transportGUID
                                }
                            };
                    break;
            }

            this.currentValuesByTransportGuidDict.Add(transportGUID, meteringDeviceCurrentValue);

            return new importMeteringDeviceValuesRequestMeteringDevicesValues
                   {
                       MeteringDeviceGUID = meteringDeviceCurrentValue.MeteringDeviceData.Guid,
                       AccountGUID = meteringDeviceCurrentValue.Account != null ? meteringDeviceCurrentValue.Account.Guid : null,
                       Item = value
                   };
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
            TransactionHelper.InsertInManyTransactions(this.Container, this.currentValuesToSave, 1000, true, true);
        }

        /// <summary>
        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент response</param>
        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (!this.currentValuesByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
                return;
            }

            var currentValue = this.currentValuesByTransportGuidDict[responseItem.TransportGUID];

            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("Текущее показание прибора учета", currentValue.Id, "Не загружено", errorNotation);

                return;
            }

            currentValue.Guid = responseItem.GUID;
            this.currentValuesToSave.Add(currentValue);

            this.AddLineToLog("Текущее показание прибора учета", currentValue.Id, "Загружено", responseItem.GUID);
        }

        /// <summary>
        /// Получить список блоков объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список блоков объектов ГИС</returns>
        protected override List<IEnumerable<RisMeteringDeviceCurrentValue>> GetPortions()
        {
            List<IEnumerable<RisMeteringDeviceCurrentValue>> result = new List<IEnumerable<RisMeteringDeviceCurrentValue>>();
            Dictionary<string, List<RisMeteringDeviceCurrentValue>> currentValuesByFiasHouseGuidDict = this.MainList.GroupBy(x => x.MeteringDeviceData.House.FiasHouseGuid).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var deviceList in currentValuesByFiasHouseGuidDict.Values)
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
