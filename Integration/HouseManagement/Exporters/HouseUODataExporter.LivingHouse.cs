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
    /// Методы создания запроса по жилым домам
    /// </summary>
    public partial class HouseUODataExporter
    {
        private importHouseUORequestLivingHouse CreateLivingHouseRequest(RisHouse house, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(RisHouse)))
            {
                transportGuidDictionary.Add(typeof(RisHouse), new Dictionary<string, long>());
            }

            var houseTransportGuid = Guid.NewGuid().ToString();

            object houseData;

            if (house.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(house.Guid))
            {
                houseData = new importHouseUORequestLivingHouseLivingHouseToCreate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToCreate(house),
                    ResidentialHouseType = !string.IsNullOrEmpty(house.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(house.ResidentialHouseTypeGuid) ?
                                                           new nsiRef
                                                           {
                                                               Code = house.ResidentialHouseTypeCode,
                                                               GUID = house.ResidentialHouseTypeGuid
                                                           } : null,
                    TransportGUID = houseTransportGuid
                };
            }
            else
            {
                houseData = new importHouseUORequestLivingHouseLivingHouseToUpdate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToUpdate(house),
                    ResidentialHouseType = !string.IsNullOrEmpty(house.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(house.ResidentialHouseTypeGuid) ?
                                           new nsiRef
                                           {
                                               Code = house.ResidentialHouseTypeCode,
                                               GUID = house.ResidentialHouseTypeGuid
                                           } : null,
                    TransportGUID = houseTransportGuid
                };
            }

            transportGuidDictionary[typeof(RisHouse)].Add(houseTransportGuid, house.Id);
            
            return new importHouseUORequestLivingHouse
            {
                Item = houseData,
                LivingRoomToCreate = this.CreateLivingHouseLivingRoomToCreateRequest(house, transportGuidDictionary).ToArray(),
                LivingRoomToUpdate = this.CreateLivingHouseLivingRoomToUpdateRequest(house, transportGuidDictionary).ToArray()
            };
        }

        private List<importHouseUORequestLivingHouseLivingRoomToCreate> CreateLivingHouseLivingRoomToCreateRequest(
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

            var result = new List<importHouseUORequestLivingHouseLivingRoomToCreate>();

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

                result.Add(new importHouseUORequestLivingHouseLivingRoomToCreate
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

        private List<importHouseUORequestLivingHouseLivingRoomToUpdate> CreateLivingHouseLivingRoomToUpdateRequest(
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

            var result = new List<importHouseUORequestLivingHouseLivingRoomToUpdate>();

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

                result.Add(new importHouseUORequestLivingHouseLivingRoomToUpdate
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
