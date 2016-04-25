namespace Bars.Gkh.Ris.ViewModel.Task
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Bars.B4;
    using Bars.B4.DataAccess;
    using Bars.B4.Modules.Security;
    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Entities;
    using Bars.Gkh.Ris.Enums;

    using Castle.Windsor;

    using global::Quartz;

    /// <summary>
    /// View - модель дерева задач
    /// </summary>
    public class TaskTreeViewModel: ITreeViewModel
    {
        /// <summary>
        /// IoC контейнер
        /// </summary>
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Получить дочерние узлы
        /// </summary>
        /// <param name="baseParams">Параметры, в т.ч. параметры текущего узла</param>
        /// <returns>Дочерние узлы</returns>
        public IDataResult List(BaseParams baseParams)
        {
            var nodeType = baseParams.Params.GetAs<string>("nodeType");
            var nodeId = baseParams.Params.GetAs<long>("nodeId");

            List<TaskTreeNode> data;

            if (nodeType == "root")
            {
                data = this.GetTasks(baseParams);
            }
            else if (nodeType == "Task")
            {
                data = this.GetPackages(nodeId);
            }
            else
            {
                data = new List<TaskTreeNode>();
            }

            return new ListDataResult(data, data.Count);
        }

        private List<TaskTreeNode> GetTasks(BaseParams baseParams)
        {
            DateTime? dateTimeFrom = null;
            DateTime? dateTimeTo = null;

            if (baseParams.Params.ContainsKey("dateTimeFrom"))
            {
                dateTimeFrom = baseParams.Params.GetAs<DateTime>("dateTimeFrom");
            }

            if (baseParams.Params.ContainsKey("dateTimeTo"))
            {
                dateTimeTo = baseParams.Params.GetAs<DateTime>("dateTimeTo");
            }

            var taskDomain = this.Container.ResolveDomain<RisTask>();
            var taskPackageDomain = this.Container.ResolveDomain<RisTaskPackage>();
            var scheduler = this.Container.Resolve<IScheduler>("TaskScheduler");
            var userIdentity = this.Container.Resolve<IUserIdentity>();
            var userDomain = this.Container.ResolveDomain<User>();

            try
            {
                var currentUser = userDomain.Get(userIdentity.UserId);
                var result = new List<TaskTreeNode>();

                var tasks = taskDomain
                    .GetAll()
                    //если текущий пользователь не админ, то показываем только его задачи
                    .WhereIf(currentUser.Roles.All(y => y.Role.Name != "Администратор"), x => x.UserName == currentUser.Login)
                    .WhereIf(dateTimeFrom.HasValue, x => x.StartTime >= dateTimeFrom.Value)
                    .WhereIf(dateTimeTo.HasValue, x => x.StartTime <= dateTimeTo.Value)
                    .ToList();

                foreach (var task in tasks)
                {
                    var taskPackages = taskPackageDomain.GetAll().Where(x => x.Task == task).ToList();

                    var taskState = this.GetTaskState(task, taskPackages, scheduler);

                    var taskViewModel = new TaskTreeNode(task);
                    taskViewModel.State = taskState.GetDisplayName();
                    taskViewModel.Leaf = taskPackages.Count == 0;

                    result.Add(taskViewModel);
                }

                return result;
            }
            finally
            {
                this.Container.Release(taskDomain);
                this.Container.Release(taskPackageDomain);
                this.Container.Release(scheduler);
                this.Container.Release(userIdentity);
                this.Container.Release(userDomain);
            }    
        }

        private TaskState GetTaskState(RisTask task, List<RisTaskPackage> taskPackages, IScheduler scheduler)
        {
            var triggerState = scheduler.GetTriggerState(new TriggerKey(task.QuartzTriggerKey));

            if (triggerState == TriggerState.Normal || triggerState == TriggerState.Blocked)
            {
                return TaskState.Processing;
            }

            if (triggerState == TriggerState.Error)
            {
                return TaskState.Error;
            }

            if (triggerState == TriggerState.Paused)
            {
                return TaskState.Paused;
            }

            if (triggerState == TriggerState.Complete)
            {
                if (taskPackages.Any(x => x.State != PackageProcessingState.Success))
                {
                    return TaskState.CompleteWithErrors;
                }

                return TaskState.CompleteSuccess;
            }

            if (triggerState == TriggerState.None)
            {
                if (taskPackages.Any(x => x.State == PackageProcessingState.Waiting))
                {
                    return TaskState.Waiting;
                }

                if (taskPackages.Any(x => x.State != PackageProcessingState.Success))
                {
                    return TaskState.CompleteWithErrors;
                }

                return TaskState.CompleteSuccess;
            }

            return TaskState.Waiting;
        }    

        private List<TaskTreeNode> GetPackages(long taskId)
        {
            var taskPackageDomain = this.Container.ResolveDomain<RisTaskPackage>();

            try
            {               
                return taskPackageDomain.GetAll().Where(x => x.Task.Id == taskId).Select(x => new TaskTreeNode(x)).ToList();
            }
            finally
            {
                this.Container.Release(taskPackageDomain);
            }
        }
    }
}
