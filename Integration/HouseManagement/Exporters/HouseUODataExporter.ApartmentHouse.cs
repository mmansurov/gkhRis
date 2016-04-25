namespace Bars.Gkh.Ris.Integration.HouseManagement.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.Gkh.Ris.Entities.HouseManagement;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.HouseManagementAsync;

    /// <summary>
    /// Класс экспортер данных домов для управляющих организаций
    /// Методы создания запроса по многоквартирным домам
    /// </summary>
    public partial class HouseUODataExporter
    {
        private importHouseUORequestApartmentHouse CreateApartmentHouseRequest(RisHouse house, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(RisHouse)))
            {
                transportGuidDictionary.Add(typeof(RisHouse), new Dictionary<string, long>());
            }

            var houseTransportGuid = Guid.NewGuid().ToString();

            object houseData;

            if (house.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(house.Guid))
            {
                houseData = new importHouseUORequestApartmentHouseApartmentHouseToCreate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToCreate(house),
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
            }
            else
            {
                houseData = new importHouseUORequestApartmentHouseApartmentHouseToUpdate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToUpdate(house),
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
            }

            transportGuidDictionary[typeof(RisHouse)].Add(houseTransportGuid, house.Id);

            return new importHouseUORequestApartmentHouse
            {
                Item = houseData,
                NonResidentialPremiseToCreate = this.CreateApartmentHouseNonResidentialPremiseToCreateRequests(house, transportGuidDictionary).ToArray(),
                NonResidentialPremiseToUpdate = this.CreateApartmentHouseNonResidentialPremiseToUpdateRequests(house, transportGuidDictionary).ToArray(),
                EntranceToCreate = this.CreateApartmentHouseEntranceToCreateRequests(house, transportGuidDictionary).ToArray(),
                EntranceToUpdate = this.CreateApartmentHouseEntranceToUpdateRequests(house, transportGuidDictionary).ToArray(),
                ResidentialPremises = this.CreateApartmentHouseResidentialPremisesRequests(house, transportGuidDictionary).ToArray()
            };
        }

        private List<importHouseUORequestApartmentHouseNonResidentialPremiseToCreate> CreateApartmentHouseNonResidentialPremiseToCreateRequests(RisHouse house, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(NonResidentialPremises)))
            {
                transportGuidDictionary.Add(typeof(NonResidentialPremises), new Dictionary<string, long>());
            }

            var premisesToCreate = this.NonResidentialPremisesList
                .Where(x => (x.ApartmentHouse.Id == house.Id)
                && (x.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseUORequestApartmentHouseNonResidentialPremiseToCreate>();

            foreach (var premise in premisesToCreate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                object noGknRelationship;

                if (string.IsNullOrEmpty(premise.CadastralNumber))
                {
                    noGknRelationship = true;
                }
                else
                {
                    noGknRelationship = premise.CadastralNumber;
                }

                result.Add(new importHouseUORequestApartmentHouseNonResidentialPremiseToCreate
                {
                    Item = noGknRelationship,
                    PremisesNum = premise.PremisesNum,
                    Purpose = new nsiRef
                    {
                        Code = premise.PurposeCode,
                        GUID = premise.PurposeGuid
                    },
                    Position = new nsiRef
                    {
                        Code = premise.PositionCode,
                        GUID = premise.PositionGuid
                    },
                    TotalArea = Decimal.Round(premise.TotalArea.GetValueOrDefault(), 2),
                    IsCommonProperty = premise.IsCommonProperty,
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(NonResidentialPremises)].Add(transportGuid, premise.Id);
            }

            return result;
        }

        private List<importHouseUORequestApartmentHouseNonResidentialPremiseToUpdate> CreateApartmentHouseNonResidentialPremiseToUpdateRequests(RisHouse house, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(NonResidentialPremises)))
            {
                transportGuidDictionary.Add(typeof(NonResidentialPremises), new Dictionary<string, long>());
            }

            var premisesToUpdate = this.NonResidentialPremisesList
                .Where(x => (x.ApartmentHouse.Id == house.Id)
                && (x.Operation == RisEntityOperation.Update && !string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseUORequestApartmentHouseNonResidentialPremiseToUpdate>();

            foreach (var premise in premisesToUpdate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                object noGknRelationship;

                if (string.IsNullOrEmpty(premise.CadastralNumber))
                {
                    noGknRelationship = true;
                }
                else
                {
                    noGknRelationship = premise.CadastralNumber;
                }

                result.Add(new importHouseUORequestApartmentHouseNonResidentialPremiseToUpdate
                {
                    Item = noGknRelationship,
                    PremisesNum = premise.PremisesNum,
                    TerminationDate = premise.TerminationDate.GetValueOrDefault(),
                    Purpose = new nsiRef
                    {
                        Code = premise.PurposeCode,
                        GUID = premise.PurposeGuid
                    },
                    Position = new nsiRef
                    {
                        Code = premise.PositionCode,
                        GUID = premise.PositionGuid
                    },
                    TotalArea = Decimal.Round(premise.TotalArea.GetValueOrDefault(), 2),
                    IsCommonProperty = premise.IsCommonProperty,
                    TransportGUID = transportGuid,
                    PremisesGUID = premise.Guid
                });

                transportGuidDictionary[typeof(NonResidentialPremises)].Add(transportGuid, premise.Id);
            }

            return result;
        }

        private List<importHouseUORequestApartmentHouseResidentialPremises> CreateApartmentHouseResidentialPremisesRequests(RisHouse house, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var residentialPremises = this.ResidentialPremisesList.Where(x => x.ApartmentHouse == house).ToList();
            var result = new List<importHouseUORequestApartmentHouseResidentialPremises>();

            if (!transportGuidDictionary.ContainsKey(typeof(ResidentialPremises)))
            {
                transportGuidDictionary.Add(typeof(ResidentialPremises), new Dictionary<string, long>());
            }

            foreach (var residentialPremise in residentialPremises)
            {
                GKNType item;

                if (residentialPremise.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(residentialPremise.Guid))
                {
                    item = this.CreateApartmentHouseResidentialPremisesToCreateRequest(residentialPremise, transportGuidDictionary);
                }
                else
                {
                    item = this.CreateApartmentHouseResidentialPremisesToUpdateRequest(residentialPremise, transportGuidDictionary);
                }

                result.Add(new importHouseUORequestApartmentHouseResidentialPremises
                {
                    Item = item,
                    LivingRoomToCreate = this.CreateApartmentHouseResidentialPremisesLivingRoomToCreateRequest(residentialPremise, transportGuidDictionary).ToArray(),
                    LivingRoomToUpdate = this.CreateApartmentHouseResidentialPremisesLivingRoomToUpdateRequest(residentialPremise, transportGuidDictionary).ToArray()
                });
            }

            return result;
        }

        private importHouseUORequestApartmentHouseResidentialPremisesResidentialPremisesToCreate CreateApartmentHouseResidentialPremisesToCreateRequest(
            ResidentialPremises residentialPremise,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var transportGuid = Guid.NewGuid().ToString();

            object noGknRelationship;

            if (string.IsNullOrEmpty(residentialPremise.CadastralNumber))
            {
                noGknRelationship = true;
            }
            else
            {
                noGknRelationship = residentialPremise.CadastralNumber;
            }

            transportGuidDictionary[typeof(ResidentialPremises)].Add(transportGuid, residentialPremise.Id);

            return new importHouseUORequestApartmentHouseResidentialPremisesResidentialPremisesToCreate
            {
                Item = noGknRelationship,
                PremisesNum = residentialPremise.PremisesNum,
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
                TotalArea = Decimal.Round(residentialPremise.TotalArea.GetValueOrDefault(), 2),
                GrossArea = Decimal.Round(residentialPremise.GrossArea.GetValueOrDefault(), 2),
                ResidentialHouseType = !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeGuid) ?
                                                            new nsiRef
                                                            {
                                                                Code = residentialPremise.ResidentialHouseTypeCode,
                                                                GUID = residentialPremise.ResidentialHouseTypeGuid
                                                            } : null,
                TransportGUID = transportGuid
            };
        }

        private importHouseUORequestApartmentHouseResidentialPremisesResidentialPremisesToUpdate CreateApartmentHouseResidentialPremisesToUpdateRequest(
            ResidentialPremises residentialPremise,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var transportGuid = Guid.NewGuid().ToString();

            object noGknRelationship;

            if (string.IsNullOrEmpty(residentialPremise.CadastralNumber))
            {
                noGknRelationship = true;
            }
            else
            {
                noGknRelationship = residentialPremise.CadastralNumber;
            }

            transportGuidDictionary[typeof(ResidentialPremises)].Add(transportGuid, residentialPremise.Id);

            return new importHouseUORequestApartmentHouseResidentialPremisesResidentialPremisesToUpdate
            {
                Item = noGknRelationship,
                PremisesNum = residentialPremise.PremisesNum,
                TerminationDate = residentialPremise.TerminationDate.GetValueOrDefault(),
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
                TotalArea = Decimal.Round(residentialPremise.TotalArea.GetValueOrDefault(), 2),
                GrossArea = Decimal.Round(residentialPremise.GrossArea.GetValueOrDefault(), 2),
                ResidentialHouseType = !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeGuid) ?
                                                            new nsiRef
                                                            {
                                                                Code = residentialPremise.ResidentialHouseTypeCode,
                                                                GUID = residentialPremise.ResidentialHouseTypeGuid
                                                            } : null,
                TransportGUID = transportGuid,
                PremisesGUID = residentialPremise.Guid
            };
        }

        private List<importHouseUORequestApartmentHouseResidentialPremisesLivingRoomToCreate>
           CreateApartmentHouseResidentialPremisesLivingRoomToCreateRequest(
            ResidentialPremises premise,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(LivingRoom)))
            {
                transportGuidDictionary.Add(typeof(LivingRoom), new Dictionary<string, long>());
            }

            var livingRoomsToCreate = this.LivingRoomList
                .Where(x => (x.ResidentialPremises == premise) && (x.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseUORequestApartmentHouseResidentialPremisesLivingRoomToCreate>();

            foreach (var livingRoom in livingRoomsToCreate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                object noGknRelationship;

                if (string.IsNullOrEmpty(livingRoom.CadastralNumber))
                {
                    noGknRelationship = true;
                }
                else
                {
                    noGknRelationship = livingRoom.CadastralNumber;
                }

                result.Add(new importHouseUORequestApartmentHouseResidentialPremisesLivingRoomToCreate
                {
                    Item = noGknRelationship,
                    RoomNumber = livingRoom.RoomNumber,
                    Square = livingRoom.Square.GetValueOrDefault(),
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(LivingRoom)].Add(transportGuid, livingRoom.Id);
            }

            return result;
        }

        private List<importHouseUORequestApartmentHouseResidentialPremisesLivingRoomToUpdate>
           CreateApartmentHouseResidentialPremisesLivingRoomToUpdateRequest(
            ResidentialPremises premise,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(LivingRoom)))
            {
                transportGuidDictionary.Add(typeof(LivingRoom), new Dictionary<string, long>());
            }

            var livingRoomsToUpdate = this.LivingRoomList
                .Where(x => (x.ResidentialPremises == premise) && (x.Operation == RisEntityOperation.Update && !string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseUORequestApartmentHouseResidentialPremisesLivingRoomToUpdate>();

            foreach (var livingRoom in livingRoomsToUpdate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                object noGknRelationship;

                if (string.IsNullOrEmpty(livingRoom.CadastralNumber))
                {
                    noGknRelationship = true;
                }
                else
                {
                    noGknRelationship = livingRoom.CadastralNumber;
                }

                result.Add(new importHouseUORequestApartmentHouseResidentialPremisesLivingRoomToUpdate
                {
                    Item = noGknRelationship,
                    RoomNumber = livingRoom.RoomNumber,
                    Square = livingRoom.Square.GetValueOrDefault(),
                    TerminationDate = livingRoom.TerminationDate.GetValueOrDefault(),
                    TransportGUID = transportGuid,
                    LivingRoomGUID = livingRoom.Guid
                });

                transportGuidDictionary[typeof(LivingRoom)].Add(transportGuid, livingRoom.Id);
            }

            return result;
        }

        private List<importHouseUORequestApartmentHouseEntranceToCreate> CreateApartmentHouseEntranceToCreateRequests(
           RisHouse house,
           Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(RisEntrance)))
            {
                transportGuidDictionary.Add(typeof(RisEntrance), new Dictionary<string, long>());
            }

            var entrancesToCreate = this.EntranceList
                .Where(x => (x.ApartmentHouse.Id == house.Id)
                && (x.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(x.Guid))).ToList();

            var result = new List<importHouseUORequestApartmentHouseEntranceToCreate>();

            foreach (var entrance in entrancesToCreate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                result.Add(new importHouseUORequestApartmentHouseEntranceToCreate
                {
                    EntranceNum = (sbyte)entrance.EntranceNum,
                    StoreysCount = (sbyte)entrance.StoreysCount,
                    CreationDate = entrance.CreationDate.GetValueOrDefault(),
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(RisEntrance)].Add(transportGuid, entrance.Id);
            }

            return result;
        }

        private List<importHouseUORequestApartmentHouseEntranceToUpdate> CreateApartmentHouseEntranceToUpdateRequests(
          RisHouse house,
          Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(RisEntrance)))
            {
                transportGuidDictionary.Add(typeof(RisEntrance), new Dictionary<string, long>());
            }

            var entrancesToUpdate = this.EntranceList
                .Where(x => (x.ApartmentHouse.Id == house.Id)
                && (x.Operation == RisEntityOperation.Update && !string.IsNullOrEmpty(x.Guid))).ToList();

            var result = new List<importHouseUORequestApartmentHouseEntranceToUpdate>();

            foreach (var entrance in entrancesToUpdate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                result.Add(new importHouseUORequestApartmentHouseEntranceToUpdate
                {
                    EntranceNum = (sbyte)entrance.EntranceNum,
                    StoreysCount = (sbyte)entrance.StoreysCount,
                    CreationDate = entrance.CreationDate.GetValueOrDefault(),
                    TerminationDate = entrance.TerminationDate.GetValueOrDefault(),
                    TransportGUID = transportGuid,
                    EntranceGUID = entrance.Guid
                });

                transportGuidDictionary[typeof(RisEntrance)].Add(transportGuid, entrance.Id);
            }

            return result;
        }
    }
}
