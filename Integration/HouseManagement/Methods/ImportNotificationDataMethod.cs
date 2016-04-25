namespace Bars.Gkh.Ris.Integration.HouseManagement.Methods
{
    using B4.DataAccess;
    using B4.Utils;
    using Domain;
    using Entities.HouseManagement;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Ris.HouseManagement;

    public class ImportNotificationDataMethod : GisIntegrationHouseManagementMethod<RisNotification, importNotificationRequest>
    {
        private readonly Dictionary<string, RisNotification> notificationsByTransportGuidDict = new Dictionary<string, RisNotification>();
        private Dictionary<long, List<RisNotificationAddressee>> fiasAddrByNotificationId = new Dictionary<long, List<RisNotificationAddressee>>();
        private Dictionary<long, List<RisNotificationAttachment>> attachmentsByNotificationId = new Dictionary<long, List<RisNotificationAttachment>>();
        private readonly List<RisNotification> notificationsToSave = new List<RisNotification>();

        protected override int ProcessedObjects
        {
            get { return this.notificationsToSave.Count; }
        }

        public override string Code
        {
            get { return "importNotificationData"; }
        }

        public override string Name
        {
            get { return "Импорт новостей для информирования граждан"; }
        }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public override int Order
        {
            get
            {
                return 34;
            }
        }

        protected override int Portion
        {
            get { return 100; }
        }

        protected override IList<RisNotification> MainList { get; set; }

        protected override void Prepare()
        {
            var notificationDomain = this.Container.ResolveDomain<RisNotification>();
            var notificationAddresseeDomain = this.Container.ResolveDomain<RisNotificationAddressee>();
            var attachmentDomain = this.Container.ResolveDomain<RisNotificationAttachment>();

            try
            {
                this.MainList = notificationDomain.GetAll().ToList();

                this.fiasAddrByNotificationId = notificationAddresseeDomain.GetAll()
                    .Where(x => x.House != null && x.Notification != null)
                    .GroupBy(x => x.Notification)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());

                this.attachmentsByNotificationId = attachmentDomain.GetAll()
                    .Where(x => x.Notification != null && x.Attachment != null)
                    .GroupBy(x => x.Notification)
                    .ToDictionary(x => x.Key.Id, x => x.ToList());
            }
            finally
            {
                this.Container.Release(notificationDomain);
                this.Container.Release(notificationAddresseeDomain);
                this.Container.Release(attachmentDomain);
            }
        }

        protected override CheckingResult CheckMainListItem(RisNotification item)
        {
            StringBuilder messages = new StringBuilder();

            if (item.Topic.IsEmpty())
            {
                messages.Append("CREATE/TOPIC ");
            }

            if (item.Content.IsEmpty())
            {
                messages.Append("CREATE/CONTENT ");
            }

            if (!item.IsAll.HasValue)
            {
                messages.Append("CREATE/ISALL ");
            }
            else
            {
                if (!(bool)item.IsAll && !item.EndDate.HasValue)
                {
                    messages.Append("CREATE/ENDDATE ");
                }
            }  

            return new CheckingResult { Result = messages.Length == 0, Messages = messages };
        }

        protected override importNotificationRequest GetRequestObject(IEnumerable<RisNotification> listForImport)
        {
            List<importNotificationRequestNotification> notifications = new List<importNotificationRequestNotification>();

            foreach (var notification in listForImport)
            {
                object[] items;
                object[] items1;
                Items1ChoiceType[] items1Names;

                if (notification.IsAll.HasValue && (bool)notification.IsAll)
                {
                    items = new object[]{ true };
                }
                else
                {
                    items = this.GetFiasGuids(notification);
                }

                if (notification.IsNotLimit.HasValue && (bool) notification.IsNotLimit)
                {
                    items1 = new object[] { true };
                    items1Names = new [] { Items1ChoiceType.IsNotLimit};
                }
                else
                {
                    items1 = new object[]
                    {(notification.StartDate ?? DateTime.MinValue), (notification.EndDate ?? DateTime.MinValue)};
                    items1Names = new [] { Items1ChoiceType.StartDate, Items1ChoiceType.EndDate };
                }

                var notificationToAdd = new importNotificationRequestNotification
                {
                    TransportGUID = Guid.NewGuid().ToString(),
                    Item = new importNotificationRequestNotificationCreate
                    {
                        Topic = notification.Topic,
                        IsImportant = notification.IsImportant ?? false,
                        content = notification.Content,
                        Items = items,
                        Items1 = items1,
                        Items1ElementName = items1Names,
                        Attachment = this.GetAttachments(notification),
                        IsShipOff = notification.IsShipOff ?? false
                    }
                };

                notifications.Add(notificationToAdd);

                this.CountObjects++;
                this.notificationsByTransportGuidDict.Add(notificationToAdd.TransportGUID, notification);
            }

            return new importNotificationRequest
            {
                notification = notifications.ToArray()
            };
        }

        protected override ImportResult1 GetRequestResult1(importNotificationRequest request)
        {
            ImportResult1 result = null;
            var soapClient = this.ServiceProvider.GetSoapClient();

            if (soapClient != null)
            {
                soapClient.importNotificationData(this.RequestHeader, request, out result);
            }

            return result;
        }

        protected override void CheckResponseItem(CommonResultType responseItem)
        {
            if (this.notificationsByTransportGuidDict.ContainsKey(responseItem.TransportGUID))
            {
                var notification = this.notificationsByTransportGuidDict[responseItem.TransportGUID];

                if (responseItem.GUID.IsEmpty())
                {
                    var error = responseItem.Items.FirstOrDefault() as CommonResultTypeError;
                    var errorNotation = string.Empty;

                    if (error != null)
                    {
                        errorNotation = error.Description;
                    }

                    this.AddLineToLog("Новость", notification.Id, "Не загружена", errorNotation);
                    return;
                }

                notification.Guid = responseItem.GUID;
                this.notificationsToSave.Add(notification);

                this.AddLineToLog("Новость", notification.Id, "Загружена", responseItem.GUID);
            }
        }

        protected override void SaveObjects()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.notificationsToSave, 1000, true, true);
        }

        private AttachmentType[] GetAttachments(RisNotification notification)
        {
            List<AttachmentType> result = new List<AttachmentType>();

            if (this.attachmentsByNotificationId.ContainsKey(notification.Id))
            {
                foreach (var attach in this.attachmentsByNotificationId[notification.Id])
                {
                    var attachment = attach.Attachment;

                    if (attachment != null)
                    {
                        result.Add(new AttachmentType
                        {
                            Name = attachment.Name,
                            Description = attachment.Description,
                            Attachment = new Attachment
                            {
                                AttachmentGUID = attachment.Guid
                            },
                            AttachmentHASH = attachment.Hash
                        });
                    }
                }
            }

            return result.ToArray();
        }

        private object[] GetFiasGuids(RisNotification notification)
        {
            List<object> result = new List<object>();

            if (this.fiasAddrByNotificationId.ContainsKey(notification.Id))
            {
                foreach (var item in this.fiasAddrByNotificationId[notification.Id])
                {
                    var house = item.House;
                    if (house != null && !house.FiasHouseGuid.IsEmpty())
                    {
                        result.Add(house.FiasHouseGuid);
                    }
                }
            }

            return result.ToArray();
        }
    }
}
