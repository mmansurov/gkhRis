namespace Bars.Gkh.Ris.Integration.Inspection.DataExtractors
{
    using B4.DataAccess;
    using B4.Utils;
    using Entities;
    using Entities.GisIntegration.Ref;
    using Entities.Inspection;
    using Domain;
    using GkhGji.Entities;
    using GkhGji.Enums;
    using Ris.Inspection;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Извлечение данных по планам и инспекциям
    /// </summary>
    public class ImportInspectionPlanDataExtractor : GisIntegrationDataExtractorBase
    {
        public override string Code
        {
            get
            {
                return "importInspectionPlan";
            }
        }

        private Dictionary<long, nsiRef> examFormDict;
        private Dictionary<long, nsiRef> examBaseDict;
        private Dictionary<long, nsiRef> nsiRefsKindCheckByIndId;
        private Dictionary<long, string> parentDispTypeSurvGoalByInspIdDict;
        private Dictionary<long, RisContragent> risContragentDict;

        protected override void FillDictionaries()
        {
            var gisDictRefDomain = this.Container.ResolveDomain<GisDictRef>();
            var childDocDomain = this.Container.ResolveDomain<DocumentGjiChildren>();
            var disposalTypeSurveyDomain = this.Container.ResolveDomain<DisposalTypeSurvey>();
            var typeSurvGoalInspGjiDomain = this.Container.ResolveDomain<TypeSurveyGoalInspGji>();
            var risContragentDomain = this.Container.ResolveDomain<RisContragent>();
            var disposalDomain = this.Container.ResolveDomain<Disposal>();

            try
            {
                var nsiRefsByKindChecksId = gisDictRefDomain.GetAll()
                    .Where(x => x.Dict.ActionCode == "Вид проверки")
                    .Select(x => new { x.GkhId, x.GisId, x.GisGuid })
                    .ToList()
                    .GroupBy(x => x.GkhId)
                    .ToDictionary(x => x.Key, x => x.Select(y => new nsiRef
                    {
                        Code = y.GisId,
                        GUID = y.GisGuid
                    }).First());

                var kindChecksByIndId = disposalDomain.GetAll()
                    .Where(x => x.Inspection != null && x.KindCheck != null)
                    .GroupBy(x => x.Inspection.Id)
                    .ToDictionary(x => x.Key, x => x.First().KindCheck);

                foreach (var checkIndPair in kindChecksByIndId)
                {
                    if (checkIndPair.Value != null && nsiRefsByKindChecksId.ContainsKey(checkIndPair.Value.Id))
                    {
                        this.nsiRefsKindCheckByIndId.Add(checkIndPair.Key, nsiRefsByKindChecksId[checkIndPair.Value.Id]);
                    }
                }

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

                this.risContragentDict = risContragentDomain.GetAll()
                    .GroupBy(x => x.GkhId)
                    .ToDictionary(x => x.Key, x => x.First());
            }
            finally
            {
                this.Container.Release(gisDictRefDomain);
                this.Container.Release(childDocDomain);
                this.Container.Release(disposalTypeSurveyDomain);
                this.Container.Release(typeSurvGoalInspGjiDomain);
                this.Container.Release(risContragentDomain);
                this.Container.Release(disposalDomain);
            }
        }

        protected override Dictionary<Type, List<BaseRisEntity>> ExtractInternal(DynamicDictionary parameters)
        {
            var gkhJurPersPlanDomain = this.Container.ResolveDomain<PlanJurPersonGji>();
            var gkhJurPersCheckDomain = this.Container.ResolveDomain<BaseJurPerson>();
            var gkhInsCheckPlanDomain = this.Container.ResolveDomain<PlanInsCheckGji>();
            var gkhInsCheckDomain = this.Container.ResolveDomain<BaseInsCheck>();
            var risJurPersPlanDomain = this.Container.ResolveDomain<InspectionPlan>();

            try
            {
                var inspectionPlanListToSave = gkhJurPersPlanDomain.GetAll().ToList()
                    .Where(x => x.DateApproval.HasValue)
                    .Select(x => new InspectionPlan
                    {
                        ExternalSystemEntityId = x.Id,
                        ExternalSystemName = "gkh",
                        Year = x.Name.Split(" ").Length > 1
                            ? x.Name.Split(" ")[1].ToInt()
                            : 0,
                        ApprovalDate = x.DateApproval ?? DateTime.MinValue
                    })
                    .ToList();

                this.SaveRisEntities<InspectionPlan, PlanJurPersonGji>(inspectionPlanListToSave);

                var insCheckPlanListToSave = gkhInsCheckPlanDomain.GetAll().ToList()
                    .Select(x => new InspectionPlan
                    {
                        ExternalSystemEntityId = x.Id,
                        ExternalSystemName = "gkh",
                        Year = x.Name.Split(" ").Length > 1
                            ? x.Name.Split(" ")[1].ToInt()
                            : 0,
                        ApprovalDate = x.DateApproval ?? DateTime.MinValue
                    })
                    .ToList();

                this.SaveRisEntities<InspectionPlan, PlanInsCheckGji>(insCheckPlanListToSave);

                var gkhInspIdToExamDict = risJurPersPlanDomain.GetAll()
                    .Select(x => new
                    {
                        x.Id,
                        x.ExternalSystemEntityId
                    })
                    .ToList()
                    .GroupBy(x => x.ExternalSystemEntityId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Id).First());

                var jurPersExaminationListToSave = gkhJurPersCheckDomain.GetAll()
                    .Where(x => x.Contragent != null)
                    .Where(x => x.TypeFact == TypeFactInspection.NotDone)
                    .Where(x => gkhInspIdToExamDict.Keys.Contains(x.Plan.Id))
                    .ToList()
                    .Select(x => new Examination
                    {
                        ExternalSystemEntityId = x.Id,
                        IsScheduled = true,
                        ExternalSystemName = "gkh",
                        InspectionPlan = gkhInspIdToExamDict.ContainsKey(x.Plan.Id) 
                            ? new InspectionPlan { Id = gkhInspIdToExamDict[x.Plan.Id] }
                            : null,
                        InspectionNumber = x.InspectionNumber.Split("-").Count() > 1
                            ? (int?)x.InspectionNumber.Split("-")[1].ToInt()
                            : null,
                        RisContragent = this.risContragentDict.ContainsKey(x.Contragent.Id)
                            ? this.risContragentDict[x.Contragent.Id]
                            : null,
                        Objective = this.parentDispTypeSurvGoalByInspIdDict.ContainsKey(x.Id)
                            ? this.parentDispTypeSurvGoalByInspIdDict[x.Id]
                            : null,
                        BaseCode = this.examBaseDict.ContainsKey((long)x.TypeBaseJuralPerson)
                            ? this.examBaseDict[(long)x.TypeBaseJuralPerson].Code
                            : null,
                        BaseGuid = this.examBaseDict.ContainsKey((long)x.TypeBaseJuralPerson)
                            ? this.examBaseDict[(long)x.TypeBaseJuralPerson].GUID
                            : null,
                        Duration = x.CountDays.ToDouble(),
                        ExaminationFormCode = this.examFormDict.ContainsKey((long)x.TypeForm)
                            ? this.examFormDict[(long)x.TypeForm].Code
                            : null,
                        ExaminationFormGuid = this.examFormDict.ContainsKey((long)x.TypeForm)
                            ? this.examFormDict[(long)x.TypeForm].GUID
                            : null
                    }).ToList();

                this.SaveRisEntities<Examination, BaseJurPerson>(jurPersExaminationListToSave);

                var insExaminationListToSave = gkhInsCheckDomain.GetAll()
                    .Where(x => x.Contragent != null)
                    .Where(x => x.TypeFact == TypeFactInspection.NotDone)
                    .Where(x => gkhInspIdToExamDict.Keys.Contains(x.Plan.Id))
                    .ToList()
                    .Select(x => new Examination
                    {
                        ExternalSystemEntityId = x.Id,
                        IsScheduled = true,
                        ExternalSystemName = "gkh",
                        InspectionPlan = gkhInspIdToExamDict.ContainsKey(x.Plan.Id)
                            ? new InspectionPlan { Id = gkhInspIdToExamDict[x.Plan.Id] }
                            : null,
                        InspectionNumber = x.InspectionNumber.Split("-").Count() > 1
                            ? (int?)x.InspectionNumber.Split("-")[1].ToInt()
                            : null,
                        RisContragent = this.risContragentDict.ContainsKey(x.Contragent.Id)
                            ? this.risContragentDict[x.Contragent.Id]
                            : null,
                        Objective = this.parentDispTypeSurvGoalByInspIdDict.ContainsKey(x.Id)
                            ? this.parentDispTypeSurvGoalByInspIdDict[x.Id]
                            : null,
                        BaseCode = "4c22401a-38e6-4fe5-bdb8-ad3191cab650",
                        Duration = x.CountDays.ToDouble(),
                        ExaminationFormCode = this.nsiRefsKindCheckByIndId.ContainsKey(x.Id)
                            ? this.nsiRefsKindCheckByIndId[x.Id].Code
                            : "2",
                        ExaminationFormGuid = this.nsiRefsKindCheckByIndId.ContainsKey(x.Id)
                            ? this.nsiRefsKindCheckByIndId[x.Id].GUID
                            : "f57e497c-a991-4ddc-919e-4a4dc1c3b4da"
                    }).ToList();

                this.SaveRisEntities<Examination, BaseInsCheck>(insExaminationListToSave);

                return new Dictionary<Type, List<BaseRisEntity>>();
            }
            finally
            {
                this.Container.Release(gkhJurPersPlanDomain);
                this.Container.Release(risJurPersPlanDomain);
                this.Container.Release(gkhJurPersCheckDomain);
                this.Container.Release(gkhInsCheckPlanDomain);
                this.Container.Release(gkhInsCheckDomain);
            }
        }
    }
}