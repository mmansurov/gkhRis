namespace Bars.Gkh.Ris.Integration.HouseManagement.Exporters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4.DataAccess;
    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Entities;
    using Bars.Gkh.Ris.Entities.HouseManagement;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.HouseManagementAsync;
    using Bars.Gkh.Ris.Tasks.HouseManagement;
    using Gkh.Entities;
    /// <summary>
    /// Базовый класс экспорта данных по домам
    /// </summary>
    /// <typeparam name="TRequestType">Тип soap запроса</typeparam>
    public abstract class BaseHouseDataExporter<TRequestType> : BaseDataExporter<TRequestType, HouseManagementPortsTypeAsyncClient>
    {
        /// <summary>
        /// Список домов для экспорта
        /// </summary>
        protected List<RisHouse> HouseList;

        /// <summary>
        /// Список жилых помещений для экспорта
        /// </summary>
        protected List<ResidentialPremises> ResidentialPremisesList;

        /// <summary>
        /// Список нежилых помещений для экспорта
        /// </summary>
        protected List<NonResidentialPremises> NonResidentialPremisesList;

        /// <summary>
        /// Список подъездов для экспорта
        /// </summary>
        protected List<RisEntrance> EntranceList;

        /// <summary>
        /// Список комнат для экспорта
        /// </summary>
        protected List<LivingRoom> LivingRoomList;

        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        protected const int Portion = 1;

        /// <summary>
        /// Собрать данные
        /// </summary>
        /// <param name="parameters">Параметры экспорта</param>
        protected override void ExtractData(DynamicDictionary parameters)
        {
            this.HouseList = this.ExtractHouses(parameters);

            // загружаем зависимые объекты только для домов из houseList

            parameters.Add("apartmentHouses", this.HouseList.Where(x => x.HouseType == HouseType.Apartment));

            this.EntranceList = this.ExtractEntrances(parameters);

            this.ResidentialPremisesList = this.ExtractResidentialPremises(parameters);
            this.NonResidentialPremisesList = this.ExtractNonResidentialPremises(parameters);
            
            var entranceDomain = this.Container.ResolveDomain<RisEntrance>();
            var livingRoomDomain = this.Container.ResolveDomain<LivingRoom>();

            try
            {            
                this.EntranceList = entranceDomain.GetAll().Where(x => this.HouseList.Contains(x.ApartmentHouse)).ToList();
                this.LivingRoomList = livingRoomDomain.GetAll().Where(x => this.HouseList.Contains(x.House) || this.ResidentialPremisesList.Contains(x.ResidentialPremises)).ToList();
            }
            finally
            {
                this.Container.Release(entranceDomain);
                this.Container.Release(livingRoomDomain);
            }
        }

        /// <summary>
        /// Валидация данных
        /// </summary>
        /// <returns>Результат валидации</returns>
        protected override List<ValidateObjectResult> ValidateData()
        {
            var result = new List<ValidateObjectResult>();

            result.AddRange(this.ValidateObjectList(this.HouseList, this.CheckHouse));
            result.AddRange(this.ValidateObjectList(this.ResidentialPremisesList, this.CheckResidentialPremise));
            result.AddRange(this.ValidateObjectList(this.NonResidentialPremisesList, this.CheckNonResidentialPremise));
            result.AddRange(this.ValidateObjectList(this.EntranceList, this.CheckEntrance));
            result.AddRange(this.ValidateObjectList(this.LivingRoomList, this.CheckLivingRoom));

            return result;
        }

        /// <summary>
        /// Получить тип задачи для получения результатов экспорта
        /// </summary>
        /// <returns>Тип задачи</returns>
        protected override Type GetTaskType()
        {
            return typeof(ExportHouseDataTask);
        }

        /// <summary>
        /// Получить новый заголовок запроса
        /// </summary>
        /// <returns>Заголовок запроса</returns>
        protected RequestHeader GetNewRequestHeader()
        {
            return new RequestHeader
            {
                Date = DateTime.Now,
                MessageGUID = Guid.NewGuid().ToStr(),
                SenderID = this.SenderId
            };
        }

        /// <summary>
        /// Получает список порций объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список порций объектов ГИС</returns>
        protected List<IEnumerable<RisHouse>> GetPortions()
        {
            List<IEnumerable<RisHouse>> result = new List<IEnumerable<RisHouse>>();

            if (this.HouseList.Count > 0)
            {
                var startIndex = 0;
                do
                {
                    result.Add(this.HouseList.Skip(startIndex).Take(BaseHouseDataExporter<TRequestType>.Portion));
                    startIndex += BaseHouseDataExporter<TRequestType>.Portion;
                }
                while (startIndex < this.HouseList.Count);
            }
            
            return result;
        }

        /// <summary>
        /// Переопределить параметры сбора данных
        /// </summary>
        /// <param name="parameters">Параметры сбора</param>
        protected virtual void OverrideExtractingParametes(DynamicDictionary parameters)
        {
        }

        /// <summary>
        /// Проверка дома перед импортом
        /// </summary>
        /// <param name="house">Дом</param>
        /// <returns>Результат проверки</returns>
        protected abstract ValidateObjectResult CheckHouse(RisHouse house);

        /// <summary>
        /// Проверка жилого помещения перед импортом
        /// </summary>
        /// <param name="premise">Жилое помещение</param>
        /// <returns>Результат проверки</returns>
        protected abstract ValidateObjectResult CheckResidentialPremise(ResidentialPremises premise);

        /// <summary>
        /// Проверка нежилого помещения перед импортом
        /// </summary>
        /// <param name="premise">Нежилое помещение</param>
        /// <returns>Результат проверки</returns>
        protected abstract ValidateObjectResult CheckNonResidentialPremise(NonResidentialPremises premise);

        /// <summary>
        /// Проверка подъезда перед импортом
        /// </summary>
        /// <param name="entrance">Подъезд</param>
        /// <returns>Результат проверки</returns>
        protected abstract ValidateObjectResult CheckEntrance(RisEntrance entrance);

        /// <summary>
        /// Проверка комнаты в жилом доме перед импортом
        /// </summary>
        /// <param name="livingRoom">Комната в жилом доме</param>
        /// <returns>Результат проверки</returns>
        protected abstract ValidateObjectResult CheckLivingRoom(LivingRoom livingRoom);

        /// <summary>
        /// Собрать данные по домам
        /// </summary>
        /// <param name="parameters">Параметры экспорта</param>
        /// <returns>Список домов</returns>
        private List<RisHouse> ExtractHouses(DynamicDictionary parameters)
        {
            var extractor = this.Container.Resolve<IDataExtractor<RisHouse, RealityObject>>("RisHouseDataExtractor");

            try
            {
                extractor.Contragent = this.Contragent;

                this.OverrideExtractingParametes(parameters);

                return extractor.Extract(parameters);
            }
            finally
            {
                this.Container.Release(extractor);
            }
        }

        private List<RisEntrance> ExtractEntrances(DynamicDictionary parameters)
        {
            var extractor = this.Container.Resolve<IDataExtractor<RisEntrance, Entrance>>("EntranceDataExtractor");

            try
            {
                extractor.Contragent = this.Contragent;

                return extractor.Extract(parameters);
            }
            finally
            {
                this.Container.Release(extractor);
            }
        }

        private List<ResidentialPremises> ExtractResidentialPremises(DynamicDictionary parameters)
        {
            var extractor = this.Container.Resolve<IDataExtractor<ResidentialPremises, Room>>("ResidentialPremisesDataExtractor");

            try
            {
                extractor.Contragent = this.Contragent;

                return extractor.Extract(parameters);
            }
            finally
            {
                this.Container.Release(extractor);
            }
        }

        private List<NonResidentialPremises> ExtractNonResidentialPremises(DynamicDictionary parameters)
        {
            var extractor = this.Container.Resolve<IDataExtractor<NonResidentialPremises, Room>>("NonResidentialPremisesDataExtractor");

            try
            {
                extractor.Contragent = this.Contragent;

                return extractor.Extract(parameters);                
            }
            finally
            {
                this.Container.Release(extractor);
            }
        }

        private List<ValidateObjectResult> ValidateObjectList<T>(List<T> objecList, Func<T, ValidateObjectResult> checker) where T : BaseRisEntity
        {
            var result = new List<ValidateObjectResult>();
            var itemsToRemove = new List<T>();

            foreach (var item in objecList)
            {
                var validateResult = checker(item);

                if (validateResult.State != ObjectValidateState.Success)
                {
                    result.Add(validateResult);
                    itemsToRemove.Add(item);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                objecList.Remove(itemToRemove);
            }

            return result;
        }
    }
}
