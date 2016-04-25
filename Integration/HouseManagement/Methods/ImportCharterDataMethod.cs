namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.HouseManagement;
    using Enums;
    using HouseManagement;
    using Ris.HouseManagement;

    /// <summary>
    /// Метод передачи данных по уставам
    /// </summary>
    public class ImportCharterDataMethod : GisIntegrationHouseManagementMethod<Charter, importCharterRequest>
    {
        private readonly Dictionary<string, Charter> charterByTransportGuidDict = new Dictionary<string, Charter>();
        private readonly List<Charter> charterToSave = new List<Charter>();

        private Dictionary<long, List<ProtocolMeetingOwner>> ProtocolMeetingOwnerToCharterDict;
        private Dictionary<long, List<ContractObject>> ContractObjectToCharterDict;

        protected override int ProcessedObjects { get { return this.charterToSave.Count; } }

        public override string Code
        {
            get
            {
                return "importCharterData";
            }
        }

        public override string Name
        {
            get
            {
                return "Импорт уставов";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get { return 11; }
        }

        protected override int Portion { get { return 1; } }

        protected override IList<Charter> MainList { get; set; }

        protected override void Prepare()
        {
            var charterDomain = this.Container.ResolveDomain<Charter>();
            var protMeetOwnDomain = this.Container.ResolveDomain<ProtocolMeetingOwner>();
            var contractObjectDomain = this.Container.ResolveDomain<ContractObject>();

            try
            {
                this.MainList = charterDomain.GetAll().ToList();

                this.ProtocolMeetingOwnerToCharterDict = protMeetOwnDomain.GetAll()
                    .GroupBy(x => x.Charter.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());

                this.ContractObjectToCharterDict = contractObjectDomain.GetAll()
                    .GroupBy(x => x.Charter.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
            finally
            {
                this.Container.Release(charterDomain);
                this.Container.Release(protMeetOwnDomain);
                this.Container.Release(contractObjectDomain);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.charterToSave, 1000, true, true);
        }

        protected override ImportResult GetRequestResult(importCharterRequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importCharterData(this.RequestHeader, request, out result);
            }

            return result;
        }

        protected override importCharterRequest GetRequestObject(IEnumerable<Charter> listForImport)
        {
            Charter iterationCharter = listForImport.First();

            var charterNotation = new StringBuilder();

            if (iterationCharter.DocNum.IsEmpty())
            {
                charterNotation.Append("DOCNUM ");
            }

            if (!iterationCharter.DocDate.HasValue)
            {
                charterNotation.Append("DOCDATE ");
            }

            if (!this.ContractObjectToCharterDict.ContainsKey(iterationCharter.Id))
            {
                charterNotation.Append("RIS_CONTRACTOBJECT ");
            }

            if (iterationCharter.Attachment == null)
            {
                charterNotation.Append("ATTACHMENT_ID ");
            }

            var headInd = iterationCharter.Head;

            if (headInd == null)
            {
                charterNotation.Append("HEAD ");
            }
            else
            {
                if (headInd.Surname.IsEmpty())
                {
                    charterNotation.Append("HEAD_SURNAME ");
                }

                if (headInd.FirstName.IsEmpty())
                {
                    charterNotation.Append("HEAD_FIRSTNAME ");
                }

                if (!headInd.DateOfBirth.HasValue)
                {
                    charterNotation.Append("HEAD_DATEOFBIRTH ");
                }

                if (headInd.Snils.IsEmpty())
                {
                    charterNotation.Append("HEAD_SNILS ");
                }
            }

            var protMeetOwners = this.ProtocolMeetingOwnerToCharterDict.ContainsKey(iterationCharter.Id)
                ? this.ProtocolMeetingOwnerToCharterDict[iterationCharter.Id]
                : null;

            var protMeetOwnRequestList = new List<AttachmentType>();

            if (protMeetOwners != null)
            {
                foreach (var protMeetOwner in protMeetOwners)
                {
                    var protMeetOwnerNotation = new StringBuilder();

                    if (protMeetOwner.Attachment.Name.IsEmpty())
                    {
                        protMeetOwnerNotation.Append("PROTMEETINGOWNER_NAME ");
                    }

                    if (protMeetOwner.Attachment.Description.IsEmpty())
                    {
                        protMeetOwnerNotation.Append("PROTMEETINGOWNER_DESCRIPTION ");
                    }

                    if (protMeetOwner.Attachment.Guid.IsEmpty())
                    {
                        protMeetOwnerNotation.Append("PROTMEETINGOWNER_GUID ");
                    }

                    if (protMeetOwner.Attachment.Hash.IsEmpty())
                    {
                        protMeetOwnerNotation.Append("PROTMEETINGOWNER_HASH ");
                    }

                    if (protMeetOwnerNotation.Length > 0)
                    {
                        this.AddLineToLog("Протокол", protMeetOwner.Id, "Отсутствуют обязательные поля", protMeetOwnerNotation);
                        continue;
                    }

                    var protMeetOwnRequest = new AttachmentType
                    {
                        Name = protMeetOwner.Attachment.Name,
                        Description = protMeetOwner.Attachment.Description,
                        Attachment = new Attachment { AttachmentGUID = protMeetOwner.Attachment.Guid },
                        AttachmentHASH = protMeetOwner.Attachment.Hash
                    };

                    protMeetOwnRequestList.Add(protMeetOwnRequest);
                }
            }

            if (!protMeetOwnRequestList.Any())
            {
                charterNotation.Append("RIS_PROTOCOLMEETINGOWNER ");
            }

            var contrObjectList = this.ContractObjectToCharterDict.ContainsKey(iterationCharter.Id)
                    ? this.ContractObjectToCharterDict[iterationCharter.Id]
                    : null;

            var contractObjectRequestList = new List<CharterTypeContractObjectList>();

            if (contrObjectList != null)
            {
                foreach (var contrObject in contrObjectList)
                {
                    var contrObjNotation = new StringBuilder();

                    if (contrObject.FiasHouseGuid.IsEmpty())
                    {
                        contrObjNotation.Append("CONTROBJECT_HOUSE ");
                    }

                    if (!contrObject.StartDate.HasValue)
                    {
                        contrObjNotation.Append("CONTROBJECT_STARTDATE ");
                    }

                    if (!contrObject.BaseMServiseCurrentCharter)
                    {
                        contrObjNotation.Append("CONTROBJECT_BASEMSERVICECURRCHARTER ");
                    }

                    if (contrObjNotation.Length > 0)
                    {
                        this.AddLineToLog("Контакт контрагента", contrObject.Id, "Отсутствуют обязательные поля", contrObjNotation);
                        continue;
                    }

                    var contractObjectRequest = new CharterTypeContractObjectList
                    {
                        ContractObject = new CharterTypeContractObjectListContractObject
                        {
                            FIASHouseGuid = contrObject.FiasHouseGuid,
                            StartDate = contrObject.StartDate ?? DateTime.MinValue,
                            BaseMService = new BaseServiceCharterType
                            {
                                Item = true
                            }
                        }
                    };

                    contractObjectRequestList.Add(contractObjectRequest);
                }
            }

            if (!contractObjectRequestList.Any())
            {
                charterNotation.Append("RIS_CONTRACTOBJECT ");
            }

            if (charterNotation.Length > 0)
            {
                this.AddLineToLog("Устав", iterationCharter.Id, "Не загружен", charterNotation);
                return null;
            }

            IndType indType = null;

            if (headInd != null)
            {
                indType = new IndType
                {
                    Surname = headInd.Surname,
                    FirstName = headInd.FirstName,
                    Sex = headInd.Sex == RisGender.F ? IndTypeSex.F : IndTypeSex.M,
                    DateOfBirth = headInd.DateOfBirth ?? DateTime.MinValue,
                    Item = headInd.Snils
                };
            }

            var attachmentArray = new[]
            {
                new AttachmentType
                {
                    Name = iterationCharter.Attachment.Name,
                    Description = iterationCharter.Attachment.Description,
                    Attachment = new Attachment { AttachmentGUID = iterationCharter.Attachment.Guid },
                    AttachmentHASH = iterationCharter.Attachment.Hash
                }
            };

            var importCharterRequest = new importCharterRequestCharter
            {
                DocNum = iterationCharter.DocNum,
                Date = iterationCharter.DocDate ?? DateTime.MinValue,
                MeetingProtocol = new CharterTypeMeetingProtocol
                {
                    Items = protMeetOwnRequestList.ToArray()
                },
                ChiefExecutive = new CharterTypeChiefExecutive
                {
                    Head = new CharterTypeChiefExecutiveHead
                    {
                        Item = indType,
                        ItemElementName = ItemChoiceType3.IndOwner
                    },
                    Managers = iterationCharter.Managers
                },
                ContractObjectList = contractObjectRequestList.ToArray(),
                AttachmentCharter = attachmentArray
            };

            importCharterRequest request = new importCharterRequest
            {
                TransportGUID = Guid.NewGuid().ToString(),
                Item = importCharterRequest
            };

            this.charterByTransportGuidDict.Add(request.TransportGUID, iterationCharter);
            this.CountObjects++;

            return request;
        }

        protected override void CheckResponseItem(CommonResultType responseCharter)
        {
            if (!this.charterByTransportGuidDict.ContainsKey(responseCharter.TransportGUID))
            {
                return;
            }

            var charter = this.charterByTransportGuidDict[responseCharter.TransportGUID];

            if (responseCharter.GUID.IsEmpty())
            {
                var error = responseCharter.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("Устав", charter.Id, "Не загружен", errorNotation);
                return;
            }

            charter.Guid = responseCharter.GUID;
            this.charterToSave.Add(charter);

            this.AddLineToLog("Устав", charter.Id, "Загружен", charter.Guid);
        }
    }
}