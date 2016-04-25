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
    /// Класс экспортер данных домов для органов местного самоуправления
    /// </summary>
    public partial class HouseOMSDataExporter : BaseHouseDataExporter<importHouseOMSRequest>
    {
        /// <summary>
        /// Наименование метода
        /// </summary>
        public override string Name
        {
            get
            {
                return "Экспорт сведений о доме для органов местного самоуправления";
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
                return 30;
            }
        }

        /// <summary>
        /// Переопределить параметры сбора данных
        /// </summary>
        /// <param name="parameters">Параметры сбора</param>
        protected override void OverrideExtractingParametes(DynamicDictionary parameters)
        {
            parameters.Add("omsId", this.Contragent.GkhId);
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

            if (!house.TotalSquare.HasValue || Decimal.Round(house.TotalSquare.Value, 1) < 0.1m)
            {
                messages.Append("TotalSquare ");
            }

            if (string.IsNullOrEmpty(house.StateCode) && !string.IsNullOrEmpty(house.StateGuid))
            {
                messages.Append("StateCode ");
            }

            if (!string.IsNullOrEmpty(house.StateCode) && string.IsNullOrEmpty(house.StateGuid))
            {
                messages.Append("StateGuid ");
            }

            if (string.IsNullOrEmpty(house.ProjectTypeCode) && !string.IsNullOrEmpty(house.ProjectTypeGuid))
            {
                messages.Append("ProjectTypeCode ");
            }

            if (!string.IsNullOrEmpty(house.ProjectTypeCode) && string.IsNullOrEmpty(house.ProjectTypeGuid))
            {
                messages.Append("ProjectTypeGuid ");
            }

            if (house.UsedYear < 1600)
            {
                messages.Append("UsedYear ");
            }

            if (string.IsNullOrEmpty(house.FloorCount))
            {
                messages.Append("FloorCount ");
            }

            if (string.IsNullOrEmpty(house.EnergyCategoryCode) && !string.IsNullOrEmpty(house.EnergyCategoryGuid))
            {
                messages.Append("EnergyCategoryCode ");
            }

            if (!string.IsNullOrEmpty(house.EnergyCategoryCode) && string.IsNullOrEmpty(house.EnergyCategoryGuid))
            {
                messages.Append("EnergyCategoryGuid ");
            }

            if (string.IsNullOrEmpty(house.OktmoCode) && !string.IsNullOrEmpty(house.OktmoName))
            {
                messages.Append("OktmoCode ");
            }

            if (string.IsNullOrEmpty(house.OlsonTZCode) || string.IsNullOrEmpty(house.OlsonTZGuid))
            {
                messages.Append("OlsonTZ ");
            }

            if (Decimal.Round(house.ResidentialSquare, 2) < 0.01m)
            {
                messages.Append("ResidentialSquare ");
            }

            if (house.HouseType == HouseType.Apartment)
            {
                if (string.IsNullOrEmpty(house.UndergroundFloorCount))
                {
                    messages.Append("UndergroundFloorCount ");
                }

                if (string.IsNullOrEmpty(house.OverhaulFormingKindCode)
                && !string.IsNullOrEmpty(house.OverhaulFormingKindGuid))
                {
                    messages.Append("OverhaulFormingKindCode ");
                }

                if (!string.IsNullOrEmpty(house.OverhaulFormingKindCode)
                    && string.IsNullOrEmpty(house.OverhaulFormingKindGuid))
                {
                    messages.Append("OverhaulFormingKindGuid ");
                }

                if (Decimal.Round(house.NonResidentialSquare, 2) < 0.01m)
                {
                    messages.Append("NonResidentialSquare ");
                }
            }

            if (house.HouseType == HouseType.Living)
            {
                if (string.IsNullOrEmpty(house.ResidentialHouseTypeCode)
                    && !string.IsNullOrEmpty(house.ResidentialHouseTypeGuid))
                {
                    messages.Append("ResidentialHouseTypeCode ");
                }

                if (!string.IsNullOrEmpty(house.ResidentialHouseTypeCode)
                    && string.IsNullOrEmpty(house.ResidentialHouseTypeGuid))
                {
                    messages.Append("ResidentialHouseTypeGuid ");
                }
            }

            if (string.IsNullOrEmpty(house.HouseManagementTypeCode)
                && !string.IsNullOrEmpty(house.HouseManagementTypeGuid))
            {
                messages.Append("HouseManagementTypeCode ");
            }

            if (!string.IsNullOrEmpty(house.HouseManagementTypeCode)
                && string.IsNullOrEmpty(house.HouseManagementTypeGuid))
            {
                messages.Append("HouseManagementTypeGuid ");
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

            if (string.IsNullOrEmpty(premise.RoomsNumCode) && !string.IsNullOrEmpty(premise.RoomsNumGuid))
            {
                messages.Append("RoomsNumCode ");
            }

            if (!string.IsNullOrEmpty(premise.RoomsNumCode) && string.IsNullOrEmpty(premise.RoomsNumGuid))
            {
                messages.Append("RoomsNumGuid ");
            }

            if (!premise.GrossArea.HasValue)
            {
                messages.Append("GrossArea ");
            }

            if (string.IsNullOrEmpty(premise.ResidentialHouseTypeCode)
                && !string.IsNullOrEmpty(premise.ResidentialHouseTypeGuid))
            {
                messages.Append("ResidentialHouseTypeCode ");
            }

            if (!string.IsNullOrEmpty(premise.ResidentialHouseTypeCode)
                && string.IsNullOrEmpty(premise.ResidentialHouseTypeGuid))
            {
                messages.Append("ResidentialHouseTypeGuid ");
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

            if (string.IsNullOrEmpty(premise.PurposeCode) && !string.IsNullOrEmpty(premise.PurposeGuid))
            {
                messages.Append("PurposeCode ");
            }

            if (!string.IsNullOrEmpty(premise.PurposeCode) && string.IsNullOrEmpty(premise.PurposeGuid))
            {
                messages.Append("PurposeGuid ");
            }

            if (string.IsNullOrEmpty(premise.PositionCode) && !string.IsNullOrEmpty(premise.PositionGuid))
            {
                messages.Append("PositionCode ");
            }

            if (!string.IsNullOrEmpty(premise.PositionCode) && string.IsNullOrEmpty(premise.PositionGuid))
            {
                messages.Append("PositionGuid ");
            }

            if (!premise.TotalArea.HasValue)
            {
                messages.Append("TotalArea ");
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

            if (!livingRoom.Square.HasValue)
            {
                messages.Append("Square ");
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
        protected override Dictionary<importHouseOMSRequest, Dictionary<Type, Dictionary<string, long>>> GetRequestData()
        {
            var result = new Dictionary<importHouseOMSRequest, Dictionary<Type, Dictionary<string, long>>>();

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
        protected override string ExecuteRequest(importHouseOMSRequest request)
        {
            AckRequest result;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient == null)
            {
                throw new Exception("Не удалось получить SOAP клиент");
            }

            soapClient.importHouseOMSData(this.GetNewRequestHeader(), request, out result);

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
        private importHouseOMSRequest GetRequestObject(
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

            return new importHouseOMSRequest { Item = item };
        }

        private HouseBasicOMSType GetBasicCharacteristictsToCreate(RisHouse house)
        {
            object noGknRelationship;

            if (string.IsNullOrEmpty(house.CadastralNumber))
            {
                noGknRelationship = true;
            }
            else
            {
                noGknRelationship = house.CadastralNumber;
            }

            return new HouseBasicOMSType
            {
                Item = noGknRelationship,
                FIASHouseGuid = house.FiasHouseGuid,
                TotalSquare = Decimal.Round(house.TotalSquare.GetValueOrDefault(), 1),
                State = !string.IsNullOrEmpty(house.StateCode) || !string.IsNullOrEmpty(house.StateGuid) ?
                    new nsiRef
                    {
                        Code = house.StateCode,
                        GUID = house.StateGuid
                    } : null,
                ProjectSeries = house.ProjectSeries,
                ProjectType = !string.IsNullOrEmpty(house.ProjectTypeCode) || !string.IsNullOrEmpty(house.ProjectTypeGuid) ?
                        new nsiRef
                        {
                            Code = house.ProjectTypeCode,
                            GUID = house.ProjectTypeGuid
                        } : null,
                BuildingYear = house.BuildingYear.GetValueOrDefault(),
                UsedYear = house.UsedYear.GetValueOrDefault(),
                TotalWear = house.TotalWear.GetValueOrDefault(),
                FloorCount = house.FloorCount,
                Energy = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) || house.EnergyDate.HasValue ?
                    this.GetEnergyToCreate(house) : null,
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
                },
                ResidentialSquare = Decimal.Round(house.ResidentialSquare, 2),
                CulturalHeritage = house.CulturalHeritage,
            };
        }

        private HouseBasicUpdateOMSType GetBasicCharacteristictsToUpdate(RisHouse house)
        {
            object noGknRelationship;

            if (string.IsNullOrEmpty(house.CadastralNumber))
            {
                noGknRelationship = true;
            }
            else
            {
                noGknRelationship = house.CadastralNumber;
            }

            return new HouseBasicUpdateOMSType
            {
                Item = noGknRelationship,
                FIASHouseGuid = house.FiasHouseGuid,
                TotalSquare = Decimal.Round(house.TotalSquare.GetValueOrDefault(), 1),
                State = !string.IsNullOrEmpty(house.StateCode) || !string.IsNullOrEmpty(house.StateGuid) ?
                    new nsiRef
                    {
                        Code = house.StateCode,
                        GUID = house.StateGuid
                    } : null,
                ProjectSeries = house.ProjectSeries,
                ProjectType = !string.IsNullOrEmpty(house.ProjectTypeCode) || !string.IsNullOrEmpty(house.ProjectTypeGuid) ?
                        new nsiRef
                        {
                            Code = house.ProjectTypeCode,
                            GUID = house.ProjectTypeGuid
                        } : null,
                BuildingYear = house.BuildingYear.GetValueOrDefault(),
                UsedYear = house.UsedYear.GetValueOrDefault(),
                TotalWear = house.TotalWear.GetValueOrDefault(),
                FloorCount = house.FloorCount,
                Energy = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) || house.EnergyDate.HasValue ?
                    this.GetEnergyToUpdate(house) : null,
                OlsonTZ = new nsiRef
                {
                    Code = house.OlsonTZCode,
                    GUID = house.OlsonTZGuid
                },
                ResidentialSquare = Decimal.Round(house.ResidentialSquare, 2),
                CulturalHeritage = house.CulturalHeritage,
            };
        }

        private HouseBasicOMSTypeEnergy GetEnergyToCreate(RisHouse house)
        {
            return new HouseBasicOMSTypeEnergy
            {
                EnergyDate = house.EnergyDate.GetValueOrDefault(),
                EnergyCategory =
                    !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid)
                        ? new nsiRef { Code = house.EnergyCategoryCode, GUID = house.EnergyCategoryGuid }
                        : null
            };
        }

        private HouseBasicUpdateOMSTypeEnergy GetEnergyToUpdate(RisHouse house)
        {
            return new HouseBasicUpdateOMSTypeEnergy
            {
                EnergyDate = house.EnergyDate.GetValueOrDefault(),
                EnergyCategory =
                    !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid)
                        ? new nsiRef { Code = house.EnergyCategoryCode, GUID = house.EnergyCategoryGuid }
                        : null
            };
        }
    }
}
