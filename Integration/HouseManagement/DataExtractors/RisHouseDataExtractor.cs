namespace Bars.Gkh.Ris.Integration.HouseManagement.DataExtractors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4.DataAccess;
    using B4.Utils;

    using Bars.Gkh.Domain;
    using Bars.Gkh.Modules.Gkh1468.Entities.PublicServiceOrg;
    using Bars.Gkh1468.Entities;

    using Gkh.Entities;
    using Gkh.Enums;
    using Entities.GisIntegration.Ref;
    using Entities.HouseManagement;
    using Enums;
    using GkhTp.Entities;

    using nsiRef = Bars.Gkh.Ris.HouseManagement.nsiRef;

    /// <summary>
    /// Экстрактор данных по домам
    /// </summary>
    public class RisHouseDataExtractor : BaseDataExtractor<RisHouse, RealityObject>
    {
        private Dictionary<long, nsiRef> HouseStateDict;
        private Dictionary<long, nsiRef> ProjectTypeDict;
        private Dictionary<long, TehPassInfoProxy> TehPassDict;

        /// <summary>
        /// Получить сущности сторонней системы - дома
        /// </summary>
        /// <param name="parameters">Параметры сбора данных</param>
        /// <returns>Сущности сторонней системы - дома</returns>
        public override List<RealityObject> GetExternalEntities(DynamicDictionary parameters)
        {
            if (parameters.ContainsKey("uoId"))
            {
                return this.GetRealityObjectListByUO(parameters);
            }
            
            if (parameters.ContainsKey("omsId"))
            {
                return this.GetRealityObjectListByOMS(parameters);
            }

            if (parameters.ContainsKey("rsoId"))
            {
                return this.GetRealityObjectListByRSO(parameters);
            }

            return this.GetRealityObjectListDefault(parameters);
        }

        /// <summary>
        /// Конвертировать тип дома
        /// </summary>
        /// <param name="houseType">Тип дома в сторонней системе</param>
        /// <returns>Тип дома Ris</returns>
        public HouseType ConvertHouseType(TypeHouse houseType)
        {
            switch (houseType)
            {
                case TypeHouse.ManyApartments:
                    return HouseType.Apartment;
                default:
                    return HouseType.Living;
            }
        }

        /// <summary>
        /// Выполнить обработку перед извлечением данных
        /// Заполнить словари
        /// </summary>
        /// <param name="parameters">Входные параметры</param>
        protected override void BeforeExtractHandle(DynamicDictionary parameters)
        {
            var gisDictRefDomain = this.Container.ResolveDomain<GisDictRef>();

            try
            {
                this.HouseStateDict =
                    gisDictRefDomain.GetAll()
                        .Where(x => x.Dict.ActionCode == "Состояние дома")
                        .Select(x => new { x.GkhId, x.GisId, x.GisGuid })
                        .ToList()
                        .GroupBy(x => x.GkhId)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(y => new nsiRef { Code = y.GisId, GUID = y.GisGuid }).First());

                this.ProjectTypeDict =
                    gisDictRefDomain.GetAll()
                        .Where(x => x.Dict.ActionCode == "Тип проекта здания")
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
        /// Выполнить обработку перед подготовкой Ris сущностей
        /// Получить данных из тех паспортов для выбранных домов
        /// </summary>
        /// <param name="parameters">Входные параметры</param>
        /// <param name="realityObjects">Выбранные дома</param>
        protected override void BeforePrepareRisEntitiesHandle(DynamicDictionary parameters, List<RealityObject> realityObjects)
        {
            var realityObjectIds = realityObjects.Select(x => x.Id).ToArray();

            var techPassValueDomain = this.Container.ResolveDomain<TehPassportValue>();

            try
            {
                this.TehPassDict =
                    techPassValueDomain.GetAll()
                        .Where(x => realityObjectIds.Contains(x.TehPassport.RealityObject.Id))
                        .Where(
                            x =>
                                (x.FormCode == "Form_6_1_1" && x.CellCode == "2:1") || (x.FormCode == "Form_6_1_1" && x.CellCode == "1:1")
                                    || (x.FormCode == "Form_2" && x.CellCode == "3:3") || (x.FormCode == "Form_1_3_3" && x.CellCode == "2:1"))
                        .GroupBy(x => x.TehPassport.RealityObject.Id)
                        .ToDictionary(
                            x => x.Key,
                            x =>
                                new TehPassInfoProxy
                                {
                                    EnergyDateValue = x.FirstOrDefault(y => y.FormCode == "Form_6_1_1" && y.CellCode == "2:1"),
                                    EnergyCategoryValue = x.FirstOrDefault(y => y.FormCode == "Form_6_1_1" && y.CellCode == "1:1"),
                                    BuiltUpAreaValue = x.FirstOrDefault(y => y.FormCode == "Form_2" && y.CellCode == "3:3"),
                                    UndergroundAreaValue = x.FirstOrDefault(y => y.FormCode == "Form_1_3_3" && y.CellCode == "2:1")
                                });


            }
            finally
            {
                this.Container.Release(techPassValueDomain);
            }

        }

        /// <summary>
        /// Обновить значения атрибутов Ris сущности
        /// </summary>
        /// <param name="realityObject">Сущность внешней системы</param>
        /// <param name="risHouse">Ris сущность</param>
        protected override void UpdateRisEntity(RealityObject realityObject, RisHouse risHouse)
        {
            var state = this.GetHouseState(realityObject);

            risHouse.HouseType = this.ConvertHouseType(realityObject.TypeHouse);
            risHouse.ExternalSystemEntityId = realityObject.Id;
            risHouse.ExternalSystemName = "gkh";
            //FiasHouseGuid = x.HouseGuid.IsNotEmpty() ? x.HouseGuid : null,
            risHouse.FiasHouseGuid = realityObject.HouseGuid;
            //CadastralNumber = x.CadastreNumber.IsNotEmpty() ? x.CadastreNumber : null,
            risHouse.CadastralNumber = realityObject.CadastreNumber;
            risHouse.TotalSquare = realityObject.AreaMkd;
            //ResidentialSquare = residentialSquare,
            risHouse.ResidentialSquare = realityObject.AreaLiving.GetValueOrDefault();
            risHouse.NonResidentialSquare = this.GetNonResidentialSquare(realityObject);
            risHouse.StateCode = state?.Code;
            risHouse.StateGuid = state?.GUID;
            risHouse.ProjectSeries = realityObject.TypeProject?.Name;
            risHouse.ProjectTypeCode = this.ProjectTypeDict.ContainsKey(realityObject.ConditionHouse.ToLong())
                ? this.ProjectTypeDict[realityObject.ConditionHouse.ToLong()].Code
                : null;
            risHouse.ProjectTypeGuid = this.ProjectTypeDict.ContainsKey(realityObject.ConditionHouse.ToLong())
                ? this.ProjectTypeDict[realityObject.ConditionHouse.ToLong()].GUID
                : null;
            risHouse.BuildingYear = realityObject.BuildYear.HasValue ? (short?)realityObject.BuildYear.Value : null;
            risHouse.UsedYear = realityObject.DateCommissioning.HasValue
                ? (short?)realityObject.DateCommissioning.Value.Year
                : null;
            risHouse.TotalWear = realityObject.PhysicalWear;
            risHouse.EnergyDate = this.TehPassDict.ContainsKey(realityObject.Id)
                ? this.TehPassDict[realityObject.Id].EnergyDate
                : null;
            // узнать как сопоставлять и сделать здесь получение исходя из TehPassDict[x.Id].EnergyCategory
            risHouse.EnergyCategoryCode = "2"; //?
            risHouse.EnergyCategoryGuid = "c8c8e97a-b76a-46a6-8e21-f83482e64eeb"; //?
            //? OktmoCode =
            //? OktmoName =
            risHouse.OlsonTZCode = "2"; //?
            risHouse.OlsonTZGuid = "a4c5f73f-baab-4b1a-b839-76de86bda050"; //?
            risHouse.CulturalHeritage = false;
            risHouse.BuiltUpArea = this.TehPassDict.ContainsKey(realityObject.Id)
                ? this.TehPassDict[realityObject.Id].BuiltUpArea
                : null;
            risHouse.MinFloorCount = realityObject.Floors;
            risHouse.FloorCount = realityObject.MaximumFloors.ToString();
            risHouse.UndergroundFloorCount = this.TehPassDict.ContainsKey(realityObject.Id)
                                             && this.TehPassDict[realityObject.Id].UndergroundArea.GetValueOrDefault()
                                             > 0
                ? "1"
                : "0";
            risHouse.OverhaulYear = realityObject.DateLastOverhaul.HasValue
                ? (short?)realityObject.DateLastOverhaul.Value.Year
                : null;
            //? OverhaulFormingKindCode = 
            //? OverhaulFormingKindGuid =
            //? HouseManagementTypeCode =
            //? HouseManagementTypeGuid =
        }

        private List<RealityObject> GetRealityObjectListByUO(DynamicDictionary parameters)
        {
            var selectedHouses = parameters.GetAs<string>("selectedHouses").ToLongArray();
            var contragentId = parameters.GetAs<long>("uoId");

            var result = new List<RealityObject>();

            var manorg = this.GetManOrgByContagentId(contragentId);

            if (manorg == null)
            {
                return result;
            }

            var manOrgContractRealityObjectDomain = this.Container.ResolveDomain<ManOrgContractRealityObject>();

            try
            {
                return
                    manOrgContractRealityObjectDomain.GetAll()
                        .WhereIf(selectedHouses.Length > 0, x => selectedHouses.Contains(x.RealityObject.Id))
                        .Where(
                            x =>
                                x.RealityObject.TypeHouse == TypeHouse.ManyApartments || x.RealityObject.TypeHouse == TypeHouse.BlockedBuilding
                                    || x.RealityObject.TypeHouse == TypeHouse.Individual || x.RealityObject.TypeHouse == TypeHouse.SocialBehavior)
                        .Where(x => x.ManOrgContract.ManagingOrganization.Id == manorg.Id)
                        .Where(
                            x =>
                                x.ManOrgContract.TypeContractManOrgRealObj == TypeContractManOrg.ManagingOrgOwners
                                    || x.ManOrgContract.TypeContractManOrgRealObj == TypeContractManOrg.JskTsj)
                        .Where(x => x.ManOrgContract.TerminateReason == null || x.ManOrgContract.TerminateReason == string.Empty)
                        .Where(x => !x.ManOrgContract.StartDate.HasValue || x.ManOrgContract.StartDate.Value <= DateTime.Now)
                        .Where(x => !x.ManOrgContract.EndDate.HasValue || x.ManOrgContract.EndDate.Value >= DateTime.Now)
                        .Select(x => x.RealityObject)
                        .ToList();
            }
            finally
            {
                this.Container.Release(manOrgContractRealityObjectDomain);
            }
        }

        private List<RealityObject> GetRealityObjectListByOMS(DynamicDictionary parameters)
        {
            var selectedHouses = parameters.GetAs<string>("selectedHouses").ToLongArray();
            var contragentId = parameters.GetAs<long>("omsId");

            var result = new List<RealityObject>();

            var localGovernment = this.GetLocalGovernmentByContagentId(contragentId);

            if (localGovernment == null)
            {
                return result;
            }

            var realityObjectDomain = this.Container.ResolveDomain<RealityObject>();
            var localGovernmentMunicipality = this.Container.ResolveDomain<LocalGovernmentMunicipality>();
            var manOrgContractRealityObjectDomain = this.Container.ResolveDomain<ManOrgContractRealityObject>();
            var moContractDirectManagServ = this.Container.ResolveDomain<RealityObjectDirectManagContract>();

            try
            {
                var municipalityIds = localGovernmentMunicipality
                    .GetAll()
                    .Where(x => x.LocalGovernment.Id == localGovernment.Id)
                    .Select(x => x.Municipality.Id)
                    .ToArray();

                return
                    manOrgContractRealityObjectDomain.GetAll()
                        .WhereIf(selectedHouses.Length > 0, x => selectedHouses.Contains(x.RealityObject.Id))
                        .Where(x => x.RealityObject.Municipality != null && municipalityIds.Contains(x.RealityObject.Municipality.Id))
                        .Where(x => x.ManOrgContract.TypeContractManOrgRealObj == TypeContractManOrg.DirectManag)
                        .Where(x => x.ManOrgContract.TerminateReason == null || x.ManOrgContract.TerminateReason == string.Empty)
                        .Where(
                            x =>
                                (!x.ManOrgContract.StartDate.HasValue || x.ManOrgContract.StartDate.Value <= DateTime.Now)
                                    && (!x.ManOrgContract.EndDate.HasValue || x.ManOrgContract.EndDate.Value >= DateTime.Now))
                        .Where(x => moContractDirectManagServ.GetAll().Any(y => y.Id == x.ManOrgContract.Id && !y.IsServiceContract))
                        .Select(x => x.RealityObject)
                        .ToList();

            }
            finally
            {
                this.Container.Release(realityObjectDomain);
                this.Container.Release(localGovernmentMunicipality);
                this.Container.Release(manOrgContractRealityObjectDomain);
                this.Container.Release(moContractDirectManagServ);
            }
        }

        private List<RealityObject> GetRealityObjectListByRSO(DynamicDictionary parameters)
        {
            var selectedHouses = parameters.GetAs<string>("selectedHouses").ToLongArray();
            var contragentId = parameters.GetAs<long>("rsoId");

            var result = new List<RealityObject>();
           
            var realObjPublicServiceOrgDomain = this.Container.ResolveDomain<RealObjPublicServiceOrg>();

            try
            {
                //Поставщик ресурсов
                var publicServiceOrg = this.GetPublicServiceOrgByContagentId(contragentId);

                if (publicServiceOrg != null)
                {
                    result.AddRange(
                        realObjPublicServiceOrgDomain.GetAll()
                            .WhereIf(selectedHouses.Length > 0, x => selectedHouses.Contains(x.RealityObject.Id))
                            .Where(x => x.PublicServiceOrg.Id == publicServiceOrg.Id)
                            .Where(x => (!x.DateStart.HasValue || x.DateStart.Value <= DateTime.Now) 
                                && (!x.DateEnd.HasValue || x.DateEnd >= DateTime.Now))
                            .Select(x => x.RealityObject)
                            .ToList());
                }
            }
            finally
            {
                this.Container.Release(realObjPublicServiceOrgDomain);
            }

            return result;
        }

        private List<RealityObject> GetRealityObjectListDefault(DynamicDictionary parameters)
        {
            var selectedHouses = parameters.GetAs<string>("selectedHouses").ToLongArray();
            var realityObjectDomain = this.Container.ResolveDomain<RealityObject>();

            try
            {
                return realityObjectDomain.GetAll()
                    .WhereIf(selectedHouses.Length > 0, x => selectedHouses.Contains(x.Id))
                    .ToList();
            }
            finally
            {
                this.Container.Release(realityObjectDomain);
            }
        }

        private ManagingOrganization GetManOrgByContagentId(long contragentId)
        {
            var manOrgDomain = this.Container.ResolveDomain<ManagingOrganization>();

            try
            {
                return
                    manOrgDomain.GetAll()
                        .Where(x => x.ActivityGroundsTermination == GroundsTermination.NotSet)
                        .FirstOrDefault(x => x.Contragent.Id == contragentId);
            }
            finally
            {
                this.Container.Release(manOrgDomain);
            }
        }

        private LocalGovernment GetLocalGovernmentByContagentId(long contragentId)
        {
            var localGovernmentDomain = this.Container.ResolveDomain<LocalGovernment>();

            try
            {
                return
                    localGovernmentDomain.GetAll()                       
                        .FirstOrDefault(x => x.Contragent.Id == contragentId);
            }
            finally
            {
                this.Container.Release(localGovernmentDomain);
            }
        }    

        private PublicServiceOrg GetPublicServiceOrgByContagentId(long contragentId)
        {
            var publicServiceOrgDomain = this.Container.ResolveDomain<PublicServiceOrg>();

            try
            {
                return publicServiceOrgDomain.GetAll()
                           .FirstOrDefault(x => x.Contragent.Id == contragentId);
            }
            finally
            {
                this.Container.Release(publicServiceOrgDomain);
            }
        }

        private nsiRef GetHouseState(RealityObject house)
        {
            var gkhState = house.ConditionHouse;
            var risState = this.GkhToRisConditionEnum(gkhState);

            return this.HouseStateDict[(int)risState];
        }

        private Ris.Enums.ConditionHouse GkhToRisConditionEnum(Gkh.Enums.ConditionHouse enumValue)
        {
            var intVaue = (int)enumValue;

            return (Ris.Enums.ConditionHouse)intVaue;
        }

        private decimal GetNonResidentialSquare(RealityObject house)
        {
            var totalSquare = house.AreaLivingNotLivingMkd;
            var livingSquare = house.AreaLiving;

            if (totalSquare.HasValue && livingSquare.HasValue)
            {
                var totalSquareValue = totalSquare.Value;
                var livingSquareValue = livingSquare.Value;

                if (totalSquareValue > livingSquareValue)
                {
                    return totalSquareValue - livingSquareValue;
                }
            }

            return 0m;
        }

        private class TehPassInfoProxy
        {
            public TehPassportValue EnergyDateValue { get; set; }
            public TehPassportValue EnergyCategoryValue { get; set; }
            public TehPassportValue BuiltUpAreaValue { get; set; }
            public TehPassportValue UndergroundAreaValue { get; set; }

            public DateTime? EnergyDate {
                get
                {
                    return this.EnergyDateValue != null? this.EnergyDateValue.Value.ToDateTime():
                    (DateTime?)null;
                }
            }
            public int? EnergyCategory {
                get
                {
                    return this.EnergyCategoryValue != null 
                        ? this.EnergyCategoryValue.Value.ToInt() 
                        : (int?)null;
                }
            }
            public decimal? BuiltUpArea {
                get
                {
                    return this.BuiltUpAreaValue != null 
                        ? this.BuiltUpAreaValue.Value.ToDecimal() 
                        : (decimal?)null;
                }
            }
            public decimal? UndergroundArea {
                get
                {
                    return this.UndergroundAreaValue != null 
                        ? this.UndergroundAreaValue.Value.ToDecimal() 
                        : (decimal?)null;
                }
            }
        }
    }
}
