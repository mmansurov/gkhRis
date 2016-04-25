namespace Bars.Gkh.Ris.Integration.HouseManagement.DataExtractors
{
    using System;

    using B4.DataAccess;
    using Entities;
    using Enums;
    using FileService;
    using GkhDi.Entities;
    using GkhDi.Enums;
    using Domain;
    using Entities.HouseManagement;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4.Utils;

    public class ImportPublicPropertyContractDataExtractor : GisIntegrationDataExtractorBase
    {
        private Dictionary<string, RisInd> risIndsBySnils = new Dictionary<string, RisInd>();
        private Dictionary<string, RisHouse> risHouseByFiasGuid = new Dictionary<string, RisHouse>();
        private Dictionary<string, RisContragent> risContrByOgrn = new Dictionary<string, RisContragent>();

        public override string Code
        {
            get
            {
                return "importPublicPropertyContract";
            }
        }

        protected override void FillDictionaries()
        {
            var risIndDomain = this.Container.ResolveDomain<RisInd>();
            var risContragentDomain = this.Container.ResolveDomain<RisContragent>();
            var risHouse = this.Container.ResolveDomain<RisHouse>();

            try
            {
                this.risIndsBySnils = risIndDomain.GetAll()
                    .Where(x => x.Snils != "")
                    .GroupBy(x => x.Snils)
                    .ToDictionary(x => x.Key, x => x.First());

                this.risContrByOgrn = risContragentDomain.GetAll()
                   .Where(x => x.Ogrn != "")
                   .GroupBy(x => x.Ogrn)
                   .ToDictionary(x => x.Key, x => x.First());

                this.risHouseByFiasGuid = risHouse.GetAll()
                    .Where(x => x.FiasHouseGuid != "")
                    .GroupBy(x => x.FiasHouseGuid)
                    .ToDictionary(x => x.Key, x => x.First());
            }
            finally
            {
                this.Container.Release(risContragentDomain);
                this.Container.Release(risIndDomain);
                this.Container.Release(risHouse);
            }
        }

        private class RisPublicPropertyContractProxy
        {
            public InfoAboutUseCommonFacilities Info { get; set; }
            public RisPublicPropertyContract RisContract { get; set; }
        }

        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var infoAboutUseFacilsDomain = this.Container.ResolveDomain<InfoAboutUseCommonFacilities>();
            var fileUploadService = this.Container.Resolve<IFileUploadService>();
            var contractAttachmentDomain = this.Container.ResolveDomain<RisContractAttachment>();
            var trustDocAttachmentDomain = this.Container.ResolveDomain<RisTrustDocAttachment>();

            try
            {
                var pubPropOrganizationContractProxies = infoAboutUseFacilsDomain.GetAll()
                    .Where(x => x.LesseeType == LesseeTypeDi.Legal && x.Ogrn != "")
                    .ToArray()
                    .Select(x => new RisPublicPropertyContractProxy
                    {
                        Info = x,
                        RisContract = new RisPublicPropertyContract
                        {
                            ExternalSystemEntityId = x.Id,
                            ExternalSystemName = "gkh",
                            Organization = this.risContrByOgrn.ContainsKey(x.Ogrn) ? this.risContrByOgrn[x.Ogrn] : null,
                            ContractObject = x.ContractSubject,
                            ContractNumber = x.ContractNumber,
                            StartDate = x.DateStart,
                            EndDate = x.DateEnd,
                            ProtocolNumber = x.Number,
                            ProtocolDate = x.From,
                            House = (x.DisclosureInfoRealityObj != null &&
                                   x.DisclosureInfoRealityObj.RealityObject != null &&
                                   x.DisclosureInfoRealityObj.RealityObject.HouseGuid != "" &&
                                   this.risHouseByFiasGuid.ContainsKey(x.DisclosureInfoRealityObj.RealityObject.HouseGuid))
                               ? this.risHouseByFiasGuid[x.DisclosureInfoRealityObj.RealityObject.HouseGuid] : null
                        }
                    });

                var pubPropEntrepreneurContractProxies = infoAboutUseFacilsDomain.GetAll()
                    .Where(x => x.LesseeType == LesseeTypeDi.Individual && x.Snils != "")
                    .ToArray()
                    .Select(x => new RisPublicPropertyContractProxy
                    {
                        Info = x,
                        RisContract = new RisPublicPropertyContract
                        {
                            ExternalSystemEntityId = x.Id,
                            ExternalSystemName = "gkh",
                            Entrepreneur = this.risIndsBySnils.ContainsKey(x.Snils) ? this.risIndsBySnils[x.Snils] : null,
                            ContractObject = x.ContractSubject,
                            ContractNumber = x.ContractNumber,
                            StartDate = x.DateStart,
                            EndDate = x.DateEnd,
                            ProtocolNumber = x.Number,
                            ProtocolDate = x.From,
                            House = (x.DisclosureInfoRealityObj != null &&
                                   x.DisclosureInfoRealityObj.RealityObject != null &&
                                   x.DisclosureInfoRealityObj.RealityObject.HouseGuid != "" &&
                                   this.risHouseByFiasGuid.ContainsKey(x.DisclosureInfoRealityObj.RealityObject.HouseGuid))
                               ? this.risHouseByFiasGuid[x.DisclosureInfoRealityObj.RealityObject.HouseGuid] : null
                        }
                    });

                var pubPropContractProxies = pubPropEntrepreneurContractProxies.ToList();
                pubPropContractProxies.AddRange(pubPropOrganizationContractProxies.ToList());

                var pubPropContractsToSave = pubPropContractProxies.Select(x => x.RisContract).ToList();
                this.SaveRisEntities<RisPublicPropertyContract, InfoAboutUseCommonFacilities>(pubPropContractsToSave);

                foreach (var contractProxy in pubPropContractProxies)
                {
                    var contractFile = contractProxy.Info.ContractFile;
                    var protocolFile = contractProxy.Info.ProtocolFile;

                    if (contractFile != null)
                    {
                        var newAttachment = new RisContractAttachment
                        {
                            Attachment = fileUploadService.SaveAttachment(
                                contractFile, contractFile.Name, FileStorageName.HomeManagement, this.Contragent.SenderId),
                            PublicPropertyContract = contractProxy.RisContract
                        };
                        contractAttachmentDomain.Save(newAttachment);
                    }

                    if (protocolFile != null)
                    {
                        var newAttachment = new RisTrustDocAttachment
                        {
                            Attachment = fileUploadService.SaveAttachment(
                                protocolFile, protocolFile.Name, FileStorageName.HomeManagement, this.Contragent.SenderId),
                            PublicPropertyContract = contractProxy.RisContract
                        };
                        trustDocAttachmentDomain.Save(newAttachment);
                    }
                }

                return new Dictionary<Type, List<BaseRisEntity>>();
            }
            finally
            {
                this.Container.Release(fileUploadService);
                this.Container.Release(infoAboutUseFacilsDomain);
                this.Container.Release(contractAttachmentDomain);
                this.Container.Release(trustDocAttachmentDomain);
            }
        }
    }
}
