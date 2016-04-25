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
    /// Методы создания запроса по жилым домам
    /// </summary>
    public partial class HouseOMSDataExporter
    {
        private importHouseOMSRequestLivingHouse CreateLivingHouseRequest(
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
                houseData = new importHouseOMSRequestLivingHouseLivingHouseToCreate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToCreate(house),
                    ResidentialHouseType = !string.IsNullOrEmpty(house.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(house.ResidentialHouseTypeGuid) ?
                                                           new nsiRef
                                                           {
                                                               Code = house.ResidentialHouseTypeCode,
                                                               GUID = house.ResidentialHouseTypeGuid
                                                           } : null,
                    HouseManagementType = !string.IsNullOrEmpty(house.HouseManagementTypeCode) || !string.IsNullOrEmpty(house.HouseManagementTypeGuid) ?
                                                          new nsiRef
                                                          {
                                                              Code = house.HouseManagementTypeCode,
                                                              GUID = house.HouseManagementTypeGuid
                                                          } : null,
                    TransportGUID = houseTransportGuid
                };
            }
            else
            {
                houseData = new importHouseOMSRequestLivingHouseLivingHouseToUpdate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToUpdate(house),
                    ResidentialHouseType = !string.IsNullOrEmpty(house.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(house.ResidentialHouseTypeGuid) ?
                                                           new nsiRef
                                                           {
                                                               Code = house.ResidentialHouseTypeCode,
                                                               GUID = house.ResidentialHouseTypeGuid
                                                           } : null,
                    HouseManagementType = !string.IsNullOrEmpty(house.HouseManagementTypeCode) || !string.IsNullOrEmpty(house.HouseManagementTypeGuid) ?
                                                          new nsiRef
                                                          {
                                                              Code = house.HouseManagementTypeCode,
                                                              GUID = house.HouseManagementTypeGuid
                                                          } : null,
                    TransportGUID = houseTransportGuid
                };
            }

            transportGuidDictionary[typeof(RisHouse)].Add(houseTransportGuid, house.Id);

            return new importHouseOMSRequestLivingHouse
            {
                Item = houseData,
                LivingRoomToCreate = this.CreateLivingHouseLivingRoomToCreateRequest(house, transportGuidDictionary).ToArray(),
                LivingRoomToUpdate = this.CreateLivingHouseLivingRoomToUpdateRequest(house, transportGuidDictionary).ToArray()
            };
        }

        private List<importHouseOMSRequestLivingHouseLivingRoomToCreate> CreateLivingHouseLivingRoomToCreateRequest(
           RisHouse house,
           Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(LivingRoom)))
            {
                transportGuidDictionary.Add(typeof(LivingRoom), new Dictionary<string, long>());
            }

            var livingRoomsToCreate = this.LivingRoomList
                .Where(x => (x.House == house)
                    && (x.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseOMSRequestLivingHouseLivingRoomToCreate>();

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

                result.Add(new importHouseOMSRequestLivingHouseLivingRoomToCreate
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

        private List<importHouseOMSRequestLivingHouseLivingRoomToUpdate> CreateLivingHouseLivingRoomToUpdateRequest(
          RisHouse house,
          Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(LivingRoom)))
            {
                transportGuidDictionary.Add(typeof(LivingRoom), new Dictionary<string, long>());
            }

            var livingRoomsToUpdate = this.LivingRoomList
                .Where(x => (x.House == house)
                    && (x.Operation == RisEntityOperation.Update && !string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseOMSRequestLivingHouseLivingRoomToUpdate>();

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

                result.Add(new importHouseOMSRequestLivingHouseLivingRoomToUpdate
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
