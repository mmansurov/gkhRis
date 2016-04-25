namespace Bars.Gkh.Ris.Integration.OrgRegistryCommon.Methods
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Timers;

    using B4;
    using B4.DataAccess;
    using B4.IoC.Lifestyles.SessionLifestyle;
    using B4.Modules.FileStorage;
    using B4.Utils;

    using Authentification;
    using Castle.Windsor;
    using Config;
    using ConfigSections;
    using Domain;
    using Entities;
    using Entities.GisIntegration;
    using Gkh.Entities;
    using Gkh.Enums;
    using Import;
    using Modules.Gkh1468.Entities.PublicServiceOrg;
    using Modules.RegOperator.Entities.RegOperator;
    using OrgRegistryCommonAsync;
    using Signature;

    /// <summary>
    /// Метод для получения orgRootEntityGuid и orgVersionGuid
    /// </summary>
    public class ExportOrgRegistryMethod : IGisIntegrationMethod
    {
        /// <summary>
        /// Размер блока данных в одном запросе
        /// </summary>
        private const int Portion = 100;

        /// <summary>
        /// Интервал опроса асинхронного сервиса для получения результатов в миллисекундах
        /// </summary>
        private const int Interval = 60000 * 7;

        /// <summary>
        /// Максимальное количество попыток получения результатов
        /// </summary>
        private const int GetResultAttemptsMaxCount = 30;

        /// <summary>
        /// Soap клиент
        /// </summary>
        private RegOrgPortsTypeAsyncClient soapClient;

        /// <summary>
        /// Таймер для получения результатов выполнения асинхронных запросов
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Количество попыток получения результатов
        /// </summary>
        private int getResultAttemptsCount;

        /// <summary>
        /// Контактные данне по контрагентам
        /// </summary>
        private Dictionary<long, string> contactsByContragentId;

        /// <summary>
        /// Все контрагенты в ЖКХ с ролями: 
        /// Управляющие организации, Поставщики коммунальных услуг, Поставщики жилищных услуг, Органы местного самоуправления, Региональные операторы, Поставщики ресурсов
        /// </summary>
        private readonly List<ContragentProxy> allGkhContragents;

        /// <summary>
        /// Контрагенты  для загрузки из ГИС
        /// </summary>
        private Dictionary<string, ContragentProxy> gkhContragentsToLoad;

        /// <summary>
        /// Все контрагенты, сохраненные в таблице Ris_
        /// </summary>
        private readonly List<RisContragent> allRisContragents;

        /// <summary>
        /// Контрагенты Ris для сохоранения
        /// </summary>
        private readonly List<RisContragent> contragentsToSave;

        /// <summary>
        /// Лог импорта
        /// </summary>
        private CsvLogImport logImport;

        /// <summary>
        /// Общее количество импортируемых объектов
        /// </summary>
        private int countObjects;

        /// <summary>
        /// Количество объектов для сохранения. Переопределять как количество объектов для сохранения
        /// </summary>
        private int processedObjects;

        /// <summary>
        /// Запросы на экспорт зарегистрированных контрагентов и результаты выполнения
        /// </summary>
        private readonly Dictionary<AckRequest, getStateResult> contragentAckRequestsStateResults;

        /// <summary>
        /// Конструктор метода экспорта
        /// </summary>
        public ExportOrgRegistryMethod()
        {
            this.allGkhContragents = new List<ContragentProxy>();
            this.contragentsToSave = new List<RisContragent>();
            this.contragentAckRequestsStateResults = new Dictionary<AckRequest, getStateResult>();
            this.allRisContragents = new List<RisContragent>();

            this.InitLog();
            this.InitTimer();
        }

        /// <summary>
        /// Ioc контейнер
        /// </summary>
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Клиент для работы с сервисом
        /// </summary>
        public RegOrgPortsTypeAsyncClient SoapClient => this.soapClient ?? (this.soapClient = this.CreateOrgRegistryClient());

        /// <summary>
        /// Код метода
        /// </summary>
        public string Code => "exportOrgRegistry";

        /// <summary>
        /// Наименование метода
        /// </summary>
        public string Name => "Импорт данных организации";

        /// <summary>
        /// Порядок импорта в списке.
        /// </summary>
        public int Order => 1;

        /// <summary>
        /// Описание метода
        /// </summary>
        public string Description => "Получение orgRootEntityGuid, orgVersionGuid";

        /// <summary>
        /// Адрес сервиса
        /// </summary>
        public string ServiceAddress => "http://127.0.0.1:8080/ext-bus-org-registry-common-service/services/OrgRegistryCommonAsync";

        /// <summary>
        /// Необходимо подписывать данные
        /// </summary>
        public bool NeedSign => true;

        /// <summary>
        /// Сертификат для подписи xml
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Объект для логирования
        /// </summary>
        public GisLog GisLog { get; set; }

        /// <summary>
        /// Идентификатор поставщика
        /// </summary>
        public RisContragent Contragent { get; set; }

        /// <summary>
        /// Идентификатор поставщика
        /// </summary>
        public string SenderId => this.Contragent != null ? this.Contragent.SenderId : string.Empty;

        /// <summary>
        /// Запуск выполнения метода - отправка асинхронных запросов
        /// </summary>
        /// <returns>Пустой DataResult для клиента</returns>
        public IDataResult Execute()
        {
            this.Prepare();

            this.WriteEmptyOgrnToLog();

            this.countObjects = this.gkhContragentsToLoad.Count;/* + this.gkhTenantsOfPublicPlacesToLoad.Count;*/

            if (this.countObjects == 0)
            {
                this.SaveLog();

                return new BaseDataResult();
            }

            if (this.gkhContragentsToLoad.Count > 0)
            {
                var ackRequestsContragents = this.RequestOrgRegistry(this.gkhContragentsToLoad.Keys.ToList());

                foreach (var ackRequest in ackRequestsContragents)
                {
                    this.contragentAckRequestsStateResults.Add(ackRequest, null);
                }
            }

            this.timer.Start();

            return new BaseDataResult();
        }

        /// <summary>
        /// Сохранить результаты выполнения метода и лог
        /// </summary>
        private void SaveResult()
        {
            TransactionHelper.InsertInManyTransactions(this.Container, this.contragentsToSave, 1000, true, true);

            this.SaveLog();
        }

        /// <summary>
        /// Записать в лог контрагентов с пустыми ОГРН
        /// </summary>
        private void WriteEmptyOgrnToLog()
        {
            foreach (var contragent in this.allGkhContragents.Where(x => x.Ogrn == null || x.Ogrn.Length != 13))
            {
                this.logImport.Write(string.Format("{0};{1};{2};{3};{4}\n",
                       contragent.Name.Replace(";", string.Empty).Replace(",", string.Empty),
                       this.contactsByContragentId.Get(contragent.Id),
                       "",
                       "Ошибка",
                       "Не заполнен ОГРН"));
            }
        }

        /// <summary>
        /// Создать soap клиент
        /// </summary>
        /// <param name="endpointAddress">Адрес конечной точки</param>
        /// <param name="noSign">Для getStateResult signature не нужна</param>
        /// <returns>Soap клиент</returns>
        private RegOrgPortsTypeAsyncClient CreateOrgRegistryClient(string endpointAddress = null, bool noSign = false)
        {
            var configProvider = this.Container.Resolve<IGkhConfigProvider>();
            var risSettingsDomain = this.Container.ResolveDomain<RisSettings>();
            var gisIntegrationConfig = configProvider.Get<GisIntegrationConfig>();

            try
            {
                var serviceAddress = endpointAddress.IsNotEmpty()
                    ? endpointAddress
                    : this.ServiceAddress;

                var isHttps = serviceAddress.Split(":")[0] == "https";

                var binding = new BasicHttpBinding
                {
                    Security =
                    {
                        Mode = isHttps
                            ? BasicHttpSecurityMode.Transport
                            : BasicHttpSecurityMode.TransportCredentialOnly,
                        Transport = new HttpTransportSecurity
                        {
                            ClientCredentialType = isHttps
                                ? HttpClientCredentialType.Certificate
                                : HttpClientCredentialType.Basic
                        }
                    },
                    MaxReceivedMessageSize = 65536 * 7,
                    MaxBufferSize = 65536 * 7
                };

                var remoteAddress = new EndpointAddress(serviceAddress);

                var client = new RegOrgPortsTypeAsyncClient(binding, remoteAddress);

                var defaultCredentials = client.Endpoint.Behaviors.Find<ClientCredentials>();
                client.Endpoint.Behaviors.Remove(defaultCredentials);

                // test
                // SDldfls4lz5@!82d
                if (gisIntegrationConfig.UseLoginCredentials)
                {
                    var loginCredentials = new ClientCredentials();
                    loginCredentials.UserName.UserName = gisIntegrationConfig.Login;
                    loginCredentials.UserName.Password = gisIntegrationConfig.Password;
                    client.Endpoint.Behaviors.Add(loginCredentials);
                }

                if (gisIntegrationConfig.SingXml && !noSign)
                {
                    var thumbprint = "d6dee60567b84287960bd9615fa4c45b4c028270";
                    var storeName = StoreName.My;
                    var storeLocation = StoreLocation.LocalMachine;
                    var store = new X509Store(storeName, storeLocation);
                    store.Open(OpenFlags.ReadOnly);
                    if (store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true).Count == 0)
                    {
                        throw new Exception("Не удалось получить сертификат");
                    }
                    var cert = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true)[0];

                    client.Endpoint.Behaviors.Add(new GisIntegrationClientMessageInspector(cert));
                }

                return client;
            }
            finally
            {
                this.Container.Release(risSettingsDomain);
            }
        }

        /// <summary>
        /// Подготовка кэша данных 
        /// </summary>
        private void Prepare()
        {
            var managingOrganizationRepo = this.Container.ResolveRepository<ManagingOrganization>();
            var supplyResourceOrgRepo = this.Container.ResolveRepository<SupplyResourceOrg>();
            var serviceOrganizationRepo = this.Container.ResolveRepository<ServiceOrganization>();
            var localGovernmentRepo = this.Container.ResolveRepository<LocalGovernment>();
            var regOperatorRepo = this.Container.ResolveRepository<RegOperator>();
            var publicServiceOrgRepo = this.Container.ResolveRepository<PublicServiceOrg>();
            var contactRepo = this.Container.ResolveRepository<ContragentContact>();
            var risContragentDomain = this.Container.ResolveDomain<RisContragent>();
            var userManager = this.Container.Resolve<IGkhUserManager>();

            try
            {
                this.logImport.UserName = userManager.GetActiveOperator() != null && userManager.GetActiveOperator().User != null
                    ? userManager.GetActiveOperator().User.Name
                    : string.Empty;

                managingOrganizationRepo.GetAll().Select(x =>
                            new ContragentProxy
                            {
                                Id = x.Contragent.Id,
                                Name = x.Contragent.Name,
                                Ogrn = x.Contragent.Ogrn,
                                ContragentState = x.Contragent.ContragentState,
                                FactAddress = x.Contragent.FactAddress,
                                JuridicalAddress = x.Contragent.JuridicalAddress,
                                OrganizationFormCode =
                                    x.Contragent.OrganizationForm != null
                                        ? x.Contragent.OrganizationForm.Code
                                        : string.Empty
                            })
                    .ToList().AddTo(this.allGkhContragents);

                supplyResourceOrgRepo.GetAll().Select(x =>
                            new ContragentProxy
                            {
                                Id = x.Contragent.Id,
                                Name = x.Contragent.Name,
                                Ogrn = x.Contragent.Ogrn,
                                ContragentState = x.Contragent.ContragentState,
                                FactAddress = x.Contragent.FactAddress,
                                JuridicalAddress = x.Contragent.JuridicalAddress,
                                OrganizationFormCode =
                                    x.Contragent.OrganizationForm != null
                                        ? x.Contragent.OrganizationForm.Code
                                        : string.Empty
                            })
                    .ToList().AddTo(this.allGkhContragents);


                serviceOrganizationRepo.GetAll().Select(x =>
                            new ContragentProxy
                            {
                                Id = x.Contragent.Id,
                                Name = x.Contragent.Name,
                                Ogrn = x.Contragent.Ogrn,
                                ContragentState = x.Contragent.ContragentState,
                                FactAddress = x.Contragent.FactAddress,
                                JuridicalAddress = x.Contragent.JuridicalAddress,
                                OrganizationFormCode =
                                    x.Contragent.OrganizationForm != null
                                        ? x.Contragent.OrganizationForm.Code
                                        : string.Empty
                            })
                    .ToList().AddTo(this.allGkhContragents);


                localGovernmentRepo.GetAll().Select(x =>
                            new ContragentProxy
                            {
                                Id = x.Contragent.Id,
                                Name = x.Contragent.Name,
                                Ogrn = x.Contragent.Ogrn,
                                ContragentState = x.Contragent.ContragentState,
                                FactAddress = x.Contragent.FactAddress,
                                JuridicalAddress = x.Contragent.JuridicalAddress,
                                OrganizationFormCode =
                                    x.Contragent.OrganizationForm != null
                                        ? x.Contragent.OrganizationForm.Code
                                        : string.Empty
                            })
                    .ToList().AddTo(this.allGkhContragents);

                regOperatorRepo.GetAll().Select(x =>
                            new ContragentProxy
                            {
                                Id = x.Contragent.Id,
                                Name = x.Contragent.Name,
                                Ogrn = x.Contragent.Ogrn,
                                ContragentState = x.Contragent.ContragentState,
                                FactAddress = x.Contragent.FactAddress,
                                JuridicalAddress = x.Contragent.JuridicalAddress,
                                OrganizationFormCode =
                                    x.Contragent.OrganizationForm != null
                                        ? x.Contragent.OrganizationForm.Code
                                        : string.Empty
                            })
                    .ToList().AddTo(this.allGkhContragents);

                publicServiceOrgRepo.GetAll().Select(x =>
                            new ContragentProxy
                            {
                                Id = x.Contragent.Id,
                                Name = x.Contragent.Name,
                                Ogrn = x.Contragent.Ogrn,
                                ContragentState = x.Contragent.ContragentState,
                                FactAddress = x.Contragent.FactAddress,
                                JuridicalAddress = x.Contragent.JuridicalAddress,
                                OrganizationFormCode =
                                    x.Contragent.OrganizationForm != null
                                        ? x.Contragent.OrganizationForm.Code
                                        : string.Empty
                            })
                    .ToList().AddTo(this.allGkhContragents);

                this.gkhContragentsToLoad = this.allGkhContragents
                    .Where(x => x.Ogrn != null && x.Ogrn.Length == 13)
                    .Where(x => x.ContragentState == ContragentState.Active)
                    .GroupBy(x => x.Ogrn)
                    .ToDictionary(x => x.Key, x => x.First());

                this.contactsByContragentId =
                    contactRepo.GetAll()
                        .Where(x => x.Contragent != null)
                        .Select(x => new { x.Contragent.Id, x.FullName, x.Phone })
                        .ToList()
                        .GroupBy(x => x.Id)
                        .ToDictionary(
                            x => x.Key,
                            x =>
                                new string(x.SelectMany(y => y.FullName + " (" + y.Phone + "), ").ToArray())
                                .Replace(" ()", "").TrimEnd(' ').TrimEnd(','));

                risContragentDomain.GetAll().ToList().AddTo(this.allRisContragents);
            }
            finally
            {
                this.Container.Release(managingOrganizationRepo);
                this.Container.Release(supplyResourceOrgRepo);
                this.Container.Release(serviceOrganizationRepo);
                this.Container.Release(localGovernmentRepo);
                this.Container.Release(regOperatorRepo);
                this.Container.Release(publicServiceOrgRepo);
                this.Container.Release(contactRepo);
                this.Container.Release(risContragentDomain);
                this.Container.Release(userManager);
            }
        }

        /// <summary>
        /// Создать критерии поиска
        /// </summary>
        /// <param name="iterationKeys">Значения ОГРН</param>
        /// <returns>Список объектов критериев</returns>
        private List<exportOrgRegistryRequestSearchCriteria> CreateSearchCriterias(IEnumerable<string> iterationKeys)
        {
            var result = new List<exportOrgRegistryRequestSearchCriteria>();

            foreach (var iterationKey in iterationKeys)
            {
                var searchCriteria = new exportOrgRegistryRequestSearchCriteria
                {
                    isRegistered = true,
                    Items = new[] { iterationKey },
                    ItemsElementName = new[] { ItemsChoiceType3.OGRN }
                };

                result.Add(searchCriteria);
            }

            return result;
        }

        /// <summary>
        /// Обработать результат выполнения для контрагентов
        /// </summary>
        /// <param name="requestResult">Результат выполнения запроса</param>
        /// <returns>Список ОГРН обработанных контрагентов</returns>
        private List<string> HandleRequestResultForContragent(getStateResult requestResult)
        {
            var result = new List<string>();

            var requestResultList = requestResult.Items.ToList();

            var risContragentExists = this.allRisContragents.GroupBy(x => x.GkhId)
                .ToDictionary(x => x.Key, x => x.First());

            foreach (var reqResult in requestResultList)
            {
                var exportOrgRegistryResult = reqResult as exportOrgRegistryResultType;

                if (exportOrgRegistryResult == null)
                {
                    var errorMessage = reqResult as ErrorMessageType;

                    if (errorMessage != null)
                    {
                        this.logImport.Write(
                            string.Format(
                                "{0};{1};{2};{3};{4}\n",
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                "Ошибка",
                                errorMessage.Description));
                    }

                    continue;
                }

                var subsidiaryTypeItem = exportOrgRegistryResult.OrgVersion.Item as LegalType;

                if (subsidiaryTypeItem == null)
                {
                    continue;
                }

                if (!this.gkhContragentsToLoad.ContainsKey(subsidiaryTypeItem.OGRN))
                {
                    continue;
                }

                var gkhContragent = this.gkhContragentsToLoad[subsidiaryTypeItem.OGRN];

                var risContragent = risContragentExists.ContainsKey(gkhContragent.Id)
                    ? risContragentExists[gkhContragent.Id]
                    : new RisContragent();

                risContragent.GkhId = gkhContragent.Id;
                risContragent.FactAddress = gkhContragent.FactAddress;
                risContragent.JuridicalAddress = gkhContragent.JuridicalAddress;
                risContragent.FullName = subsidiaryTypeItem.FullName;
                risContragent.Ogrn = subsidiaryTypeItem.OGRN;
                risContragent.OrgRootEntityGuid = exportOrgRegistryResult.orgRootEntityGUID;
                risContragent.OrgVersionGuid = exportOrgRegistryResult.OrgVersion.orgVersionGUID;
                risContragent.IsIndividual = gkhContragent.OrganizationFormCode == "91";
                this.contragentsToSave.Add(risContragent);

                if (risContragent.OrgRootEntityGuid.IsEmpty())
                {
                    this.logImport.Write(
                        string.Format(
                            "{0};{1};{2};{3};{4}\n",
                            risContragent.FullName,
                            this.contactsByContragentId.Get(risContragent.GkhId),
                            risContragent.Ogrn,
                            "Предупреждение",
                            "В ГИС ЖКХ не указан идентификатор данной ограницазии"));
                }

                this.logImport.Write(
                    string.Format(
                        "{0};{1};{2};{3};{4}\n",
                        risContragent.FullName,
                        this.contactsByContragentId.Get(risContragent.GkhId),
                        risContragent.Ogrn,
                        "Успешно",
                        risContragent.OrgRootEntityGuid));
                result.Add(risContragent.Ogrn);
                this.processedObjects++;
            }

            return result;
        }

        /// <summary>
        /// Инициализировать лог
        /// </summary>
        private void InitLog()
        {
            var logHeader = string.Format("{0};{1};{2};{3};{4}\n",
            "Наименование контрагента",
            "Контакты",
            "ОГРН",
            "Результат",
            "Примечание");
            this.logImport = new CsvLogImport(logHeader);
            this.logImport.SetFileName(this.Name);
        }

        /// <summary>
        /// Инициализировать таймер
        /// </summary>
        private void InitTimer()
        {
            this.timer = new Timer(Interval);
            this.timer.Elapsed += this.OnTimedEvent;
            this.timer.Enabled = false;
            this.timer.AutoReset = true;
        }

        /// <summary>
        /// Сохранить лог
        /// </summary>
        private void SaveLog()
        {
            var gisLogDomain = this.Container.ResolveRepository<GisLog>();
            var fileManager = this.Container.Resolve<IFileManager>();

            try
            {
                var gisLog = new GisLog
                {
                    ServiceLink = this.ServiceAddress,
                    MethodName = this.Name,
                    DateStart = DateTime.Now,
                    CountObjects = 0
                };

                var percent = this.countObjects != 0 ? decimal.Round((this.processedObjects * 100m) / this.countObjects, 2) : 0;

                this.logImport.Write(string.Format("\n{0};{1}\n", "Общее количество", this.countObjects.ToStr()));
                this.logImport.Write(string.Format("{0};{1}\n", "Количество выполненных", this.processedObjects.ToStr()));
                this.logImport.Write(string.Format("{0};{1}\n", "Процент выполненных", percent.ToStr()));

                FileInfo logFileInfo = fileManager.SaveFile(this.logImport.GetFile(), this.logImport.FileName);

                gisLog.DateEnd = DateTime.Now;
                gisLog.CountObjects = this.countObjects;
                gisLog.ProcessedObjects = this.processedObjects;
                gisLog.ProcessedPercent = percent;
                gisLog.FileLog = logFileInfo;
                gisLog.UserName = this.logImport.UserName;

                gisLogDomain.Save(gisLog);
            }
            finally
            {
                this.Container.Release(gisLogDomain);
                this.Container.Release(fileManager);
            }
        }

        /// <summary>
        /// Создать и отправить асинхронные запросы
        /// </summary>
        /// <param name="ogrnValues">Список огрн для поиска</param>
        /// <returns>Список ответов от асинхронного сервиса для получения результатов</returns>
        private List<AckRequest> RequestOrgRegistry(List<string> ogrnValues)
        {
            var startIndex = 0;

            var result = new List<AckRequest>();

            if (ogrnValues.Count > 0)
            {
                do
                {
                    var iterationKeys = ogrnValues.Skip(startIndex).Take(Portion);

                    startIndex += Portion;

                    var searchCriteriaList = this.CreateSearchCriterias(iterationKeys);
                    var request = new exportOrgRegistryRequest { SearchCriteria = searchCriteriaList.ToArray(), Id = "block-to-sign" };
                    var isRequestHeader = new HeaderType { Date = DateTime.Now, MessageGUID = Guid.NewGuid().ToStr() };

                    AckRequest response;
                    this.SoapClient.exportOrgRegistry(isRequestHeader, request, out response);

                    result.Add(response);
                }
                while (startIndex < ogrnValues.Count);
            }

            return result;
        }

        /// <summary>
        /// Обработчик сработки таймера - выполняется с заданным интервалом
        /// Реализует опрос асинхронного сервиса для получения результатов асинхронных запросов
        /// </summary>
        /// <param name="source">Объект</param>
        /// <param name="e">Аргументы события</param>
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            this.timer.Stop();

            this.getResultAttemptsCount++;

            ExplicitSessionScope.CallInNewScope(
                () =>
                {
                    var success = this.GetStateResult(this.contragentAckRequestsStateResults);

                    if (success)
                    {
                        var continueTimer = this.contragentAckRequestsStateResults.Values.Any(x => x == null);

                        if (continueTimer)
                        {
                            if (this.getResultAttemptsCount < ExportOrgRegistryMethod.GetResultAttemptsMaxCount)
                            {
                                this.timer.Enabled = true;
                            }
                            else
                            {
                                this.logImport.Write(string.Format("{0};{1};{2};{3};{4}\n",
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                "Ошибка получения результата асинхронного запроса",
                                string.Format("Превышено максимальное число попыток получения результата асинхронного запроса = {0}", GetResultAttemptsMaxCount)));
                                this.SaveResult();
                            }
                        }
                        else
                        {
                            this.HandleRequestResults();
                            this.SaveResult();
                        }
                    }
                    else
                    {
                        this.SaveResult();
                    }
                });
        }

        /// <summary>
        /// Получить результаты асинхронных запросов
        /// </summary>
        /// <param name="ackRequestsStateResults">Словарь запросов/ответов</param>
        /// <returns>Признак успешности выполнения операции</returns>
        private bool GetStateResult(Dictionary<AckRequest, getStateResult> ackRequestsStateResults)
        {
            var ackRequests = ackRequestsStateResults.Keys.ToList();

            foreach (var ackRequest in ackRequests)
            {
                if (ackRequestsStateResults[ackRequest] == null)
                {
                    try
                    {
                        var result = this.GetStateResult(ackRequest);

                        //Статус обработки сообщения в асинхронном обмене (1- получено; 2 - в обработке; 3- обработано)
                        if (result.RequestState == 3)
                        {
                            ackRequestsStateResults[ackRequest] = result;
                        }
                    }
                    catch (Exception exception)
                    {
                        this.logImport.Write(string.Format("{0};{1};{2};{3};{4}\n",
                        "MessageGuid",
                        ackRequest.Ack.MessageGUID,
                        string.Empty,
                        "Ошибка получения результата асинхронного запроса", exception.Message));
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Получить результат асинхронного запроса
        /// </summary>
        /// <param name="ackRequest">Объект с идентификатором сообщения для получения результата</param>
        /// <returns>Объект со статусом выполнения асинхронного запроса</returns>
        private getStateResult GetStateResult(AckRequest ackRequest)
        {
            getStateResult result;

            var requestHeader = new HeaderType
            {
                Date = DateTime.Now,
                MessageGUID = Guid.NewGuid().ToStr()
            };

            var request = new getStateRequest
            {
                MessageGUID = ackRequest.Ack.MessageGUID
            };

            var client = this.CreateOrgRegistryClient(noSign: true);

            client.getState(requestHeader, request, out result);

            return result;
        }

        /// <summary>
        /// Обработать результаты по асинхронным запросам
        /// </summary>
        private void HandleRequestResults()
        {
            var contragentStateResults = this.contragentAckRequestsStateResults.Values.ToList();
            var processedContragents = new List<string>();

            foreach (var contragentStateResult in contragentStateResults)
            {
                processedContragents.AddRange(this.HandleRequestResultForContragent(contragentStateResult));
            }

            var notProcessedContragents = this.gkhContragentsToLoad.Where(x => !processedContragents.Contains(x.Key));

            foreach (var notProcessedContragent in notProcessedContragents)
            {
                this.logImport.Write(string.Format("{0};{1};{2};{3};{4}\n",
                    notProcessedContragent.Value.Name.Replace(";", string.Empty).Replace(",", string.Empty),
                    this.contactsByContragentId.Get(notProcessedContragent.Value.Id),
                    notProcessedContragent.Value.Ogrn,
                    "Ошибка",
                    "Контрагент не зарегистрирован в ГИС ЖКХ"));
            }
        }

        /// <summary>
        /// Контрагент
        /// </summary>
        private class ContragentProxy
        {
            /// <summary>
            /// Идентификатор
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            /// Наименование
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// ОГРН
            /// </summary>
            public string Ogrn { get; set; }

            /// <summary>
            /// Статус
            /// </summary>
            public ContragentState ContragentState { get; set; }

            /// <summary>
            /// Фактический адрес
            /// </summary>
            public string FactAddress { get; set; }

            /// <summary>
            /// Юридический адрес
            /// </summary>
            public string JuridicalAddress { get; set; }

            /// <summary>
            /// Код организационно-правовой формы
            /// </summary>
            public string OrganizationFormCode { get; set; }
        }
    }
}