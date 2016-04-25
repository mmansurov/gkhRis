namespace Bars.Gkh.Ris.Integration
{
    using B4;
    using B4.DataAccess;
    using B4.Utils;
    using Castle.Windsor;
    using Domain;
    using Entities;
    using global::Quartz;
    using Quartz.Scheduler;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;
    using GisServiceProvider;
    using Bars.Gkh.Ris.Enums;

    /// <summary>
    /// Базовый экспортер данных
    /// </summary>
    /// <typeparam name="TRequestType">Тип запроса к асинхронному сервису ГИС</typeparam>
    /// <typeparam name="TSoapClient">Тип SOAP клиента</typeparam>
    public abstract class BaseDataExporter<TRequestType, TSoapClient> : IDataExporter
    {
        /// <summary>
        /// IoC контейнер
        /// </summary>
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Soap клиент
        /// </summary>
        public IGisServiceProvider<TSoapClient> ServiceProvider { get; set; }

        /// <summary>
        /// Идентификатор поставщика данных
        /// </summary>
        public string SenderId
        {
            get
            {
                return this.Contragent != null ? this.Contragent.SenderId : string.Empty;
            }
        }

        /// <summary>
        /// Поставщик данных
        /// </summary>
        public RisContragent Contragent { get; set; }

        /// <summary>
        /// Наименование экспортера
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Порядок экспортера
        /// </summary>
        public abstract int Order { get; }

        /// <summary>
        /// Описание экспортера
        /// </summary>
        public virtual string Description
        {
            get
            {
                return this.Name;
            }
        }

        /// <summary>
        /// Необходимо подписывать данные
        /// </summary>
        public virtual bool NeedSign
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Максимальное количество повторов триггера получения результатов экспорта
        /// </summary>
        public virtual int MaxRepeatCount
        {
            get
            {
                return 10;
            }
        }

        /// <summary>
        /// Интервал запуска триггера получения результатов экспорта - в секундах
        /// </summary>
        public virtual int Interval
        {
            get
            {
                return 30;
            }
        }

        /// <summary>
        /// Метод подготовки данных к экспорту, включая валидацию, формирование пакетов
        /// </summary>
        /// <param name="parameters">Параметры экспорта</param>
        /// <returns>Результат подготовки данных</returns>
        public PrepareDataResult PrepareData(DynamicDictionary parameters)
        {
            this.ExtractData(parameters);

            var validateResult = this.ValidateData();

            var requestData = this.GetRequestData();

            var packages = new List<RisPackage>();

            var i = 1;

            foreach (var requestDataItem in requestData)
            {
                var package = new RisPackage();
                package.SetNotSignedXml(this.SerializeRequest(requestDataItem.Key));
                package.SetTransportGuidDictionary(requestDataItem.Value);
                package.Name = string.Format("Пакет {0}", i);
                package.RisContragent = this.Contragent;

                packages.Add(package);
                i++;
            }

            this.SavePackages(packages);

            return new PrepareDataResult
            {
                ValidateResult = validateResult,
                Packages = packages
            };
        }

        /// <summary>
        /// Сохранить подписанные данные
        /// </summary>
        /// <param name="signedData">Словарь подписанных данных: Идентификатор пакета - Подписанный XML</param>
        public void SaveSignedData(Dictionary<long, string> signedData)
        {
            var packagesDomain = this.Container.ResolveDomain<RisPackage>();

            try
            {
                foreach (var signedPackage in signedData)
                {
                    var package = packagesDomain.Get(signedPackage.Key);

                    package.SetSignedXml(signedPackage.Value);

                    packagesDomain.Update(package);
                }
            }
            finally
            {
                this.Container.Release(packagesDomain);
            }
        }

        /// <summary>
        /// Запустить экспорт
        /// </summary>
        /// <param name="packageIds">Идентификаторы пакетов</param>
        /// <returns>Результат отправки данных на обработку</returns>
        public DataSendingResult Execute(long[] packageIds)
        {
            if (packageIds.Length == 0)
            {
                throw new Exception("Нет данных для выполнения экспорта");
            }

            var result = new DataSendingResult
            {
                PackageSendingResults = this.SendPackages(packageIds)
            };

            var successResults = result.PackageSendingResults.Where(x => x.Success).ToList();

            if (successResults.Count > 0)
            {
                result.Task = this.CreateTask();

                var taskPackages = new List<RisTaskPackage>();

                foreach (var packageSendingResult in successResults)
                {
                    var taskPackage = new RisTaskPackage
                    {
                        Package = packageSendingResult.Package,
                        Task = result.Task,
                        AckMessageGuid = packageSendingResult.AckMessageGuid,
                        State = PackageProcessingState.Waiting
                    };

                    taskPackages.Add(taskPackage);
                }

                this.SaveTaskPackages(taskPackages);
                this.ScheduleExportTask(result.Task, taskPackages);
            }

            return result;
        }

        /// <summary>
        /// Собрать данные
        /// </summary>
        /// <param name="parameters">Параметры экспорта</param>
        protected abstract void ExtractData(DynamicDictionary parameters);

        /// <summary>
        /// Валидация данных
        /// </summary>
        /// <returns>Результат валидации</returns>
        protected abstract List<ValidateObjectResult> ValidateData();

        /// <summary>
        /// Сформировать объекты запросов к асинхронному сервису ГИС
        /// </summary>
        /// <returns>Словарь Объект запроса - Словарь Транспортных идентификаторов: Тип обектов - Словарь: Транспортный идентификатор - Идентификатор объекта</returns>
        protected abstract Dictionary<TRequestType, Dictionary<Type, Dictionary<string, long>>> GetRequestData();

        /// <summary>
        /// Выполнить запрос к асинхронному сервису ГИС
        /// </summary>
        /// <param name="request">Объект запроса</param>
        /// <returns>Идентификатор сообщения для получения результата</returns>
        protected abstract string ExecuteRequest(TRequestType request);

        /// <summary>
        /// Получит тип задачи получения результатов экспорта
        /// </summary>
        /// <returns>Тип задачи</returns>
        protected abstract Type GetTaskType();

        /// <summary>
        /// Получить входные данные для выполнения задачи получения результатов экспорта
        /// </summary>
        /// <returns>Входные данные</returns>
        protected virtual JobDataMap GetJobDataMap()
        {
            return new JobDataMap();
        }

        private XmlDocument SerializeRequest(TRequestType data)
        {
            XmlDocument result;

            var attr = (XmlTypeAttribute)typeof(TRequestType).GetCustomAttribute(typeof(XmlTypeAttribute));
            var xmlSerializer = new XmlSerializer(typeof(TRequestType), attr.Namespace);

            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    xmlSerializer.Serialize(xmlWriter, data);

                    result = new XmlDocument();
                    result.LoadXml(stringWriter.ToString());
                }
            }

            return result;
        }

        private TRequestType DeserializeRequest(string data)
        {
            TRequestType result;

            var attr = (XmlTypeAttribute)typeof(TRequestType).GetCustomAttribute(typeof(XmlTypeAttribute));
            var xmlSerializer = new XmlSerializer(typeof(TRequestType), attr.Namespace);

            using (XmlReader reader = XmlReader.Create(new StringReader(data)))
            {
                result = (TRequestType)xmlSerializer.Deserialize(reader);
            }

            return result;
        }

        private void SavePackages(List<RisPackage> packages)
        {
            TransactionHelper.InsertInManyTransactions(this.Container, packages);
        }

        private void SaveTaskPackages(List<RisTaskPackage> taskPackages)
        {
            TransactionHelper.InsertInManyTransactions(this.Container, taskPackages, 1000, true, true);
        }

        private List<RisPackage> GetPackages(long[] packageIds)
        {
            var packagesDomain = this.Container.ResolveDomain<RisPackage>();

            try
            {
                return packagesDomain.GetAll().Where(x => packageIds.Contains(x.Id)).ToList();
            }
            finally
            {
                this.Container.Release(packagesDomain);
            }
        }

        private List<PackageSendingResult> SendPackages(long[] packageIds)
        {
            var result = new List<PackageSendingResult>();
            var packages = this.GetPackages(packageIds);

            foreach (var package in packages)
            {
                var request = this.DeserializeRequest(
                    this.NeedSign ? package.GetSignedXmlString() : package.GetNotSignedXmlString());

                if (this.Contragent == null)
                {
                    this.Contragent = package.RisContragent;
                }

                try
                {
                    var ackMessageGuid = this.ExecuteRequest(request);

                    var packageSendingResult = new PackageSendingResult
                    {
                        Package = package,
                        Success = true,
                        AckMessageGuid = ackMessageGuid
                    };

                    result.Add(packageSendingResult);
                }
                catch (Exception exception)
                {
                    var packageSendingResult = new PackageSendingResult
                    {
                        Package = package,
                        Success = false,
                        Message = exception.Message
                    };

                    result.Add(packageSendingResult);
                }
            }

            return result;
        }

        private RisTask CreateTask()
        {
            var task = new RisTask
            {
                ClassName = this.GetTaskType().Name,
                Description = this.Name,
                QuartzTriggerKey = this.GetTriggerIdentity(),
                MaxRepeatCount = this.MaxRepeatCount,
                Interval = this.Interval,
                StartTime = DateTime.Now
            };

            var taskDomain = this.Container.ResolveDomain<RisTask>();

            try
            {
                taskDomain.Save(task);
            }
            finally
            {
                this.Container.Release(taskDomain);
            }

            return task;
        }

        private void ScheduleExportTask(RisTask task, List<RisTaskPackage> taskPackages)
        {
            var scheduler = this.Container.Resolve<IScheduler>("TaskScheduler");
            var taskType = this.GetTaskType();

            try
            {
                var triggerIdentity = task.QuartzTriggerKey;
                var jobIdentity = this.GetJobIdentity(taskType, triggerIdentity);
                var jobDataMap = this.GetJobDataMap();

                this.AddRequiredJobData(jobDataMap, task, taskPackages);

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerIdentity)
                    .WithDescription(this.Name)
                    .ForJob(jobIdentity)
                    .WithSimpleSchedule(x => x
                     .WithIntervalInSeconds(this.Interval)
                     .WithRepeatCount(this.MaxRepeatCount)
                     .WithMisfireHandlingInstructionFireNow())
                    .StartNow()
                    .UsingJobData(jobDataMap)
                    .Build();

                var jobDetail = scheduler.GetJobDetail(JobKey.Create(jobIdentity));

                if (jobDetail == null)
                {
                    jobDetail = JobBuilder.Create(taskType)
                        .WithIdentity(jobIdentity)
                        .Build();

                    scheduler.ScheduleJob(jobDetail, trigger);
                }
                else
                {
                    scheduler.ScheduleJob(trigger);
                }
            }
            finally
            {
                this.Container.Release(scheduler);
            }
        }

        private string GetTriggerIdentity()
        {
            return Guid.NewGuid().ToStr();
        }

        private string GetJobIdentity(Type taskType, string triggerIdentity)
        {
            return string.Format("{0}_{1}", taskType.Name, triggerIdentity);
        }

        private void AddRequiredJobData(JobDataMap jobDataMap, RisTask task, List<RisTaskPackage> taskPackages)
        {
            jobDataMap.Put("TaskId", task.Id);

            jobDataMap.Put("ServiceAddress", this.ServiceProvider.ServiceAddress);

            jobDataMap.Put("Name", this.Name);

            jobDataMap.Put("SenderId", this.SenderId);

            jobDataMap.Put("ExecutionOwner", this.GetExecutionOwner());
        }

        private IExecutionOwner GetExecutionOwner()
        {
            try
            {
                var userInfo = this.Container.Resolve<RequestingUserInformation>();
                return new UserExecutionOwner
                {
                    UserId = userInfo.UserIdentity.UserId,
                    TrackId = userInfo.UserIdentity.TrackId,
                    RequestIpAddress = userInfo.RequestIpAddress,
                    Name = userInfo.UserIdentity.Name
                };
            }
            catch
            {
                return new SystemExecutionOwner();
            }
        }
    }
}
