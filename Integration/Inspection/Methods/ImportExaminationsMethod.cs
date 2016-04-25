namespace Bars.Gkh.Ris.Integration.Inspection.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using B4.DataAccess;
    using B4.Utils;

    using Domain;
    using Entities.Inspection;
    using Ris.Inspection;

    /// <summary>
    /// Метод передачи данных по планам и инспекциям
    /// </summary>
    public class ImportExaminationsMethod : GisIntegrationInspectionMethod<Examination, importExaminationsRequest>
    {
        private Dictionary<long, List<Precept>> preceptsToExamDict;
        private Dictionary<long, List<Offence>> offencesToExamDict;

        private Dictionary<string, Examination> examByTransportGuidDict = new Dictionary<string, Examination>();
        private Dictionary<string, Precept> preceptByTransportGuidDict = new Dictionary<string, Precept>();
        private Dictionary<string, Offence> offenceByTransportGuidDict = new Dictionary<string, Offence>();

        private List<Examination> examsToSave = new List<Examination>();
        private List<Precept> preceptsToSave = new List<Precept>();
        private List<Offence> offencesToSave = new List<Offence>();

        protected override int ProcessedObjects { get { return this.examsToSave.Count; } }

        public override string Code
        {
            get
            {
                return "importExaminations";
            }
        }

        public override string Name
        {
            get
            {
                return "Импорт проверок ГЖИ";
            }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 20;
            }
        }

        protected override int Portion { get { return 1000; } }

        protected override IList<Examination> MainList { get; set; }

        protected override void Prepare()
        {
            var examDomain = this.Container.ResolveDomain<Examination>();
            var preceptDomain = this.Container.ResolveDomain<Precept>();
            var offenceDomain = this.Container.ResolveDomain<Offence>();

            try
            {
                this.MainList = examDomain.GetAll().ToList();

                this.preceptsToExamDict = preceptDomain.GetAll()
                    .GroupBy(x => x.Examination.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());

                this.offencesToExamDict = offenceDomain.GetAll()
                    .GroupBy(x => x.Examination.Id)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }
            finally
            {
                this.Container.Release(examDomain);
                this.Container.Release(preceptDomain);
                this.Container.Release(offenceDomain);
            }
        }

       
       
        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.examsToSave, 1000, true, true);
            TransactionHelper.InsertInManyTransactions(this.Container, this.preceptsToSave, 1000, true, true);
            TransactionHelper.InsertInManyTransactions(this.Container, this.offencesToSave, 1000, true, true);
        }

        protected override ImportResult GetRequestResult(importExaminationsRequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importExaminations(this.RequestHeader, request, out result);
            }

            return result;
        }

        protected override importExaminationsRequest GetRequestObject(IEnumerable<Examination> listForImport)
        {
            var importExamRequestList = new List<importExaminationsRequestImportExamination>();

            foreach (var exam in listForImport)
            {
                var examRequest = new importExaminationsRequestImportExamination
                {
                    TransportGUID = Guid.NewGuid().ToString()
                };

                ExaminationType examType = this.GetExamType(exam);
                examRequest.Examination = examType;

                var precepts = this.preceptsToExamDict.ContainsKey(exam.Id)
                    ? this.preceptsToExamDict[exam.Id]
                    : null;
                if (precepts != null)
                {
                    var preceptRequestList = new List<importExaminationsRequestImportExaminationImportPrecept>();

                    foreach (var precept in precepts)
                    {
                        var preceptRequest = new importExaminationsRequestImportExaminationImportPrecept
                        {
                            TransportGUID = Guid.NewGuid().ToString()
                        };

                        if (precept.CancelDate != DateTime.MinValue)
                        {
                            preceptRequest.Item = new PreceptType
                            {
                                Number = precept.Number,
                                Date = precept.Date
                            };
                        }
                        else
                        {
                            preceptRequest.Item = new importExaminationsRequestImportExaminationImportPreceptCancelPrecept
                            {
                                CancelledInfo = new CancelledInfoWithAttachmentsType
                                {
                                    Reason = precept.CancelReason,
                                    Date = precept.CancelDate ?? DateTime.MinValue
                                }
                            };
                        }

                        this.preceptByTransportGuidDict.Add(preceptRequest.TransportGUID, precept);
                        preceptRequestList.Add(preceptRequest);

                    }

                    examRequest.ImportPrecept = preceptRequestList.ToArray();
                }

                var offences = this.offencesToExamDict.ContainsKey(exam.Id)
                    ? this.offencesToExamDict[exam.Id]
                    : null;
                if (offences != null)
                {
                    var offenceRequestList = new List<importExaminationsRequestImportExaminationImportOffence>();

                    foreach (var offence in offences)
                    {
                        var offenceRequest = new importExaminationsRequestImportExaminationImportOffence
                        {
                            TransportGUID = Guid.NewGuid().ToString(),
                            Item = new OffenceType
                            {
                                Number = offence.Number,
                                Date = offence.Date
                            }
                        };

                        this.offenceByTransportGuidDict.Add(offenceRequest.TransportGUID, offence);
                        offenceRequestList.Add(offenceRequest);
                        this.CountObjects++;
                    }

                    examRequest.ImportOffence = offenceRequestList.ToArray();
                }

                this.examByTransportGuidDict.Add(examRequest.TransportGUID, exam);
                importExamRequestList.Add(examRequest);
            }

            this.CountObjects += importExamRequestList.Count;

            return new importExaminationsRequest
            {
                ImportExamination = importExamRequestList.ToArray()
            };
        }

        protected override void CheckResponseItem(CommonResultType responseExam)
        {
            if (!this.examByTransportGuidDict.ContainsKey(responseExam.TransportGUID))
            {
                return;
            }

            var exam = this.examByTransportGuidDict[responseExam.TransportGUID];

            if (responseExam.GUID.IsEmpty())
            {
                var error = responseExam.Items.FirstOrDefault() as CommonResultTypeError;

                var errorNotation = string.Empty;

                if (error != null)
                {
                    errorNotation = error.Description;
                }

                this.AddLineToLog("Проверка", exam.Id, "Не загружена", errorNotation);
                return;
            }

            exam.Guid = responseExam.GUID;
            this.examsToSave.Add(exam);

            this.AddLineToLog("Проверка", exam.Id, "Загружена", exam.Guid);
        }

        protected override CheckingResult CheckMainListItem(Examination exam)
        {
            var notation = new StringBuilder();

            if (exam.ExaminationFormCode.IsEmpty())
            {
                notation.Append("EXAMFORM_CODE, ");
            }
            if (exam.ExaminationFormGuid.IsEmpty())
            {
                notation.Append("EXAMFORM_GUID, ");
            }
            if (exam.OrderNumber.IsEmpty())
            {
                notation.Append("ORDER_NUMBER, ");
            }
            if (exam.OrderDate == null)
            {
                notation.Append("ORDER_DATE, ");
            }
            if (exam.RisContragent == null)
            {
                notation.Append("CONTRAGENT_ID, ");
            }
            if (!exam.IsScheduled && exam.IsPhysicalPerson)
            {
                if (exam.LastName == null)
                {
                    notation.Append("LASTNAME, ");
                }
                if (exam.FirstName == null)
                {
                    notation.Append("FIRSTNAME, ");
                }
                if (exam.MiddleName == null)
                {
                    notation.Append("MIDDLENAME, ");
                }
            }
            if (exam.OversightActivitiesCode.IsEmpty())
            {
                notation.Append("OVERSIGHT_ACT_CODE, ");
            }
            if (exam.OversightActivitiesGuid.IsEmpty())
            {
                notation.Append("OVERSIGHT_ACT_GUID, ");
            }
            if (exam.BaseCode.IsEmpty())
            {
                notation.Append("BASE_CODE, ");
            }
            if (exam.BaseGuid.IsEmpty())
            {
                notation.Append("BASE_GUID, ");
            }
            if (exam.Objective.IsEmpty())
            {
                notation.Append("OBJECTIVE, ");
            }
            if (exam.ObjectCode.IsEmpty())
            {
                notation.Append("OBJECT_CODE, ");
            }
            if (exam.ObjectGuid.IsEmpty())
            {
                notation.Append("OBJECT_GUID, ");
            }
            if (exam.Tasks.IsEmpty())
            {
                notation.Append("TASKS, ");
            }

            return new CheckingResult { Result = notation.Length == 0, Messages = notation };
        }

        private ExaminationType GetExamType(Examination exam)
        {
            var examType = new ExaminationType();

            examType.ExaminationForm = new nsiRef
            {
                Code = exam.ExaminationFormCode,
                GUID = exam.ExaminationFormGuid
            };

            examType.OrderNumber = exam.OrderNumber;
            examType.OrderDate = exam.OrderDate ?? DateTime.MinValue;

            if (exam.IsScheduled)
            {
                examType.ExaminationTypeType = new ExaminationTypeExaminationTypeType
                {
                    Item = new ExaminationTypeExaminationTypeTypeScheduled
                    {
                        Subject = exam.RisContragent.IsIndividual
                            ? new ScheduledExaminationSubjectInfoType
                            {
                                Item = new ScheduledExaminationSubjectInfoTypeIndividual
                                {
                                    orgRootEntityGUID = exam.RisContragent.OrgRootEntityGuid
                                }
                            }
                            : new ScheduledExaminationSubjectInfoType
                            {
                                Item = new ScheduledExaminationSubjectInfoTypeOrganization
                                {
                                    orgRootEntityGUID = exam.RisContragent.OrgRootEntityGuid
                                }
                            }
                    }
                };
            }
            else
            {
                if (exam.IsPhysicalPerson)
                {
                    examType.ExaminationTypeType = new ExaminationTypeExaminationTypeType
                    {
                        Item = new ExaminationTypeExaminationTypeTypeUnscheduled
                        {
                            Subject = new UnscheduledExaminationSubjectInfoType
                            {
                                Item = new CitizenInfoType
                                {
                                    LastName = exam.LastName,
                                    FirstName = exam.FirstName,
                                    MiddleName = exam.MiddleName
                                }
                            }
                        }
                    };
                }
                else
                {
                    examType.ExaminationTypeType = new ExaminationTypeExaminationTypeType
                    {
                        Item = new ExaminationTypeExaminationTypeTypeUnscheduled
                        {
                            Subject = exam.RisContragent.IsIndividual
                            ? new UnscheduledExaminationSubjectInfoType
                            {
                                Item = new ScheduledExaminationSubjectInfoTypeIndividual
                                {
                                    orgRootEntityGUID = exam.RisContragent.OrgRootEntityGuid
                                }
                            }
                            : new UnscheduledExaminationSubjectInfoType
                            {
                                Item = new ScheduledExaminationSubjectInfoTypeOrganization
                                {
                                    orgRootEntityGUID = exam.RisContragent.OrgRootEntityGuid
                                }
                            }
                        }
                    };
                }
            }

            examType.OversightActivitiesRef = new nsiRef
            {
                Code = exam.OversightActivitiesCode,
                GUID = exam.OversightActivitiesGuid
            };

            examType.Base = new nsiRef
            {
                Code = exam.BaseCode,
                GUID = exam.BaseGuid
            };

            examType.Objective = exam.Objective;

            if (exam.From != null)
            {
                examType.From = exam.From.Value;
            }

            if (exam.To != null)
            {
                examType.To = exam.To.Value;
            }

            examType.Duration = new ExaminationTypeDuration
            {
                Item = exam.Duration
            };

            examType.Object = new[] { new nsiRef
                            { Code = exam.ObjectCode, GUID = exam.ObjectGuid }
                        };

            examType.Tasks = exam.Tasks;

            return examType;
        }
    }
}