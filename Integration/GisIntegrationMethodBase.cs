namespace Bars.Gkh.Ris.Integration
{
    using B4;
    using B4.DataAccess;
    using B4.Modules.FileStorage;
    using B4.Utils;
    using Castle.Windsor;
    using Entities;
    using Entities.GisIntegration;
    using Import;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using GisServiceProvider;
    /// <summary>
    /// Базовый класс импорта объектов ГИС
    /// </summary>
    /// <typeparam name="T">Тип объекта ГИС для сохранения</typeparam>
    /// <typeparam name="K">Тип объекта request</typeparam>
    /// <typeparam name="M">Тип SOAP клиента</typeparam>
    public abstract class GisIntegrationMethodBase<T, K, M> : IGisIntegrationMethod where T : BaseRisEntity
    {
        private int protocolLineNum;
        protected const string blockToSignId = "block-to-sign";

        /// <summary>
        /// Конструктор
        /// </summary>
        protected GisIntegrationMethodBase()
        {
            this.InitLog();
        }

        /// <summary>
        /// Общее количество импортируемых объектов
        /// </summary>
        protected int CountObjects { get; set; }

        /// <summary>
        /// Лог импорта
        /// </summary>
        protected CsvLogImport LogImport { get; set; }

        /// <summary>
        /// IoC контейнер
        /// </summary>
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Количество объектов для сохранения. Переопределять как количество объектов для сохранения
        /// </summary>
        protected abstract int ProcessedObjects { get; }

        /// <summary>
        /// Код импорта.
        /// </summary>
        public abstract string Code { get; }

        /// <summary>
        /// Наименование импорта в списке
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Порядок импорта в списке
        /// </summary>
        public abstract int Order { get; }

        /// <summary>
        /// Описание метода
        /// </summary>
        public virtual string Description
        {
            get
            {
                return this.Name;
            }
        }

        /// <summary>
        /// Максимаьлное количество объектов ГИС в одном файле импорта
        /// </summary>
        protected abstract int Portion { get; }

        /// <summary>
        /// Сертификат для подписи xml
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Лог импорта.
        /// </summary>
        public GisLog GisLog { get; set; }

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

        public IGisServiceProvider<M> ServiceProvider { get; set; }

        /// <summary>
        /// Список объектов ГИС, по которым производится импорт.
        /// </summary>
        protected abstract IList<T> MainList { get; set; }

        /// <summary>
        /// Наименование метода импорта для сохранения логов.
        /// </summary>
        protected string MethodName
        {
            get
            {
                return this.Name;
            }
        }


        /// <summary>
        /// Идентификатор поставщика
        /// </summary>
        public RisContragent Contragent
        {
            get; set;
        }

        /// <summary>
        /// Идентификатор поставщика
        /// </summary>
        public string SenderId
        {
            get { return this.Contragent != null ? this.Contragent.SenderId : string.Empty; }
        }

        /// <summary>
        /// Выполнить запрос импорта объектов ГИС.
        /// </summary>
        /// <returns>Результат выполнения запроса</returns>
        public IDataResult Execute()
        {
            this.Prepare();
            this.ClearMainList();

            if (this.MainList.Count == 0)
            {
                this.SaveLog();
                return new BaseDataResult();
            }

            foreach (var iterationList in this.GetPortions())
            {
                K request = this.GetRequestObject(iterationList);
                this.HandleRequestResult(request);
            }

            this.ExecuteStateResult();
            this.SaveObjects();
            this.SaveLog();

            return new BaseDataResult();
        }

        /// <summary>
        /// Подготовка кэша данных для PrepareRequest. Заполнить MainList.
        /// </summary>
        protected virtual void Prepare()
        {
            this.MainList = new List<T>();
        }

        /// <summary>
        /// Чистим список объектов для импорта от невалидных
        /// </summary>
        protected void ClearMainList()
        {
            List<T> itemsToRemove = new List<T>();

            foreach (var item in this.MainList)
            {
                var checkingResult = this.CheckMainListItem(item);

                if (checkingResult.Messages.Length > 0)
                {
                    itemsToRemove.Add(item);
                    this.AddLineToLog(string.Format("Объект типа {0}", typeof(T).Name), item.Id, "Не загружен", checkingResult.Messages);
                }
            }

            foreach (var itemToRemove in itemsToRemove)
            {
                this.MainList.Remove(itemToRemove);
            }
        }

        /// <summary>
        /// Проверка объекта
        /// </summary>
        /// <param name="item">Объект</param>
        /// <returns>Результат проверки</returns>
        protected virtual CheckingResult CheckMainListItem(T item)
        {
            return new CheckingResult { Result = true };
        }

        /// <summary>
        /// Получает список порций объектов ГИС для формирования объектов для запроса.
        /// </summary>
        /// <returns>Список порций объектов ГИС</returns>
        protected virtual List<IEnumerable<T>> GetPortions()
        {
            List<IEnumerable<T>> result = new List<IEnumerable<T>>();

            var startIndex = 0;
            do
            {
                result.Add(this.MainList.Skip(startIndex).Take(this.Portion));
                startIndex += this.Portion;
            }
            while (startIndex < this.MainList.Count);

            return result;
        }

        /// <summary>
        /// Получить объект для запроса.
        /// </summary>
        /// <param name="listForImport">Список объектов</param>
        /// <returns>Объект для запроса</returns>
        protected abstract K GetRequestObject(IEnumerable<T> listForImport);

        /// <summary>
        /// Выполнить запрос и обработать результат.
        /// </summary>
        /// <param name="request">Объект для запроса</param>
        protected abstract void HandleRequestResult(K request);

        protected virtual void ExecuteStateResult()
        {

        }

        /// <summary>
        /// Сохранение объектов ГИС после импорта.
        /// </summary>
        protected abstract void SaveObjects();

        private void SaveLog()
        {
            var gisLogDomain = this.Container.ResolveRepository<GisLog>();
            var fileManager = this.Container.Resolve<IFileManager>();

            try
            {
                var gisLog = new GisLog
                {
                    ServiceLink = this.ServiceProvider.ServiceAddress,
                    MethodName = this.MethodName,
                    DateStart = DateTime.Now,
                    CountObjects = 0
                };

                var percent = this.CountObjects != 0 ? decimal.Round((this.ProcessedObjects * 100m) / this.CountObjects, 2) : 0;

                this.LogImport.Write(string.Format("\n{0};{1}\n", "Общее количество", this.CountObjects.ToStr()));
                this.LogImport.Write(string.Format("{0};{1}\n", "Количество выполненных", this.ProcessedObjects.ToStr()));
                this.LogImport.Write(string.Format("{0};{1}\n", "Процент выполненных", percent.ToStr()));

                FileInfo logFileInfo = fileManager.SaveFile(this.LogImport.GetFile(), this.LogImport.FileName);

                gisLog.DateEnd = DateTime.Now;
                gisLog.CountObjects = this.CountObjects;
                gisLog.ProcessedObjects = this.ProcessedObjects;
                gisLog.ProcessedPercent = percent;
                gisLog.FileLog = logFileInfo;

                gisLogDomain.Save(gisLog);
            }
            finally
            {
                this.Container.Release(gisLogDomain);
                this.Container.Release(fileManager);
            }
        }

        /// <summary>
        /// Добавить запись в лог импорта.
        /// </summary>
        /// <param name="objectName">Наименование объекта</param>
        /// <param name="id">ID объекта</param>
        /// <param name="state">Результат</param>
        /// <param name="line">Комментарий</param>
        protected void AddLineToLog(string objectName, long id, string state, StringBuilder line)
        {
            if (line.Length > 0)
            {
                this.AddLineToLog(objectName, id, state, line.ToString());
            }
        }

        /// <summary>
        /// Добавить запись в лог импорта.
        /// </summary>
        /// <param name="objectName">Наименование объекта</param>
        /// <param name="id">ID объекта</param>
        /// <param name="state">Результат</param>
        /// <param name="line">Комментарий</param>
        protected void AddLineToLog(string objectName, long id, string state, string line)
        {
            if (line.Length > 0)
            {
                this.protocolLineNum++;
                line = line.TrimEnd(' ');
                this.LogImport.Write(string.Format("{0};{1};{2};{3};{4}\n",
                    this.protocolLineNum, objectName, id, state, line));
            }
        }



        private void InitLog()
        {
            var logHeader = string.Format("{0};{1};{2};{3};{4}\n",
                   "№ п/п",
                   "Передаваемый объект",
                   "Идентификатор объекта",
                   "Результат",
                   "Примечание");
            this.LogImport = new CsvLogImport(logHeader);
            this.LogImport.SetFileName(this.MethodName);
        }
    }

    /// <summary>
    /// Результат проверки
    /// </summary>
    public class CheckingResult
    {
        /// <summary>
        /// Проверка пройдена (true - положительный результат, false - отрицательный)
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Сообщения, сформированные при проверке
        /// </summary>
        public StringBuilder Messages { get; set; }
    }
}