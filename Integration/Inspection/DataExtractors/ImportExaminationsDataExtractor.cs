namespace Bars.Gkh.Ris.Integration.Inspection.DataExtractors
{
    using B4.DataAccess;
    using B4.Utils;
    using Entities;
    using Entities.GisIntegration.Ref;
    using Entities.Inspection;
    using GkhGji.Entities;
    using GkhGji.Enums;
    using Ris.Inspection;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Извлечение данных по проверкам
    /// </summary>
    public class ImportExaminationsDataExtractor : GisIntegrationDataExtractorBase
    {
        public override string Code
        {
            get
            {
                return "importExaminations";
            }
        }

        private Dictionary<long, nsiRef> examFormDict;
        private Dictionary<long, nsiRef> examBaseDict;
        private Dictionary<long, Disposal> parentDispByInspIdDict;
        private Dictionary<long, string> parentDispTypeSurvGoalByInspIdDict;
        private Dictionary<long, string> parentDispTypeSurvTaskByInspIdDict;
        private Dictionary<long, RisContragent> risContragentDict;
        private Dictionary<long, string[]> physicalPersByJurPerson;

        protected override void FillDictionaries()
        {
            var gisDictRefDomain = this.Container.ResolveDomain<GisDictRef>();
            var childDocDomain = this.Container.ResolveDomain<DocumentGjiChildren>();
            var gkhDisposalDomain = this.Container.ResolveDomain<Disposal>();
            var disposalTypeSurveyDomain = this.Container.ResolveDomain<DisposalTypeSurvey>();
            var typeSurvGoalInspGjiDomain = this.Container.ResolveDomain<TypeSurveyGoalInspGji>();
            var typeSurvTaskInspGjiDomain = this.Container.ResolveDomain<TypeSurveyTaskInspGji>();
            var risContragentDomain = this.Container.ResolveDomain<RisContragent>();
            var jurPersonDomain = this.Container.ResolveDomain<BaseJurPerson>();

            try
            {
                this.examFormDict = gisDictRefDomain.GetAll()
                    .Where(x => x.Dict.ActionCode == "Формы проверки")
                    .Select(x => new { x.GkhId, x.GisId, x.GisGuid })
                    .ToList()
                    .GroupBy(x => x.GkhId)
                    .ToDictionary(x => x.Key, x => x.Select(y => new nsiRef
                    { Code = y.GisId, GUID = y.GisGuid }).First());

                this.examBaseDict = gisDictRefDomain.GetAll()
                    .Where(x => x.Dict.ActionCode == "Основание проверки")
                    .Select(x => new { x.GkhId, x.GisId, x.GisGuid })
                    .ToList()
                    .GroupBy(x => x.GkhId)
                    .ToDictionary(x => x.Key, x => x.Select(y => new nsiRef
                    { Code = y.GisId, GUID = y.GisGuid }).First());

                var childDisposalIds = childDocDomain.GetAll()
                    .Where(x => x.Children.TypeDocumentGji == TypeDocumentGji.Disposal)
                    .Select(x => x.Children.Id)
                    .Distinct();

                this.parentDispByInspIdDict = gkhDisposalDomain.GetAll()
                    .Where(x => childDisposalIds.All(y => y != x.Id))
                    .GroupBy(x => x.Inspection.Id)
                    .ToDictionary(x => x.Key, x => x.First());

                this.parentDispTypeSurvGoalByInspIdDict = disposalTypeSurveyDomain.GetAll()
                    .Where(x => childDisposalIds.All(y => y != x.Disposal.Id))
                    .Join(typeSurvGoalInspGjiDomain.GetAll(),
                        dispTypeSurv => dispTypeSurv.TypeSurvey.Id,
                        typeSurvGoal => typeSurvGoal.TypeSurvey.Id,
                        (dispTypeSurv, typeSurvGoal) => new
                        {
                            InspectionId = dispTypeSurv.Disposal.Inspection.Id,
                            Goal = typeSurvGoal.SurveyPurpose.Name
                        })
                    .ToList()
                    .GroupBy(x => x.InspectionId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Goal).First());

                this.parentDispTypeSurvTaskByInspIdDict = disposalTypeSurveyDomain.GetAll()
                    .Where(x => childDisposalIds.All(y => y != x.Disposal.Id))
                    .Join(typeSurvTaskInspGjiDomain.GetAll(),
                        dispTypeSurv => dispTypeSurv.TypeSurvey.Id,
                        typeSurvTask => typeSurvTask.TypeSurvey.Id,
                        (dispTypeSurv, typeSurvTask) => new
                        {
                            InspectionId = dispTypeSurv.Disposal.Inspection.Id,
                            Task = typeSurvTask.SurveyObjective.Name
                        })
                    .ToList()
                    .GroupBy(x => x.InspectionId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Task).First());

                this.risContragentDict = risContragentDomain.GetAll()
                    .GroupBy(x => x.GkhId)
                    .ToDictionary(x => x.Key, x => x.First());

                this.physicalPersByJurPerson = jurPersonDomain.GetAll()
                   .GroupBy(x => x.Id)
                   .ToDictionary(x => x.Key, y => !string.IsNullOrWhiteSpace(y.First().PhysicalPerson) ? y.First().PhysicalPerson.Split(' ') : new string[0]);
            }
            finally
            {
                this.Container.Release(gisDictRefDomain);
                this.Container.Release(childDocDomain);
                this.Container.Release(gkhDisposalDomain);
                this.Container.Release(disposalTypeSurveyDomain);
                this.Container.Release(typeSurvGoalInspGjiDomain);
                this.Container.Release(typeSurvTaskInspGjiDomain);
                this.Container.Release(risContragentDomain);
                this.Container.Release(jurPersonDomain);
            }
        }

        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var jurPersonDomain = this.Container.ResolveDomain<BaseJurPerson>();
            var prosClaimDomain = this.Container.ResolveDomain<BaseProsClaim>();
            var dispHeadDomain = this.Container.ResolveDomain<BaseDispHead>();
            var baseStatementDomain = this.Container.ResolveDomain<BaseStatement>();
            var insCheckDomain = this.Container.ResolveDomain<BaseInsCheck>();
            var childDocDomain = this.Container.ResolveDomain<DocumentGjiChildren>();
            var examinationDomain = this.Container.ResolveDomain<Examination>();
            var prescrDomain = this.Container.ResolveDomain<Prescription>();
            var prescrCloseDocDomain = this.Container.ResolveDomain<PrescriptionCloseDoc>();
            var protocolDomain = this.Container.ResolveDomain<Protocol>();

            try
            {
                var examinationListToSave = new List<Examination>();

                var jurPersonList = jurPersonDomain.GetAll()
                    .Where(x => this.parentDispByInspIdDict.Keys.Contains(x.Id))
                    .ToList()
                    .Select(x => new Examination
                    {
                        ExternalSystemEntityId = x.Id,
                        ExternalSystemName = "gkh",
                        ExaminationFormCode = this.examFormDict.ContainsKey((long)x.TypeForm)
                            ? this.examFormDict[(long)x.TypeForm].Code
                            : null,
                        ExaminationFormGuid = this.examFormDict.ContainsKey((long)x.TypeForm)
                            ? this.examFormDict[(long)x.TypeForm].GUID
                            : null,
                        OrderGkhId = this.parentDispByInspIdDict[x.Id].Id,
                        OrderNumber = this.parentDispByInspIdDict.ContainsKey(x.Id)
                            ? this.parentDispByInspIdDict[x.Id].DocumentNumber
                            : null,
                        OrderDate = this.parentDispByInspIdDict.ContainsKey(x.Id)
                            ? this.parentDispByInspIdDict[x.Id].DocumentDate
                            : null,
                        IsScheduled = true,
                        RisContragent = this.risContragentDict.ContainsKey(x.Contragent.Id)
                            ? this.risContragentDict[x.Contragent.Id]
                            : null,
                        IsPhysicalPerson = x.PersonInspection == PersonInspection.PhysPerson,
                        OversightActivitiesCode = "1",
                        OversightActivitiesGuid = "cb8a08ad-44a7-41a3-b43c-3253e1a198da",
                        BaseCode = this.examBaseDict.ContainsKey((long)x.TypeBaseJuralPerson)
                            ? this.examBaseDict[(long)x.TypeBaseJuralPerson].Code
                            : null,
                        BaseGuid = this.examBaseDict.ContainsKey((long)x.TypeBaseJuralPerson)
                            ? this.examBaseDict[(long)x.TypeBaseJuralPerson].GUID
                            : null,
                        Objective = this.parentDispTypeSurvGoalByInspIdDict.ContainsKey(x.Id)
                            ? this.parentDispTypeSurvGoalByInspIdDict[x.Id]
                            : null,
                        From = this.parentDispByInspIdDict.ContainsKey(x.Id)
                            ? this.parentDispByInspIdDict[x.Id].DateStart
                            : null,
                        To = this.parentDispByInspIdDict.ContainsKey(x.Id)
                            ? this.parentDispByInspIdDict[x.Id].DateEnd
                            : null,
                        Duration = x.CountDays.ToDouble(),
                        ObjectCode = "9",
                        ObjectGuid = "2af020d8-2ffd-4ccc-a4a5-9bdc6cd67b82",
                        Tasks = this.parentDispTypeSurvTaskByInspIdDict.ContainsKey(x.Id)
                            ? this.parentDispTypeSurvTaskByInspIdDict[x.Id]
                            : null
                    }).ToList();

                this.SetNames(jurPersonList);

                examinationListToSave.AddRange(jurPersonList);
                this.SaveRisEntities<Examination, BaseJurPerson>(examinationListToSave);

                var gkhInspIdToExamDict = examinationDomain.GetAll()
                    .Select(x => new
                    {
                        x.Id,
                        x.ExternalSystemEntityId
                    })
                    .ToList()
                    .GroupBy(x => x.ExternalSystemEntityId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Id).First());

                var parentDisposalIds = examinationListToSave.Select(x => x.OrderGkhId).ToArray();

                var prescrIds = childDocDomain.GetAll()
                    .Where(x => parentDisposalIds.Contains(x.Parent.Id))
                    .Where(x => x.Children.TypeDocumentGji == TypeDocumentGji.Prescription)
                    .Select(x => x.Children.Id)
                    .ToArray();

                var prescrCloseDocDict = prescrCloseDocDomain.GetAll()
                    .Where(x => prescrIds.Contains(x.Prescription.Id))
                    .Select(x => new
                    {
                        x.Prescription.Id,
                        x.Date
                    })
                    .ToList()
                    .GroupBy(x => x.Id)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Date).First());

                if (prescrIds.Length > 0)
                {
                    var preceptListToSave = prescrDomain.GetAll()
                        .Where(x => prescrIds.Contains(x.Id))
                        .Where(x => x.DocumentDate.HasValue && x.DocumentNumber.IsNotEmpty())
                        .ToList()
                        .Select(x => new Precept
                        {
                            ExternalSystemEntityId = x.Id,
                            ExternalSystemName = "gkh",
                            Examination = new Examination { Id = gkhInspIdToExamDict[x.Inspection.Id] },
                            Number = x.DocumentNumber,
                            Date = x.DocumentDate ?? DateTime.MinValue,
                            CancelReason = x.CloseReason.GetEnumMeta().Display,
                            CancelDate = prescrCloseDocDict.ContainsKey(x.Id)
                                ? prescrCloseDocDict[x.Id]
                                : DateTime.MinValue
                        })
                        .ToList();

                    this.SaveRisEntities<Precept, Prescription>(preceptListToSave);
                }

                if (parentDisposalIds.Length > 0)
                {
                    var protocolIds = childDocDomain.GetAll()
                        .Where(x => parentDisposalIds.Contains(x.Parent.Id))
                        .Where(x => x.Children.TypeDocumentGji == TypeDocumentGji.Protocol)
                        .Select(x => x.Children.Id)
                        .ToArray();

                    if (protocolIds.Length > 0)
                    {
                        var offenceListToSave = protocolDomain.GetAll()
                            .Where(x => protocolIds.Contains(x.Id))
                            .Where(x => x.DocumentDate.HasValue && x.DocumentNumber.IsNotEmpty())
                            .ToList()
                            .Select(x => new Offence
                            {
                                ExternalSystemEntityId = x.Id,
                                ExternalSystemName = "gkh",
                                Examination = new Examination { Id = gkhInspIdToExamDict[x.Inspection.Id] },
                                Number = x.DocumentNumber,
                                Date = x.DocumentDate ?? DateTime.MinValue
                            })
                            .ToList();

                        this.SaveRisEntities<Offence, Protocol>(offenceListToSave);
                    }
                }

                return new Dictionary<Type, List<BaseRisEntity>>();
            }
            finally
            {
                this.Container.Release(jurPersonDomain);
                this.Container.Release(prosClaimDomain);
                this.Container.Release(dispHeadDomain);
                this.Container.Release(baseStatementDomain);
                this.Container.Release(insCheckDomain);
                this.Container.Release(childDocDomain);
                this.Container.Release(examinationDomain);
                this.Container.Release(prescrDomain);
                this.Container.Release(prescrCloseDocDomain);
                this.Container.Release(protocolDomain);
            }
        }

        private void SetNames(List<Examination> examination)
        {
            foreach (var exam in examination)
            {
                if (this.physicalPersByJurPerson.ContainsKey(exam.ExternalSystemEntityId))
                {
                    var nameItems = this.physicalPersByJurPerson[exam.ExternalSystemEntityId];

                    if (nameItems.Length > 0)
                    {
                        exam.LastName = nameItems[0];
                    }
                    if (nameItems.Length > 1)
                    {
                        exam.FirstName = nameItems[1];
                    }
                    if (nameItems.Length > 2)
                    {
                        exam.MiddleName = nameItems[2];
                    }
                }
            }
        }
    }
}