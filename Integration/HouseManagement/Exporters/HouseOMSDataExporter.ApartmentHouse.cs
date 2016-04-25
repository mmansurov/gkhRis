namespace Bars.Gkh.Ris.Integration.HouseManagement.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.Gkh.Ris.Entities.HouseManagement;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.HouseManagementAsync;

    /// <summary>
    /// Класс экспортер данных домов для органов местного самоуправления
    /// Методы создания запроса по многоквартирным домам
    /// </summary>
    public partial class HouseOMSDataExporter
    {
        private importHouseOMSRequestApartmentHouse CreateApartmentHouseRequest(
            RisHouse house,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(RisHouse)))
            {
                transportGuidDictionary.Add(typeof(RisHouse), new Dictionary<string, long>());
            }

            var houseTransportGuid = Guid.NewGuid().ToString();

            object houseData;

            if (house.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(house.Guid))
            {
                houseData = new importHouseOMSRequestApartmentHouseApartmentHouseToCreate
                            {
                                BasicCharacteristicts = this.GetBasicCharacteristictsToCreate(house),
                                BuiltUpArea = Decimal.Round(house.BuiltUpArea.GetValueOrDefault(), 2),
                                UndergroundFloorCount = house.UndergroundFloorCount,
                                MinFloorCount = (sbyte)house.MinFloorCount.GetValueOrDefault(),
                                OverhaulYear = house.OverhaulYear.GetValueOrDefault(),
                                OverhaulFormingKind = !string.IsNullOrEmpty(house.OverhaulFormingKindCode)|| !string.IsNullOrEmpty(house.OverhaulFormingKindGuid)
                                        ? new nsiRef
                                          {
                                              Code = house.OverhaulFormingKindCode,
                                              GUID = house.OverhaulFormingKindGuid
                                          }: null,
                                NonResidentialSquare = Decimal.Round(house.NonResidentialSquare, 2),
                                HouseManagementType = !string.IsNullOrEmpty(house.HouseManagementTypeCode)|| !string.IsNullOrEmpty(house.HouseManagementTypeGuid)
                                        ? new nsiRef
                                          {
                                              Code = house.HouseManagementTypeCode,
                                              GUID = house.HouseManagementTypeGuid
                                          }: null,
                                TransportGUID = houseTransportGuid
                            };
            }
            else
            {
                houseData = new importHouseOMSRequestApartmentHouseApartmentHouseToUpdate
                            {
                                BasicCharacteristicts = this.GetBasicCharacteristictsToUpdate(house),
                                BuiltUpArea = Decimal.Round(house.BuiltUpArea.GetValueOrDefault(), 2),
                                UndergroundFloorCount = house.UndergroundFloorCount,
                                MinFloorCount = (sbyte)house.MinFloorCount.GetValueOrDefault(),
                                OverhaulYear = house.OverhaulYear.GetValueOrDefault(),
                                OverhaulFormingKind = !string.IsNullOrEmpty(house.OverhaulFormingKindCode) || !string.IsNullOrEmpty(house.OverhaulFormingKindGuid)
                                        ? new nsiRef
                                          {
                                              Code = house.OverhaulFormingKindCode,
                                              GUID = house.OverhaulFormingKindGuid
                                          }
                                        : null,
                                NonResidentialSquare = Decimal.Round(house.NonResidentialSquare, 2),
                                HouseManagementType = !string.IsNullOrEmpty(house.HouseManagementTypeCode) || !string.IsNullOrEmpty(house.HouseManagementTypeGuid)
                                        ? new nsiRef
                                          {
                                              Code = house.HouseManagementTypeCode,
                                              GUID = house.HouseManagementTypeGuid
                                          }
                                        : null,
                                TransportGUID = houseTransportGuid
                            };
            }

            transportGuidDictionary[typeof(RisHouse)].Add(houseTransportGuid, house.Id);

            return new importHouseOMSRequestApartmentHouse
            {
                Item = houseData,
                NonResidentialPremiseToCreate = this.CreateApartmentHouseNonResidentialPremiseToCreateRequests(house, transportGuidDictionary).ToArray(),
                NonResidentialPremiseToUpdate = this.CreateApartmentHouseNonResidentialPremiseToUpdateRequests(house, transportGuidDictionary).ToArray(),
                EntranceToCreate = this.CreateApartmentHouseEntranceToCreateRequests(house, transportGuidDictionary).ToArray(),
                EntranceToUpdate = this.CreateApartmentHouseEntranceToUpdateRequests(house, transportGuidDictionary).ToArray(),
                ResidentialPremises = this.CreateApartmentHouseResidentialPremisesRequests(house, transportGuidDictionary).ToArray()
            };
        }

        private List<importHouseOMSRequestApartmentHouseNonResidentialPremiseToCreate> CreateApartmentHouseNonResidentialPremiseToCreateRequests(RisHouse house, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(NonResidentialPremises)))
            {
                transportGuidDictionary.Add(typeof(NonResidentialPremises), new Dictionary<string, long>());
            }

            var premisesToCreate = this.NonResidentialPremisesList
                .Where(x => (x.ApartmentHouse.Id == house.Id)
                && (x.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseOMSRequestApartmentHouseNonResidentialPremiseToCreate>();

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

                result.Add(new importHouseOMSRequestApartmentHouseNonResidentialPremiseToCreate
                {
                    Item = noGknRelationship,
                    PremisesNum = premise.PremisesNum,
                    Purpose = !string.IsNullOrEmpty(premise.PurposeCode) || !string.IsNullOrEmpty(premise.PurposeGuid) ?
                              new nsiRef
                              {
                                  Code = premise.PurposeCode,
                                  GUID = premise.PurposeGuid
                              } : null,
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

                transportGuidDictionary[typeof(NonResidentialPremises)].Add(transportGuid, premise.Id);
            }

            return result;
        }

        private List<importHouseOMSRequestApartmentHouseNonResidentialPremiseToUpdate> CreateApartmentHouseNonResidentialPremiseToUpdateRequests(
            RisHouse house,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(NonResidentialPremises)))
            {
                transportGuidDictionary.Add(typeof(NonResidentialPremises), new Dictionary<string, long>());
            }

            var premisesToUpdate = this.NonResidentialPremisesList
                .Where(x => (x.ApartmentHouse.Id == house.Id)
                && (x.Operation == RisEntityOperation.Update && !string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseOMSRequestApartmentHouseNonResidentialPremiseToUpdate>();

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

                result.Add(new importHouseOMSRequestApartmentHouseNonResidentialPremiseToUpdate
                {
                    Item = noGknRelationship,
                    PremisesNum = premise.PremisesNum,
                    Purpose = !string.IsNullOrEmpty(premise.PurposeCode) || !string.IsNullOrEmpty(premise.PurposeGuid) ?
                              new nsiRef
                              {
                                  Code = premise.PurposeCode,
                                  GUID = premise.PurposeGuid
                              } : null,
                    Position = !string.IsNullOrEmpty(premise.PositionCode) || !string.IsNullOrEmpty(premise.PositionGuid) ?
                               new nsiRef
                               {
                                   Code = premise.PositionCode,
                                   GUID = premise.PositionGuid
                               } : null,
                    TotalArea = Decimal.Round(premise.TotalArea.GetValueOrDefault(), 2),
                    IsCommonProperty = premise.IsCommonProperty,
                    TerminationDate = premise.TerminationDate.GetValueOrDefault(),
                    PremisesGUID = premise.Guid,
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(NonResidentialPremises)].Add(transportGuid, premise.Id);
            }

            return result;
        }

        private List<importHouseOMSRequestApartmentHouseEntranceToCreate> CreateApartmentHouseEntranceToCreateRequests(RisHouse house,
           Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(RisEntrance)))
            {
                transportGuidDictionary.Add(typeof(RisEntrance), new Dictionary<string, long>());
            }

            var entrancesToCreate = this.EntranceList
                .Where(x => (x.ApartmentHouse.Id == house.Id)
                && (x.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(x.Guid))).ToList();

            var result = new List<importHouseOMSRequestApartmentHouseEntranceToCreate>();

            foreach (var entrance in entrancesToCreate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                result.Add(new importHouseOMSRequestApartmentHouseEntranceToCreate
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

        private List<importHouseOMSRequestApartmentHouseEntranceToUpdate> CreateApartmentHouseEntranceToUpdateRequests(
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

            var result = new List<importHouseOMSRequestApartmentHouseEntranceToUpdate>();

            foreach (var entrance in entrancesToUpdate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                result.Add(new importHouseOMSRequestApartmentHouseEntranceToUpdate
                {
                    EntranceNum = (sbyte)entrance.EntranceNum,
                    StoreysCount = (sbyte)entrance.StoreysCount,
                    CreationDate = entrance.CreationDate.GetValueOrDefault(),
                    TerminationDate = entrance.TerminationDate.GetValueOrDefault(),
                    EntranceGUID = entrance.Guid,
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(RisEntrance)].Add(transportGuid, entrance.Id);
            }

            return result;
        }

        private List<importHouseOMSRequestApartmentHouseResidentialPremises> CreateApartmentHouseResidentialPremisesRequests(RisHouse house, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var residentialPremises = this.ResidentialPremisesList.Where(x => x.ApartmentHouse == house).ToList();
            var result = new List<importHouseOMSRequestApartmentHouseResidentialPremises>();

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

                result.Add(new importHouseOMSRequestApartmentHouseResidentialPremises
                {
                    Item = item,
                    LivingRoomToCreate = this.CreateApartmentHouseResidentialPremisesLivingRoomToCreateRequest(residentialPremise, transportGuidDictionary).ToArray(),
                    LivingRoomToUpdate = this.CreateApartmentHouseResidentialPremisesLivingRoomToUpdateRequest(residentialPremise, transportGuidDictionary).ToArray()
                });
            }

            return result;
        }

        private importHouseOMSRequestApartmentHouseResidentialPremisesResidentialPremisesToCreate CreateApartmentHouseResidentialPremisesToCreateRequest(
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

            return new importHouseOMSRequestApartmentHouseResidentialPremisesResidentialPremisesToCreate
                   {
                       Item = noGknRelationship,
                       PremisesNum = residentialPremise.PremisesNum,
                       EntranceNum = (sbyte)residentialPremise.EntranceNum,
                       PremisesCharacteristic = !string.IsNullOrEmpty(residentialPremise.PremisesCharacteristicCode) || !string.IsNullOrEmpty(residentialPremise.PremisesCharacteristicGuid)
                               ? new nsiRef
                                 {
                                     Code = residentialPremise.PremisesCharacteristicCode,
                                     GUID = residentialPremise.PremisesCharacteristicGuid
                                 }
                               : null,
                       RoomsNum = !string.IsNullOrEmpty(residentialPremise.RoomsNumCode) || !string.IsNullOrEmpty(residentialPremise.RoomsNumGuid)
                               ? new nsiRef
                                 {
                                     Code = residentialPremise.RoomsNumCode,
                                     GUID = residentialPremise.RoomsNumGuid
                                 }
                               : null,
                       TotalArea = Decimal.Round(residentialPremise.TotalArea.GetValueOrDefault(), 2),
                       GrossArea = Decimal.Round(residentialPremise.GrossArea.GetValueOrDefault(), 2),
                       ResidentialHouseType = !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeCode)
                               || !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeGuid)
                               ? new nsiRef { Code = residentialPremise.ResidentialHouseTypeCode, GUID = residentialPremise.ResidentialHouseTypeGuid }
                               : null,
                       TransportGUID = transportGuid
                   };
        }

        private importHouseOMSRequestApartmentHouseResidentialPremisesResidentialPremisesToUpdate CreateApartmentHouseResidentialPremisesToUpdateRequest(
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

            return new importHouseOMSRequestApartmentHouseResidentialPremisesResidentialPremisesToUpdate
            {
                Item = noGknRelationship,
                PremisesNum = residentialPremise.PremisesNum,
                TerminationDate = residentialPremise.TerminationDate.GetValueOrDefault(),
                EntranceNum = (sbyte)residentialPremise.EntranceNum,
                PremisesCharacteristic = !string.IsNullOrEmpty(residentialPremise.PremisesCharacteristicCode) || !string.IsNullOrEmpty(residentialPremise.PremisesCharacteristicGuid)
                               ? new nsiRef
                               {
                                   Code = residentialPremise.PremisesCharacteristicCode,
                                   GUID = residentialPremise.PremisesCharacteristicGuid
                               }
                               : null,
                RoomsNum = !string.IsNullOrEmpty(residentialPremise.RoomsNumCode) || !string.IsNullOrEmpty(residentialPremise.RoomsNumGuid)
                               ? new nsiRef
                               {
                                   Code = residentialPremise.RoomsNumCode,
                                   GUID = residentialPremise.RoomsNumGuid
                               }
                               : null,
                TotalArea = Decimal.Round(residentialPremise.TotalArea.GetValueOrDefault(), 2),
                GrossArea = Decimal.Round(residentialPremise.GrossArea.GetValueOrDefault(), 2),
                ResidentialHouseType = !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeCode)
                               || !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeGuid)
                               ? new nsiRef { Code = residentialPremise.ResidentialHouseTypeCode, GUID = residentialPremise.ResidentialHouseTypeGuid }
                               : null,
                TransportGUID = transportGuid,
                PremisesGUID = residentialPremise.Guid
            };
        }

        private List<importHouseOMSRequestApartmentHouseResidentialPremisesLivingRoomToCreate>
            CreateApartmentHouseResidentialPremisesLivingRoomToCreateRequest(
            ResidentialPremises premise,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(LivingRoom)))
            {
                transportGuidDictionary.Add(typeof(LivingRoom), new Dictionary<string, long>());
            }

            var livingRoomsToCreate = this.LivingRoomList
                .Where(x => (x.ResidentialPremises == premise) 
                && (x.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseOMSRequestApartmentHouseResidentialPremisesLivingRoomToCreate>();

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

                result.Add(new importHouseOMSRequestApartmentHouseResidentialPremisesLivingRoomToCreate
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

        private List<importHouseOMSRequestApartmentHouseResidentialPremisesLivingRoomToUpdate>
            CreateApartmentHouseResidentialPremisesLivingRoomToUpdateRequest(
            ResidentialPremises premise,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(LivingRoom)))
            {
                transportGuidDictionary.Add(typeof(LivingRoom), new Dictionary<string, long>());
            }

            var livingRoomsToUpdate = this.LivingRoomList
                .Where(x => (x.ResidentialPremises == premise) 
                && (x.Operation == RisEntityOperation.Update && !string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseOMSRequestApartmentHouseResidentialPremisesLivingRoomToUpdate>();

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

                result.Add(new importHouseOMSRequestApartmentHouseResidentialPremisesLivingRoomToUpdate
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
    }
}
