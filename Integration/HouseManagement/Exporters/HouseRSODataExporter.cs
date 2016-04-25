namespace Bars.Gkh.Ris.Integration.HouseManagement.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Entities.HouseManagement;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.HouseManagementAsync;

    /// <summary>
    /// Класс экспортер данных домов для ресурсоснабжающих организаций
    /// </summary>
    public partial class HouseRSODataExporter : BaseHouseDataExporter<importHouseRSORequest>
    {
        /// <summary>
        /// Наименование метода
        /// </summary>
        public override string Name
        {
            get
            {
                return "Экспорт сведений о доме для ресурсоснабжающих организаций";
            }
        }

        /// <summary>
        /// Описание экспортера
        /// </summary>
        public override string Description
        {
            get
            {
                return "Операция позволяет импортировать в ГИС ЖКХ сведения о многоквартирном или жилом доме, включая сведения о подъездах, помещениях, квартирах. Операция используется как для первичной загрузки сведений, так и для их обновлений.";
            }
        }

        /// <summary>
        /// Порядок экспортера в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 29;
            }
        }

        /// <summary>
        /// Переопределить параметры сбора данных
        /// </summary>
        /// <param name="parameters">Параметры сбора</param>
        protected override void OverrideExtractingParametes(DynamicDictionary parameters)
        {
            parameters.Add("rsoId", this.Contragent.GkhId);
        }

        /// <summary>
        /// Проверка дома перед импортом
        /// </summary>
        /// <param name="house">Дом</param>
        /// <returns>Результат проверки</returns>
        protected override ValidateObjectResult CheckHouse(RisHouse house)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(house.FiasHouseGuid))
            {
                messages.Append("FIASHouseGuid ");
            }

            if (string.IsNullOrEmpty(house.OktmoCode) && !string.IsNullOrEmpty(house.OktmoName))
            {
                messages.Append("OktmoCode ");
            }

            if (string.IsNullOrEmpty(house.OlsonTZCode) || string.IsNullOrEmpty(house.OlsonTZGuid))
            {
                messages.Append("OlsonTZ ");
            }

            return new ValidateObjectResult
            {
                Id = house.Id,
                State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                Message = messages.ToString(),
                Description = "Дом"
            };
        }

        /// <summary>
        /// Проверка жилого помещения перед импортом
        /// </summary>
        /// <param name="premise">Жилое помещение</param>
        /// <returns>Результат проверки</returns>
        protected override ValidateObjectResult CheckResidentialPremise(ResidentialPremises premise)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(premise.PremisesNum))
            {
                messages.Append("PremisesNum ");
            }

            if (!premise.EntranceNum.HasValue)
            {
                messages.Append("EntranceNum ");
            }

            if (string.IsNullOrEmpty(premise.PremisesCharacteristicCode)
                && !string.IsNullOrEmpty(premise.PremisesCharacteristicGuid))
            {
                messages.Append("PremisesCharacteristicCode ");
            }

            if (!string.IsNullOrEmpty(premise.PremisesCharacteristicCode)
                && string.IsNullOrEmpty(premise.PremisesCharacteristicGuid))
            {
                messages.Append("PremisesCharacteristicGuid ");
            }

            return new ValidateObjectResult
            {
                Id = premise.Id,
                State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                Message = messages.ToString(),
                Description = "Жилое помещение"
            };
        }

        /// <summary>
        /// Проверка нежилого помещения перед импортом
        /// </summary>
        /// <param name="premise">Нежилое помещение</param>
        /// <returns>Результат проверки</returns>
        protected override ValidateObjectResult CheckNonResidentialPremise(NonResidentialPremises premise)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(premise.PremisesNum))
            {
                messages.Append("PremisesNum ");
            }

            return new ValidateObjectResult
            {
                Id = premise.Id,
                State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                Message = messages.ToString(),
                Description = "Нежилое помещение"
            };
        }

        /// <summary>
        /// Проверка подъезда перед импортом
        /// </summary>
        /// <param name="entrance">Подъезд</param>
        /// <returns>Результат проверки</returns>
        protected override ValidateObjectResult CheckEntrance(RisEntrance entrance)
        {
            StringBuilder messages = new StringBuilder();

            if (!entrance.EntranceNum.HasValue)
            {
                messages.Append("EntranceNum ");
            }

            return new ValidateObjectResult
            {
                Id = entrance.Id,
                State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                Message = messages.ToString(),
                Description = "Подъезд"
            };
        }

        /// <summary>
        /// Проверка комнаты в жилом доме перед импортом
        /// </summary>
        /// <param name="livingRoom">Комната в жилом доме</param>
        /// <returns>Результат проверки</returns>
        protected override ValidateObjectResult CheckLivingRoom(LivingRoom livingRoom)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(livingRoom.RoomNumber))
            {
                messages.Append("RoomNumber ");
            }

            return new ValidateObjectResult
            {
                Id = livingRoom.Id,
                State = messages.Length == 0 ? ObjectValidateState.Success : ObjectValidateState.Error,
                Message = messages.ToString(),
                Description = "Комната"
            };
        }

        /// <summary>
        /// Сформировать объекты запросов к асинхронному сервису ГИС
        /// </summary>
        /// <returns>Словарь Объект запроса - Словарь Транспортных идентификаторов: Тип обектов - Словарь: Транспортный идентификатор - Идентификатор объекта</returns>
        protected override Dictionary<importHouseRSORequest, Dictionary<Type, Dictionary<string, long>>> GetRequestData()
        {
            var result = new Dictionary<importHouseRSORequest, Dictionary<Type, Dictionary<string, long>>>();

            foreach (var iterationList in this.GetPortions())
            {
                var transportGuidDictionary = new Dictionary<Type, Dictionary<string, long>>();
                var request = this.GetRequestObject(iterationList, transportGuidDictionary);
                request.Id = Guid.NewGuid().ToString();

                result.Add(request, transportGuidDictionary);
            }

            return result;
        }

        /// <summary>
        /// Выполнить запрос к асинхронному сервису ГИС
        /// </summary>
        /// <param name="request">Объект запроса</param>
        /// <returns>Идентификатор сообщения для получения результата</returns>
        protected override string ExecuteRequest(importHouseRSORequest request)
        {
            AckRequest result;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient == null)
            {
                throw new Exception("Не удалось получить SOAP клиент");
            }

            soapClient.importHouseRSOData(this.GetNewRequestHeader(), request, out result);

            if (result == null || result.Ack == null)
            {
                throw new Exception("Пустой результат выполенния запроса");
            }

            return result.Ack.MessageGUID;
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта (в текущем методе в списке 1 объект)</param>
        /// <param name="transportGuidDictionary">Словарь транспортных идентификаторов</param>
        /// <returns>Объект запроса</returns>
        private importHouseRSORequest GetRequestObject(
            IEnumerable<RisHouse> listForImport,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var house = listForImport.First();
            object item = null;

            switch (house.HouseType)
            {
                case HouseType.Apartment:
                    item = this.CreateApartmentHouseRequest(house, transportGuidDictionary);
                    break;
                case HouseType.Living:
                    item = this.CreateLivingHouseRequest(house, transportGuidDictionary);
                    break;
            }

            return new importHouseRSORequest { Item = item };
        }

        private HouseBasicRSOType GetBasicCharacteristictsToCreate(RisHouse house)
        {
            object noKNData;

            ItemChoiceType9 itemElementName;

            if (string.IsNullOrEmpty(house.CadastralNumber))
            {
                noKNData = true;
                itemElementName = ItemChoiceType9.NoKNData;
            }
            else
            {
                noKNData = house.CadastralNumber;
                itemElementName = ItemChoiceType9.CadastralNumber;
            }

            return new HouseBasicRSOType
            {
                Item = noKNData,
                ItemElementName = itemElementName,
                FIASHouseGuid = house.FiasHouseGuid,
                OKTMO = !string.IsNullOrEmpty(house.OktmoCode) ?
                    new OKTMORefType
                    {
                        code = house.OktmoCode,
                        name = house.OktmoName
                    } : null,
                OlsonTZ = new nsiRef
                {
                    Code = house.OlsonTZCode,
                    GUID = house.OlsonTZGuid
                }
            };
        }

        private HouseBasicUpdateRSOType GetBasicCharacteristictsToUpdate(RisHouse house)
        {
            object noKNData;

            ItemChoiceType9 itemElementName;

            if (string.IsNullOrEmpty(house.CadastralNumber))
            {
                noKNData = true;
                itemElementName = ItemChoiceType9.NoKNData;
            }
            else
            {
                noKNData = house.CadastralNumber;
                itemElementName = ItemChoiceType9.CadastralNumber;
            }

            return new HouseBasicUpdateRSOType
            {
                Item = noKNData,
                ItemElementName = itemElementName,
                FIASHouseGuid = house.FiasHouseGuid,              
                OlsonTZ = new nsiRef
                {
                    Code = house.OlsonTZCode,
                    GUID = house.OlsonTZGuid
                }
            };
        }
    }
}
