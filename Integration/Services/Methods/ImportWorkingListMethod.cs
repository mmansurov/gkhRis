namespace Bars.Gkh.Ris.Integration.Services.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.Services;
    using Ris.Services;
    using Attachment = Entities.Attachment;

    public class ImportWorkingListMethod : GisIntegrationServicesMethod<WorkList, importWorkingListRequest>
    {
        private Dictionary<string, WorkList> worksByTransportGuid = new Dictionary<string, WorkList>();
        private List<WorkList> worksToSave = new List<WorkList>();
        private Dictionary<string, WorkListItem> workListItemsByTransportGuid = new Dictionary<string, WorkListItem>();
        private List<WorkListItem> workListItemsToSave = new List<WorkListItem>();
        private Dictionary<long, List<Attachment>> attachmentsByWorkListId = new Dictionary<long, List<Attachment>>();
        private Dictionary<long, List<WorkListItem>> workListItemsByWorkListId = new Dictionary<long, List<WorkListItem>>();


        /// <summary>
        /// Количество объектов для сохранения. Переопределять как количество объектов для сохранения
        /// </summary>
        protected override int ProcessedObjects
        {
            get { return this.worksToSave.Count; }
        }

        /// <summary>
        /// Код импорта.
        /// </summary>
        public override string Code
        {
            get { return "importWorkingList"; }
        }

        /// <summary>
        /// Наименование импорта в списке.
        /// </summary>
        public override string Name
        {
            get { return "Импорт перечня работ и услуг на период"; }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 39;
            }
        }

        /// <summary>
        /// Максимаьлное количество объектов ГИС в одном файле импорта.
        /// </summary>
        protected override int Portion
        {
            get { return 1; }
        }

        /// <summary>
        /// Список объектов ГИС, по которым производится импорт.
        /// </summary>
        protected override IList<WorkList> MainList { get; set; }

        /// <summary>
        /// Подготовка кэша данных для PrepareRequest. Заполнить MainList.
        /// </summary>
        protected override void Prepare()
        {
            var serviceDomain = this.Container.ResolveDomain<WorkList>();
            var attachDomain = this.Container.ResolveDomain<WorkingListAttachment>();
            var workListItemDomain = this.Container.ResolveDomain<WorkListItem>();

            try
            {
                this.MainList = serviceDomain.GetAll().ToList();

                this.attachmentsByWorkListId = attachDomain.GetAll()
                    .Where(x => x.Attachment != null && x.WorkList != null)
                    .ToArray()
                    .Select(x => new
                    {
                        WorkListId = x.WorkList.Id,
                        Attach = x.Attachment
                    })
                    .GroupBy(x => x.WorkListId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Attach).ToList());

                this.workListItemsByWorkListId = workListItemDomain.GetAll()
                    .Where(x => x.WorkList != null)
                    .ToArray()
                    .Select(x => new
                    {
                        WorkListId = x.WorkList.Id,
                        Item = x
                    })
                    .GroupBy(x => x.WorkListId)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Item).ToList());
            }
            finally
            {
                this.Container.Release(serviceDomain);
                this.Container.Release(attachDomain);
                this.Container.Release(workListItemDomain);
            }
        }

        /// <summary>
        /// Проверка объекта
        /// </summary>
        /// <param name="item">Объект</param>
        /// <returns>Результат проверки</returns>
        protected override CheckingResult CheckMainListItem(WorkList item)
        {
            StringBuilder messages = new StringBuilder();

            if (item.House == null || item.House.FiasHouseGuid.IsEmpty())
            {
                messages.Append("FIASHOUSEGUID ");
            }

            if (item.YearFrom == 0)
            {
                messages.Append("MONTHYEARFROM/YEAR ");
            }

            if (item.MonthFrom == 0)
            {
                messages.Append("MONTHYEARFROM/MONTH ");
            }

            if (item.YearTo == 0)
            {
                messages.Append("MONTHYEARTO/YEAR ");
            }

            if (item.MonthTo == 0)
            {
                messages.Append("MONTHYEARTO/MONTH ");
            }

            if (!this.attachmentsByWorkListId.ContainsKey(item.Id))
            {
                messages.Append("ATTACHMENT ");
            }

            if (!this.workListItemsByWorkListId.ContainsKey(item.Id))
            {
                messages.Append("WORKLISTITEM ");
            }

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        protected override importWorkingListRequest GetRequestObject(IEnumerable<WorkList> listForImport)
        {
            var workList = listForImport.ToList()[0]; // Portion == 1

            var item = new importWorkingListRequestApprovedWorkingListData
            {
                TransportGUID = Guid.NewGuid().ToString(),
                FIASHouseGuid = workList.House.FiasHouseGuid,
                MonthYearFrom = new WorkingListBaseTypeMonthYearFrom
                {
                    Month = workList.MonthFrom,
                    Year = workList.YearFrom
                },
                MonthYearTo = new WorkingListBaseTypeMonthYearTo
                {
                    Year = workList.YearTo,
                    Month = workList.MonthTo
                },
                Attachment = this.GetAttachments(workList.Id),
                ContractGUID = workList.Contract.Guid,
                WorkListItem = this.GetWorkLists(workList.Id)
            };

            this.CountObjects++;
            this.worksByTransportGuid.Add(item.TransportGUID, workList);

            var result = new importWorkingListRequest
            {
                Item = item
            };

            return result;
        }

        protected override ImportResult GetRequestResult(importWorkingListRequest request)
        {
            ImportResult result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importWorkingList(this.RequestHeader, request, out result);
            }

            return result;
        }

        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (this.worksByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                var work = this.worksByTransportGuid[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;
                    var errorNotation = string.Empty;

                    if (error != null)
                    {
                        errorNotation = error.Description;
                    }

                    this.AddLineToLog("Перечень работ", work.Id, "Не загружен", errorNotation);
                    return;
                }

                work.Guid = responseItem.GUID;
                this.worksToSave.Add(work);

                this.AddLineToLog("Перечень работ", work.Id, "Загружен", responseItem.GUID);
            }

            if (this.workListItemsByTransportGuid.ContainsKey(responseItem.TransportGUID))
            {
                var workList = this.workListItemsByTransportGuid[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;
                    var errorNotation = string.Empty;

                    if (error != null)
                    {
                        errorNotation = error.Description;
                    }

                    this.AddLineToLog("Работа перечня", workList.Id, "Не загружена", errorNotation);
                    return;
                }

                workList.Guid = responseItem.GUID;
                this.workListItemsToSave.Add(workList);

                this.AddLineToLog("Работа перечня", workList.Id, "Загружена", responseItem.GUID);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.worksToSave, 1000, true, true);
            TransactionHelper.InsertInManyTransactions(this.Container, this.workListItemsToSave, 1000, true, true);
        }

        private importWorkingListRequestApprovedWorkingListDataWorkListItem[] GetWorkLists(long workListId)
        {
            var result = new List<importWorkingListRequestApprovedWorkingListDataWorkListItem>();

            foreach (var workList in this.workListItemsByWorkListId[workListId])
            {
                var newWorkList = new importWorkingListRequestApprovedWorkingListDataWorkListItem
                {
                    TransportGUID = Guid.NewGuid().ToString(),
                    ItemsElementName = new [] { ItemsChoiceType4.TotalCost },
                    Items = new object[] { workList.TotalCost },
                    WorkItemNSI = new nsiRef
                    {
                        Code = workList.WorkItemNsi.GisDictRef.GisId,
                        GUID = workList.WorkItemNsi.GisDictRef.GisGuid
                    },
                    Index = workList.Index.ToString()
                };

                this.workListItemsByTransportGuid.Add(newWorkList.TransportGUID, workList);
                result.Add(newWorkList);
            }

            return result.ToArray();
        }

        private AttachmentType[] GetAttachments(long workListId)
        {
            var result = new List<AttachmentType>();

            foreach (var attach in this.attachmentsByWorkListId[workListId])
            {
                var newAttach = new AttachmentType
                {
                    Name = attach.Name,
                    Description = attach.Description,
                    AttachmentHASH = attach.Hash,
                    Attachment = new Ris.Services.Attachment
                    {
                        AttachmentGUID = attach.Guid
                    }
                };

                result.Add(newAttach);
            }

            return result.ToArray();
        }
    }
}
