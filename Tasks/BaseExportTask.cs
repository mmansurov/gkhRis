namespace Bars.Gkh.Ris.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using B4;
    using B4.DataAccess;
    using B4.Modules.FileStorage;
    using Domain;
    using Import;
    using Quartz.Scheduler;
    using Entities;
    using Entities.GisIntegration;
    using Enums;
    using GisServiceProvider;
    using global::Quartz;

    /// <summary>
    /// Базовый класс задачи получения результатов экспорта пакетов данных
    /// </summary>
    /// <typeparam name="TResponceType">Тип объектов, содержащих ответ от сервиса</typeparam>
    /// <typeparam name="TSoapClient">Тип soap клиента</typeparam>
    public abstract class BaseExportTask<TResponceType, TSoapClient> : BaseTask
    {
        /// <summary>
        /// Soap клиент
        /// </summary>
        public IGisServiceProvider<TSoapClient> ServiceProvider { get; set; }

        /// <summary>
        /// Наименование экспортера - резолвится из контекста
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор поставщика данных - резолвится из контекста
        /// </summary>
        public string SenderId { get; set; }

        /// <summary>
        /// Хранимый объект Задача
        /// </summary>
        public RisTask Task { get; set; }

        /// <summary>
        /// Список пакетов на обработку
        /// </summary>
        public List<RisTaskPackage> Packages { get; set; }

        /// <summary>
        /// Метод выполнения задачи. При выполнении проверять значение флага InterruptRequested.
        /// </summary>
        /// <param name="context">Контекст выполнения задачи</param>
        /// <returns>Результат выполнения задачи</returns>
        protected override object DoWork(IJobExecutionContext context)
        {
            this.Initialize(context);

            var processedPackages = new List<RisTaskPackage>();

            foreach (var package in this.Packages)
            {
                if (this.InterruptRequested)
                {
                    break;
                }

                if (this.ProcessPackage(package))
                {
                    processedPackages.Add(package);
                } 
            }

            var notProcessedPackages = this.Packages.Where(x => !processedPackages.Contains(x)).ToList();

            if (notProcessedPackages.Count == 0 || this.InterruptRequested)
            {
                context.Scheduler.UnscheduleJob(context.Trigger.Key);
                this.StopTask();
            }
            else if (notProcessedPackages.Count > 0 && !context.Trigger.GetMayFireAgain())
            {
                var simpleTrigger = (ISimpleTrigger)context.Trigger;
                this.UpdateNotProcessedPackages(notProcessedPackages, simpleTrigger.RepeatCount);
                this.StopTask();
            }
            
            return processedPackages;
        }

        /// <summary>
        /// Сохранить объекты
        /// </summary>
        /// <param name="objects">Список объектов</param>
        public virtual void SaveObjects(List<PersistentObject> objects)
        {
            var objectsDictionary = objects.GroupBy(x => x.GetType()).ToDictionary(group => group.Key, group => group.ToList());

            foreach (var objectList in objectsDictionary.Values)
            {
                TransactionHelper.InsertInManyTransactions(this.Container, objectList);
            }
        }

        /// <summary>
        /// Получить результат экспорта пакета данных
        /// </summary>
        /// <param name="messageGuid">Идентификатор сообщения</param>
        /// <param name="result">Результат экспорта</param>
        /// <returns>Статус обработки запроса</returns>
        protected abstract sbyte GetSatateResult(string messageGuid, out TResponceType result);

        /// <summary>
        /// Обработать результат экспорта пакета данных
        /// </summary>
        /// <param name="responce">Ответ от сервиса</param>
        /// <param name="transportGuidDictByType">Словарь transportGuid-ов для типа</param>
        /// <returns>Результат обработки пакета</returns>
        protected abstract PackageProcessingResult ProcessResult(
            TResponceType responce, 
            Dictionary<Type, Dictionary<string, long>> transportGuidDictByType);

        private void Initialize(IJobExecutionContext context)
        {
            long taskId;

            try
            {
                taskId = (long)this.GetPropertyValue(context, "TaskId");
            }
            catch (Exception exception)
            {
                throw new JobExecutionException("Obtaining task error.", exception, false);
            }

            var taskDomain = this.Container.ResolveDomain<RisTask>();
            var taskPackageDomain = this.Container.ResolveDomain<RisTaskPackage>();

            try
            {
                this.Task = taskDomain.Get(taskId);

                this.Packages = taskPackageDomain.GetAll()
                    .Where(x => x.Task == this.Task && x.State == PackageProcessingState.Waiting)
                    .ToList();
            }
            finally
            {
                this.Container.Release(taskDomain);
                this.Container.Release(taskPackageDomain);
            }
        }

        /// <summary>
        /// Обработать пакет
        /// </summary>
        /// <param name="package">Пакет</param>
        /// <returns>Флаг: true - пакет обработан, false - в противном случае</returns>
        private bool ProcessPackage(RisTaskPackage package)
        {
            TResponceType requestResult;

            sbyte state;

            try
            {
                state = this.GetSatateResult(package.AckMessageGuid, out requestResult);
            }
            catch (Exception exception)
            {
                this.SaveResult(package, new PackageProcessingResult
                {
                    State = PackageProcessingState.GettingStateError,
                    Message = exception.Message
                });

                return true;
            }

            //Статус обработки сообщения в асинхронном обмене (1- получено; 2 - в обработке; 3- обработано)
            if (state == 3)
            {
                try
                {
                    var packageProcessingResult = this.ProcessResult(requestResult, package.Package.GetTransportGuidDictionary());
                    this.SaveResult(package, packageProcessingResult);
                }
                catch (Exception exception)
                {
                    this.SaveResult(package, new PackageProcessingResult
                    {
                        State = PackageProcessingState.ProcessingResultError,
                        Message = exception.Message
                    });
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Сохранить результаты экспорта
        /// </summary>
        /// <param name="package">Пакет</param>
        /// <param name="packageProcessingResult">Результат обработки пакета</param>
        private void SaveResult(RisTaskPackage package, PackageProcessingResult packageProcessingResult)
        {
            this.SaveObjects(packageProcessingResult);

            var log = this.PrepareLog(package, packageProcessingResult);

            var logFileInfo = this.SaveLog(log);

            this.UpdateTaskPackage(package, packageProcessingResult, logFileInfo);

            //Сохранение GisLog оставлено временно
            //После реализации реестра задач выпилить
            this.SaveGisLog(packageProcessingResult, logFileInfo);
        }

        private CsvLogImport PrepareLog(RisTaskPackage package, PackageProcessingResult packageProcessingResult)
        {
            var logHeader = string.Format("{0};{1};{2};{3};{4};{5}\n",
                "Описание объекта",
                "Идентификатор ЖКХ",
                "Идентификатор РИС",
                "Идентификатор ГИС",
                "Результат",
                "Сообщение");
            var result = new CsvLogImport(logHeader);
            result.SetFileName(this.Name);

            result.Write(
                    string.Format(
                        "{0};{1};{2};{3};{4};{5}\n",
                        "Пакет данных",
                        string.Empty,
                        package.Package.Id,
                        string.Empty,
                        packageProcessingResult.State,
                        packageProcessingResult.Message));

            if (packageProcessingResult.State == PackageProcessingState.Success)
            {
                foreach (var objectProcessingResult in packageProcessingResult.Objects)
                {
                    result.Write(
                        string.Format(
                            "{0};{1};{2};{3};{4};{5}\n",
                            objectProcessingResult.Description,
                            objectProcessingResult.GkhId,
                            objectProcessingResult.RisId,
                            objectProcessingResult.GisId,
                            objectProcessingResult.State,
                            objectProcessingResult.Message));
                }

                var objectsCount = packageProcessingResult.ObjectsCount;
                var processedObjectsCount = packageProcessingResult.SuccessProcessedObjectsCount;
                var percent = objectsCount != 0 ? decimal.Round((processedObjectsCount * 100m) / objectsCount, 2) : 0;
                result.Write(string.Format("\n{0};{1}\n", "Общее количество", objectsCount));
                result.Write(string.Format("{0};{1}\n", "Количество выполненных", processedObjectsCount));
                result.Write(string.Format("{0};{1}\n", "Процент выполненных", percent));
            }

            return result;
        }

        private void SaveObjects(PackageProcessingResult packageProcessingResult)
        {
            var objectsToSave = new List<PersistentObject>();

            if (packageProcessingResult.State == PackageProcessingState.Success)
            {
                foreach (var objectProcessingResult in packageProcessingResult.Objects)
                {
                    if (objectProcessingResult.State == ObjectProcessingState.Success)
                    {
                        objectsToSave.AddRange(objectProcessingResult.ObjectsToSave);
                    }
                }

                this.SaveObjects(objectsToSave);
            }
        }

        private FileInfo SaveLog(CsvLogImport log)
        {
            var fileManager = this.Container.Resolve<IFileManager>();

            try
            {
                return fileManager.SaveFile(log.GetFile(), log.FileName);
            }
            finally
            {
                this.Container.Release(fileManager);
            }
        }

        private void UpdateTaskPackage(RisTaskPackage taskPackage, PackageProcessingResult packageProcessingResult, FileInfo logFileInfo)
        {
            var taskPackageDomain = this.Container.ResolveDomain<RisTaskPackage>();

            try
            {
                taskPackage.State = packageProcessingResult.State;
                taskPackage.Message = packageProcessingResult.Message;
                taskPackage.ResultLog = logFileInfo;
                taskPackageDomain.Update(taskPackage);
            }
            finally
            {
                this.Container.Release(taskPackageDomain);
            }
        }

        private void SaveGisLog(PackageProcessingResult packageProcessingResult, FileInfo logFileInfo)
        {
            var gisLogDomain = this.Container.ResolveDomain<GisLog>();

            try
            {
                var objectsCount = packageProcessingResult.ObjectsCount;
                var processedObjectsCount = packageProcessingResult.SuccessProcessedObjectsCount;
                var percent = objectsCount != 0 ? decimal.Round((processedObjectsCount * 100m) / objectsCount, 2) : 0;

                var gisLog = new GisLog
                {
                    ServiceLink = this.ServiceProvider.ServiceAddress,
                    UserName = this.ExecutionOwner != null ? this.ExecutionOwner.Name : string.Empty,
                    MethodName = this.Name,
                    DateStart = DateTime.Now,
                    CountObjects = objectsCount,
                    ProcessedObjects = processedObjectsCount,
                    ProcessedPercent = percent,
                    DateEnd = DateTime.Now,
                    FileLog = logFileInfo
                };

                gisLogDomain.Save(gisLog);
            }
            finally
            {
                this.Container.Release(gisLogDomain);
            }
        }

        private void UpdateNotProcessedPackages(List<RisTaskPackage> packages, int repeatCount)
        {
            foreach (var package in packages)
            {
                this.SaveResult(package, new PackageProcessingResult
                {
                    State = PackageProcessingState.TimeoutError,
                    Message = string.Format("Превышено максимальное количество  = {0} попыток получения результата асинхронного запроса", repeatCount)
                });
            }
        }

        private void StopTask()
        {
            var taskDomain = this.Container.ResolveDomain<RisTask>();

            try
            {
                this.Task.EndTime = DateTime.Now;
                taskDomain.Update(this.Task);
            }
            finally
            {
                this.Container.Release(taskDomain);
            }
        }
    }
}
