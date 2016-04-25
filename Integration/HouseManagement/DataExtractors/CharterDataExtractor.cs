namespace Bars.Gkh.Ris.Integration.HouseManagement.DataExtractors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using B4.DataAccess;
    using B4.Modules.FileStorage;

    using Bars.B4.Utils;
    using Bars.Gkh.Entities;
    using Bars.Gkh.Enums;
    using Bars.GkhGji.Entities;
    using Bars.Gkh.Ris.Entities;
    using Bars.Gkh.Ris.Entities.HouseManagement;
    using Bars.Gkh.Ris.Enums;
    using Bars.Gkh.Ris.Extensions;
    using Bars.Gkh.Ris.Integration.FileService;

    /// <summary>
    /// Извлечение данных по уставам
    /// </summary>
    public class CharterDataExtractor : GisIntegrationDataExtractorBase
    {
        private Dictionary<long, List<ProtocolTsjProxy>> ProtocolListByActivityTsjDict;
        private Dictionary<long, string> ManagersByContragentDict;
        private Dictionary<long, HeadInfoProxy> HeadInfoByContragentDict;
        private Dictionary<long, long> StatuteIdByActTsjIdDict;

        protected override void FillDictionaries()
        {
            var activityTsjProtocolDomain = this.Container.ResolveDomain<ActivityTsjProtocol>();
            var contragentContactDomain = this.Container.ResolveDomain<ContragentContact>();
            var manOrgDomain = this.Container.ResolveDomain<ManagingOrganization>();
            var activityTsjDomain = this.Container.ResolveDomain<ActivityTsj>();
            var activityTsjStatuteDomain = this.Container.ResolveDomain<ActivityTsjStatute>();

            try
            {
                this.StatuteIdByActTsjIdDict = activityTsjStatuteDomain.GetAll()
                    .Select(x => new
                    {
                        ActivityTsjId = x.ActivityTsj.Id,
                        StatuteId = x.Id
                    })
                    .ToList()
                    .GroupBy(x => x.ActivityTsjId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.StatuteId).First());

                var actTsjContragentIds = activityTsjDomain.GetAll()
                    .Where(x => this.StatuteIdByActTsjIdDict.Keys.Contains(x.Id))
                    .Select(x => x.ManagingOrganization.Contragent.Id)
                    .ToArray();

                var contragentContactList = contragentContactDomain.GetAll()
                    .Where(x => actTsjContragentIds.Contains(x.Contragent.Id))
                    .ToList();

                this.ProtocolListByActivityTsjDict = activityTsjProtocolDomain.GetAll()
                    .Where(x => this.StatuteIdByActTsjIdDict.Keys.Contains(x.ActivityTsj.Id))
                    .Where(x => x.File != null && x.KindProtocolTsj != null)
                    .Select(x => new
                    {
                        ActivityTsjId = x.ActivityTsj.Id,
                        x.File,
                        Type = x.KindProtocolTsj.Name
                    })
                    .ToList()
                    .GroupBy(x => x.ActivityTsjId)
                    .ToDictionary(x => x.Key, x => x.Select(y => new ProtocolTsjProxy
                                                    {
                                                        File = y.File,
                                                        Type = y.Type
                                                    })
                                                    .ToList());

                this.ManagersByContragentDict = contragentContactList
                    .Where(x => x.Position != null)
                    .Select(x => new
                    {
                        x.Contragent.Id,
                        x.Position.Name
                    })
                    .ToList()
                    .GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Name).AggregateWithSeparator(", "));

                this.HeadInfoByContragentDict = contragentContactList
                    .Where(x => x.Position.Name == "Председатель правления ТСЖ")
                    .Select(x => new
                    {
                        x.Contragent.Id,
                        x.Surname,
                        Firstname = x.Name,
                        x.Patronymic,
                        Gender = x.Gender == Gender.Male
                            ? RisGender.M
                            : RisGender.F,
                        DateOfBirth = x.BirthDate,
                        x.Snils
                    })
                    .GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.Select(y => new HeadInfoProxy
                                                    {
                                                        Surname = y.Surname,
                                                        Firstname = y.Firstname,
                                                        Patronymic = y.Patronymic,
                                                        Gender = y.Gender,
                                                        DateOfBirth = y.DateOfBirth,
                                                        Snils = y.Snils
                                                    }).First());
            }
            finally
            {
                this.Container.Release(activityTsjProtocolDomain);
                this.Container.Release(contragentContactDomain);
                this.Container.Release(manOrgDomain);
                this.Container.Release(activityTsjDomain);
                this.Container.Release(activityTsjStatuteDomain);
            }
        }

        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var fileUploadService = this.Container.Resolve<IFileUploadService>();
            var activityTsjStatuteDomain = this.Container.ResolveDomain<ActivityTsjStatute>();
            var actTsjProtRoDomain = this.Container.ResolveDomain<ActivityTsjProtocolRealObj>();
            var charterDomain = this.Container.ResolveDomain<Charter>();
            var protocolMeetOwnerDomain = this.Container.ResolveDomain<ProtocolMeetingOwner>();

            try
            {
                var charterListToSave = activityTsjStatuteDomain.GetAll()
                    .Where(x => x.ActivityTsj != null)
                    .Where(x => x.ActivityTsj.ManagingOrganization != null)
                    .Select(x => new Charter
                    {
                        ExternalSystemEntityId = x.Id,
                        ExternalSystemName = "gkh",
                        DocNum = x.ConclusionNum,
                        DocDate = x.ActivityTsj.ManagingOrganization.Contragent.DateRegistration,
                        Managers = this.ManagersByContragentDict.ContainsKey(x.ActivityTsj.ManagingOrganization.Contragent.Id)
                            ? this.ManagersByContragentDict[x.ActivityTsj.ManagingOrganization.Contragent.Id]
                            : null,
                        Head = new RisInd
                        {
                            Surname = this.HeadInfoByContragentDict.ContainsKey(x.ActivityTsj.ManagingOrganization.Contragent.Id)
                                ? this.HeadInfoByContragentDict[x.ActivityTsj.ManagingOrganization.Contragent.Id].Surname
                                : null,
                            FirstName = this.HeadInfoByContragentDict.ContainsKey(x.ActivityTsj.ManagingOrganization.Contragent.Id)
                                ? this.HeadInfoByContragentDict[x.ActivityTsj.ManagingOrganization.Contragent.Id].Firstname
                                : null,
                            Patronymic = this.HeadInfoByContragentDict.ContainsKey(x.ActivityTsj.ManagingOrganization.Contragent.Id)
                                ? this.HeadInfoByContragentDict[x.ActivityTsj.ManagingOrganization.Contragent.Id].Patronymic
                                : null,
                            Sex = this.HeadInfoByContragentDict.ContainsKey(x.ActivityTsj.ManagingOrganization.Contragent.Id)
                                ? this.HeadInfoByContragentDict[x.ActivityTsj.ManagingOrganization.Contragent.Id].Gender
                                : RisGender.F,
                            DateOfBirth = this.HeadInfoByContragentDict.ContainsKey(x.ActivityTsj.ManagingOrganization.Contragent.Id)
                                ? this.HeadInfoByContragentDict[x.ActivityTsj.ManagingOrganization.Contragent.Id].DateOfBirth
                                : null,
                            Snils = this.HeadInfoByContragentDict.ContainsKey(x.ActivityTsj.ManagingOrganization.Contragent.Id)
                                ? this.HeadInfoByContragentDict[x.ActivityTsj.ManagingOrganization.Contragent.Id].Snils
                                : null,
                        },
                        Attachment = x.StatuteFile != null 
                            ? fileUploadService.SaveAttachment(
                                x.StatuteFile, x.ConclusionDescription, FileStorageName.HomeManagement, this.Contragent.SenderId)
                            : null
                    });

                var headIndListToSave = charterListToSave.Where(x => x.Head != null).Select(x => x.Head).ToList();

                this.SaveRisEntities<RisInd, ActivityTsjStatute>(headIndListToSave);
                this.SaveRisEntities<Charter, ActivityTsjStatute>(charterListToSave.ToList());

                var protocolMeetOwnerList = new List<ProtocolMeetingOwner>();

                foreach (var charter in charterListToSave)
                {
                    if (!this.StatuteIdByActTsjIdDict.ContainsKey(charter.Id))
                    {
                        continue;
                    }
                    if (!this.ProtocolListByActivityTsjDict.ContainsKey(this.StatuteIdByActTsjIdDict[charter.Id]))
                    {
                        continue;
                    }

                    var protocolTsjProxyList = this.ProtocolListByActivityTsjDict[this.StatuteIdByActTsjIdDict[charter.Id]];

                    foreach (var protocolTsjProxy in protocolTsjProxyList)
                    {
                        var protocolMeetingOwner = new ProtocolMeetingOwner
                        {
                            Charter = charter,
                            Attachment = fileUploadService.SaveAttachment(
                                protocolTsjProxy.File, protocolTsjProxy.Type, FileStorageName.HomeManagement, this.Contragent.SenderId)
                        };

                        protocolMeetOwnerDomain.Save(protocolMeetingOwner);
                        protocolMeetOwnerList.Add(protocolMeetingOwner);
                    }
                }

                var charterByStatuteIdDict = charterDomain.GetAll().ToDictionary(x => x.ExternalSystemEntityId);

                var contractObjectListToSave = actTsjProtRoDomain.GetAll()
                    .Select(x => new ContractObject
                    {
                        ExternalSystemEntityId = x.Id,
                        ExternalSystemName = "gkh",
                        Charter = this.StatuteIdByActTsjIdDict.ContainsKey(x.ActivityTsjProtocol.ActivityTsj.Id)
                            ? charterByStatuteIdDict.ContainsKey(this.StatuteIdByActTsjIdDict[x.ActivityTsjProtocol.ActivityTsj.Id])
                                ? charterByStatuteIdDict[this.StatuteIdByActTsjIdDict[x.ActivityTsjProtocol.ActivityTsj.Id]]
                                : null
                            : null,
                        FiasHouseGuid = x.RealityObject.HouseGuid,
                        BaseMServiseCurrentCharter = true
                    })
                    .Where(x => x.Charter != null)
                    .Where(x => x.FiasHouseGuid != "")
                    .ToList();

                this.SaveRisEntities<ContractObject, ActivityTsjProtocolRealObj>(contractObjectListToSave);

                var extractedDataDict = new Dictionary<Type, List<BaseRisEntity>>();

                extractedDataDict.Add(typeof(RisInd), headIndListToSave.Cast<BaseRisEntity>().ToList());
                extractedDataDict.Add(typeof(Charter), charterListToSave.Cast<BaseRisEntity>().ToList());
                extractedDataDict.Add(typeof(ProtocolMeetingOwner), protocolMeetOwnerList.Cast<BaseRisEntity>().ToList());
                extractedDataDict.Add(typeof(ContractObject), contractObjectListToSave.Cast<BaseRisEntity>().ToList());

                return extractedDataDict;
            }
            finally
            {
                this.Container.Release(fileUploadService);
                this.Container.Release(activityTsjStatuteDomain);
                this.Container.Release(actTsjProtRoDomain);
                this.Container.Release(charterDomain);
                this.Container.Release(protocolMeetOwnerDomain);
            }
        }

        private class HeadInfoProxy
        {
            public string Surname { get; set; }
            public string Firstname { get; set; }
            public string Patronymic { get; set; }
            public RisGender Gender { get; set; }
            public DateTime? DateOfBirth { get; set; }
            public string Snils { get; set; }
        }

        private class ProtocolTsjProxy
        {
            public FileInfo File { get; set; }
            public string Type { get; set; }
        }
    }
}