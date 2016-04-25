namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.HouseManagement;
    using Enums;
    using Ris.HouseManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Метод обмена сведениями о жилищном фонде о поставщиках информации
    /// </summary>
    public class ImportPublicPropertyContractRequestMethod : GisIntegrationHouseManagementMethod<RisPublicPropertyContract, importPublicPropertyContractRequest>
    {
        private List<RisPublicPropertyContract> contractsToSave = new List<RisPublicPropertyContract>();
        private Dictionary<long, List<RisContractAttachment>> contractFilesByContrId = new Dictionary<long, List<RisContractAttachment>>();
        private Dictionary<long, List<RisTrustDocAttachment>> protocolFilesByContrId = new Dictionary<long, List<RisTrustDocAttachment>>();
        private readonly Dictionary<string, RisPublicPropertyContract> contractsByTransportGuid = new Dictionary<string, RisPublicPropertyContract>();

        protected override int ProcessedObjects
        {
            get
            {
                return this.contractsToSave.Count;
            }
        }

        public override string Code
        {
            get
            {
                return "importPublicPropertyContract";
            }
        }

        public override string Name
        {
            get
            {
                return "Импорт договоров на пользование общим имуществом";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 13;
            }
        }

        protected override IList<RisPublicPropertyContract> MainList { get; set; }

        protected override int Portion
        {
            get
            {
                return this.MainList.Count;
            }
        }

        protected override void Prepare()
        {
            var contractsDomain = this.Container.ResolveDomain<RisPublicPropertyContract>();
            var contractAttachmentDomain = this.Container.ResolveDomain<RisContractAttachment>();
            var trustDocAttachmentDomain = this.Container.ResolveDomain<RisTrustDocAttachment>();

            try
            {
                this.MainList = contractsDomain.GetAll().ToList();

                this.contractFilesByContrId = contractAttachmentDomain.GetAll()
                    .GroupBy(x => x.PublicPropertyContract.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());

                this.protocolFilesByContrId = trustDocAttachmentDomain.GetAll()
                    .GroupBy(x => x.PublicPropertyContract.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());

            }
            finally
            {
                this.Container.Release(contractsDomain);
                this.Container.Release(contractAttachmentDomain);
                this.Container.Release(trustDocAttachmentDomain);
            }
        }

        protected override importPublicPropertyContractRequest GetRequestObject(IEnumerable<RisPublicPropertyContract> listForImport)
        {
            var items = new List<importPublicPropertyContractRequestContract>();

            foreach (var item in listForImport)
            {
                if (this.CheckRequestItem(item))
                {
                    object orgIndItem = null;

                    if (item.Organization != null)
                    {
                        orgIndItem = new RegOrgType
                        {
                            orgRootEntityGUID = item.Organization.OrgRootEntityGuid
                        };
                    }
                    else if (item.Entrepreneur != null)
                    {
                        var ind = item.Entrepreneur;

                        orgIndItem = new IndType
                        {
                            Surname = ind.Surname,
                            FirstName = ind.FirstName,
                            Sex = ind.Sex == RisGender.F ? IndTypeSex.F : IndTypeSex.M,
                            DateOfBirth = ind.DateOfBirth ?? DateTime.MinValue,
                            Item = ind.Snils,
                            PlaceBirth = ind.PlaceBirth
                        };
                    }

                    var importPublicPropertyContract = new importPublicPropertyContractRequestContract
                    {
                        TransportGUID = Guid.NewGuid().ToString(),
                        Item = new PublicPropertyContractType
                        {
                            Item = orgIndItem,
                            FIASHouseGuid = item.House.FiasHouseGuid,
                            ContractObject = item.ContractObject,
                            ContractNumber = item.ContractNumber,
                            StartDate = item.StartDate ?? DateTime.MinValue,
                            EndDate = item.EndDate ?? DateTime.MinValue,
                            ContractAttachment = this.GetContractAttachments(item),
                            RentAgrConfirmationDocument = this.GetProtocolAttachments(item)
                        }
                    };

                    items.Add(importPublicPropertyContract);

                    this.CountObjects++;
                    this.contractsByTransportGuid.Add(importPublicPropertyContract.TransportGUID, item);
                }
            }

            var result = new importPublicPropertyContractRequest
            {
                Contract = items.ToArray()
            };

            return result;
        }

        protected override ImportResult1 GetRequestResult1(importPublicPropertyContractRequest request)
        {
            ImportResult1 result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importPublicPropertyContract(this.RequestHeader, request, out result);
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

                    this.AddLineToLog("ДОИ", contract.Id, "Не загружен", errorNotation);
                    return;
                }

                contract.Guid = responseItem.GUID;
                this.contractsToSave.Add(contract);

                this.AddLineToLog("ДОИ", contract.Id, "Загружен", responseItem.GUID);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.contractsToSave, 1000, true, true);
        }

        private AttachmentType[] GetContractAttachments(RisPublicPropertyContract item)
        {
            List<AttachmentType> result = new List<AttachmentType>();

            if (this.contractFilesByContrId.ContainsKey(item.Id))
            {
                foreach (var attachment in this.contractFilesByContrId[item.Id])
                {
                    result.Add(new AttachmentType
                    {
                        Name = attachment.Attachment.Name,
                        Description = attachment.Attachment.Description,
                        Attachment = new Ris.HouseManagement.Attachment
                        {
                            AttachmentGUID = attachment.Attachment.Guid
                        },
                        AttachmentHASH = attachment.Attachment.Hash
                    });
                }
            }

            return result.ToArray();
        }

        private PublicPropertyContractTypeRentAgrConfirmationDocument[] GetProtocolAttachments(RisPublicPropertyContract item)
        {
            var result = new List<PublicPropertyContractTypeRentAgrConfirmationDocument>();
            var protocolList = new List<PublicPropertyContractTypeRentAgrConfirmationDocumentProtocolMeetingOwners>();

            if (this.protocolFilesByContrId.ContainsKey(item.Id))
            {
                foreach (var attachment in this.protocolFilesByContrId[item.Id])
                {
                    protocolList.Add(new PublicPropertyContractTypeRentAgrConfirmationDocumentProtocolMeetingOwners
                    {
                        ProtocolDate = attachment.PublicPropertyContract.ProtocolDate ?? DateTime.MinValue,
                        ProtocolNum = attachment.PublicPropertyContract.ProtocolNumber,
                        TrustDocAttachment = new []{new AttachmentType
                        {
                            Name = attachment.Attachment.Name,
                            Description = attachment.Attachment.Description,
                            Attachment = new Ris.HouseManagement.Attachment
                            {
                                AttachmentGUID = attachment.Attachment.Guid
                            },
                            AttachmentHASH = attachment.Attachment.Hash
                        } }
                    });
                }
            }

            result.Add(new PublicPropertyContractTypeRentAgrConfirmationDocument
            {
                ProtocolMeetingOwners = protocolList.ToArray()
            });

            return result.ToArray();
        }

        /// <summary>
        /// Проверить элемент на обязательные поля.
        /// </summary>
        /// <param name="item">Эдемент</param>
        /// <returns>true - проверка пройдена, элемент можно добавлять; в противном случае - false</returns>
        private bool CheckRequestItem(RisPublicPropertyContract item)
        {
            var contractAttachmentDomain = this.Container.ResolveDomain<RisContractAttachment>();
            var trustDocAttachmentDomain = this.Container.ResolveDomain<RisTrustDocAttachment>();

            var result = true;
            var errorMsg = new StringBuilder();

            if (item.Organization != null)
            {
                if (item.Organization.OrgRootEntityGuid.IsEmpty())
                {
                    errorMsg.Append("ORGANIZATION/ORGROOTENTITYGUID ");
                }
            }
            else if (item.Entrepreneur != null)
            {
                if (item.Entrepreneur.Surname.IsEmpty())
                {
                    errorMsg.Append("ENTREPRENEUR/SURNAME ");
                }
                if (item.Entrepreneur.FirstName.IsEmpty())
                {
                    errorMsg.Append("ENTREPRENEUR/FIRSTNAME ");
                }
                if (!item.Entrepreneur.DateOfBirth.HasValue)
                {
                    errorMsg.Append("ENTREPRENEUR/DATEOFBIRTH ");
                }
                if (item.Entrepreneur.Snils.IsEmpty())
                {
                    errorMsg.Append("ENTREPRENEUR/SNILS ");
                }
            }

            if (item.House == null || item.House.FiasHouseGuid.IsEmpty())
            {
                errorMsg.Append("FIASHOUSEGUID ");
            }

            if (item.ContractNumber.IsEmpty())
            {
                errorMsg.Append("CONTRACTNUMBER ");
            }

            if (!item.StartDate.HasValue)
            {
                errorMsg.Append("STARTDATE ");
            }

            if (!item.EndDate.HasValue)
            {
                errorMsg.Append("ENDDATE ");
            }

            if (!this.contractFilesByContrId.ContainsKey(item.Id))
            {
                errorMsg.Append("CONTRACTATTACHMENT ");
            }

            if (!this.protocolFilesByContrId.ContainsKey(item.Id))
            {
                errorMsg.Append("TRUSTDOCATTACHMENT ");
            }

            if (errorMsg.Length > 0)
            {
                this.AddLineToLog("ДОИ", item.Id, "Не загружен", errorMsg);
                result = false;
            }

            this.Container.Release(contractAttachmentDomain);
            this.Container.Release(trustDocAttachmentDomain);

            return result;
        }
    }
}
