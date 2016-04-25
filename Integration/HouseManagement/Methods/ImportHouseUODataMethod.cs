namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using Entities.HouseManagement;
    using Enums;
    using Ris.HouseManagement;

    /// <summary>
    /// Класс экспортер данных домов для полномочия УО
    /// </summary>
    public class ImportHouseUODataMethod : ImportHouseBaseMethod<importHouseUORequest>
    {
        /// <summary>
        /// Код метода
        /// </summary>
        public override string Code
        {
            get
            {
                return "importHouseUOData";
            }
        }

        /// <summary>
        /// Наименование метода
        /// </summary>
        public override string Name
        {
            get
            {
                return "Импорт сведений о доме для полномочия УО";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 28;
            }
        }

        /// <summary>
        /// Метод получения списка домов для импорта
        /// </summary>
        /// <returns>Список объектов для импорта</returns>
        protected override List<RisHouse> GetMainList()
        {
            var houseDomain = this.Container.ResolveDomain<RisHouse>();

            //            Для реализации выборки домов для отправки необходимо:
            //            Получить контрагента (поставщика информации) >> его актуальные договора >> дома по договорам
            //              1) пока не реализовано определение контрагента,
            //              2) таблицы связи между договорами и домами (сущность жкх ManOrgContractRealityObject) пока нет в проекте Ris
            //              соответственно, пока выгружаются все дома - необходима доработка


            //TODO Сделать выборку домов в разрезе контрагента, актуальных договоров

            try
            {
                return houseDomain.GetAll().Take(50).ToList();
                //return houseDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(houseDomain);
            }
        }

        /// <summary>
        /// Проверка дома
        /// </summary>
        /// <param name="item">Дом</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(RisHouse item)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(item.FiasHouseGuid))
            {
                messages.Append("FIASHouseGuid ");
            }

            if (!item.TotalSquare.HasValue || Decimal.Round(item.TotalSquare.Value, 1) < 0.1m)
            {
                messages.Append("TotalSquare ");
            }

            if (item.BuildingYear < 1600)
            {
                messages.Append("BuildingYear ");
            }

            if (item.UsedYear < 1600)
            {
                messages.Append("UsedYear ");
            }

            if (string.IsNullOrEmpty(item.FloorCount))
            {
                messages.Append("FloorCount ");
            }

            if (string.IsNullOrEmpty(item.ProjectTypeCode) && !string.IsNullOrEmpty(item.ProjectTypeGuid))
            {
                messages.Append("ProjectTypeCode ");
            }

            if (!string.IsNullOrEmpty(item.ProjectTypeCode) && string.IsNullOrEmpty(item.ProjectTypeGuid))
            {
                messages.Append("ProjectTypeGuid ");
            }

            if (string.IsNullOrEmpty(item.EnergyCategoryCode) && !string.IsNullOrEmpty(item.EnergyCategoryGuid))
            {
                messages.Append("EnergyCategoryCode ");
            }

            if (!string.IsNullOrEmpty(item.EnergyCategoryCode) && string.IsNullOrEmpty(item.EnergyCategoryGuid))
            {
                messages.Append("EnergyCategoryGuid ");
            }

            if (string.IsNullOrEmpty(item.OktmoCode) && !string.IsNullOrEmpty(item.OktmoName))
            {
                messages.Append("OktmoCode ");
            }

            if (string.IsNullOrEmpty(item.OlsonTZCode) || string.IsNullOrEmpty(item.OlsonTZGuid))
            {
                messages.Append("OlsonTZ ");
            }

            if (Decimal.Round(item.ResidentialSquare, 2) < 0.01m)
            {
                messages.Append("ResidentialSquare ");
            }

            if (item.HouseType == HouseType.Apartment)
            {
                if (string.IsNullOrEmpty(item.StateCode) && !string.IsNullOrEmpty(item.StateGuid))
                {
                    messages.Append("StateCode ");
                }

                if (!string.IsNullOrEmpty(item.StateCode) && string.IsNullOrEmpty(item.StateGuid))
                {
                    messages.Append("StateGuid ");
                }

                if (string.IsNullOrEmpty(item.OverhaulFormingKindCode)
                && !string.IsNullOrEmpty(item.OverhaulFormingKindGuid))
                {
                    messages.Append("OverhaulFormingKindCode ");
                }

                if (!string.IsNullOrEmpty(item.OverhaulFormingKindCode)
                    && string.IsNullOrEmpty(item.OverhaulFormingKindGuid))
                {
                    messages.Append("OverhaulFormingKindGuid ");
                }

                if (Decimal.Round(item.NonResidentialSquare, 2) < 0.01m)
                {
                    messages.Append("NonResidentialSquare ");
                }

                if (Decimal.Round(item.BuiltUpArea.GetValueOrDefault(), 2) < 0.01m)
                {
                    messages.Append("BuiltUpArea ");
                }

                if (string.IsNullOrEmpty(item.UndergroundFloorCount))
                {
                    messages.Append("UndergroundFloorCount ");
                }
            }

            if (item.HouseType == HouseType.Living)
            {
                if (string.IsNullOrEmpty(item.StateCode) || string.IsNullOrEmpty(item.StateGuid))
                {
                    messages.Append("State ");
                }

                if (string.IsNullOrEmpty(item.ResidentialHouseTypeCode)
                    && !string.IsNullOrEmpty(item.ResidentialHouseTypeGuid))
                {
                    messages.Append("ResidentialHouseTypeCode ");
                }

                if (!string.IsNullOrEmpty(item.ResidentialHouseTypeCode)
                    && string.IsNullOrEmpty(item.ResidentialHouseTypeGuid))
                {
                    messages.Append("ResidentialHouseTypeGuid ");
                }
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта (в текущем методе в списке 1 объект)</param>
        /// <returns>Объект запроса</returns>
        protected override importHouseUORequest GetRequestObject(
            IEnumerable<RisHouse> listForImport)
        {
            var house = listForImport.First();
            object item = null;

            switch (house.HouseType)
            {
                case HouseType.Apartment:
                    item = this.CreateApartmentHouseRequest(house);
                    break;
                case HouseType.Living:
                    item = this.CreateLivingHouseRequest(house);
                    break;
            }

            return new importHouseUORequest { Item = item };
        }

        /// <summary>
        /// Получить ответ от сервиса.
        /// </summary>
        /// <param name="request">Объект запроса</param>
        /// <returns>Ответ от сервиса</returns>
        protected override ImportResult GetRequestResult(importHouseUORequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();
            var requestHeader = this.RequestHeader;

            if (soapClient != null)
            {
                soapClient.importHouseUOData(requestHeader, request, out result);
            }

            return result;
        }

        private importHouseUORequestApartmentHouse CreateApartmentHouseRequest(RisHouse house)
        {
            object gknRelationship;

            if (string.IsNullOrEmpty(house.CadastralNumber))
            {
                gknRelationship = true;
            }
            else
            {
                gknRelationship = house.CadastralNumber;
            }

            var houseTransportGuid = Guid.NewGuid().ToString();

            var basicCharacteristics = new HouseBasicUOType
            {
                Item = gknRelationship,
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
                TotalWear = Decimal.Round(house.TotalWear.GetValueOrDefault(),2),
                FloorCount = house.FloorCount,
                Energy = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) || house.EnergyDate.HasValue ?
                    new HouseBasicUOTypeEnergy
                    {
                        EnergyDate = house.EnergyDate.GetValueOrDefault(),
                        EnergyCategory = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) ?
                                              new nsiRef
                                              {
                                                  Code = house.EnergyCategoryCode,
                                                  GUID = house.EnergyCategoryGuid
                                              } : null
                    } : null,
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

            var houseToCreate = new importHouseUORequestApartmentHouseApartmentHouseToCreate
            {
                BasicCharacteristicts = basicCharacteristics,
                BuiltUpArea = Decimal.Round(house.BuiltUpArea.GetValueOrDefault(), 2),
                UndergroundFloorCount = house.UndergroundFloorCount,
                MinFloorCount = (sbyte)house.MinFloorCount.GetValueOrDefault(),
                OverhaulYear = house.OverhaulYear.GetValueOrDefault(),
                OverhaulFormingKind = !string.IsNullOrEmpty(house.OverhaulFormingKindCode) || !string.IsNullOrEmpty(house.OverhaulFormingKindGuid) ?
                                             new nsiRef
                                             {
                                                 Code = house.OverhaulFormingKindCode,
                                                 GUID = house.OverhaulFormingKindGuid
                                             } : null,
                NonResidentialSquare = Decimal.Round(house.NonResidentialSquare, 2),
                TransportGUID = houseTransportGuid
            };

            this.housesByTransportGuid.Add(houseTransportGuid, house);

            return new importHouseUORequestApartmentHouse
            {
                Item = houseToCreate,
                NonResidentialPremiseToCreate = this.CreateApartmentHouseNonResidentialPremiseToCreateRequests(house).ToArray(),
                EntranceToCreate = this.CreateApartmentHouseEntranceToCreateRequests(house).ToArray(),
                ResidentialPremises = this.CreateApartmentHouseResidentialPremisesRequests(house).ToArray()
            };
        }

        private List<importHouseUORequestApartmentHouseNonResidentialPremiseToCreate> CreateApartmentHouseNonResidentialPremiseToCreateRequests(RisHouse house)
        {
            var premises = this.nonResidentialPremisesList.Where(x => x.ApartmentHouse == house).ToList();
            var result = new List<importHouseUORequestApartmentHouseNonResidentialPremiseToCreate>();
            var transportGuid = Guid.NewGuid().ToString();

            this.ClearObjectList(premises, this.CheckNonResidentialPremise);

            foreach (var premise in premises)
            {
                object gknRelationship;

                if (string.IsNullOrEmpty(premise.CadastralNumber))
                {
                    gknRelationship = true;
                }
                else
                {
                    gknRelationship = house.CadastralNumber;
                }

                result.Add(new importHouseUORequestApartmentHouseNonResidentialPremiseToCreate
                {
                    Item = gknRelationship,
                    PremisesNum = premise.PremisesNum,
                    //Floor = premise.Floor,
                    Purpose = new nsiRef
                              {
                                  Code = premise.PurposeCode,
                                  GUID = premise.PurposeGuid
                              },
                    Position = !string.IsNullOrEmpty(premise.PositionCode) || !string.IsNullOrEmpty(premise.PositionGuid) ?
                               new nsiRef
                               {
                                   Code = premise.PositionCode,
                                   GUID = premise.PositionGuid
                               } : null,
                    TotalArea = Decimal.Round(premise.TotalArea.GetValueOrDefault(), 2),
                    IsCommonProperty = premise.IsCommonProperty,
                    TransportGUID = transportGuid
                });

                this.nonResidentialPremisesByTransportGuid.Add(transportGuid, premise);
            }

            return result;
        }

        private List<importHouseUORequestApartmentHouseEntranceToCreate> CreateApartmentHouseEntranceToCreateRequests(
            RisHouse house)
        {
            var entrances = this.entranceList.Where(x => x.ApartmentHouse == house).ToList();
            var result = new List<importHouseUORequestApartmentHouseEntranceToCreate>();
            var transportGuid = Guid.NewGuid().ToString();

            this.ClearObjectList(entrances, this.CheckEntrance);

            foreach (var entrance in entrances)
            {
                result.Add(new importHouseUORequestApartmentHouseEntranceToCreate
                {
                    EntranceNum = (sbyte)entrance.EntranceNum,
                    StoreysCount = (sbyte)entrance.StoreysCount,
                    CreationDate = entrance.CreationDate.GetValueOrDefault(),
                    TransportGUID = transportGuid
                });

                this.entrancesByTransportGuid.Add(transportGuid, entrance);
            }

            return result;
        }

        private List<importHouseUORequestApartmentHouseResidentialPremises> CreateApartmentHouseResidentialPremisesRequests(RisHouse house)
        {
            var residentialPremises = this.residentialPremisesList.Where(x => x.ApartmentHouse == house).ToList();
            var result = new List<importHouseUORequestApartmentHouseResidentialPremises>();
            var transportGuid = Guid.NewGuid().ToString();

            this.ClearObjectList(residentialPremises, this.CheckResidentialPremise);

            foreach (var residentialPremise in residentialPremises)
            {
                object gknRelationship;

                if (string.IsNullOrEmpty(residentialPremise.CadastralNumber))
                {
                    gknRelationship = true;
                }
                else
                {
                    gknRelationship = house.CadastralNumber;
                }

                result.Add(new importHouseUORequestApartmentHouseResidentialPremises
                {
                    Item = new importHouseUORequestApartmentHouseResidentialPremisesResidentialPremisesToCreate
                    {
                        Item = gknRelationship,
                        PremisesNum = residentialPremise.PremisesNum,
                        //Floor = residentialPremise.Floor,
                        EntranceNum = (sbyte)residentialPremise.EntranceNum,
                        PremisesCharacteristic =
                            new nsiRef
                            {
                                Code = residentialPremise.PremisesCharacteristicCode,
                                GUID = residentialPremise.PremisesCharacteristicGuid
                            },
                        RoomsNum =
                            new nsiRef
                            {
                                Code = residentialPremise.RoomsNumCode,
                                GUID = residentialPremise.RoomsNumGuid
                            },
                        TotalArea = Decimal.Round(residentialPremise.TotalArea.GetValueOrDefault(),2),
                        GrossArea = Decimal.Round(residentialPremise.GrossArea.GetValueOrDefault(),2),
                        ResidentialHouseType = !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeGuid) ?
                                                                new nsiRef
                                                                {
                                                                    Code = residentialPremise.ResidentialHouseTypeCode,
                                                                    GUID = residentialPremise.ResidentialHouseTypeGuid
                                                                } : null,
                        TransportGUID = transportGuid
                    },
                    LivingRoomToCreate = this.CreateApartmentHouseResidentialPremisesLivingRoomToCreateRequest(residentialPremise).ToArray()
                });

                this.residentialPremisesByTransportGuid.Add(transportGuid, residentialPremise);
            }

            return result;
        }

        private List<importHouseUORequestApartmentHouseResidentialPremisesLivingRoomToCreate>
           CreateApartmentHouseResidentialPremisesLivingRoomToCreateRequest(ResidentialPremises premise)
        {
            var livingRooms = this.livingRoomList.Where(x => x.ResidentialPremises == premise).ToList();
            var transportGuid = Guid.NewGuid().ToString();
            var result = new List<importHouseUORequestApartmentHouseResidentialPremisesLivingRoomToCreate>();

            this.ClearObjectList(livingRooms, this.CheckLivingRoom);

            foreach (var livingRoom in livingRooms)
            {
                object gknRelationship;

                if (string.IsNullOrEmpty(livingRoom.CadastralNumber))
                {
                    gknRelationship = true;
                }
                else
                {
                    gknRelationship = livingRoom.CadastralNumber;
                }

                result.Add(new importHouseUORequestApartmentHouseResidentialPremisesLivingRoomToCreate
                {
                    Item = gknRelationship,
                    RoomNumber = livingRoom.RoomNumber,
                    Square = livingRoom.Square.GetValueOrDefault(),
                    //Floor = livingRoom.Floor,
                    TransportGUID = transportGuid
                });

                this.livingRoomsByTransportGuid.Add(transportGuid, livingRoom);
            }

            return result;
        }

        private importHouseUORequestLivingHouse CreateLivingHouseRequest(RisHouse house)
        {
            object gknRelationship;

            if (string.IsNullOrEmpty(house.CadastralNumber))
            {
                gknRelationship = true;
            }
            else
            {
                gknRelationship = house.CadastralNumber;
            }

            var houseTransportGuid = Guid.NewGuid().ToString();

            var basicCharacteristicts = new HouseBasicUOType
            {
                Item = gknRelationship,
                FIASHouseGuid = house.FiasHouseGuid,
                TotalSquare = Decimal.Round(house.TotalSquare.GetValueOrDefault(), 1),
                State = new nsiRef
                              {
                                   Code = house.StateCode,
                                   GUID = house.StateGuid
                              },
                //InnerWallMaterial = 
                ProjectSeries = house.ProjectSeries,
                ProjectType = !string.IsNullOrEmpty(house.ProjectTypeCode) || !string.IsNullOrEmpty(house.ProjectTypeGuid) ?
                                                    new nsiRef
                                                    {
                                                        Code = house.ProjectTypeCode,
                                                        GUID = house.ProjectTypeGuid
                                                    } : null,
                BuildingYear = house.BuildingYear.GetValueOrDefault(),
                UsedYear = house.UsedYear.GetValueOrDefault(),
                TotalWear = Decimal.Round(house.TotalWear.GetValueOrDefault(), 2),
                FloorCount = house.FloorCount,
                Energy = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) || house.EnergyDate.HasValue ?
                                                    new HouseBasicUOTypeEnergy
                                                    {
                                                        EnergyDate = house.EnergyDate.GetValueOrDefault(),
                                                        EnergyCategory = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) ?
                                                                                new nsiRef
                                                                                {
                                                                                    Code = house.EnergyCategoryCode,
                                                                                    GUID = house.EnergyCategoryGuid
                                                                                } : null,
                                                        //ConfirmDoc = 
                                                    } : null,
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

            var houseToCreate = new importHouseUORequestLivingHouseLivingHouseToCreate
            {
                BasicCharacteristicts = basicCharacteristicts,
                ResidentialHouseType = !string.IsNullOrEmpty(house.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(house.ResidentialHouseTypeGuid) ?
                                                           new nsiRef
                                                           {
                                                               Code = house.ResidentialHouseTypeCode,
                                                               GUID = house.ResidentialHouseTypeGuid
                                                           } : null,
                TransportGUID = houseTransportGuid
            };

            this.housesByTransportGuid.Add(houseTransportGuid, house);

            return new importHouseUORequestLivingHouse
            {
                Item = houseToCreate,
                LivingRoomToCreate = this.CreateLivingHouseLivingRoomToCreateRequest(house).ToArray()
            };
        }

        private List<importHouseUORequestLivingHouseLivingRoomToCreate> CreateLivingHouseLivingRoomToCreateRequest(
           RisHouse house)
        {
            var livingRooms = this.livingRoomList.Where(x => x.House == house).ToList();
            var result = new List<importHouseUORequestLivingHouseLivingRoomToCreate>();
            var transportGuid = Guid.NewGuid().ToString();

            this.ClearObjectList(livingRooms, this.CheckLivingRoom);

            foreach (var livingRoom in livingRooms)
            {
                object gknRelationship;

                if (string.IsNullOrEmpty(livingRoom.CadastralNumber))
                {
                    gknRelationship = true;
                }
                else
                {
                    gknRelationship = livingRoom.CadastralNumber;
                }

                result.Add(new importHouseUORequestLivingHouseLivingRoomToCreate
                {
                    Item = gknRelationship,
                    RoomNumber = livingRoom.RoomNumber,
                    Square = livingRoom.Square.GetValueOrDefault(),
                    //Floor = livingRoom.Floor,
                    TransportGUID = transportGuid
                });

                this.livingRoomsByTransportGuid.Add(transportGuid, livingRoom);
            }

            return result;
        }

        private CheckingResult CheckNonResidentialPremise(NonResidentialPremises premise)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(premise.PremisesNum))
            {
                messages.Append("PremisesNum ");
            }

            if (string.IsNullOrEmpty(premise.Floor))
            {
                messages.Append("Floor ");
            }

            if (string.IsNullOrEmpty(premise.PurposeCode) || string.IsNullOrEmpty(premise.PurposeGuid))
            {
                messages.Append("Purpose ");
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

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        private CheckingResult CheckEntrance(RisEntrance entrance)
        {
            StringBuilder messages = new StringBuilder();

            if (!entrance.EntranceNum.HasValue)
            {
                messages.Append("EntranceNum ");
            }

            if (entrance.StoreysCount < 1)
            {
                messages.Append("StoreysCount ");
            }

            if (!entrance.CreationDate.HasValue)
            {
                messages.Append("CreationDate ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        private CheckingResult CheckResidentialPremise(ResidentialPremises premise)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(premise.PremisesNum))
            {
                messages.Append("PremisesNum ");
            }

            if (string.IsNullOrEmpty(premise.Floor))
            {
                messages.Append("Floor ");
            }

            if (!premise.EntranceNum.HasValue)
            {
                messages.Append("EntranceNum ");
            }

            if (string.IsNullOrEmpty(premise.PremisesCharacteristicCode)
                || string.IsNullOrEmpty(premise.PremisesCharacteristicGuid))
            {
                messages.Append("PremisesCharacteristic ");
            }

            if (string.IsNullOrEmpty(premise.RoomsNumCode) || string.IsNullOrEmpty(premise.RoomsNumGuid))
            {
                messages.Append("RoomsNum ");
            }

            if (!premise.TotalArea.HasValue)
            {
                messages.Append("TotalArea ");
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

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        private CheckingResult CheckLivingRoom(LivingRoom livingRoom)
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

            if (string.IsNullOrEmpty(livingRoom.Floor))
            {
                messages.Append("Floor ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }
    }
}