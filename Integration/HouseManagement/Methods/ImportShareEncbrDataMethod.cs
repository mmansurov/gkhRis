namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.HouseManagement;
    using Ris.HouseManagement;

    /// <summary>
    /// Метод передачи данных о жилищном фонде о поставщиках информации
    /// </summary>
    public class ImportShareEncbrDataMethod : GisIntegrationHouseManagementMethod<RisShare, importShareEncbrDataRequest>
    {
        private Dictionary<long, List<RisEcnbr>> ecnbrsByShareId = null;
        private Dictionary<long, List<RisShareEncbrLivingHouse>> livingHouseEcnbrs = null;
        private Dictionary<long, List<RisShareEncbrLivingHouse>> livingHouseShares = null;
        private Dictionary<long, List<RisShareEncbrLivingRoom>> livingRoomEcnbrs = null;
        private Dictionary<long, List<RisShareEncbrLivingRoom>> livingRoomShares = null;
        private Dictionary<long, List<RisShareEncbrNonResPrem>> nonResPremEcnbrs = null;
        private Dictionary<long, List<RisShareEncbrNonResPrem>> nonResPremShares = null;
        private Dictionary<long, List<RisShareEncbrResidentialPremises>> residentialPremisesEcnbrs = null;
        private Dictionary<long, List<RisShareEncbrResidentialPremises>> residentialPremisesShares = null;
        private Dictionary<long, List<RisShareInd>> shareInds = null;
        private Dictionary<long, List<RisEcnbrInd>> ecnbrInds = null;

        private readonly Dictionary<string, RisShare> sharesByTransportGuid = new Dictionary<string, RisShare>();
        private readonly Dictionary<string, RisEcnbr> ecnbrsByTransportGuid = new Dictionary<string, RisEcnbr>();
        private readonly List<RisShare> sharesToSave = new List<RisShare>();
        private readonly List<RisEcnbr> ecnbrsToSave = new List<RisEcnbr>();

        protected override int ProcessedObjects
        {
            get { return this.sharesToSave.Count; }
        }

        public override string Code
        {
            get { return "importShareEncbrData"; }
        }

        public override string Name
        {
            get { return "Импорт данных о жилищном фонде о поставщиках информации"; }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 32;
            }
        }

        protected override int Portion
        {
            get { return 1000; }
        }

        protected override IList<RisShare> MainList { get; set; }

        protected override void Prepare()
        {
            var shareDomain = this.Container.ResolveDomain<RisShare>();
            var ecnbrDomain = this.Container.ResolveDomain<RisEcnbr>();
            var shareEncbrLivingHouseDomain = this.Container.ResolveDomain<RisShareEncbrLivingHouse>();
            var shareEncbrLivingRoomDomain = this.Container.ResolveDomain<RisShareEncbrLivingRoom>();
            var shareEncbrNonResPremDomain = this.Container.ResolveDomain<RisShareEncbrNonResPrem>();
            var shareEncbrResidentialPremisesDomain = this.Container.ResolveDomain<RisShareEncbrResidentialPremises>();
            var shareIndDomain = this.Container.ResolveDomain<RisShareInd>();
            var ecnbrIndDomain = this.Container.ResolveDomain<RisEcnbrInd>();

            try
            {
                this.MainList = shareDomain.GetAll().ToList();
                this.ecnbrsByShareId = ecnbrDomain.GetAll()
                    .GroupBy(x => x.Share.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());

                this.livingHouseEcnbrs = shareEncbrLivingHouseDomain.GetAll()
                    .Where(x => x.Ecnbr != null)
                    .GroupBy(x => x.Ecnbr.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());
                this.livingHouseShares = shareEncbrLivingHouseDomain.GetAll()
                   .Where(x => x.Share != null)
                   .GroupBy(x => x.Share.Id)
                   .ToDictionary(x => x.Key, y => y.ToList());

                this.livingRoomEcnbrs = shareEncbrLivingRoomDomain.GetAll()
                    .Where(x => x.Ecnbr != null)
                    .GroupBy(x => x.Ecnbr.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());
                this.livingRoomShares = shareEncbrLivingRoomDomain.GetAll()
                    .Where(x => x.Share != null)
                    .GroupBy(x => x.Share.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());

                this.nonResPremEcnbrs = shareEncbrNonResPremDomain.GetAll()
                    .Where(x => x.Ecnbr != null)
                    .GroupBy(x => x.Ecnbr.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());
                this.nonResPremShares = shareEncbrNonResPremDomain.GetAll()
                    .Where(x => x.Share != null)
                    .GroupBy(x => x.Share.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());

                this.residentialPremisesEcnbrs = shareEncbrResidentialPremisesDomain.GetAll()
                    .Where(x => x.Ecnbr != null)
                    .GroupBy(x => x.Ecnbr.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());
                this.residentialPremisesShares = shareEncbrResidentialPremisesDomain.GetAll()
                    .Where(x => x.Share != null)
                    .GroupBy(x => x.Share.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());

                this.shareInds = shareIndDomain.GetAll()
                    .GroupBy(x => x.Share.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());
                this.ecnbrInds = ecnbrIndDomain.GetAll()
                    .GroupBy(x => x.Ecnbr.Id)
                    .ToDictionary(x => x.Key, y => y.ToList());
            }
            finally
            {
                this.Container.Release(shareDomain); 
                this.Container.Release(ecnbrDomain);
                this.Container.Release(shareEncbrLivingHouseDomain);
                this.Container.Release(shareEncbrLivingRoomDomain);
                this.Container.Release(shareEncbrNonResPremDomain);
                this.Container.Release(shareEncbrResidentialPremisesDomain);
                this.Container.Release(shareIndDomain);
                this.Container.Release(ecnbrIndDomain);
            }
        }

        protected override importShareEncbrDataRequest GetRequestObject(IEnumerable<RisShare> listForImport)
        {
            var importShareRequestList = new List<importShareEncbrDataRequestShareEnc>();

            foreach (var share in listForImport)
            {
                long shareId = share.Id;

                string checkShareLog = this.CheckShare(share);
                if (!string.IsNullOrWhiteSpace(checkShareLog))
                {
                    this.AddLineToLog("Доля собственности", shareId, "Не загружен", checkShareLog);
                    continue;
                }

                var shareEnc = new importShareEncbrDataRequestShareEncShare
                {
                    TransportGUID = Guid.NewGuid().ToString(),
                    Item = new ShareDataType
                    {
                        IsPrivatized = share.IsPrivatized != null && (bool)share.IsPrivatized,
                        RealEstateType = new ShareDataTypeRealEstateType
                        {
                            Items = this.GetShareRealEstateType(shareId)
                        },
                        Terminated = new ShareDataTypeTerminated
                        {
                            TermDate = share.TermDate ?? DateTime.MinValue
                        },
                        Owners = new ShareDataTypeOwners
                        {
                            Items = this.GetShareOwner(share)
                        }
                    }
                };

                var ecnbrList = new List<importShareEncbrDataRequestShareEncEcnbr>();

                if (this.ecnbrsByShareId.ContainsKey(shareId))
                {
                    foreach (var ecnbr in this.ecnbrsByShareId[shareId])
                    {
                        string checkEcnbrLog = this.CheckEcnbr(ecnbr);
                        if (!string.IsNullOrWhiteSpace(checkEcnbrLog))
                        {
                            this.AddLineToLog("Доля обременения", ecnbr.Id, "Не загружена", checkEcnbrLog);
                            continue;
                        }

                        var ecnbrToAdd = new importShareEncbrDataRequestShareEncEcnbr
                        {
                            TransportGUID = Guid.NewGuid().ToStr(),
                            Item = new EncbrDataToCreate
                            {
                                Items = this.GetEcnbrHouse(ecnbr.Id),
                                EndDate = ecnbr.EndDate ?? DateTime.MinValue,
                                EncbrKind = new nsiRef
                                {
                                    Code = ecnbr.KindCode,
                                    GUID = ecnbr.KindGuid
                                },
                                EncbrSubject = new EncbrDataTypeEncbrSubject
                                {
                                    Items = this.GetEcnbrSubject(ecnbr)
                                },
                                Item = !string.IsNullOrWhiteSpace(ecnbr.Share.Guid) ? ecnbr.Share.Guid : shareEnc.TransportGUID,
                                ItemElementName = !string.IsNullOrWhiteSpace(ecnbr.Share.Guid) ? ItemChoiceType1.ShareGUID : ItemChoiceType1.TransportGUID
                            }
                        };

                        this.ecnbrsByTransportGuid.Add(ecnbrToAdd.TransportGUID, ecnbr);
                        ecnbrList.Add(ecnbrToAdd);
                    }
                }

                importShareRequestList.Add(new importShareEncbrDataRequestShareEnc
                {
                    Share = shareEnc,
                    Ecnbr = ecnbrList.ToArray()
                });

                this.sharesByTransportGuid.Add(shareEnc.TransportGUID, share);
                this.CountObjects++;
            }

            return new importShareEncbrDataRequest
            {
                ShareEnc = importShareRequestList.ToArray()
            };
        }

        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (this.sharesByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                var share = this.sharesByTransportGuid[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;
                    var errorNotation = string.Empty;

                    if (error != null)
                    {
                        errorNotation = error.Description;
                    }

                    this.AddLineToLog("Доля собственности", share.Id, "Не загружена", errorNotation);
                    return;
                }

                share.Guid = responseItem.GUID;
                this.sharesToSave.Add(share);

                this.AddLineToLog("Доля собственности", share.Id, "Загружена", responseItem.GUID);
                return;
            }

            if (this.ecnbrsByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                var ecnbr = this.ecnbrsByTransportGuid[responseItem.TransportGUID];
                ecnbr.Guid = responseItem.GUID;
                this.ecnbrsToSave.Add(ecnbr);

                this.AddLineToLog("Доля обременения", ecnbr.Id, "Загружена", responseItem.GUID);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.sharesToSave, 1000, true, true);
            TransactionHelper.InsertInManyTransactions(this.Container, this.ecnbrsToSave, 1000, true, true);
        }

        protected override ImportResult1 GetRequestResult1(importShareEncbrDataRequest request)
        {
            ImportResult1 result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importShareEncbrData(this.RequestHeader, request, out result);
            }

            return result;
        }

        private string CheckShare(RisShare share)
        {
            string result = string.Empty;

            if (share.RisShareContragent == null && !this.shareInds.ContainsKey(share.Id))
            {
                result = "OWNERS";
            }

            return result;
        }

        private string CheckEcnbr(RisEcnbr ecnbr)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(ecnbr.KindCode))
            {
                result = "ENCBRKINDCODE ";
            }

            if (string.IsNullOrWhiteSpace(ecnbr.KindGuid))
            {
                result += "ENCBRKINDGUID ";
            }

            if (ecnbr.RisEcnbrContragent == null && !this.shareInds.ContainsKey(ecnbr.Id))
            {
                result += "ENCBRSUBJECT";
            }

            return result;
        }

        private object[] GetEcnbrSubject(RisEcnbr ecnbr)
        {
            List<object> result = new List<object>();

            if (ecnbr.RisEcnbrContragent != null)
            {
                result.Add(new RegOrgType
                {
                    orgRootEntityGUID = ecnbr.RisEcnbrContragent.OrgRootEntityGuid
                });
            }
            else if (this.ecnbrInds.ContainsKey(ecnbr.Id))
            {
                foreach (var ind in this.ecnbrInds[ecnbr.Id])
                {
                    object item = null;

                    if (ind.Ind != null)
                    {
                        if (!string.IsNullOrWhiteSpace(ind.Ind.Snils))
                        {
                            item = ind.Ind.Snils;
                        }
                        else if (!string.IsNullOrWhiteSpace(ind.Ind.IdNumber))
                        {
                            item = new ID
                            {
                                Type = new nsiRef
                                {
                                    Code = ind.Ind.IdTypeCode,
                                    GUID = ind.Ind.IdTypeGuid
                                },
                                Series = ind.Ind.IdSeries,
                                Number = ind.Ind.IdNumber,
                                IssueDate = ind.Ind.IdIssueDate ?? DateTime.MinValue
                            };
                        }

                    }

                    result.Add(new ShareDataTypeOwnersInd
                    {
                        Surname = ind.Ind != null ? ind.Ind.Surname : string.Empty,
                        FirstName = ind.Ind != null ? ind.Ind.FirstName : string.Empty,
                        Patronymic = ind.Ind != null ? ind.Ind.Patronymic : string.Empty,
                        Sex = (IndTypeSex)Enum.Parse(typeof(IndTypeSex), ind.Ind != null ? ind.Ind.Sex.ToString() : "M"),
                        DateOfBirth = ind.Ind != null ? (ind.Ind.DateOfBirth ?? DateTime.MinValue) : DateTime.MinValue,
                        Item = item,
                        PlaceBirth = ind.Ind != null ? ind.Ind.PlaceBirth : string.Empty,
                        IsRegistered = ind.Ind != null && (ind.Ind.IsRegistered ?? false),
                        IsResides = ind.Ind != null && (ind.Ind.IsResides ?? false)
                    });
                }
            }

            return result.ToArray();
        }

        private object[] GetShareOwner(RisShare share)
        {
            List<object> result = new List<object>();

            if (share.RisShareContragent != null)
            {
                result.Add(new RegOrgType
                {
                    orgRootEntityGUID = share.RisShareContragent.OrgRootEntityGuid
                });
            }
            else if (this.shareInds.ContainsKey(share.Id))
            {
                foreach (var ind in this.shareInds[share.Id])
                {

                    object item = null;

                    if (ind.Ind != null)
                    {
                        if (!string.IsNullOrWhiteSpace(ind.Ind.Snils))
                        {
                            item = ind.Ind.Snils;
                        }
                        else if (!string.IsNullOrWhiteSpace(ind.Ind.IdNumber))
                        {
                            item = new ID
                            {
                                Type = new nsiRef
                                {
                                    Code = ind.Ind.IdTypeCode,
                                    GUID = ind.Ind.IdTypeGuid
                                },
                                Series = ind.Ind.IdSeries,
                                Number = ind.Ind.IdNumber,
                                IssueDate = ind.Ind.IdIssueDate ?? DateTime.MinValue
                            };
                        }

                    }

                    result.Add(new ShareDataTypeOwnersInd
                    {
                        Surname = ind.Ind != null ? ind.Ind.Surname : string.Empty,
                        FirstName = ind.Ind != null ? ind.Ind.FirstName : string.Empty,
                        Patronymic = ind.Ind != null ? ind.Ind.Patronymic : string.Empty,
                        Sex = (IndTypeSex)Enum.Parse(typeof(IndTypeSex), ind.Ind != null ? ind.Ind.Sex.ToString() : "M"),
                        DateOfBirth = ind.Ind != null ? (ind.Ind.DateOfBirth ?? DateTime.MinValue) : DateTime.MinValue,
                        Item = item,
                        PlaceBirth = ind.Ind != null ? ind.Ind.PlaceBirth : string.Empty,
                        IsRegistered = ind.Ind != null && (ind.Ind.IsRegistered ?? false),
                        IsResides = ind.Ind != null && (ind.Ind.IsResides ?? false)
                    });
                }
            }

            return result.ToArray();
        }

        private object[] GetShareRealEstateType(long shareId)
        {
            List<object> result = new List<object>();

            if (this.livingHouseShares.ContainsKey(shareId))
            {
                var house = this.livingHouseShares[shareId][0];
                result.Add(new ShareDataTypeRealEstateTypeLivingHouse
                {
                    FIASHouseGuid = house.House != null ? house.House.FiasHouseGuid : string.Empty,
                    Fraction = new PartSizeTypeFraction
                    {
                        FracPart = house.FracPart,
                        IntPart = house.IntPart
                    }
                });
            }
            else if (this.nonResPremShares.ContainsKey(shareId))
            {
                foreach (var perm in this.nonResPremShares[shareId])
                {
                    result.Add(new ShareDataTypeRealEstateTypeNonResidentialPremises
                    {
                        NonResidentialPremisesGUID = perm.NonResidentialPremises != null ? perm.NonResidentialPremises.Guid : string.Empty,
                        Fraction = new PartSizeTypeFraction
                        {
                            IntPart = perm.IntPart,
                            FracPart = perm.FracPart
                        }
                    });
                }
            }
            else
            {
                var resPrems = new List<ShareDataTypeRealEstateTypePremisesAndRoomsResidentialPremises>();
                var livRooms = new List<ShareDataTypeRealEstateTypePremisesAndRoomsLivingRoom>();

                if (this.residentialPremisesShares.ContainsKey(shareId))
                {
                    foreach (var resPrem in this.residentialPremisesShares[shareId])
                    {
                        resPrems.Add(new ShareDataTypeRealEstateTypePremisesAndRoomsResidentialPremises
                        {
                            ResidentialPremisesGUID = resPrem.ResidentialPremises != null ? resPrem.ResidentialPremises.Guid : string.Empty,
                            Fraction = new PartSizeTypeFraction
                            {
                                IntPart = resPrem.IntPart,
                                FracPart = resPrem.FracPart
                            }
                        });
                    }
                }

                if (this.livingRoomShares.ContainsKey(shareId))
                {
                    foreach (var liveRoom in this.livingRoomShares[shareId])
                    {
                        livRooms.Add(new ShareDataTypeRealEstateTypePremisesAndRoomsLivingRoom
                        {
                            LivingRoomGUID = liveRoom.LivingRoom != null ? liveRoom.LivingRoom.Guid : string.Empty,
                            Fraction = new PartSizeTypeFraction
                            {
                                IntPart = liveRoom.IntPart,
                                FracPart = liveRoom.FracPart
                            }
                        });
                    }
                }

                result.Add(new ShareDataTypeRealEstateTypePremisesAndRooms
                {
                    LivingRoom = livRooms.ToArray(),
                    ResidentialPremises = resPrems.ToArray()
                });
            }

            return result.ToArray();
        }

        private object[] GetEcnbrHouse(long ecnbrId)
        {
            List<object> result = new List<object>();

            if (this.livingHouseEcnbrs.ContainsKey(ecnbrId))
            {
                var house = this.livingHouseEcnbrs[ecnbrId][0];
                result.Add(new EncbrDataTypeLivingHouse
                {
                  // FIASHouseGuid = house.House != null ? house.House.FiasHouseGuid : string.Empty,
                    Fraction = new PartSizeTypeFraction
                    {
                        FracPart = house.FracPart,
                        IntPart = house.IntPart
                    }
                });
            }
            else if (this.nonResPremEcnbrs.ContainsKey(ecnbrId))
            {
                foreach (var perm in this.nonResPremEcnbrs[ecnbrId])
                {
                    result.Add(new EncbrDataTypeNonResidentialPremises
                    {
                        NonResidentialPremisesGUID = perm.NonResidentialPremises != null ? perm.NonResidentialPremises.Guid : string.Empty,
                        Fraction = new PartSizeTypeFraction
                        {
                            IntPart = perm.IntPart,
                            FracPart = perm.FracPart
                        }
                    });
                }
            }
            else
            {
                var resPrems = new List<EncbrDataTypePremisesAndRoomsResidentialPremises>();
                var livRooms = new List<EncbrDataTypePremisesAndRoomsLivingRoom>();

                if (this.residentialPremisesEcnbrs.ContainsKey(ecnbrId))
                {
                    foreach (var resPrem in this.residentialPremisesEcnbrs[ecnbrId])
                    {
                        resPrems.Add(new EncbrDataTypePremisesAndRoomsResidentialPremises
                        {
                            ResidentialPremisesGUID = resPrem.ResidentialPremises != null ? resPrem.ResidentialPremises.Guid : string.Empty,
                            Fraction = new PartSizeTypeFraction
                            {
                                IntPart = resPrem.IntPart,
                                FracPart = resPrem.FracPart
                            }
                        });
                    }
                }

                if (this.livingRoomEcnbrs.ContainsKey(ecnbrId))
                {
                    foreach (var liveRoom in this.livingRoomEcnbrs[ecnbrId])
                    {
                        livRooms.Add(new EncbrDataTypePremisesAndRoomsLivingRoom
                        {
                            LivingRoomGUID = liveRoom.LivingRoom != null ? liveRoom.LivingRoom.Guid : string.Empty,
                            Fraction = new PartSizeTypeFraction
                            {
                                IntPart = liveRoom.IntPart,
                                FracPart = liveRoom.FracPart
                            }
                        });
                    }
                }

                result.Add(new EncbrDataTypePremisesAndRooms
                {
                    LivingRoom = livRooms.ToArray(),
                    ResidentialPremises = resPrems.ToArray()
                });
            }

            return result.ToArray();
        }
    }
}
