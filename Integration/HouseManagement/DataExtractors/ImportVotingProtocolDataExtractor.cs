namespace Bars.Gkh.Ris.Integration.HouseManagement.DataExtractors
{
    using B4.DataAccess;
    using Gkh.Entities;
    using Overhaul.Tat.Entities;
    using Entities.HouseManagement;
    using Enums.HouseManagement;
    using Enums;
    using FileService;
    using Overhaul.Tat.Enum;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4.Utils;

    using Bars.Gkh.Ris.Entities;

    public class ImportVotingProtocolDataExtractor : GisIntegrationDataExtractorBase
    {
        private Dictionary<long, string> adressesProtocolIds = new Dictionary<long, string>();
        private Dictionary<string, RisHouse> risHouseByFiasGuid = new Dictionary<string, RisHouse>();

        public override string Code
        {
            get
            {
                return "importVotingProtocol";
            }
        }
        protected override void FillDictionaries()
        {
            var roDomain = this.Container.ResolveDomain<RealityObject>();
            var protocolDomain = this.Container.ResolveDomain<PropertyOwnerProtocols>();
            var risHouse = this.Container.ResolveDomain<RisHouse>();

            try
            {
                this.adressesProtocolIds = protocolDomain.GetAll()
                    .Where(x => x.RealityObject != null && x.RealityObject.Address != "")
                    .Select(x => new
                    {
                        ProtocolId = x.Id,
                        RoAddress = x.RealityObject != null ? x.RealityObject.Address : string.Empty
                    })
                    .ToList()
                    .GroupBy(x => x.ProtocolId)
                    .ToDictionary(x => x.Key, x => x.First().RoAddress);

                this.risHouseByFiasGuid = risHouse.GetAll()
                    .Where(x => x.FiasHouseGuid != "")
                    .GroupBy(x => x.FiasHouseGuid)
                    .ToDictionary(x => x.Key, x => x.First());

            }
            finally
            {
                this.Container.Release(roDomain);
                this.Container.Release(protocolDomain);
                this.Container.Release(risHouse);
            }
        }

        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var decisionDomain = this.Container.ResolveDomain<BasePropertyOwnerDecision>();
            var protocolDomain = this.Container.ResolveDomain<PropertyOwnerProtocols>();
            var protocolAttachmentDomain = this.Container.ResolveDomain<RisVotingProtocolAttachment>();
            var fileUploadService = this.Container.Resolve<IFileUploadService>();

            try
            {
                var protocols = protocolDomain.GetAll()
                    .ToList()
                    .Select(x => new
                    {
                        ProtocolId = x.Id,
                        RisVotingProtocol = new RisVotingProtocol
                        {
                            ExternalSystemEntityId = x.Id,
                            ExternalSystemName = "gkh",
                            House = (x.RealityObject != null && x.RealityObject.HouseGuid != null &&
                                   x.RealityObject.HouseGuid != "" && 
                                   this.risHouseByFiasGuid.ContainsKey(x.RealityObject.HouseGuid))
                               ? this.risHouseByFiasGuid[x.RealityObject.HouseGuid] : null,
                            ProtocolNum = x.DocumentNumber,
                            ProtocolDate = x.DocumentDate ?? DateTime.MinValue,
                            VotingPlace = this.adressesProtocolIds.Get(x.Id),
                            VotingTimeType = RisVotingTimeType.AnnualVoting,
                            MeetingEligibility = RisMeetingEligibility.C
                        }
                    })
                    .ToList();

                var protocolsToSave = protocols.Select(x => x.RisVotingProtocol).ToList();
                this.SaveRisEntities<RisVotingProtocol, PropertyOwnerProtocols>(protocolsToSave);

                var protocolsById = protocols.GroupBy(x => x.ProtocolId)
                    .ToDictionary(x => x.Key, x => x.First().RisVotingProtocol);

                var decisionsProtocol = decisionDomain.GetAll()
                    .Where(x => x.PropertyOwnerProtocol != null)
                    .ToArray()
                    .Select((x, index) => new
                    {
                        Protocol = x.PropertyOwnerProtocol,
                        DecisionList = new RisDecisionList
                        {
                            ExternalSystemEntityId = x.Id,
                            ExternalSystemName = "gkh",
                            QuestionNumber = index,
                            QuestionName = x.PropertyOwnerDecisionType.ToString(),
                            DecisionsTypeCode = this.GetDecisionsType(x.PropertyOwnerDecisionType, true),
                            DecisionsTypeGuid = this.GetDecisionsType(x.PropertyOwnerDecisionType, false),
                            VotingProtocol = protocolsById.Get(x.PropertyOwnerProtocol.Id),
                            Agree = x.PropertyOwnerProtocol.NumberOfVotes,
                            VotingResume = RisVotingResume.M
                        }
                    }).ToList();

                
                var decisionsToSave = decisionsProtocol.Select(x => x.DecisionList).ToList();
                this.SaveRisEntities<RisDecisionList, BasePropertyOwnerDecision>(decisionsToSave);

                foreach (var protocol in decisionsProtocol)
                {
                    var file = protocol.Protocol.DocumentFile;
                    RisVotingProtocolAttachment newProtocol = null;

                    try
                    {
                        newProtocol = new RisVotingProtocolAttachment
                        {
                            VotingProtocol = protocol.DecisionList.VotingProtocol,
                            Attachment = fileUploadService.SaveAttachment(
                                file, protocol.Protocol.Description, FileStorageName.HomeManagement, this.Contragent.SenderId)
                        };
                    }
                    catch // в локальном хранилище нет файлов
                    {
                        // ignored
                    }

                    if (newProtocol != null)
                    {
                        protocolAttachmentDomain.Save(newProtocol);
                    }
                }

                return new Dictionary<Type, List<BaseRisEntity>>();
            }
            finally
            {
                this.Container.Release(decisionDomain);
                this.Container.Release(protocolAttachmentDomain);
                this.Container.Release(fileUploadService);
                this.Container.Release(protocolDomain);
            }
        }

        private string GetDecisionsType(PropertyOwnerDecisionType propertyOwnerDecisionType, bool code)
        {
            string result = string.Empty;

            switch (propertyOwnerDecisionType)
            {
                case PropertyOwnerDecisionType.ListOverhaulServices:
                    {
                        result = code ? "12.1" : "57780902-0b1f-4a02-88a1-cdb0caebf882";
                        break;
                    }
                case PropertyOwnerDecisionType.MinCrFundSize:
                    {
                        result = code ? "2.3" : "c5e77964-0cff-490a-818c-e8f66596e710";
                        break;
                    }
                case PropertyOwnerDecisionType.SelectMethodForming:
                    {
                        result = code ? "2.1" : "2fe62c9f-965c-4891-83ac-5872251a8a99";
                        break;
                    }
                case PropertyOwnerDecisionType.SetMinAmount:
                    {
                        result = code ? "2.2" : "a0597bd9-b422-4e0e-af3e-adbc4e8ebe35";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return result;
        }
    }
}
