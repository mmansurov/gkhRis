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
    /// Экстрактор данных по нежилым помещениям
    /// </summary>
    public class NonResidentialPremisesDataExtractor : BaseDataExtractor<NonResidentialPremises, Room>
    {
        private List<RisHouse> houses;

        private Dictionary<long, nsiRef> purposeDict;

        private Dictionary<long, nsiRef> positionDict;

        /// <summary>
        /// Получить сущности сторонней системы - нежилые помещения
        /// </summary>
        /// <param name="parameters">Параметры сбора данных</param>
        /// <returns>Сущности сторонней системы - нежилые помещения</returns>
        public override List<Room> GetExternalEntities(DynamicDictionary parameters)
        {
            var houses = parameters.GetAs<List<RisHouse>>("apartmentHouses");
            var houseIds = houses != null ? houses.Select(x => x.ExternalSystemEntityId).ToArray() : new long[0];

            var roomDomain = this.Container.ResolveDomain<Room>();

            try
            {
                return roomDomain.GetAll()
                    .WhereIf(houses != null, x => houseIds.Contains(x.RealityObject.Id))
                    .Where(x => x.Type == RoomType.NonLiving)
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
                this.purposeDict =
                    gisDictRefDomain.GetAll()
                        .Where(x => x.Dict.ActionCode == "Назначение помещения")
                        .Select(x => new { x.GkhId, x.GisId, x.GisGuid })
                        .ToList()
                        .GroupBy(x => x.GkhId)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(y => new nsiRef { Code = y.GisId, GUID = y.GisGuid }).First());

                this.positionDict =
                    gisDictRefDomain.GetAll()
                        .Where(x => x.Dict.ActionCode == "Расположение помещения")
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
        /// <param name="nonResidentialPremise">Ris сущность</param>
        protected override void UpdateRisEntity(Room room, NonResidentialPremises nonResidentialPremise)
        {
            var purpose = this.GetPurpose(room);
            var position = this.GetPosition(room);

            nonResidentialPremise.ExternalSystemEntityId = room.Id;
            nonResidentialPremise.ExternalSystemName = "gkh";
            nonResidentialPremise.CadastralNumber = room.CadastralNumber;
            nonResidentialPremise.PremisesNum = room.RoomNum;
            nonResidentialPremise.Floor = room.Floor.HasValue ? room.Floor.Value.ToString() : null;
            nonResidentialPremise.PurposeCode = purpose.Code;
            nonResidentialPremise.PurposeGuid = purpose.GUID;
            nonResidentialPremise.PositionCode = position.Code;
            nonResidentialPremise.PositionGuid = position.GUID;
            nonResidentialPremise.TotalArea = room.Area;
            //nonResidentialPremise.IsCommonProperty=
            nonResidentialPremise.GrossArea = room.LivingArea;
            nonResidentialPremise.TerminationDate = room.RealityObject.DateDemolition;

            if (nonResidentialPremise.ApartmentHouse == null)
            {
                nonResidentialPremise.ApartmentHouse = this.GetRisHouse(room);
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
                throw new Exception(string.Format("Для нежилого помещения ExternalSystemEntityId = {0} не загружен дом", room.Id));
            }

            return result;
        }

        private nsiRef GetPurpose(Room room)
        {
            //пока передаем для всех Pharmacy
            return this.purposeDict[(int)Purpose.Pharmacy];
        }

        private nsiRef GetPosition(Room room)
        {
            //пока передаем для всех "Встроенное"
            return this.positionDict[(int)Enums.HouseManagement.Position.BuiltIn];
        }
    }
}
