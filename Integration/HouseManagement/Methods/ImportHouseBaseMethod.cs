namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.HouseManagement;
    using Entities;
    using Ris.HouseManagement;

    /// <summary>
    /// Базовый класс экспортер данных домов
    /// </summary>
    public abstract class ImportHouseBaseMethod<K> : GisIntegrationHouseManagementMethod<RisHouse, K>
    {
        protected List<ResidentialPremises> residentialPremisesList;
        protected List<NonResidentialPremises> nonResidentialPremisesList;
        protected List<RisEntrance> entranceList;
        protected List<LivingRoom> livingRoomList;
        protected Dictionary<string, RisHouse> housesByTransportGuid = new Dictionary<string, RisHouse>();
        protected Dictionary<string, NonResidentialPremises> nonResidentialPremisesByTransportGuid = new Dictionary<string, NonResidentialPremises>();
        protected Dictionary<string, RisEntrance> entrancesByTransportGuid = new Dictionary<string, RisEntrance>();
        protected Dictionary<string, ResidentialPremises> residentialPremisesByTransportGuid = new Dictionary<string, ResidentialPremises>();
        protected Dictionary<string, LivingRoom> livingRoomsByTransportGuid = new Dictionary<string, LivingRoom>();
        protected List<RisHouse> housesToSave = new List<RisHouse>();
        protected List<NonResidentialPremises> nonResidentialPremisesesToSave = new List<NonResidentialPremises>();
        protected List<RisEntrance> entrancesToSave = new List<RisEntrance>();
        protected List<ResidentialPremises> residentialPremisesesToSave = new List<ResidentialPremises>();
        protected List<LivingRoom> livingRoomsToSave = new List<LivingRoom>();

        /// <summary>
        /// Размер блока предаваемых данных (максимальное количество записей)
        /// </summary>
        protected override int Portion
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Количество объектов для сохранения
        /// </summary>
        protected override int ProcessedObjects
        {
            get
            {
                return this.housesToSave.Count + this.nonResidentialPremisesesToSave.Count
                       + this.residentialPremisesesToSave.Count + this.entrancesToSave.Count
                       + this.livingRoomsToSave.Count;
            }
        }

        /// <summary>
        /// Общее количество импортируемых объектов
        /// </summary>
        new  protected int CountObjects
        {
            get
            {
                return this.housesByTransportGuid.Count + this.nonResidentialPremisesByTransportGuid.Count
                       + this.residentialPremisesByTransportGuid.Count + this.entrancesByTransportGuid.Count
                       + this.livingRoomsByTransportGuid.Count;
            }
        }

        /// <summary>
        /// Список объектов ГИС, по которым производится импорт.
        /// </summary>
        protected override IList<RisHouse> MainList { get; set; }

        /// <summary>
        /// Подготовка кэша данных 
        /// </summary>
        protected override void Prepare()
        {
            var residentialPremisesDomain = this.Container.ResolveDomain<ResidentialPremises>();
            var nonResidentialPremisesDomain = this.Container.ResolveDomain<NonResidentialPremises>();
            var entranceDomain = this.Container.ResolveDomain<RisEntrance>();
            var livingRoomDomain = this.Container.ResolveDomain<LivingRoom>();

            this.MainList = this.GetMainList();

            try
            {
                //TODO загружать зависимые объекты только для домов из MainList

                this.residentialPremisesList = residentialPremisesDomain.GetAll().ToList();
                this.nonResidentialPremisesList = nonResidentialPremisesDomain.GetAll().ToList();
                this.entranceList = entranceDomain.GetAll().ToList();
                this.livingRoomList = livingRoomDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(residentialPremisesDomain);
                this.Container.Release(nonResidentialPremisesDomain);
                this.Container.Release(entranceDomain);
                this.Container.Release(livingRoomDomain);
            }
        }

        /// <summary>
        /// Метод получения списка домов для импорта
        /// </summary>
        /// <returns>Списко объектов для импорта</returns>
        protected abstract List<RisHouse> GetMainList();

        /// <summary>
        /// Проверить строку элемент response и добавить объекты ГИС на сохранение.
        /// </summary>
        /// <param name="responseItem">Элемент response</param>
        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            var transportGuid = responseItem.TransportGUID;

            if (this.housesByTransportGuid.ContainsKey(transportGuid))
            {
                var house = this.housesByTransportGuid[transportGuid];

                this.CheckResponseItem(house, this.housesToSave, responseItem);

                return;
            }

            if (this.nonResidentialPremisesByTransportGuid.ContainsKey(transportGuid))
            {
                var nonResidentialPremise = this.nonResidentialPremisesByTransportGuid[transportGuid];

                this.CheckResponseItem(nonResidentialPremise, this.nonResidentialPremisesesToSave, responseItem);

                return;
            }

            if (this.residentialPremisesByTransportGuid.ContainsKey(transportGuid))
            {
                var residentialPremise = this.residentialPremisesByTransportGuid[transportGuid];

                this.CheckResponseItem(residentialPremise, this.residentialPremisesesToSave, responseItem);

                return;
            }

            if (this.entrancesByTransportGuid.ContainsKey(transportGuid))
            {
                var entrance = this.entrancesByTransportGuid[transportGuid];

                this.CheckResponseItem(entrance, this.entrancesToSave, responseItem);

                return;
            }

            if (this.livingRoomsByTransportGuid.ContainsKey(transportGuid))
            {
                var livingRoom = this.livingRoomsByTransportGuid[transportGuid];

                this.CheckResponseItem(livingRoom, this.livingRoomsToSave, responseItem);
            }
        }

        /// <summary>
        /// Сохранение объектов ГИС после импорта.
        /// </summary>
        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.housesToSave, 1000, true, true);
            TransactionHelper.InsertInManyTransactions(this.Container, this.nonResidentialPremisesesToSave, 1000, true, true);
            TransactionHelper.InsertInManyTransactions(this.Container, this.residentialPremisesesToSave, 1000, true, true);
            TransactionHelper.InsertInManyTransactions(this.Container, this.entrancesToSave, 1000, true, true);
            TransactionHelper.InsertInManyTransactions(this.Container, this.livingRoomsToSave, 1000, true, true);
        }

        /// <summary>
        /// Чистим список объектов для импорта от невалидных
        /// </summary>
        protected void ClearObjectList<T>(List<T> objecList, Func<T, CheckingResult> checker) where T : BaseRisEntity
        {
            List<T> itemsToRemove = new List<T>();

            foreach (var item in objecList)
            {
                var checkingResult = checker(item);

                if (!checkingResult.Result)
                {
                    itemsToRemove.Add(item);
                    this.AddLineToLog(string.Format("Объект типа {0}", typeof(T).Name), item.Id, "Не загружен", checkingResult.Messages);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                objecList.Remove(itemToRemove);
            }
        }

        private void CheckResponseItem<T>(T item, List<T> itemsToSave, CommonResultType responseItem)
           where T : BaseRisEntity
        {
            if (responseItem.GUID.IsEmpty())
            {
                var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog(
                    string.Format("Объект типа {0}", typeof(T).Name),
                    item.Id,
                    "Не загружен",
                    errorNotation);
            }
            else
            {
                item.Guid = responseItem.GUID;
                itemsToSave.Add(item);

                this.AddLineToLog(
                    string.Format("Объект типа {0}", typeof(T).Name),
                    item.Id,
                    "Загружен",
                    responseItem.GUID);
            }
        }
    }
}
