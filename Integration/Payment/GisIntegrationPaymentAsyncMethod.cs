namespace Bars.Gkh.Ris.Integration.Payment
{
    using System;
    using System.Collections.Generic;
    using B4.Utils;
    using Entities;
    using global::Quartz;
    using PaymentAsync;

    public abstract class GisIntegrationPaymentAsyncMethod<T, K> : GisIntegrationMethodBase<T, K, PaymentPortsTypeAsyncClient> where T : BaseRisEntity
    {
        protected List<AckRequest> AckRequests;
        
        /// <summary>
        /// Заголовок запроса
        /// </summary>
        protected RequestHeader RequestHeader
        {
            get
            {
                return new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToStr(),
                    SenderID = this.SenderId
                };
            }
        }

        /// <summary>
        /// Выполнить запрос и обработать результат.
        /// </summary>
        /// <param name="request">Объект для запроса</param>
        protected override void HandleRequestResult(K request)
        {
            AckRequest ackRequest;
            try
            {
                ackRequest = this.GetRequestAckRequest(request);
            }
            catch (Exception exception)
            {
                this.AddLineToLog(typeof(T).ToString(), 0, string.Empty, exception.Message);
                return;
            }

            if (ackRequest != null)
            {
                this.AckRequests.Add(ackRequest);
            }
        }

        /// <summary>
        /// Получить ответ от сервиса.
        /// </summary>
        /// <param name="request">Запрос</param>
        /// <returns>Результат выполнения запроса</returns>
        protected abstract AckRequest GetRequestAckRequest(K request);

        /// <summary>
        /// Получить и обработать ImportResult от getState
        /// </summary>
        protected override void ExecuteStateResult()
        {
            var taskType = this.GetTaskType();

            if (taskType != null)
            {
                this.ScheduleExportTask(taskType);
                return;
            }

            foreach (var ackRequest in this.AckRequests)
            {
                var request = new getStateRequest
                {
                    MessageGUID = ackRequest.Ack.MessageGUID
                };

                try
                {
                    getStateResult stateResult = null;

                    var soapClient = this.ServiceProvider.GetSoapClient();

                    if (soapClient != null)
                    {
                        soapClient.getState(this.RequestHeader, request, out stateResult);
                    }

                    if (stateResult == null)
                    {
                        this.AddLineToLog(string.Empty, 0, string.Empty, 
                            string.Format("Не удалось получить ответ для MessageGuid {0}", ackRequest.Ack.MessageGUID));
                    }

                    this.ParseStateResult(stateResult);
                }
                catch (Exception exception)
                {
                    this.AddLineToLog(string.Empty, 0, string.Empty, exception.Message);
                    return;
                }
            }
        }

        /// <summary>
        /// Распарсить полученный результат и записать в сущности 
        /// или лог в случае ошибки
        /// </summary>
        /// <param name="stateResult">Результат выполнения getState</param>
        protected abstract void ParseStateResult(getStateResult stateResult);

        /// <summary>
        /// Получить входные данные для выполнения задачи получения результатов экспорта
        /// </summary>
        /// <returns>Входные данных</returns>
        protected virtual JobDataMap GetJobDataMap()
        {
            return new JobDataMap();
        }

        /// <summary>
        /// Получить тип задачи для получения результатов экспорта
        /// </summary>
        /// <returns>Тип задачи</returns>
        protected virtual Type GetTaskType()
        {
            return null;
        }

        private void ScheduleExportTask(Type taskType)
        {
            var scheduler = this.Container.Resolve<IScheduler>("TaskScheduler");

            try
            {
                var triggerIdentity = Guid.NewGuid().ToStr();
                var jobIdentity = string.Format("{0}_{1}",taskType.Name, triggerIdentity);
                var jobDataMap = this.GetJobDataMap();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(triggerIdentity)
                    .WithDescription(this.Name)
                    .ForJob(jobIdentity)
                    .WithSimpleSchedule(x => x
                     .WithIntervalInSeconds(10)
                     .WithRepeatCount(10)
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
    }
}