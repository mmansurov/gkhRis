namespace Bars.Gkh.Ris.Integration.Inspection.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using B4.DataAccess;
    using B4.Utils;

    using Entities.GisIntegration;
    using Entities.Inspection;
    using Gkh.Domain;
    using Inspection;
    using Ris.Inspection;

    /// <summary>
    /// Метод передачи данных по инспекциям
    /// </summary>
    public class ImportInspectionPlanMethod : GisIntegrationInspectionMethod<InspectionPlan, importInspectionPlanRequest>
    {
        private readonly Dictionary<string, InspectionPlan> inspPlanByTransportGuidDict = new Dictionary<string, InspectionPlan>();
        private readonly Dictionary<string, Examination> examinationByTransportGuidDict = new Dictionary<string, Examination>();

        private readonly List<InspectionPlan> inspPlansToSave = new List<InspectionPlan>();
        private readonly List<Examination> examinationsToSave = new List<Examination>();

        protected override int ProcessedObjects
        {
            get
            {
                return this.inspPlansToSave.Count + this.examinationsToSave.Count;
            }
        }

        public override string Code
        {
            get
            {
                return "importInspectionPlan";
            }
        }

        public override string Name
        {
            get
            {
                return "Импорт планов и проверок ГЖИ";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 18;
            }
        }

        protected override int Portion { get { return 1000; } }

        protected override IList<InspectionPlan> MainList { get; set; }

        private Dictionary<long, List<Examination>> ExaminationsToInspPlanDict;

        protected override void Prepare()
        {
            var inspPlanDomain = this.Container.ResolveDomain<InspectionPlan>();
            var examDomain = this.Container.ResolveDomain<Examination>();

            try
            {
                this.MainList = inspPlanDomain.GetAll().ToList();

                this.ExaminationsToInspPlanDict = examDomain.GetAll()
                    .Where(x => x.InspectionPlan != null)
                    .GroupBy(x => x.InspectionPlan.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
            finally
            {
                this.Container.Release(inspPlanDomain);
                this.Container.Release(examDomain);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.inspPlansToSave, 1000, true, true);
            TransactionHelper.InsertInManyTransactions(this.Container, this.examinationsToSave, 1000, true, true);
        }

        protected override ImportResult GetRequestResult(importInspectionPlanRequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importInspectionPlan(this.RequestHeader, request, out result);
            }

            return result;
        }

        protected override importInspectionPlanRequest GetRequestObject(IEnumerable<InspectionPlan> listForImport)
        {
            var importInspRequestList = new List<importInspectionPlanRequestImportInspectionPlan>();

            foreach (var plan in listForImport)
            {
                var planNotation = new StringBuilder();

                if (plan.Year == 0)
                {
                    planNotation.Append("YEAR ");
                }

                if (!plan.ApprovalDate.HasValue)
                {
                    planNotation.Append("APPROVAL_DATE ");
                }

                if (planNotation.Length > 0)
                {
                    this.AddLineToLog("План проверок", plan.Id, "Не загружен", planNotation);
                    continue;
                }

                var planRequest = new importInspectionPlanRequestImportInspectionPlan
                {
                    TransportGUID = Guid.NewGuid().ToString(),
                    InspectionPlan = new InspectionPlanType
                    {
                        Year = (short)plan.Year,
                        ApprovalDate = plan.ApprovalDate ?? DateTime.MinValue
                    }
                };

                var examinations = this.ExaminationsToInspPlanDict.ContainsKey(plan.Id)
                    ? this.ExaminationsToInspPlanDict[plan.Id]
                    : null;
                if (examinations != null)
                {
                    var examinationRequestList =
                        new List<importInspectionPlanRequestImportInspectionPlanImportPlannedExamination>();

                    foreach (var examination in examinations)
                    {
                        StringBuilder examNotation = this.CheckExamination(examination);

                        if (examNotation.Length > 0)
                        {
                            this.AddLineToLog("Проверка", plan.Id, "Не загружена", examNotation);
                            continue;
                        }

                        var examinationRequest = new importInspectionPlanRequestImportInspectionPlanImportPlannedExamination
                        {
                            TransportGUID = Guid.NewGuid().ToString(),
                            PlannedExamination = this.GetPlan(examination)
                        };

                        this.examinationByTransportGuidDict.Add(examinationRequest.TransportGUID, examination);
                        examinationRequestList.Add(examinationRequest);
                        this.CountObjects++;
                    }

                    if (examinationRequestList.Any())
                    {
                        planRequest.ImportPlannedExamination = examinationRequestList.ToArray();
                    }
                }

                this.inspPlanByTransportGuidDict.Add(planRequest.TransportGUID, plan);
                importInspRequestList.Add(planRequest);
                this.CountObjects++;
            }

            return new importInspectionPlanRequest
            {
                Id = blockToSignId,
                ImportInspectionPlan = importInspRequestList.ToArray()
            };
        }

        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (this.inspPlanByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
                var plan = this.inspPlanByTransportGuidDict[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;

                    var errorNotation = string.Empty;

                    if (error != null)
                    {
                        errorNotation = error.Description;
                    }

                    this.AddLineToLog("План проверок", plan.Id, "Не загружен", errorNotation);
                    return;
                }

                plan.Guid = responseItem.GUID;
                this.inspPlansToSave.Add(plan);

                this.AddLineToLog("План проверок", plan.Id, "Загружен", responseItem.GUID);
                return;
            }

            if (this.examinationByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
                var examination = this.examinationByTransportGuidDict[responseItem.TransportGUID];
                examination.Guid = responseItem.GUID;
                this.examinationsToSave.Add(examination);

                this.AddLineToLog("Проверка", examination.Id, "Загружена", responseItem.GUID);
            }
        }

        private StringBuilder CheckExamination(Examination examination)
        {
            var examNotation = new StringBuilder();

            if (!examination.InspectionNumber.HasValue)
            {
                examNotation.Append("INSPECTIONNUMBER ");
            }

            if (examination.RisContragent == null)
            {
                examNotation.Append("CONTRAGENT_ID ");
            }

            if (examination.Objective.IsEmpty())
            {
                examNotation.Append("OBJECTIVE ");
            }

            if (examination.BaseCode.IsEmpty())
            {
                examNotation.Append("BASE_CODE ");
            }

            if (examination.BaseGuid.IsEmpty())
            {
                examNotation.Append("BASE_GUID ");
            }

            if (examination.ExaminationFormCode.IsEmpty())
            {
                examNotation.Append("EXAMFORM_CODE ");
            }

            if (examination.ExaminationFormGuid.IsEmpty())
            {
                examNotation.Append("EXAMFORM_GUID ");
            }

            return examNotation;
        }

        private PlannedExaminationType GetPlan(Examination examination)
        {
            return new PlannedExaminationType
            {
                NumberInPlan = examination.InspectionNumber ?? 0,
                Subject = new ScheduledExaminationSubjectInfoType
                {
                    Item = new ScheduledExaminationSubjectInfoTypeOrganization
                    {
                        orgRootEntityGUID = examination.RisContragent.OrgRootEntityGuid,
                        ActualActivityPlace = examination.RisContragent.FactAddress
                    }
                },
                Objective = examination.Objective,
                Base = new nsiRef
                {
                    Code = examination.BaseCode,
                    GUID = examination.BaseGuid
                },
                Duration = new PlannedExaminationTypeDuration
                {
                    Item = examination.Duration,
                    ItemElementName = ItemChoiceType.WorkDays
                },
                ExaminationForm = new nsiRef
                {
                    Code = examination.ExaminationFormCode,
                    GUID = examination.ExaminationFormGuid
                }
            };
        }
    }
}