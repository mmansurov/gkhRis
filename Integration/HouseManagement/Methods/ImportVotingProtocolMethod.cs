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
    using Enums.HouseManagement;
    using Ris.HouseManagement;

    public class ImportVotingProtocolMethod : GisIntegrationHouseManagementMethod<RisVotingProtocol, importVotingProtocolRequest>
    {
        private readonly Dictionary<string, RisVotingProtocol> protocolByTransportGuidDict = new Dictionary<string, RisVotingProtocol>();
        private Dictionary<long, List<RisDecisionList>> decisionsByProtocolId = new Dictionary<long, List<RisDecisionList>>();
        private Dictionary<long, List<RisVotingProtocolAttachment>> attachmentsByProtocolId = new Dictionary<long, List<RisVotingProtocolAttachment>>();
        private readonly List<RisVotingProtocol> protocolsToSave = new List<RisVotingProtocol>();

        protected override int ProcessedObjects
        {
            get { return this.protocolsToSave.Count; }
        }

        public override string Code
        {
            get { return "importVotingProtocol"; }
        }

        public override string Name
        {
            get { return "Импорт протоколов голосования"; }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 45;
            }
        }

        protected override int Portion
        {
            get { return 1; }
        }

        protected override IList<RisVotingProtocol> MainList { get; set; }

        protected override void Prepare()
        {
            var protocolDomain = this.Container.ResolveDomain<RisVotingProtocol>();
            var decisionDomain = this.Container.ResolveDomain<RisDecisionList>();
            var attachmentDomain = this.Container.ResolveDomain<RisVotingProtocolAttachment>();

            try
            {
                this.MainList = protocolDomain.GetAll().ToList();

                this.decisionsByProtocolId = decisionDomain.GetAll()
                    .Where(x => x.VotingProtocol != null)
                    .GroupBy(x => x.VotingProtocol)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());

                this.attachmentsByProtocolId = attachmentDomain.GetAll()
                    .Where(x => x.VotingProtocol != null)
                    .GroupBy(x => x.VotingProtocol)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());
            }
            finally
            {
                this.Container.Release(protocolDomain);
                this.Container.Release(decisionDomain);
                this.Container.Release(attachmentDomain);
            }
        }

        protected override CheckingResult CheckMainListItem(RisVotingProtocol item)
        {
            StringBuilder messages = new StringBuilder();

            if (!item.VotingTimeType.HasValue)
            {
                messages.Append("ANNUALVOTING ");
            }

            if (!item.MeetingEligibility.HasValue)
            {
                messages.Append("MEETINGELIGIBILITY ");
            }

            if (!this.decisionsByProtocolId.ContainsKey(item.Id))
            {
                messages.Append("DECISIONLIST ");
            }

            if (!this.attachmentsByProtocolId.ContainsKey(item.Id))
            {
                messages.Append("ATTACHMENTS ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        protected override importVotingProtocolRequest GetRequestObject(IEnumerable<RisVotingProtocol> listForImport)
        {
            var protocol = listForImport.ToList()[0]; // Portion == 1
            DateTime protocolDate = protocol.ProtocolDate ?? DateTime.MinValue;

            List<ProtocolTypeDecisionList> decisions = new List<ProtocolTypeDecisionList>();
            List<Attachments> attachments = new List<Attachments>();

            if (this.decisionsByProtocolId.ContainsKey(protocol.Id))
            {
                foreach (var decision in this.decisionsByProtocolId[protocol.Id])
                {
                    decisions.Add(new ProtocolTypeDecisionList
                    {
                        QuestionNumber = (decision.QuestionNumber ?? 0).ToString(),
                        QuestionName = decision.QuestionName,
                        DecisionsType = new nsiRef
                        {
                            Code = decision.DecisionsTypeCode,
                            GUID = decision.DecisionsTypeGuid
                        },
                        Agree = decision.Agree ?? 0m,
                        votingResume = decision.VotingResume == RisVotingResume.M ? ProtocolTypeDecisionListVotingResume.M : ProtocolTypeDecisionListVotingResume.N
                    });
                }
            }

            if (this.attachmentsByProtocolId.ContainsKey(protocol.Id))
            {
                foreach (var attach in this.attachmentsByProtocolId[protocol.Id])
                {
                    attachments.Add(new Attachments
                    {
                        Attachment = new Attachment
                        {
                            AttachmentGUID = attach.Attachment.Guid
                        }
                    });
                }
            }

            var result = new importVotingProtocolRequest
            {
                TransportGUID = Guid.NewGuid().ToString(),
                ItemElementName = ItemChoiceType8.Protocol,
                Item = new importVotingProtocolRequestProtocol
                {
                    FIASHouseGuid = protocol.House != null ? protocol.House.FiasHouseGuid : string.Empty,
                    ProtocolNum = protocol.ProtocolNum,
                    ProtocolDate = protocolDate,
                    Item = new ProtocolTypeMeeting
                    {
                        VotingPlace = protocol.VotingPlace,
                        MeetingDate = new DateTime(protocolDate.Year, protocolDate.Month, protocolDate.Day, 19, 0, 0, DateTimeKind.Utc)
                    },
                    Item1 = true,
                    MeetingEligibility = ProtocolTypeMeetingEligibility.C,
                    DecisionList = decisions.ToArray(),
                    Attachments = attachments.ToArray()
                }
            };

            this.CountObjects++;
            this.protocolByTransportGuidDict.Add(result.TransportGUID, protocol);

            return result;
        }

        protected override ImportResult1 GetRequestResult1(importVotingProtocolRequest request)
        {
            ImportResult1 result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importVotingProtocol(this.RequestHeader, request, out result);
            }

            return result;
        }

        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (this.protocolByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
                var protocol = this.protocolByTransportGuidDict[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;
                    var errorNotation = string.Empty;

                    if (error != null)
                    {
                        errorNotation = error.Description;
                    }

                    this.AddLineToLog("Протокол", protocol.Id, "Не загружен", errorNotation);
                    return;
                }

                protocol.Guid = responseItem.GUID;
                this.protocolsToSave.Add(protocol);

                this.AddLineToLog("Протокол", protocol.Id, "Загружен", responseItem.GUID);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.protocolsToSave, 1000, true, true);
        }

        /// <summary>
        /// Плучить экстракторы данных
        /// </summary>
        /// <returns>Список экстракторов</returns>
        protected virtual List<IGisIntegrationDataExtractor> ResolveExtractors()
        {
            return new List<IGisIntegrationDataExtractor>
                   {
                       this.Container.Resolve<IGisIntegrationDataExtractor>("ImportVotingProtocolDataExtractor")
                   };
        }
    }
}
