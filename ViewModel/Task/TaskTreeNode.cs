namespace Bars.Gkh.Ris.ViewModel.Task
{
    using System;
    using System.Globalization;

    using Bars.B4.Modules.FileStorage;
    using Bars.B4.Utils;
    using Bars.Gkh.Ris.Entities;

    /// <summary>
    /// Узел дерева задач
    /// </summary>
    public class TaskTreeNode: BaseNode
    {
        /// <summary>
        /// Конструктор узла дерева задач
        /// </summary>
        public TaskTreeNode()
        {
        }

        /// <summary>
        /// Конструктор узла дерева задач
        /// </summary>
        /// <param name="task">Экземпляр сущности Задача</param>
        public TaskTreeNode(RisTask task)
        {
            this.Id = task.Id;
            this.Type = "Task";
            this.Name = task.Description;
            this.StartTime = task.StartTime.ToString("dd.MM.yyyy HH:mm");
            this.EndTime = task.EndTime == DateTime.MinValue ? string.Empty : task.EndTime.ToString("dd.MM.yyyy HH:mm");
            this.Author = task.UserName;
            this.IconCls = "icon_task";
        }

        /// <summary>
        /// Конструктор узла дерева задач
        /// </summary>
        /// <param name="taskPackage">Экземпляр сущности, связывающей задачу и пакет</param>
        public TaskTreeNode(RisTaskPackage taskPackage)
        {
            this.Id = taskPackage.Package.Id;
            this.Type = "Package";
            this.Name = taskPackage.Package.Name;
            this.Author = taskPackage.Package.UserName;
            this.State = taskPackage.State.GetDisplayName();
            this.Message = taskPackage.Message;
            this.ResultLog = taskPackage.ResultLog;
            this.Leaf = true;
            this.IconCls = "icon_package";
        }

        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Тип узла, сущности ему соответствующей
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Автор
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Время начала выполнения
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Время окончания выполнения
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// Статус выполнения
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Файл лога с результатом обработки пакета в рамках задачи
        /// </summary>
        public FileInfo ResultLog { get; set; }
    }
}
