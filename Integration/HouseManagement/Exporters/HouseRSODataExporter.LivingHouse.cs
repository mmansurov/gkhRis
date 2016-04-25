namespace Bars.Gkh.Ris.Integration.HouseManagement.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.Gkh.Ris.Entities.HouseManagement;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.HouseManagementAsync;

    /// <summary>
    /// Класс экспортер данных домов для ресурсоснабжающих организаций
    /// Методы создания запроса по жилым домам
    /// </summary>
    public partial class HouseRSODataExporter
    {
        private importHouseRSORequestLivingHouse CreateLivingHouseRequest(
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
                houseData = new importHouseRSORequestLivingHouseLivingHouseToCreate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToCreate(house),
                    TransportGUID = houseTransportGuid
                };
            }
            else
            {
                houseData = new importHouseRSORequestLivingHouseLivingHouseToUpdate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToUpdate(house),
                    TransportGUID = houseTransportGuid
                };
            }

            transportGuidDictionary[typeof(RisHouse)].Add(houseTransportGuid, house.Id);

            return new importHouseRSORequestLivingHouse
            {
                Item = houseData,
                LivingRoomToCreate = this.CreateLivingHouseLivingRoomToCreateRequest(house, transportGuidDictionary).ToArray(),
                LivingRoomToUpdate = this.CreateLivingHouseLivingRoomToUpdateRequest(house, transportGuidDictionary).ToArray()
            };
        }

        private List<importHouseRSORequestLivingHouseLivingRoomToCreate> CreateLivingHouseLivingRoomToCreateRequest(
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

            var result = new List<importHouseRSORequestLivingHouseLivingRoomToCreate>();

            foreach (var livingRoom in livingRoomsToCreate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                object noKNData;

                ItemChoiceType9 itemElementName;

                if (string.IsNullOrEmpty(livingRoom.CadastralNumber))
                {
                    noKNData = true;
                    itemElementName = ItemChoiceType9.NoKNData;
                }
                else
                {
                    noKNData = livingRoom.CadastralNumber;
                    itemElementName = ItemChoiceType9.CadastralNumber;
                }

                result.Add(new importHouseRSORequestLivingHouseLivingRoomToCreate
                {
                    Item = noKNData,
                    ItemElementName = itemElementName,
                    RoomNumber = livingRoom.RoomNumber,
                    Square = livingRoom.Square.GetValueOrDefault(),
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(LivingRoom)].Add(transportGuid, livingRoom.Id);
            }

            return result;
        }

        private List<importHouseRSORequestLivingHouseLivingRoomToUpdate> CreateLivingHouseLivingRoomToUpdateRequest(
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

            var result = new List<importHouseRSORequestLivingHouseLivingRoomToUpdate>();

            foreach (var livingRoom in livingRoomsToUpdate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                object noKNData;

                ItemChoiceType9 itemElementName;

                if (string.IsNullOrEmpty(livingRoom.CadastralNumber))
                {
                    noKNData = true;
                    itemElementName = ItemChoiceType9.NoKNData;
                }
                else
                {
                    noKNData = livingRoom.CadastralNumber;
                    itemElementName = ItemChoiceType9.CadastralNumber;
                }

                result.Add(new importHouseRSORequestLivingHouseLivingRoomToUpdate
                {
                    Item = noKNData,
                    ItemElementName = itemElementName,
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
