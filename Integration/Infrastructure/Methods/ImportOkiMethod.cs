namespace Bars.Gkh.Ris.Integration.Infrastructure.Methods
{
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.Infrastructure;
    using Entities.Services;
    using Ris.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Attachment = Entities.Attachment;

    public class ImportOkiMethod : GisIntegrationInfrastructureMethod<RisRkiItem, importOKIRequest>
    {
        private readonly List<RisRkiItem> itemsToSave = new List<RisRkiItem>();
        private readonly Dictionary<string, RisRkiItem> itemsByTransportGuid = new Dictionary<string, RisRkiItem>();
        private Dictionary<long, List<RisHouseService>> servicesByItemId = new Dictionary<long, List<RisHouseService>>();
        private Dictionary<long, List<Attachment>> attachmentsByItemId = new Dictionary<long, List<Attachment>>();
        private Dictionary<long, List<Attachment>> attachmentsEnergyEfficiencyByItemId = new Dictionary<long, List<Attachment>>();
        private Dictionary<long, List<RisResource>> resourcesByItemId = new Dictionary<long, List<RisResource>>();
        private Dictionary<long, List<RisTransportationResources>> transportationResourcesByItemId = new Dictionary<long, List<RisTransportationResources>>();
        private Dictionary<long, List<RisNetPieces>> netPiecesByItemId = new Dictionary<long, List<RisNetPieces>>();
        private Dictionary<long, List<RisSourceOki>> sourceOkiByItemId = new Dictionary<long, List<RisSourceOki>>();
        private Dictionary<long, List<RisReceiverOki>> receiverOkiByItemId = new Dictionary<long, List<RisReceiverOki>>();

        protected override int ProcessedObjects
        {
            get { return this.itemsToSave.Count; }
        }

        public override string Code
        {
            get { return "importOKI"; }
        }

        public override string Name
        {
            get { return "Управление ОКИ в РКИ"; }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 22;
            }
        }

        protected override int Portion
        {
            get { return this.MainList.Count; }
        }

        protected override IList<RisRkiItem> MainList { get; set; }

        protected override void Prepare()
        {
            var itemsDomain = this.Container.ResolveDomain<RisRkiItem>();
            var serviceDomain = this.Container.ResolveDomain<RisHouseService>();
            var attachmentDomain = this.Container.ResolveDomain<RisRkiAttachment>();
            var attachmentEnergyEfficiencyDomain = this.Container.ResolveDomain<RisAttachmentsEnergyEfficiency>();
            var resourcesDomain = this.Container.ResolveDomain<RisResource>();
            var transportationResourcesDomain = this.Container.ResolveDomain<RisTransportationResources>();
            var netPiecesDomain = this.Container.ResolveDomain<RisNetPieces>();
            var sourceOkiDomain = this.Container.ResolveDomain<RisSourceOki>();
            var receiverOkiDomain = this.Container.ResolveDomain<RisReceiverOki>();

            try
            {
                this.MainList = itemsDomain.GetAll().ToList();

                var servicesByHouseId = serviceDomain.GetAll()
                    .Where(x => x.House != null)
                    .Select(x => new
                    {
                        HouseId = x.House.Id,
                        Service = x
                    })
                    .GroupBy(x => x.HouseId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Service).ToList());

                this.servicesByItemId = itemsDomain.GetAll()
                    .Where(x => x.House != null)
                    .ToArray()
                    .Select(x => new
                    {
                        x.Id,
                        Services = servicesByHouseId.Get(x.House.Id)
                    })
                    .GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.SelectMany(y => y.Services).ToList());

                this.attachmentsByItemId = attachmentDomain.GetAll()
                    .Where(x => x.Attachment != null && x.RkiItem != null)
                    .Select(x => new
                    {
                        x.RkiItem.Id,
                        Attach = x.Attachment
                    })
                    .ToArray()
                    .GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Attach).ToList());

                this.attachmentsEnergyEfficiencyByItemId = attachmentEnergyEfficiencyDomain.GetAll()
                    .Where(x => x.Attachment != null && x.RkiItem != null)
                    .Select(x => new
                    {
                        x.RkiItem.Id,
                        Attach = x.Attachment
                    })
                    .ToArray()
                    .GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Attach).ToList());

                this.resourcesByItemId = resourcesDomain.GetAll()
                    .Where(x => x.RkiItem != null)
                    .GroupBy(x => x.RkiItem.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());

                this.transportationResourcesByItemId = transportationResourcesDomain.GetAll()
                    .Where(x => x.RkiItem != null)
                    .GroupBy(x => x.RkiItem.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());

                this.netPiecesByItemId = netPiecesDomain.GetAll()
                    .Where(x => x.RkiItem != null)
                    .GroupBy(x => x.RkiItem.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());

                this.sourceOkiByItemId = sourceOkiDomain.GetAll()
                    .Where(x => x.RkiItem != null)
                    .GroupBy(x => x.RkiItem.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());

                this.receiverOkiByItemId = receiverOkiDomain.GetAll()
                    .Where(x => x.RkiItem != null)
                    .GroupBy(x => x.RkiItem.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
            finally
            {
                this.Container.Release(itemsDomain);
                this.Container.Release(serviceDomain);
                this.Container.Release(attachmentDomain);
                this.Container.Release(attachmentEnergyEfficiencyDomain);
                this.Container.Release(resourcesDomain);
                this.Container.Release(transportationResourcesDomain);
                this.Container.Release(netPiecesDomain);
                this.Container.Release(sourceOkiDomain);
                this.Container.Release(receiverOkiDomain);
            }
        }

        protected override CheckingResult CheckMainListItem(RisRkiItem item)
        {
            StringBuilder messages = new StringBuilder();

            if (item.Name.IsEmpty())
            {
                messages.Append("OKI/NAME ");
            }

            if (item.BaseCode.IsEmpty())
            {
                messages.Append("OKI/BASE/CODE ");
            }

            if (item.BaseGuid.IsEmpty())
            {
                messages.Append("OKI/BASE/GUID ");
            }

            if (!item.EndManagmentDate.HasValue &&
                (!item.IndefiniteManagement.HasValue || !((bool)item.IndefiniteManagement)))
            {
                messages.Append("OKI/ENDMANAGMENTDATE ");
            }

            if (item.TypeCode.IsEmpty())
            {
                messages.Append("OKI/OKITYPE/CODE ");
            }

            if (item.TypeGuid.IsEmpty())
            {
                messages.Append("OKI/OKITYPE/GUID ");
            }

            if ((item.WaterIntakeCode.IsEmpty() || item.WaterIntakeGuid.IsEmpty()) &&
                (item.ESubstationCode.IsEmpty() || item.ESubstationGuid.IsEmpty()) &&
                (item.PowerPlantCode.IsEmpty() || item.PowerPlantGuid.IsEmpty()) &&
                (item.FuelCode.IsEmpty() || item.FuelGuid.IsEmpty()) &&
                (item.GasNetworkCode.IsEmpty() || item.GasNetworkGuid.IsEmpty()))
            {
                messages.Append("OKI/OKITYPEITEM ");
            }

            if (!this.servicesByItemId.ContainsKey(item.Id))
            {
                messages.Append("OKI/SERVICES ");
            }

            if (item.OktmoCode.IsEmpty())
            {
                messages.Append("OKI/OKTMO/CODE ");
            }

            if (item.House == null || !item.House.UsedYear.HasValue)
            {
                messages.Append("OKI/COMMISSIONINGYEAR ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        protected override importOKIRequest GetRequestObject(IEnumerable<RisRkiItem> listForImport)
        {
            List<importOKIRequestRKIItem> itemsToSend = new List<importOKIRequestRKIItem>();

            foreach (RisRkiItem risItem in listForImport)
            {
                object item = true;
                object itemManagerOKI = true;

                if (risItem.EndManagmentDate.HasValue)
                {
                    item = (DateTime)risItem.EndManagmentDate;
                }

                if (risItem.RisRkiContragent != null)
                {
                    itemManagerOKI = new ManagerOKITypeRSO
                    {
                        RSOOrganizationGUID = risItem.RisRkiContragent.OrgRootEntityGuid,
                        Name = risItem.RisRkiContragent.FullName
                    };
                }

                importOKIRequestRKIItem newItem = new importOKIRequestRKIItem
                {
                    TransportGUID = Guid.NewGuid().ToString(),
                    Item = new importOKIRequestRKIItemOKI
                    {
                        Name = risItem.Name,
                        Base = new nsiRef
                        {
                            Code = risItem.BaseCode,
                            GUID = risItem.BaseGuid
                        },
                        AttachmentList = this.GetAttachmentList(risItem.Id),
                        Item = item,
                        ManagerOKI = new ManagerOKIType
                        {
                            Item = itemManagerOKI
                        },
                        OKIType = this.GetOKIType(risItem),
                        Services = this.GetServices(risItem.Id),
                        OKTMO = new OKTMORefType
                        {
                            code = risItem.OktmoCode,
                            name = risItem.OktmoName
                        },
                        Adress = risItem.House != null ? risItem.House.Adress : string.Empty,
                        CommissioningYear = (risItem.House != null && risItem.House.UsedYear.HasValue) ? (short)risItem.House.UsedYear : (short)0,
                        IndependentSource = risItem.IndependentSource ?? false,
                        Deterioration = risItem.Deterioration ?? 0m,
                        ObjectProperty = this.GetObjectProperty(risItem),
                        AddInfo = risItem.AddInfo,
                        AttachmentsEnergyEfficiency = this.GetAttachmentsEnergyEfficiency(risItem.Id)
                    }
                };

                itemsToSend.Add(newItem);

                this.CountObjects++;
                this.itemsByTransportGuid.Add(newItem.TransportGUID, risItem);
            }

            return new importOKIRequest
            {
                RKIItem = itemsToSend.ToArray()
            };
        }

        protected override ImportResult GetRequestResult(importOKIRequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importOKI(this.RequestHeader, request, out result);
            }

            return result;
        }

        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (this.itemsByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                var item = this.itemsByTransportGuid[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;
                    var errorNotation = string.Empty;

                    if (error != null)
                    {
                        errorNotation = error.Description;
                    }

                    this.AddLineToLog("Объект коммунальной инфраструктуры", item.Id, "Не загружен", errorNotation);
                    return;
                }

                item.Guid = responseItem.GUID;
                this.itemsToSave.Add(item);

                this.AddLineToLog("Объект коммунальной инфраструктуры", item.Id, "Загружен", responseItem.GUID);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.itemsToSave, 1000, true, true);
        }

        private InfrastriuctureTypeObjectProperty GetObjectProperty(RisRkiItem risItem)
        {
            InfrastriuctureTypeObjectProperty result = null;
            long id = risItem.Id;
            List<object> items = new List<object>();

            if (risItem.CountAccidents.HasValue)
            {
                items.Add(((int)risItem.CountAccidents).ToString());
            }
            else if (this.resourcesByItemId.ContainsKey(id))
            {
                foreach (var res in this.resourcesByItemId[id])
                {
                    items.Add(new InfrastriuctureTypeObjectPropertyResources
                    {
                        MunicipalResource = new nsiRef
                        {
                            Code = res.MunicipalResourceCode,
                            GUID = res.MunicipalResourceGuid,
                            Name = res.MunicipalResourceName
                        },
                        TotalLoad = res.TotalLoad ?? 0m,
                        IndustrialLoad = res.IndustrialLoad ?? 0m,
                        SocialLoad = res.SocialLoad ?? 0m,
                        PopulationLoad = res.PopulationLoad ?? 0m,
                        SetPower = res.SetPower ?? 0m,
                        SitingPower = res.SitingPower ?? 0m
                    });
                }
            }
            else if (this.transportationResourcesByItemId.ContainsKey(id))
            {
                foreach (var res in this.transportationResourcesByItemId[id])
                {
                    items.Add(new InfrastriuctureTypeObjectPropertyTransportationResources
                    {
                        MunicipalResource = new nsiRef
                        {
                            Code = res.MunicipalResourceCode,
                            GUID = res.MunicipalResourceGuid,
                            Name = res.MunicipalResourceName
                        },
                        TotalLoad = res.TotalLoad ?? 0m,
                        IndustrialLoad = res.IndustrialLoad ?? 0m,
                        SocialLoad = res.SocialLoad ?? 0m,
                        PopulationLoad = res.PopulationLoad ?? 0m,
                        VolumeLosses = res.VolumeLosses ?? 0m,
                        CoolantType = new nsiRef
                        {
                            GUID = res.CoolantGuid,
                            Code = res.CoolantCode,
                            Name = res.CoolantName
                        }
                    });
                }
            }
            else if (this.netPiecesByItemId.ContainsKey(id))
            {
                foreach (var res in this.netPiecesByItemId[id])
                {
                    items.Add(new InfrastriuctureTypeObjectPropertyNetPieces
                    {
                        Name = res.Name,
                        Diameter = res.Diameter ?? 0m,
                        Length = res.Length ?? 0m,
                        NeedReplaced = res.NeedReplaced ?? 0m,
                        Wearout = res.Wearout ?? 0m,
                        PressureType = new nsiRef
                        {
                            GUID = res.PressureGuid,
                            Code = res.PressureCode,
                            Name = res.PressureName
                        },
                        VoltageType = new nsiRef
                        {
                            GUID = res.VoltageGuid,
                            Code = res.VoltageCode,
                            Name = res.VoltageName
                        }
                    });
                }
            }
            else if (this.sourceOkiByItemId.ContainsKey(id))
            {
                InfrastriuctureTypeObjectPropertyOKILinks item = new InfrastriuctureTypeObjectPropertyOKILinks();
                List<string> sources = new List<string>();
                List<string> receivers = new List<string>();

                sources.AddRange(this.sourceOkiByItemId[id].Select(x => x.SourceOki));
                item.SourceOKI = sources.ToArray();

                if (this.receiverOkiByItemId.ContainsKey(id))
                {
                    receivers.AddRange(this.receiverOkiByItemId[id].Select(x => x.ReceiverOki));
                    item.ReceiverOKI = receivers.ToArray();
                }

                items.Add(item);
            }

            if (items.Count > 0)
            {
                result = new InfrastriuctureTypeObjectProperty
                {
                    Items = items.ToArray()
                };
            }

            return result;
        }

        private InfrastriuctureTypeOKIType GetOKIType(RisRkiItem risItem)
        {
            InfrastriuctureTypeOKIType result = new InfrastriuctureTypeOKIType();

            result.Item = new nsiRef
            {
                Code = risItem.TypeCode,
                GUID = risItem.TypeGuid
            };

            if (!risItem.WaterIntakeCode.IsEmpty() && !risItem.WaterIntakeGuid.IsEmpty())
            {
                result.Code = risItem.WaterIntakeCode;
                result.GUID = risItem.WaterIntakeGuid;
                result.ItemElementName = ItemChoiceType.WaterIntakeType;
            }
            else if (!risItem.ESubstationCode.IsEmpty() && !risItem.ESubstationGuid.IsEmpty())
            {
                result.Code = risItem.ESubstationCode;
                result.GUID = risItem.ESubstationGuid;
                result.ItemElementName = ItemChoiceType.ESubstationType;
            }
            else if (!risItem.PowerPlantCode.IsEmpty() && !risItem.PowerPlantGuid.IsEmpty())
            {
                result.Code = risItem.PowerPlantCode;
                result.GUID = risItem.PowerPlantGuid;
                result.ItemElementName = ItemChoiceType.PowerPlantType;
            }
            else if (!risItem.FuelCode.IsEmpty() && !risItem.FuelGuid.IsEmpty())
            {
                result.Code = risItem.FuelCode;
                result.GUID = risItem.FuelGuid;
                result.ItemElementName = ItemChoiceType.FuelType;
            }
            else if (!risItem.GasNetworkCode.IsEmpty() && !risItem.GasNetworkGuid.IsEmpty())
            {
                result.Code = risItem.GasNetworkCode;
                result.GUID = risItem.GasNetworkGuid;
                result.ItemElementName = ItemChoiceType.GasNetworkType;
            }

            return result;
        }

        private nsiRef[] GetServices(long id)
        {
            List<nsiRef> result = new List<nsiRef>();

            if (this.servicesByItemId.ContainsKey(id))
            {
                foreach (var service in this.servicesByItemId[id])
                {
                    result.Add(new nsiRef
                    {
                        GUID = service.MsTypeGuid,
                        Code = service.MsTypeCode
                    });
                }
            }

            return result.ToArray();
        }

        private AttachmentType[] GetAttachmentList(long id)
        {
            List<AttachmentType> result = new List<AttachmentType>();

            if (this.attachmentsByItemId.ContainsKey(id))
            {
                foreach (var attach in this.attachmentsByItemId[id])
                {
                    AttachmentType newAttach = new AttachmentType
                    {
                        Name = attach.Name,
                        Description = attach.Description,
                        Attachment = new Ris.Infrastructure.Attachment
                        {
                            AttachmentGUID = attach.Guid
                        },
                        AttachmentHASH = attach.Hash
                    };

                    result.Add(newAttach);
                }
            }

            return result.ToArray();
        }

        private AttachmentType[] GetAttachmentsEnergyEfficiency(long id)
        {
            List<AttachmentType> result = new List<AttachmentType>();

            if (this.attachmentsEnergyEfficiencyByItemId.ContainsKey(id))
            {
                foreach (var attach in this.attachmentsEnergyEfficiencyByItemId[id])
                {
                    AttachmentType newAttach = new AttachmentType
                    {
                        Name = attach.Name,
                        Description = attach.Description,
                        Attachment = new Ris.Infrastructure.Attachment
                        {
                            AttachmentGUID = attach.Guid
                        },
                        AttachmentHASH = attach.Hash
                    };

                    result.Add(newAttach);
                }
            }

            return result.ToArray();
        }
    }
}
