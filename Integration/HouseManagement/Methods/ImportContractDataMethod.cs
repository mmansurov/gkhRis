namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using Entities.HouseManagement;
    using Domain;
    using Enums.HouseManagement;
    using Ris.HouseManagement;
    using B4.Utils;

    using Bars.Gkh.Ris.Enums;

    using Attachment = Ris.HouseManagement.Attachment;

    /// <summary>
    /// Метод Импорт договора управления
    /// </summary>
    public class ImportContractDataMethod : GisIntegrationHouseManagementMethod<RisContract, importContractRequest>
    {
        private List<RisContract> contractsToSave = new List<RisContract>();
        private Dictionary<string, RisContract> contractsByTransportGuid = new Dictionary<string, RisContract>();
        private Dictionary<long, List<ContractObject>> contractObjectsByContractId = new Dictionary<long, List<ContractObject>>();
        private Dictionary<long, List<RisContractAttachment>> attachmentsByContractId = new Dictionary<long, List<RisContractAttachment>>();
        private Dictionary<long, List<RisProtocolOk>> protocolOksByContractId = new Dictionary<long, List<RisProtocolOk>>();
        private Dictionary<long, List<ProtocolMeetingOwner>> protocolMeetingOwnersByContractId = new Dictionary<long, List<ProtocolMeetingOwner>>();
        private Dictionary<long, List<HouseManService>> houseServByContractObjId = new Dictionary<long, List<HouseManService>>();
        private Dictionary<long, List<AddService>> addServByContractObjId = new Dictionary<long, List<AddService>>();

        protected override int ProcessedObjects
        {
            get { return this.contractsToSave.Count; }
        }
        public override string Code
        {
            get { return "importContractData"; }
        }

        public override string Name
        {
            get { return "Импорт договора управления"; }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get { return 10; }
        }

        protected override int Portion
        {
            get { return 100; }
        }

        protected override IList<RisContract> MainList { get; set; }

        protected override void Prepare()
        {
            var contractsDomain = this.Container.ResolveDomain<RisContract>();
            var contractObjectDomain = this.Container.ResolveDomain<ContractObject>();
            var attachmentDomain = this.Container.ResolveDomain<RisContractAttachment>();
            var protocolOkDomain = this.Container.ResolveDomain<RisProtocolOk>();
            var protocolMeetingOwnersDomain = this.Container.ResolveDomain<ProtocolMeetingOwner>();
            var houseManServiceDomain = this.Container.ResolveDomain<HouseManService>();
            var addServiceDomain = this.Container.ResolveDomain<AddService>();

            try
            {
                this.MainList = contractsDomain.GetAll()
                    .WhereIf(this.Contragent != null, x => x.Contragent == this.Contragent)
                    .ToList();

                this.contractObjectsByContractId = contractObjectDomain.GetAll()
                    .Where(x => x.Contract != null && this.MainList.Contains(x.Contract))
                    .GroupBy(x => x.Contract)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());

                this.attachmentsByContractId = attachmentDomain.GetAll()
                    .Where(x => x.Contract != null && this.MainList.Contains(x.Contract))
                    .GroupBy(x => x.Contract)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());

                this.protocolOksByContractId = protocolOkDomain.GetAll()
                    .Where(x => x.Contract != null && this.MainList.Contains(x.Contract))
                    .GroupBy(x => x.Contract)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());

                this.protocolMeetingOwnersByContractId = protocolMeetingOwnersDomain.GetAll()
                    .Where(x => x.Contract != null && this.MainList.Contains(x.Contract))
                    .GroupBy(x => x.Contract)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());

                this.houseServByContractObjId = houseManServiceDomain.GetAll()
                    .Where(x => x.ContractObject != null && this.MainList.Contains(x.ContractObject.Contract))
                    .GroupBy(x => x.ContractObject)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());

                this.addServByContractObjId = addServiceDomain.GetAll()
                    .Where(x => x.ContractObject != null && this.MainList.Contains(x.ContractObject.Contract))
                    .GroupBy(x => x.ContractObject)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());

            }
            finally
            {
                this.Container.Release(contractsDomain);
                this.Container.Release(attachmentDomain);
                this.Container.Release(contractObjectDomain);
                this.Container.Release(protocolOkDomain);
                this.Container.Release(protocolMeetingOwnersDomain);
                this.Container.Release(houseManServiceDomain);
                this.Container.Release(addServiceDomain);
            }
        }

        protected override CheckingResult CheckMainListItem(RisContract item)
        {
            StringBuilder messages = new StringBuilder();

            if (item.DocNum.IsEmpty())
            {
                messages.Append("PLACINGCONTRACT/DOCNUM ");
            }

            if (!item.SigningDate.HasValue)
            {
                messages.Append("PLACINGCONTRACT/SIGNINGDATE ");
            }

            if (!item.EffectiveDate.HasValue)
            {
                messages.Append("PLACINGCONTRACT/EFFECTIVEDATE ");
            }

            if (!item.PlanDateComptetion.HasValue)
            {
                messages.Append("PLACINGCONTRACT/PLANDATECOMPTETION ");
            }

            if (item.ContractBaseCode.IsEmpty())
            {
                messages.Append("PLACINGCONTRACT/CONTRACTBASE/CODE ");
            }

            if (item.ContractBaseGuid.IsEmpty())
            {
                messages.Append("PLACINGCONTRACT/CONTRACTBASE/GUID ");
            }

            if (!this.contractObjectsByContractId.ContainsKey(item.Id) ||
                (item.OwnersType == RisContractOwnersType.Owners && this.contractObjectsByContractId[item.Id].Count != 1))
            {
                messages.Append("PLACINGCONTRACT/CONTRACTOBJECT ");
            }

            foreach (var contractObject in this.contractObjectsByContractId[item.Id])
            {
                if (contractObject.FiasHouseGuid.IsEmpty())
                {
                    messages.Append("PLACINGCONTRACT/CONTRACTOBJECT/FIASHOUSEGUID");
                }

                //if (contractObject.Contract.Org.)
                //{
                //    messages.Append("PLACINGCONTRACT/CONTRACTOBJECT/FIASHOUSEGUID");
                //}
            }

            if (!this.attachmentsByContractId.ContainsKey(item.Id))
            {
                messages.Append("PLACINGCONTRACT/CONTRACTATTACHMENT ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        protected override importContractRequest GetRequestObject(IEnumerable<RisContract> listForImport)
        {
            var contractList = new List<importContractRequestContract>();

            foreach (var contract in listForImport)
            {
                object item = null;
                ItemChoiceType2 itemName = ItemChoiceType2.Owners;

                switch (contract.OwnersType)
                {
                    case RisContractOwnersType.BuildingOwner:
                        item = new RegOrgType
                        {
                            orgRootEntityGUID = contract.Org.ReturnSafe(x => x.OrgRootEntityGuid)
                        };
                        itemName = ItemChoiceType2.BuildingOwner;
                        break;
                    case RisContractOwnersType.Cooperative:
                        item = new RegOrgType
                        {
                            orgRootEntityGUID = contract.Org.ReturnSafe(x => x.OrgRootEntityGuid)
                        };
                        itemName = ItemChoiceType2.Cooperative;
                        break;
                    case RisContractOwnersType.MunicipalHousing:
                        item = new RegOrgType
                        {
                            orgRootEntityGUID = contract.Org.ReturnSafe(x => x.OrgRootEntityGuid)
                        };
                        itemName = ItemChoiceType2.MunicipalHousing;
                        break;
                    case RisContractOwnersType.Owners:
                        item = true;
                        break;
                }

                var protocolOKs = this.GetProtocolOKs(contract.Id);
                var protocolMeetingOwners = this.GetProtocolMeetingOwners(contract.Id);

                var items = new List<object>();

                if (protocolOKs.Length > 0)
                {
                    items.Add(new ContractTypeProtocolProtocolAdd
                    {
                        Items = this.GetProtocolOKs(contract.Id),
                        ItemsElementName = new[] {ItemsChoiceType5.ProtocolOK}
                    });
                }

                if (protocolMeetingOwners.Length > 0)
                {
                    items.Add(new ContractTypeProtocolProtocolAdd
                    {
                        Items = this.GetProtocolMeetingOwners(contract.Id),
                        ItemsElementName = new[] {ItemsChoiceType5.ProtocolMeetingOwners}
                    });
                }

                var importContractRequest = new importContractRequestContract
                {
                    TransportGUID = Guid.NewGuid().ToString(),
                    Item = new importContractRequestContractPlacingContract
                    {
                        DocNum = contract.DocNum,
                        SigningDate = contract.SigningDate ?? DateTime.MinValue,
                        EffectiveDate = contract.EffectiveDate ?? DateTime.MinValue,
                        PlanDateComptetion = contract.PlanDateComptetion ?? DateTime.MinValue,
                        Validity = new ContractTypeValidity(),
                        //{
                        //    Month = (contract.ValidityMonth ?? 0).ToString(),
                        //    Year = (contract.ValidityYear ?? 0).ToString()
                        //},
                        Item = item,
                        ItemElementName = itemName,
                        Protocol = new ContractTypeProtocol
                        {
                            Items = items.ToArray()
                        },
                        ContractBase = new nsiRef
                        {
                            Code = contract.ContractBaseCode,
                            GUID = contract.ContractBaseGuid
                        },
                        ContractObject = this.GetContractObjects(contract.Id),
                        DateDetails = new DateDetailsType
                        {
                            PeriodMetering = new DateDetailsTypePeriodMetering
                            {
                                StartDate = (sbyte) (contract.InputMeteringDeviceValuesBeginDate ?? 0),
                                Item = (sbyte) (contract.InputMeteringDeviceValuesEndDate ?? 0)
                            },
                            PaymentDate = new DateDetailsTypePaymentDate
                            {
                                Item = (sbyte) (contract.DrawingPaymentDocumentDate ?? 0),
                                Item1ElementName =
                                    contract.ThisMonthPaymentDocDate
                                        ? Item1ChoiceType.CurrentMounth
                                        : Item1ChoiceType.NextMounth,
                                Item1 = true
                            }
                        },
                        ContractAttachment = this.GetContractAttachments(contract.Id)
                    }
                };

                if (contract.Operation == RisEntityOperation.Update)
                {
                    importContractRequest.ContractVersionGUID = contract.Guid;
                }
                

                contractList.Add(importContractRequest);

                this.CountObjects++;
                this.contractsByTransportGuid.Add(importContractRequest.TransportGUID, contract);
            }

            return new importContractRequest {Contract = contractList.ToArray()};
        }

        protected override ImportResult GetRequestResult(importContractRequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importContractData(this.RequestHeader, request, out result);
            }

            return result;
        }

        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (this.contractsByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                var contract = this.contractsByTransportGuid[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;
                    var errorNotation = string.Empty;

                    if (error != null)
                    {
                        errorNotation = error.Description;
                    }

                    this.AddLineToLog("Договора управления", contract.Id, "Не загружен", errorNotation);
                    return;
                }

                contract.Guid = responseItem.GUID;
                this.contractsToSave.Add(contract);

                this.AddLineToLog("Договора управления", contract.Id, "Загружен", responseItem.GUID);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.contractsToSave, 1000, true, true);
        }

        private ContractTypeContractObject[] GetContractObjects(long contractId)
        {
            List<ContractTypeContractObject> result = new List<ContractTypeContractObject>();

            if (this.contractObjectsByContractId.ContainsKey(contractId))
            {
                foreach (var contractObject in this.contractObjectsByContractId[contractId])
                {
                    result.Add(new ContractTypeContractObject
                    {
                        FIASHouseGuid = contractObject.FiasHouseGuid,
                        StartDate = contractObject.StartDate ?? DateTime.MinValue,
                        EndDate = contractObject.EndDate ?? DateTime.MinValue,
                        BaseMService = new BaseServiceType
                        {
                            Item = true
                        },
                        AddService = this.GetAddServices(contractObject.Id),
                        HouseService = this.GetHouseServices(contractObject.Id)
                    });
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Сформировать список прочих услуг по дому
        /// </summary>
        /// <param name="contractObjectId">Иднетификатор объекта контракта</param>
        /// <returns>Список услуг</returns>
        private ContractTypeContractObjectAddService[] GetAddServices(long contractObjectId)
        {
           var result = new List<ContractTypeContractObjectAddService>();

            var listServices = this.addServByContractObjId.Get(contractObjectId) ?? new List<AddService>();

            foreach (var service in listServices)
            {
                result.Add(new ContractTypeContractObjectAddService
                {
                    ServiceType = new nsiRef
                    {
                        Code = service.ServiceTypeCode,
                        GUID = service.ServiceTypeGuid
                    },
                    StartDate = service.StartDate ?? DateTime.MinValue,
                    EndDate = service.EndDate ?? DateTime.MinValue,
                    BaseService = new BaseServiceType
                    {
                        Item = service.BaseServiceCurrentDoc
                    }
                });
            }

            return result.ToArray();
        }

        /// <summary>
        /// Сформировать список коммунальных услуг по дому
        /// </summary>
        /// <param name="contractObjectId">Иднетификатор объекта контракта</param>
        /// <returns>Список услуг</returns>
        private ContractTypeContractObjectHouseService[] GetHouseServices(long contractObjectId)
        {
            var result = new List<ContractTypeContractObjectHouseService>();

            var listHouseManServices = this.houseServByContractObjId.Get(contractObjectId) ?? new List<HouseManService>();

            foreach (var service in listHouseManServices)
            {
                result.Add(new ContractTypeContractObjectHouseService
                {
                    ServiceType = new nsiRef
                    {
                        Code = service.ServiceTypeCode,
                        GUID = service.ServiceTypeGuid
                    },
                    StartDate = service.StartDate ?? DateTime.MinValue,
                    EndDate = service.EndDate ?? DateTime.MinValue,
                    BaseService = new BaseServiceType
                    {
                        Item = service.BaseServiceCurrentDoc
                    }
                });
            }

            return result.ToArray();
        }

        private AttachmentType[] GetContractAttachments(long contractId)
        {
            List<AttachmentType> result = new List<AttachmentType>();

            if (this.attachmentsByContractId.ContainsKey(contractId))
            {
                var listContractAttachment = this.attachmentsByContractId[contractId] ?? new List<RisContractAttachment>();

                foreach (var attachment in listContractAttachment)
                {
                    result.Add(new AttachmentType
                    {
                        Name = attachment.Attachment.Name,
                        Description = attachment.Attachment.Description.IsEmpty() ? attachment.Attachment.Name : attachment.Attachment.Description,
                        Attachment = new Attachment
                        {
                            AttachmentGUID = attachment.Attachment.Guid
                        },
                        AttachmentHASH = attachment.Attachment.Hash
                    });
                }
            }

            return result.ToArray();
        }

        private object[] GetProtocolOKs(long contractId)
        {
            var result = new List<object>();

            if (this.protocolOksByContractId.ContainsKey(contractId))
            {
                var listProtocolOk = this.protocolOksByContractId[contractId] ?? new List<RisProtocolOk>();

                foreach (var attachment in listProtocolOk)
                {
                    result.Add(new AttachmentType
                    {
                        Name = attachment.Attachment.Name,
                        Description = attachment.Attachment.Description.IsEmpty() ? attachment.Attachment.Name : attachment.Attachment.Description,
                        Attachment = new Attachment
                        {
                            AttachmentGUID = attachment.Attachment.Guid
                        },
                        AttachmentHASH = attachment.Attachment.Hash
                    });
                }
            }

            return result.ToArray();
        }

        private object[] GetProtocolMeetingOwners(long contractId)
        {
            var result = new List<object>();

            if (this.protocolMeetingOwnersByContractId.ContainsKey(contractId))
            {
                var listProtocolMeetingOwner = this.protocolMeetingOwnersByContractId[contractId] ?? new List<ProtocolMeetingOwner>();

                foreach (var attachment in listProtocolMeetingOwner)
                {
                    result.Add(new AttachmentType
                    {
                        Name = attachment.Attachment.Name,
                        Description = attachment.Attachment.Description.IsEmpty() ? attachment.Attachment.Name : attachment.Attachment.Description,
                        Attachment = new Attachment
                        {
                            AttachmentGUID = attachment.Attachment.Guid
                        },
                        AttachmentHASH = attachment.Attachment.Hash
                    });
                }
            }

            return result.ToArray();
        }
    }
}
