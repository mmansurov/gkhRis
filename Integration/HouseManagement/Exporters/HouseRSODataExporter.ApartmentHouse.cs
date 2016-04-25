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
    /// Методы создания запроса по многоквартирным домам
    /// </summary>
    public partial class HouseRSODataExporter
    {
        private importHouseRSORequestApartmentHouse CreateApartmentHouseRequest(
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
                houseData = new importHouseRSORequestApartmentHouseApartmentHouseToCreate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToCreate(house),
                    TransportGUID = houseTransportGuid
                };
            }
            else
            {
                houseData = new importHouseRSORequestApartmentHouseApartmentHouseToUpdate
                {
                    BasicCharacteristicts = this.GetBasicCharacteristictsToUpdate(house),
                    TransportGUID = houseTransportGuid
                };
            }

            transportGuidDictionary[typeof(RisHouse)].Add(houseTransportGuid, house.Id);

            return new importHouseRSORequestApartmentHouse
            {
                Item = houseData,
                NonResidentialPremiseToCreate = this.CreateApartmentHouseNonResidentialPremiseToCreateRequests(house, transportGuidDictionary).ToArray(),
                NonResidentialPremiseToUpdate = this.CreateApartmentHouseNonResidentialPremiseToUpdateRequests(house, transportGuidDictionary).ToArray(),
                EntranceToCreate = this.CreateApartmentHouseEntranceToCreateRequests(house, transportGuidDictionary).ToArray(),
                EntranceToUpdate = this.CreateApartmentHouseEntranceToUpdateRequests(house, transportGuidDictionary).ToArray(),
                ResidentialPremises = this.CreateApartmentHouseResidentialPremisesRequests(house, transportGuidDictionary).ToArray()
            };
        }

        private List<importHouseRSORequestApartmentHouseNonResidentialPremiseToCreate> CreateApartmentHouseNonResidentialPremiseToCreateRequests(RisHouse house, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(NonResidentialPremises)))
            {
                transportGuidDictionary.Add(typeof(NonResidentialPremises), new Dictionary<string, long>());
            }

            var premisesToCreate = this.NonResidentialPremisesList
                .Where(x => (x.ApartmentHouse.Id == house.Id)
                && (x.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(x.Guid)))
                .ToList();

            var result = new List<importHouseRSORequestApartmentHouseNonResidentialPremiseToCreate>();

            foreach (var premise in premisesToCreate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                object noKNData;

                ItemChoiceType9 itemElementName;

                if (string.IsNullOrEmpty(premise.CadastralNumber))
                {
                    noKNData = true;
                    itemElementName = ItemChoiceType9.NoKNData;
                }
                else
                {
                    noKNData = premise.CadastralNumber;
                    itemElementName = ItemChoiceType9.CadastralNumber;
                }

                result.Add(new importHouseRSORequestApartmentHouseNonResidentialPremiseToCreate
                {
                    Item = noKNData,
                    ItemElementName = itemElementName,
                    PremisesNum = premise.PremisesNum,
                    TotalArea = Decimal.Round(premise.TotalArea.GetValueOrDefault(), 2),
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(NonResidentialPremises)].Add(transportGuid, premise.Id);
            }

            return result;
        }

        private List<importHouseRSORequestApartmentHouseNonResidentialPremiseToUpdate> CreateApartmentHouseNonResidentialPremiseToUpdateRequests(
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

            var result = new List<importHouseRSORequestApartmentHouseNonResidentialPremiseToUpdate>();

            foreach (var premise in premisesToUpdate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                object noKNData;

                ItemChoiceType9 itemElementName;

                if (string.IsNullOrEmpty(premise.CadastralNumber))
                {
                    noKNData = true;
                    itemElementName = ItemChoiceType9.NoKNData;
                }
                else
                {
                    noKNData = premise.CadastralNumber;
                    itemElementName = ItemChoiceType9.CadastralNumber;
                }

                result.Add(new importHouseRSORequestApartmentHouseNonResidentialPremiseToUpdate
                {
                    Item = noKNData,
                    ItemElementName = itemElementName,
                    PremisesNum = premise.PremisesNum,                  
                    TotalArea = Decimal.Round(premise.TotalArea.GetValueOrDefault(), 2),
                    TerminationDate = premise.TerminationDate.GetValueOrDefault(),
                    PremisesGUID = premise.Guid,
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(NonResidentialPremises)].Add(transportGuid, premise.Id);
            }

            return result;
        }

        private List<importHouseRSORequestApartmentHouseEntranceToCreate> CreateApartmentHouseEntranceToCreateRequests(RisHouse house,
          Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            if (!transportGuidDictionary.ContainsKey(typeof(RisEntrance)))
            {
                transportGuidDictionary.Add(typeof(RisEntrance), new Dictionary<string, long>());
            }

            var entrancesToCreate = this.EntranceList
                .Where(x => (x.ApartmentHouse.Id == house.Id)
                && (x.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(x.Guid))).ToList();

            var result = new List<importHouseRSORequestApartmentHouseEntranceToCreate>();

            foreach (var entrance in entrancesToCreate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                result.Add(new importHouseRSORequestApartmentHouseEntranceToCreate
                {
                    EntranceNum = (sbyte)entrance.EntranceNum,
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(RisEntrance)].Add(transportGuid, entrance.Id);
            }

            return result;
        }

        private List<importHouseRSORequestApartmentHouseEntranceToUpdate> CreateApartmentHouseEntranceToUpdateRequests(
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

            var result = new List<importHouseRSORequestApartmentHouseEntranceToUpdate>();

            foreach (var entrance in entrancesToUpdate)
            {
                var transportGuid = Guid.NewGuid().ToString();

                result.Add(new importHouseRSORequestApartmentHouseEntranceToUpdate
                {
                    EntranceNum = (sbyte)entrance.EntranceNum,
                    TerminationDate = entrance.TerminationDate.GetValueOrDefault(),
                    EntranceGUID = entrance.Guid,
                    TransportGUID = transportGuid
                });

                transportGuidDictionary[typeof(RisEntrance)].Add(transportGuid, entrance.Id);
            }

            return result;
        }

        private List<importHouseRSORequestApartmentHouseResidentialPremises> CreateApartmentHouseResidentialPremisesRequests(RisHouse house, Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var residentialPremises = this.ResidentialPremisesList.Where(x => x.ApartmentHouse == house).ToList();
            var result = new List<importHouseRSORequestApartmentHouseResidentialPremises>();

            if (!transportGuidDictionary.ContainsKey(typeof(ResidentialPremises)))
            {
                transportGuidDictionary.Add(typeof(ResidentialPremises), new Dictionary<string, long>());
            }

            foreach (var residentialPremise in residentialPremises)
            {
                GKNRSOType item;

                if (residentialPremise.Operation == RisEntityOperation.Create || string.IsNullOrEmpty(residentialPremise.Guid))
                {
                    item = this.CreateApartmentHouseResidentialPremisesToCreateRequest(residentialPremise, transportGuidDictionary);
                }
                else
                {
                    item = this.CreateApartmentHouseResidentialPremisesToUpdateRequest(residentialPremise, transportGuidDictionary);
                }

                result.Add(new importHouseRSORequestApartmentHouseResidentialPremises
                {
                    Item = item,
                    LivingRoomToCreate = this.CreateApartmentHouseResidentialPremisesLivingRoomToCreateRequest(residentialPremise, transportGuidDictionary).ToArray(),
                    LivingRoomToUpdate = this.CreateApartmentHouseResidentialPremisesLivingRoomToUpdateRequest(residentialPremise, transportGuidDictionary).ToArray()
                });
            }

            return result;
        }

        private importHouseRSORequestApartmentHouseResidentialPremisesResidentialPremisesToCreate CreateApartmentHouseResidentialPremisesToCreateRequest(
           ResidentialPremises residentialPremise,
           Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var transportGuid = Guid.NewGuid().ToString();

            object noKNData;

            ItemChoiceType9 itemElementName;

            if (string.IsNullOrEmpty(residentialPremise.CadastralNumber))
            {
                noKNData = true;
                itemElementName = ItemChoiceType9.NoKNData;
            }
            else
            {
                noKNData = residentialPremise.CadastralNumber;
                itemElementName = ItemChoiceType9.CadastralNumber;
            }

            transportGuidDictionary[typeof(ResidentialPremises)].Add(transportGuid, residentialPremise.Id);

            return new importHouseRSORequestApartmentHouseResidentialPremisesResidentialPremisesToCreate
            {
                Item = noKNData,
                ItemElementName = itemElementName,
                PremisesNum = residentialPremise.PremisesNum,
                EntranceNum = (sbyte)residentialPremise.EntranceNum,
                PremisesCharacteristic = !string.IsNullOrEmpty(residentialPremise.PremisesCharacteristicCode) || !string.IsNullOrEmpty(residentialPremise.PremisesCharacteristicGuid)
                               ? new nsiRef
                               {
                                   Code = residentialPremise.PremisesCharacteristicCode,
                                   GUID = residentialPremise.PremisesCharacteristicGuid
                               }
                               : null,
                TotalArea = Decimal.Round(residentialPremise.TotalArea.GetValueOrDefault(), 2),
                TransportGUID = transportGuid
            };
        }

        private importHouseRSORequestApartmentHouseResidentialPremisesResidentialPremisesToUpdate CreateApartmentHouseResidentialPremisesToUpdateRequest(
            ResidentialPremises residentialPremise,
            Dictionary<Type, Dictionary<string, long>> transportGuidDictionary)
        {
            var transportGuid = Guid.NewGuid().ToString();

            object noKNData;

            ItemChoiceType9 itemElementName;

            if (string.IsNullOrEmpty(residentialPremise.CadastralNumber))
            {
                noKNData = true;
                itemElementName = ItemChoiceType9.NoKNData;
            }
            else
            {
                noKNData = residentialPremise.CadastralNumber;
                itemElementName = ItemChoiceType9.CadastralNumber;
            }

            transportGuidDictionary[typeof(ResidentialPremises)].Add(transportGuid, residentialPremise.Id);

            return new importHouseRSORequestApartmentHouseResidentialPremisesResidentialPremisesToUpdate
            {
                Item = noKNData,
                ItemElementName = itemElementName,
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
                TotalArea = Decimal.Round(residentialPremise.TotalArea.GetValueOrDefault(), 2),
                TransportGUID = transportGuid,
                PremisesGUID = residentialPremise.Guid
            };
        }

        private List<importHouseRSORequestApartmentHouseResidentialPremisesLivingRoomToCreate>
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

            var result = new List<importHouseRSORequestApartmentHouseResidentialPremisesLivingRoomToCreate>();

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

                result.Add(new importHouseRSORequestApartmentHouseResidentialPremisesLivingRoomToCreate
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

        private List<importHouseRSORequestApartmentHouseResidentialPremisesLivingRoomToUpdate>
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

            var result = new List<importHouseRSORequestApartmentHouseResidentialPremisesLivingRoomToUpdate>();

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

                result.Add(new importHouseRSORequestApartmentHouseResidentialPremisesLivingRoomToUpdate
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
