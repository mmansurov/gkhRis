namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using Entities.HouseManagement;
    using Enums;
    using Ris.HouseManagement;

    /// <summary>
    /// Класс экспортер данных домов для полномочия РСО
    /// </summary>
    class ImportHouseRSODataMethod : ImportHouseBaseMethod<importHouseRSORequest>
    {
        /// <summary>
        /// Код метода
        /// </summary>
        public override string Code
        {
            get
            {
                return "importHouseRSOData";
            }
        }

        /// <summary>
        /// Наименование метода
        /// </summary>
        public override string Name
        {
            get
            {
                return "Импорт сведений о доме для полномочия РСО";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 29;
            }
        }

        /// <summary>
        /// Метод получения списка домов для импорта
        /// </summary>
        /// <returns>Список объектов для импорта</returns>
        protected override List<RisHouse> GetMainList()
        {
            var houseDomain = this.Container.ResolveDomain<RisHouse>();

            //            Для реализации выборки домов для отправки необходимо:
            //            Получить контрагента (поставщика информации) - поставщика жилищных/коммунальных услуг >> его актуальные договора >> дома по договорам
            //              1) пока не реализовано определение контрагента,
            //              2) договоров на поставки услуг, их связи с домами  (сущности жкх RealityObjectResOrg, ServiceOrgRealityObjectContract, ServiceOrgContract) пока нет в проекте Ris
            //              соответственно, пока выгружаются все дома - необходима доработка


            //TODO Сделать выборку домов в разрезе контрагента, актуальных договоров

            try
            {
                return houseDomain.GetAll().Take(50).ToList();
                //return houseDomain.GetAll().ToList();
            }
            finally
            {
                this.Container.Release(houseDomain);
            }
        }

        /// <summary>
        /// Проверка дома
        /// </summary>
        /// <param name="item">Дом</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(RisHouse item)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(item.FiasHouseGuid))
            {
                messages.Append("FIASHouseGuid ");
            }

            if (string.IsNullOrEmpty(item.ProjectTypeCode) && !string.IsNullOrEmpty(item.ProjectTypeGuid))
            {
                messages.Append("ProjectTypeCode ");
            }

            if (!string.IsNullOrEmpty(item.ProjectTypeCode) && string.IsNullOrEmpty(item.ProjectTypeGuid))
            {
                messages.Append("ProjectTypeGuid ");
            }

            if (string.IsNullOrEmpty(item.EnergyCategoryCode) && !string.IsNullOrEmpty(item.EnergyCategoryGuid))
            {
                messages.Append("EnergyCategoryCode ");
            }

            if (!string.IsNullOrEmpty(item.EnergyCategoryCode) && string.IsNullOrEmpty(item.EnergyCategoryGuid))
            {
                messages.Append("EnergyCategoryGuid ");
            }

            if (string.IsNullOrEmpty(item.OktmoCode) && !string.IsNullOrEmpty(item.OktmoName))
            {
                messages.Append("OktmoCode ");
            }

            if (string.IsNullOrEmpty(item.OlsonTZCode) || string.IsNullOrEmpty(item.OlsonTZGuid))
            {
                messages.Append("OlsonTZ ");
            }

            if (item.HouseType == HouseType.Apartment)
            {
                if (string.IsNullOrEmpty(item.OverhaulFormingKindCode)
                && !string.IsNullOrEmpty(item.OverhaulFormingKindGuid))
                {
                    messages.Append("OverhaulFormingKindCode ");
                }

                if (!string.IsNullOrEmpty(item.OverhaulFormingKindCode)
                    && string.IsNullOrEmpty(item.OverhaulFormingKindGuid))
                {
                    messages.Append("OverhaulFormingKindGuid ");
                }

                if (Decimal.Round(item.NonResidentialSquare, 2) < 0.01m)
                {
                    messages.Append("NonResidentialSquare ");
                }
            }

            if (item.HouseType == HouseType.Living)
            {
                if (string.IsNullOrEmpty(item.ResidentialHouseTypeCode)
                    && !string.IsNullOrEmpty(item.ResidentialHouseTypeGuid))
                {
                    messages.Append("ResidentialHouseTypeCode ");
                }

                if (!string.IsNullOrEmpty(item.ResidentialHouseTypeCode)
                    && string.IsNullOrEmpty(item.ResidentialHouseTypeGuid))
                {
                    messages.Append("ResidentialHouseTypeGuid ");
                }
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        /// <summary>
        /// Получить объект запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов для импорта (в текущем методе в списке 1 объект)</param>
        /// <returns>Объект запроса</returns>
        protected override importHouseRSORequest GetRequestObject(
            IEnumerable<RisHouse> listForImport)
        {
            var house = listForImport.First();
            object item = null;

            switch (house.HouseType)
            {
                case HouseType.Apartment:
                    item = this.CreateApartmentHouseRequest(house);
                    break;
                case HouseType.Living:
                    item = this.CreateLivingHouseRequest(house);
                    break;
            }

            return new importHouseRSORequest { Item = item };
        }

        /// <summary>
        /// Получить ответ от сервиса.
        /// </summary>
        /// <param name="request">Объект запроса</param>
        /// <returns>Ответ от сервиса</returns>
        protected override ImportResult GetRequestResult(importHouseRSORequest request)
        {
            ImportResult result = null;
            var requestHeader = this.RequestHeader;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importHouseRSOData(requestHeader, request, out result);
            }

            return result;
        }

        private importHouseRSORequestApartmentHouse CreateApartmentHouseRequest(RisHouse house)
        {
            object gknRelationship;

            if (string.IsNullOrEmpty(house.CadastralNumber))
            {
                gknRelationship = true;
            }
            else
            {
                gknRelationship = house.CadastralNumber;
            }

            var houseTransportGuid = Guid.NewGuid().ToString();

            var basicCharacteristics = new HouseBasicRSOType
            {
                Item = gknRelationship,
                FIASHouseGuid = house.FiasHouseGuid,               
                //ProjectSeries = house.ProjectSeries,
                //ProjectType = !string.IsNullOrEmpty(house.ProjectTypeCode) || !string.IsNullOrEmpty(house.ProjectTypeGuid) ?
                //        new nsiRef
                //        {
                //            Code = house.ProjectTypeCode,
                //            GUID = house.ProjectTypeGuid
                //        } : null,
                //Energy = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) || house.EnergyDate.HasValue ?
                //    new HouseBasicRSOTypeEnergy
                //    {
                //        EnergyDate = house.EnergyDate.GetValueOrDefault(),
                //        EnergyCategory = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) ?
                //                              new nsiRef
                //                              {
                //                                  Code = house.EnergyCategoryCode,
                //                                  GUID = house.EnergyCategoryGuid
                //                              } : null
                //    } : null,
                OKTMO = !string.IsNullOrEmpty(house.OktmoCode) ?
                    new OKTMORefType
                    {
                        code = house.OktmoCode,
                        name = house.OktmoName
                    } : null,
                OlsonTZ = new nsiRef
                {
                    Code = house.OlsonTZCode,
                    GUID = house.OlsonTZGuid
                }
            };

            var houseToCreate = new importHouseRSORequestApartmentHouseApartmentHouseToCreate
            {
                BasicCharacteristicts = basicCharacteristics,
                //MinFloorCount = (sbyte)house.MinFloorCount.GetValueOrDefault(),
                //OverhaulYear = house.OverhaulYear.GetValueOrDefault(),
                //OverhaulFormingKind = !string.IsNullOrEmpty(house.OverhaulFormingKindCode) || !string.IsNullOrEmpty(house.OverhaulFormingKindGuid) ?
                //                             new nsiRef
                //                             {
                //                                 Code = house.OverhaulFormingKindCode,
                //                                 GUID = house.OverhaulFormingKindGuid
                //                             } : null,
                TransportGUID = houseTransportGuid
            };

            this.housesByTransportGuid.Add(houseTransportGuid, house);

            return new importHouseRSORequestApartmentHouse
            {
                Item = houseToCreate,
                NonResidentialPremiseToCreate = this.CreateApartmentHouseNonResidentialPremiseToCreateRequests(house).ToArray(),
                EntranceToCreate = this.CreateApartmentHouseEntranceToCreateRequests(house).ToArray(),
                ResidentialPremises = this.CreateApartmentHouseResidentialPremisesRequests(house).ToArray()
            };
        }

        private List<importHouseRSORequestApartmentHouseNonResidentialPremiseToCreate> CreateApartmentHouseNonResidentialPremiseToCreateRequests(RisHouse house)
        {
            var premises = this.nonResidentialPremisesList.Where(x => x.ApartmentHouse == house).ToList();
            var result = new List<importHouseRSORequestApartmentHouseNonResidentialPremiseToCreate>();
            var transportGuid = Guid.NewGuid().ToString();

            this.ClearObjectList(premises, this.CheckNonResidentialPremise);

            foreach (var premise in premises)
            {
                object gknRelationship;

                if (string.IsNullOrEmpty(premise.CadastralNumber))
                {
                    gknRelationship = true;
                }
                else
                {
                    gknRelationship = house.CadastralNumber;
                }

                result.Add(new importHouseRSORequestApartmentHouseNonResidentialPremiseToCreate
                {
                    Item = gknRelationship,
                    PremisesNum = premise.PremisesNum,
                    TotalArea = Decimal.Round(premise.TotalArea.GetValueOrDefault(), 2),
                    TransportGUID = transportGuid
                });

                this.nonResidentialPremisesByTransportGuid.Add(transportGuid, premise);
            }

            return result;
        }

        private List<importHouseRSORequestApartmentHouseEntranceToCreate> CreateApartmentHouseEntranceToCreateRequests(
            RisHouse house)
        {
            var entrances = this.entranceList.Where(x => x.ApartmentHouse == house).ToList();
            var result = new List<importHouseRSORequestApartmentHouseEntranceToCreate>();
            var transportGuid = Guid.NewGuid().ToString();

            this.ClearObjectList(entrances, this.CheckEntrance);

            foreach (var entrance in entrances)
            {
                result.Add(new importHouseRSORequestApartmentHouseEntranceToCreate
                {
                    EntranceNum = (sbyte)entrance.EntranceNum,
                    TransportGUID = transportGuid
                });

                this.entrancesByTransportGuid.Add(transportGuid, entrance);
            }

            return result;
        }

        private List<importHouseRSORequestApartmentHouseResidentialPremises> CreateApartmentHouseResidentialPremisesRequests(RisHouse house)
        {
            var residentialPremises = this.residentialPremisesList.Where(x => x.ApartmentHouse == house).ToList();
            var result = new List<importHouseRSORequestApartmentHouseResidentialPremises>();
            var transportGuid = Guid.NewGuid().ToString();

            this.ClearObjectList(residentialPremises, this.CheckResidentialPremise);

            foreach (var residentialPremise in residentialPremises)
            {
                object gknRelationship;

                if (string.IsNullOrEmpty(residentialPremise.CadastralNumber))
                {
                    gknRelationship = true;
                }
                else
                {
                    gknRelationship = house.CadastralNumber;
                }

                result.Add(new importHouseRSORequestApartmentHouseResidentialPremises
                {
                    Item = new importHouseRSORequestApartmentHouseResidentialPremisesResidentialPremisesToCreate
                    {
                        Item = gknRelationship,
                        PremisesNum = residentialPremise.PremisesNum,
                        EntranceNum = (sbyte)residentialPremise.EntranceNum,
                        PremisesCharacteristic = !string.IsNullOrEmpty(residentialPremise.PremisesCharacteristicCode) || !string.IsNullOrEmpty(residentialPremise.PremisesCharacteristicGuid) ?
                            new nsiRef
                            {
                                Code = residentialPremise.PremisesCharacteristicCode,
                                GUID = residentialPremise.PremisesCharacteristicGuid
                            }: null,
                        TotalArea = Decimal.Round(residentialPremise.TotalArea.GetValueOrDefault(), 2),                      
                        //ResidentialHouseType = !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(residentialPremise.ResidentialHouseTypeGuid) ?
                        //                                        new nsiRef
                        //                                        {
                        //                                            Code = residentialPremise.ResidentialHouseTypeCode,
                        //                                            GUID = residentialPremise.ResidentialHouseTypeGuid
                        //                                        } : null,
                        TransportGUID = transportGuid
                    },
                    LivingRoomToCreate = this.CreateApartmentHouseResidentialPremisesLivingRoomToCreateRequest(residentialPremise).ToArray()
                });

                this.residentialPremisesByTransportGuid.Add(transportGuid, residentialPremise);
            }

            return result;
        }

        private List<importHouseRSORequestApartmentHouseResidentialPremisesLivingRoomToCreate>
           CreateApartmentHouseResidentialPremisesLivingRoomToCreateRequest(ResidentialPremises premise)
        {
            var livingRooms = this.livingRoomList.Where(x => x.ResidentialPremises == premise).ToList();
            var transportGuid = Guid.NewGuid().ToString();
            var result = new List<importHouseRSORequestApartmentHouseResidentialPremisesLivingRoomToCreate>();

            this.ClearObjectList(livingRooms, this.CheckLivingRoom);

            foreach (var livingRoom in livingRooms)
            {
                object gknRelationship;

                if (string.IsNullOrEmpty(livingRoom.CadastralNumber))
                {
                    gknRelationship = true;
                }
                else
                {
                    gknRelationship = livingRoom.CadastralNumber;
                }

                result.Add(new importHouseRSORequestApartmentHouseResidentialPremisesLivingRoomToCreate
                {
                    Item = gknRelationship,
                    RoomNumber = livingRoom.RoomNumber,
                    Square = livingRoom.Square.GetValueOrDefault(),
                    TransportGUID = transportGuid
                });

                this.livingRoomsByTransportGuid.Add(transportGuid, livingRoom);
            }

            return result;
        }

        private importHouseRSORequestLivingHouse CreateLivingHouseRequest(RisHouse house)
        {
            object gknRelationship;

            if (string.IsNullOrEmpty(house.CadastralNumber))
            {
                gknRelationship = true;
            }
            else
            {
                gknRelationship = house.CadastralNumber;
            }

            var houseTransportGuid = Guid.NewGuid().ToString();

            var basicCharacteristicts = new HouseBasicRSOType
            {
                Item = gknRelationship,
                FIASHouseGuid = house.FiasHouseGuid,
                //ProjectSeries = house.ProjectSeries,
                //ProjectType = !string.IsNullOrEmpty(house.ProjectTypeCode) || !string.IsNullOrEmpty(house.ProjectTypeGuid) ?
                //                                    new nsiRef
                //                                    {
                //                                        Code = house.ProjectTypeCode,
                //                                        GUID = house.ProjectTypeGuid
                //                                    } : null,
                //Energy = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) || house.EnergyDate.HasValue ?
                //                                    new HouseBasicRSOTypeEnergy
                //                                    {
                //                                        EnergyDate = house.EnergyDate.GetValueOrDefault(),
                //                                        EnergyCategory = !string.IsNullOrEmpty(house.EnergyCategoryCode) || !string.IsNullOrEmpty(house.EnergyCategoryGuid) ?
                //                                                                new nsiRef
                //                                                                {
                //                                                                    Code = house.EnergyCategoryCode,
                //                                                                    GUID = house.EnergyCategoryGuid
                //                                                                } : null,
                //                                        //ConfirmDoc = 
                //                                    } : null,
                OKTMO = !string.IsNullOrEmpty(house.OktmoCode) ?
                                                        new OKTMORefType
                                                        {
                                                            code = house.OktmoCode,
                                                            name = house.OktmoName
                                                        } : null,
                OlsonTZ = new nsiRef
                {
                    Code = house.OlsonTZCode,
                    GUID = house.OlsonTZGuid
                }
            };

            var houseToCreate = new importHouseRSORequestLivingHouseLivingHouseToCreate
            {
                BasicCharacteristicts = basicCharacteristicts,
                //ResidentialHouseType = !string.IsNullOrEmpty(house.ResidentialHouseTypeCode) || !string.IsNullOrEmpty(house.ResidentialHouseTypeGuid) ?
                //                                           new nsiRef
                //                                           {
                //                                               Code = house.ResidentialHouseTypeCode,
                //                                               GUID = house.ResidentialHouseTypeGuid
                //                                           } : null,
                TransportGUID = houseTransportGuid
            };

            this.housesByTransportGuid.Add(houseTransportGuid, house);

            return new importHouseRSORequestLivingHouse
            {
                Item = houseToCreate,
                LivingRoomToCreate = this.CreateLivingHouseLivingRoomToCreateRequest(house).ToArray()
            };
        }

        private List<importHouseRSORequestLivingHouseLivingRoomToCreate> CreateLivingHouseLivingRoomToCreateRequest(
           RisHouse house)
        {
            var livingRooms = this.livingRoomList.Where(x => x.House == house).ToList();
            var result = new List<importHouseRSORequestLivingHouseLivingRoomToCreate>();
            var transportGuid = Guid.NewGuid().ToString();

            this.ClearObjectList(livingRooms, this.CheckLivingRoom);

            foreach (var livingRoom in livingRooms)
            {
                object gknRelationship;

                if (string.IsNullOrEmpty(livingRoom.CadastralNumber))
                {
                    gknRelationship = true;
                }
                else
                {
                    gknRelationship = livingRoom.CadastralNumber;
                }

                result.Add(new importHouseRSORequestLivingHouseLivingRoomToCreate
                {
                    Item = gknRelationship,
                    RoomNumber = livingRoom.RoomNumber,
                    Square = livingRoom.Square.GetValueOrDefault(),
                    TransportGUID = transportGuid
                });

                this.livingRoomsByTransportGuid.Add(transportGuid, livingRoom);
            }

            return result;
        }

        private CheckingResult CheckNonResidentialPremise(NonResidentialPremises premise)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(premise.PremisesNum))
            {
                messages.Append("PremisesNum ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        private CheckingResult CheckEntrance(RisEntrance entrance)
        {
            StringBuilder messages = new StringBuilder();

            if (!entrance.EntranceNum.HasValue)
            {
                messages.Append("EntranceNum ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        private CheckingResult CheckResidentialPremise(ResidentialPremises premise)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(premise.PremisesNum))
            {
                messages.Append("PremisesNum ");
            }

            if (!premise.EntranceNum.HasValue)
            {
                messages.Append("EntranceNum ");
            }

            if (string.IsNullOrEmpty(premise.PremisesCharacteristicCode)
                && !string.IsNullOrEmpty(premise.PremisesCharacteristicGuid))
            {
                messages.Append("PremisesCharacteristicCode ");
            }

            if (!string.IsNullOrEmpty(premise.PremisesCharacteristicCode)
                && string.IsNullOrEmpty(premise.PremisesCharacteristicGuid))
            {
                messages.Append("PremisesCharacteristicGuid ");
            }

            if (string.IsNullOrEmpty(premise.ResidentialHouseTypeCode)
                && !string.IsNullOrEmpty(premise.ResidentialHouseTypeGuid))
            {
                messages.Append("ResidentialHouseTypeCode ");
            }

            if (!string.IsNullOrEmpty(premise.ResidentialHouseTypeCode)
                && string.IsNullOrEmpty(premise.ResidentialHouseTypeGuid))
            {
                messages.Append("ResidentialHouseTypeGuid ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        private CheckingResult CheckLivingRoom(LivingRoom livingRoom)
        {
            StringBuilder messages = new StringBuilder();

            if (string.IsNullOrEmpty(livingRoom.RoomNumber))
            {
                messages.Append("RoomNumber ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }
    }
}
