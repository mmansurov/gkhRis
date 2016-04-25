namespace Bars.Gkh.Ris.Integration.HouseManagement.DataExtractors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4.DataAccess;

    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Entities.GisIntegration.Ref;
    using Bars.Gkh.Ris.Enums.HouseManagement;
    using Bars.Gkh.Ris.HouseManagement;

    using Gkh.Entities;
    using Gkh.Enums;
    using Entities.HouseManagement;

    /// <summary>
    /// Экстрактор данных по жилым помещениям
    /// </summary>
    public class ResidentialPremisesDataExtractor : BaseDataExtractor<ResidentialPremises, Room>
    {
        private List<RisHouse> houses;

        private Dictionary<long, nsiRef> premisesCharacteristicDict;

        private Dictionary<long, nsiRef> roomsNumDict;

        /// <summary>
        /// Получить сущности сторонней системы - жилые помещения
        /// </summary>
        /// <param name="parameters">Параметры сбора данных</param>
        /// <returns>Сущности сторонней системы - жилые помещения</returns>
        public override List<Room> GetExternalEntities(DynamicDictionary parameters)
        {
            var houses = parameters.GetAs<List<RisHouse>>("apartmentHouses");

            var houseIds = houses != null ? houses.Select(x => x.ExternalSystemEntityId).ToArray() : new long[0];

            var roomDomain = this.Container.ResolveDomain<Room>();

            try
            {
                return roomDomain.GetAll()
                    .WhereIf(houses != null, x => houseIds.Contains(x.RealityObject.Id))
                    .Where(x => x.Type == RoomType.Living)
                    .ToList();
            }
            finally
            {
                this.Container.Release(roomDomain);
            }
        }

        /// <summary>
        /// Выполнить обработку перед извлечением данных
        /// Заполнить словари
        /// </summary>
        /// <param name="parameters">Входные параметры</param>
        protected override void BeforeExtractHandle(DynamicDictionary parameters)
        {
            this.houses = parameters.GetAs<List<RisHouse>>("apartmentHouses");

            var gisDictRefDomain = this.Container.ResolveDomain<GisDictRef>();

            try
            {
                this.premisesCharacteristicDict =
                    gisDictRefDomain.GetAll()
                        .Where(x => x.Dict.ActionCode == "Характеристика помещения")
                        .Select(x => new { x.GkhId, x.GisId, x.GisGuid })
                        .ToList()
                        .GroupBy(x => x.GkhId)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(y => new nsiRef { Code = y.GisId, GUID = y.GisGuid }).First());

                this.roomsNumDict =
                    gisDictRefDomain.GetAll()
                        .Where(x => x.Dict.ActionCode == "Количество комнат")
                        .Select(x => new { x.GkhId, x.GisId, x.GisGuid })
                        .ToList()
                        .GroupBy(x => x.GkhId)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(y => new nsiRef { Code = y.GisId, GUID = y.GisGuid }).First());
            }
            finally
            {
                this.Container.Release(gisDictRefDomain);
            }
        }

        /// <summary>
        /// Обновить значения атрибутов Ris сущности
        /// </summary>
        /// <param name="room">Сущность внешней системы</param>
        /// <param name="residentialPremise">Ris сущность</param>
        protected override void UpdateRisEntity(Room room, ResidentialPremises residentialPremise)
        {
            var premisesCharacteristic = this.GetPremisesCharacteristic(room);
            var roomsNum = this.GetRoomsNum(room);

            residentialPremise.ExternalSystemEntityId = room.Id;
            residentialPremise.ExternalSystemName = "gkh";
            residentialPremise.CadastralNumber = room.CadastralNumber;
            residentialPremise.PremisesNum = room.RoomNum;
            residentialPremise.Floor = room.Floor.HasValue ? room.Floor.Value.ToString() : null;
            residentialPremise.EntranceNum = room.Entrance != null ? (short?)room.Entrance.Number : null;

            residentialPremise.PremisesCharacteristicCode = premisesCharacteristic.Code;
            residentialPremise.PremisesCharacteristicGuid = premisesCharacteristic.GUID;

            residentialPremise.RoomsNumCode = roomsNum.Code;
            residentialPremise.RoomsNumGuid = roomsNum.GUID;

            residentialPremise.TotalArea = room.Area;
            residentialPremise.GrossArea = room.LivingArea;

            //residentialPremise.ResidentialHouseTypeCode = 
            //residentialPremise.ResidentialHouseTypeGuid = 

            residentialPremise.TerminationDate = room.RealityObject.DateDemolition;

            if (residentialPremise.ApartmentHouse == null)
            {
                residentialPremise.ApartmentHouse = this.GetRisHouse(room);
            }
        }

        private RisHouse GetRisHouse(Room room)
        {
            RisHouse result;

            if (this.houses != null)
            {
                result = this.houses.FirstOrDefault(x => x.ExternalSystemEntityId == room.RealityObject.Id);
            }
            else
            {
                var risHouseDomain = this.Container.ResolveDomain<RisHouse>();

                try
                {
                    result = risHouseDomain.GetAll()
                         .Where(x => x.Contragent.Id == this.Contragent.Id)
                         .FirstOrDefault(x => x.ExternalSystemEntityId == room.RealityObject.Id);
                }
                finally
                {
                    this.Container.Release(risHouseDomain);
                }                
            }

            if (result == null)
            {
                throw new Exception(string.Format("Для жилого помещения ExternalSystemEntityId = {0} не загружен дом", room.Id));
            }

            return result;
        }

        private nsiRef GetPremisesCharacteristic(Room room)
        {
            //пока жилые помещения передаются только для многоквартиных домов - для всех помещений тип квартира

            return this.premisesCharacteristicDict[(int)PremisesCharacteristic.CertainApartment];
        }

        private nsiRef GetRoomsNum(Room room)
        {
            var gkhRoomsNum = room.RoomsCount.GetValueOrDefault();

            if (gkhRoomsNum <= 1)
            {
                return this.roomsNumDict[(int)RoomsNum.OneRoom];
            }

            if (gkhRoomsNum <= 7)
            {
                var risRoomsNum = (RoomsNum)gkhRoomsNum;
                return this.roomsNumDict[(int)risRoomsNum];
            }

            return this.roomsNumDict[(int)RoomsNum.SevenAndMoreRoom];
        }
    }
}
