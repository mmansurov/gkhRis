namespace Bars.Gkh.Ris.Integration.HouseManagement.DataExtractors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4.DataAccess;
    using Bars.B4.Utils;
    using Bars.Gkh.Entities;
    using Bars.Gkh.Ris.Entities.HouseManagement;

    /// <summary>
    /// Экстрактор данных по подъездам
    /// </summary>
    public class EntranceDataExtractor : BaseDataExtractor<RisEntrance, Entrance>
    {
        private List<RisHouse> houses;

        /// <summary>
        /// Получить сущности сторонней системы - подъезды
        /// </summary>
        /// <param name="parameters">Параметры сбора данных</param>
        /// <returns>Сущности сторонней системы - подъезды</returns>
        public override List<Entrance> GetExternalEntities(DynamicDictionary parameters)
        {
            var houses = parameters.GetAs<List<RisHouse>>("apartmentHouses");

            var houseIds = houses != null ? houses.Select(x => x.ExternalSystemEntityId).ToArray() : new long[0];

            var entranceDomain = this.Container.ResolveDomain<Entrance>();

            try
            {
                return entranceDomain.GetAll()
                    .WhereIf(houses != null, x => houseIds.Contains(x.RealityObject.Id))
                    .ToList();
            }
            finally
            {
                this.Container.Release(entranceDomain);
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
        }

        /// <summary>
        /// Обновить значения атрибутов Ris сущности
        /// </summary>
        /// <param name="entrance">Сущность внешней системы</param>
        /// <param name="risEntrance">Ris сущность</param>
        protected override void UpdateRisEntity(Entrance entrance, RisEntrance risEntrance)
        {
            risEntrance.ExternalSystemEntityId = entrance.Id;
            risEntrance.ExternalSystemName = "gkh";
            risEntrance.EntranceNum = (short)entrance.Number;
            risEntrance.StoreysCount = entrance.RealityObject.MaximumFloors.HasValue ? (short)entrance.RealityObject.MaximumFloors.Value : (short)1;
            risEntrance.CreationDate = this.GetCreationDate(entrance);
            risEntrance.TerminationDate = entrance.RealityObject.DateDemolition;

            if (risEntrance.ApartmentHouse == null)
            {
                risEntrance.ApartmentHouse = this.GetRisHouse(entrance);
            }
        }

        private DateTime GetCreationDate(Entrance entrance)
        {
            if (entrance.RealityObject.BuildYear.HasValue)
            {
                return new DateTime(entrance.RealityObject.BuildYear.Value, 1, 1);
            }

            return DateTime.MinValue;
        }

        private RisHouse GetRisHouse(Entrance entrance)
        {
            RisHouse result;

            if (this.houses != null)
            {
                result = this.houses.FirstOrDefault(x => x.ExternalSystemEntityId == entrance.RealityObject.Id);
            }
            else
            {
                var risHouseDomain = this.Container.ResolveDomain<RisHouse>();

                try
                {
                    result = risHouseDomain.GetAll()
                       .Where(x => x.Contragent.Id == this.Contragent.Id)
                       .FirstOrDefault(x => x.ExternalSystemEntityId == entrance.RealityObject.Id);
                }
                finally
                {
                    this.Container.Release(risHouseDomain);
                }              
            }

            if (result == null)
            {
                throw new Exception(string.Format("Для подъезда ExternalSystemEntityId = {0} не загружен дом", entrance.Id));
            }

            return result;
        }
    }
}
