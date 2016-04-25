namespace Bars.Gkh.Ris.DomainService.GisIntegration.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    using B4;
    using B4.DataAccess;
    using B4.Utils;
    using Authentification;

    using Castle.Windsor;
    using Config;
    using ConfigSections;
    using Domain;
    using Entities;
    using Entities.GisIntegration;
    using Enums;
    using Extensions;
    using Integration;
    using Integration.Nsi;
    using Integration.Signature;
    using NsiCommon;

    public class GisIntegrationService : IGisIntegrationService
    {
        public IWindsorContainer Container { get; set; }

        /// <summary>
        /// Получить список методов
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        public IDataResult GetMethodList(BaseParams baseParams)
        {
            var methodList = this.Container.ResolveAll<IGisIntegrationMethod>();
            var exporterList = this.Container.ResolveAll<IDataExporter>();
            var authorizationService = this.Container.ResolveAll<IAuthorizationService>().FirstOrDefault();
            var userIdentity = this.Container.Resolve<IUserIdentity>();

            if (authorizationService == null)
            {
                throw new NullReferenceException();
            }

            var methods = methodList
                .Where(x => authorizationService.Grant(
                    userIdentity,
                    "Administration.OutsideSystemIntegrations.Gis.Methods.List." + x.Code))
                .Select(x => new MethodProxy
                {
                    Id = x.GetType().Name,
                    Name = x.Name,
                    Order = x.Order,
                    Description = x.Description,
                    Type = "method",
                    NeedSign = x.NeedSign
                });

            var exporters = exporterList
                .Where(x => authorizationService.Grant(
                    userIdentity,
                    "Administration.OutsideSystemIntegrations.Gis.Exporters." + x.GetType().Name))
                .Select(x => new MethodProxy
                {
                    Id = x.GetType().Name,
                    Name = x.Name,
                    Order = x.Order,
                    Description = x.Description,
                    Type = "exporter",
                    NeedSign = x.NeedSign
                });

            var dataList = new List<MethodProxy>();
            dataList.AddRange(methods);
            dataList.AddRange(exporters);

            var loadParams = baseParams.GetLoadParam();

            try
            {
                var data = dataList
                    .AsQueryable()
                    .OrderBy(x => x.Order)
                    .Filter(loadParams, this.Container);

                return new ListDataResult(data.Paging(loadParams).ToList(), data.Count());
            }
            finally
            {
                this.Container.Release(methodList);
            }
        }

        /// <summary>
        /// Метод проверки необходим ли для выполнения метода сертификат
        /// </summary>
        /// <param name="baseParams">Базовые параметры</param>
        /// <returns>Результат, содержащий true - сертификат нужен, false - в противном случае</returns>
        public IDataResult NeedCertificate(BaseParams baseParams)
        {
            if (!this.SingXmlConfig())
            {
                return new BaseDataResult
                {
                    Data = false
                };
            }

            var methodId = baseParams.Params.GetAs("method_Id", string.Empty);
            var method = this.Container.Resolve<IGisIntegrationMethod>(methodId);

            return new BaseDataResult
            {
                Data = method.NeedSign
            };
        }

        /// <summary>
        /// Получение списка справочников
        /// </summary>
        public IDataResult GetDictList(BaseParams baseParams)
        {
            var loadParams = baseParams.GetLoadParam();
            var soapClient = this.CreateNsiClient();

            var requestHeader = new HeaderType
            {
                Date = DateTime.Now,
                MessageGUID = Guid.NewGuid().ToStr()
            };

            exportNsiListResult listResult;

            soapClient.exportNsiList(requestHeader, new exportNsiListRequest { Id = "block-to-sign" }, out listResult);

            var error = listResult.Item as ErrorMessageType;

            if (error != null)
            {
                var messageText = string.Format("Ошибка при загрузке списка справочников: ErrorCode: {0}, Description: {1}", error.ErrorCode, error.Description);

                return new ListDataResult { Success = false, Message = messageText };
            }

            var nsiList = (NsiListType)listResult.Item;

            var data = nsiList.NsiItemInfo.ToList()
            .Select(x => new
            {
                Id = x.RegistryNumber,
                x.Name
            })
            .AsQueryable()
            .Order(loadParams)
            .OrderIf(loadParams.Order.Length == 0, true, x => x.Name)
            .Filter(loadParams, this.Container);

            return new ListDataResult(data.Paging(loadParams).ToList(), data.Count());
        }

        /// <summary>
        ///  Метод обновления таблицы каталогов
        /// </summary>
        public IDataResult GetDictActions(BaseParams baseParams)
        {
            var actions = this.Container.ResolveAll<IGisIntegrDictAction>();

            try
            {
                return new BaseDataResult(actions.Select(x => new { Name = x.Code }).ToList());
            }
            finally
            {
                this.Container.Release(actions);
            }
        }

        /// <summary>
        ///  Метод получения записей справочника
        /// </summary>
        public IDataResult GetDictRecordList(BaseParams baseParams)
        {
            var loadParams = baseParams.GetLoadParam();
            var gisDictDomain = this.Container.ResolveDomain<GisDict>();
            var actions = this.Container.ResolveAll<IGisIntegrDictAction>();

            var dictId = loadParams.Filter.GetAs("dictId", 0L);

            try
            {
                var dict = gisDictDomain.GetAll().FirstOrDefault(x => x.Id == dictId);

                if (dict == null)
                {
                    return new BaseDataResult(false, "Не удалось опредлить спраовчник ГИС");
                }

                if (dict.NsiRegistryNumber.IsEmpty())
                {
                    return new BaseDataResult(false, "Не задан Идентифификатор ГИС");
                }

                if (string.IsNullOrEmpty(dict.ActionCode))
                {
                    return new BaseDataResult(false, "Не задан метод интеграции");
                }

                var action = actions.FirstOrDefault(x => x.Code == dict.ActionCode);

                if (action == null)
                {
                    return new BaseDataResult(false, string.Format("Не найден метод интеграции {0}", dict.ActionCode));
                }

                action.Dict = dict;
                action.SoapClient = this.CreateNsiClient();
                var data = action
                    .GetGisRecords()
                    .AsQueryable()
                    .Filter(loadParams, this.Container)
                    .Order(loadParams)
                    .ToList();

                return new ListDataResult(data, data.Count);
            }
            finally
            {
                this.Container.Release(actions);
            }
        }

        /// <summary>
        /// Получение списка сертификатов для подписи сообщения
        /// </summary>
        public IDataResult GetCertificates(BaseParams baseParams)
        {
            var certificates = new List<CertificateProxy>();

            var storeCurrentUser = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            storeCurrentUser.Open(OpenFlags.ReadOnly);

            foreach (var certificate in storeCurrentUser.Certificates)
            {
                if (certificate.SignatureAlgorithm.FriendlyName != "ГОСТ Р 34.11/34.10-2001")
                {
                    continue;
                }

                var certProxy = new CertificateProxy
                {
                    SubjectName = certificate.SubjectName.Name,
                    Thumbprint = certificate.Thumbprint
                };

                certificates.Add(certProxy);
            }

            storeCurrentUser.Close();

            var storeLocalMachine = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            storeLocalMachine.Open(OpenFlags.ReadOnly);

            foreach (var certificate in storeLocalMachine.Certificates)
            {
                if (certificate.SignatureAlgorithm.FriendlyName != "ГОСТ Р 34.11/34.10-2001")
                {
                    continue;
                }

                var certProxy = new CertificateProxy
                {
                    SubjectName = certificate.SubjectName.Name,
                    Thumbprint = certificate.Thumbprint
                };

                certificates.Add(certProxy);
            }

            storeLocalMachine.Close();

            return new ListDataResult(certificates, certificates.Count);
        }

        public NsiPortsTypeClient CreateNsiClient()
        {
            var configProvider = this.Container.Resolve<IGkhConfigProvider>();

            try
            {
                var gisIntegrationConfig = configProvider.Get<GisIntegrationConfig>();

                var serviceAddress = gisIntegrationConfig.GetServiceAddress(
                    IntegrationService.NsiCommon,
                    false,
                    "http://127.0.0.1:8080/ext-bus-nsi-common-service/services/NsiCommon");

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
                    }
                };

                var remoteAddress = new EndpointAddress(serviceAddress);

                var client = new NsiPortsTypeClient(binding, remoteAddress);

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

                if (gisIntegrationConfig.SingXml)
                {
                    var thumbprint = "d9459e191b645c21238cb6ab001fd3d33925914c";
                    //var thumbprint = "29966f5a905574c91f167efb9f54fb6633719211";
                    var storeName = StoreName.My;
                    var storeLocation = StoreLocation.CurrentUser;
                    var store = new X509Store(storeName, storeLocation);
                    store.Open(OpenFlags.ReadOnly);
                    var cert = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true)[0];

                    client.Endpoint.Behaviors.Add(new GisIntegrationClientMessageInspector(cert));
                }

                return client;
            }
            finally
            {
                this.Container.Release(configProvider);
            }
        }

        /// <summary>
        /// Получить список экстракторов для метода
        /// </summary>
        /// <returns>Список экстракторов для метода</returns>
        private List<IGisIntegrationDataExtractor> ResolveExtractors(string methodCode)
        {
            return this.Container.ResolveAll<IGisIntegrationDataExtractor>()
                .Where(x => x.Code == methodCode)
                .ToList();
        }

        /// <summary>
        /// Выполнение метода 
        /// </summary>
        /// <param name="baseParams">Параметры запроса</param>
        /// <returns>Результат выполнения</returns>
        public IDataResult ExecuteMethod(BaseParams baseParams)
        {
            var methodId = baseParams.Params.GetAs("method_Id", string.Empty);
            var onlyMethod = baseParams.Params.GetAs<bool>("onlyMethod");

            if (string.IsNullOrEmpty(methodId))
            {
                return new BaseDataResult(false, "Не удалось получить метод c пустым идентификатором.");
            }

            var method = this.Container.Resolve<IGisIntegrationMethod>(methodId);
            var contragentDomain = this.Container.ResolveDomain<RisContragent>();

            if (method == null)
            {
                return new BaseDataResult(false, string.Format("Не удалось получить метод c идентификатором {0}.", methodId));
            }

            try
            {
                if (methodId == "ExportOrgRegistryMethod")
                {
                    return method.Execute();
                }

                var currentContragent = this.GetCurrentContragent();

                if (currentContragent == null && method.NeedSign)
                {
                    throw new Exception("Текущий контрагент не найден в системе РИС.");
                }


                var extractors = this.ResolveExtractors(method.Code);

                if (!onlyMethod)
                {
                    foreach (var extractor in extractors)
                    {
                        try
                        {
                            extractor.Contragent = currentContragent;
                            extractor.Extract();
                        }
                        finally
                        {
                            this.Container.Release(extractor);
                        }
                    }
                }

                method.Contragent = currentContragent;

                return method.Execute();
            }
            catch (Exception exc)
            {
                return new BaseDataResult(false, exc.Message);
            }
            finally
            {
                this.Container.Release(method);
                this.Container.Release(contragentDomain);
            }
        }

        /// <summary>
        /// Метод подготовки данных к экспорту, включая валидацию, формирование пакетов
        /// </summary>
        /// <param name="baseParams">Параметры экспорта</param>
        /// <returns>Результат подготовки данных</returns>
        public IDataResult PrepareDataForExport(BaseParams baseParams)
        {
            var exporterId = baseParams.Params.GetAs("exporter_Id", string.Empty);
            var onlyMethod = baseParams.Params.GetAs<bool>("onlyMethod");

            if (string.IsNullOrEmpty(exporterId))
            {
                return new BaseDataResult(false, "Не удалось получить экспортер c пустым идентификатором.");
            }

            var exporter = this.Container.Resolve<IDataExporter>(exporterId);
            var containerDomain = this.Container.ResolveDomain<RisContainer>();
            var contragentDomain = this.Container.ResolveDomain<RisContragent>();

            if (exporter == null)
            {
                return new BaseDataResult(false, string.Format("Не удалось получить экспортер c идентификатором {0}.", exporterId));
            }

            try
            {
                if (exporterId != "DataProviderExporter")
                {
                    var currentContragent = this.GetCurrentContragent();

                    if (currentContragent == null)
                    {
                        throw new Exception("Текущий контрагент не найден в системе РИС.");
                    }

                    exporter.Contragent = currentContragent;
                }

                var prepareDataResult = exporter.PrepareData(baseParams.Params);

                var result = new BaseDataResult(
                        new
                        {
                            prepareDataResult.ValidateResult,
                            Packages = prepareDataResult.Packages.Select(x => new
                            {
                                x.Id,
                                x.Name
                            }).ToList()
                        });

                return result;
            }
            catch (Exception exc)
            {
                return new BaseDataResult(false, exc.Message);
            }
            finally
            {
                this.Container.Release(exporter);
                this.Container.Release(containerDomain);
                this.Container.Release(contragentDomain);
            }
        }

        /// <summary>
        /// Получить неподписанные данные форматированные для просмотра
        /// либо неформатированные для подписи
        /// </summary>
        /// <param name="baseParams">Параметры, содержащие идентификатор пакета</param>
        /// <returns>Неподписанные данные форматированной xml строкой</returns>
        public IDataResult GetNotSignedData(BaseParams baseParams)
        {
            var packageIds = baseParams.Params.GetAs("package_Ids", string.Empty).ToLongArray();
            var forPreview = baseParams.Params.GetAs<bool>("forPreview");

            if (packageIds == null)
            {
                return new BaseDataResult(false, "Не удалось получить идентификаторы пакетов");
            }

            var packageDomain = this.Container.ResolveDomain<RisPackage>();

            try
            {
                var packages = packageDomain.GetAll()
                    .Where(x => packageIds.Contains(x.Id))
                    .Select(x => new
                    {
                        x.Id,
                        NotSignedData = forPreview ? x.GetNotSignedDataXmlString() : x.GetNotSignedXmlString()
                    })
                    .ToArray();

                if (!packages.Any())
                {
                    return new BaseDataResult(false, "Не удалось получить неподписанные xml для пакетов");
                }

                return new BaseDataResult(packages);
            }
            finally
            {
                this.Container.Release(packageDomain);
            }
        }

        /// <summary>
        /// Сохранить подписанные xml
        /// </summary>
        /// <param name="baseParams"></param>
        /// <returns></returns>
        public IDataResult SaveSignedData(BaseParams baseParams)
        {
            var exporterId = baseParams.Params.GetAs("exporter_Id", string.Empty);
            var packages = baseParams.Params.GetAs<List<SignedDataProxy>>("packages");

            var exporter = this.Container.Resolve<IDataExporter>(exporterId);

            try
            {
                if (string.IsNullOrEmpty(exporterId))
                {
                    return new BaseDataResult(false, "Не удалось получить экспортер");
                }

                if (packages == null || packages.Count == 0)
                {
                    return new BaseDataResult(false, "Нет удалось получить пакеты с подписанными xml");
                }

                var dict = new Dictionary<long, string>();
                foreach (var package in packages)
                {
                    dict.Add(package.Id, Uri.UnescapeDataString(package.SignedData));
                }

                exporter.SaveSignedData(dict);

                return new BaseDataResult();
            }
            finally
            {
                Container.Release(exporter);
            }
        }

        /// <summary>
        /// Запустить экспорт
        /// </summary>
        /// <param name="baseParams">Параметры, содержащие идентификаторы пакетов</param>
        /// <returns>Идентификатор запланированной задачи получения результатов экспорта</returns>
        public IDataResult ExecuteExporter(BaseParams baseParams)
        {
            var exporterId = baseParams.Params.GetAs("exporter_Id", string.Empty);
            var packageIds = baseParams.Params.GetAs("package_Ids", string.Empty).ToLongArray();

            if (string.IsNullOrEmpty(exporterId))
            {
                return new BaseDataResult(false, "Не удалось получить экспортер c пустым идентификатором.");
            }

            var exporter = this.Container.Resolve<IDataExporter>(exporterId);
            var dataSendingResult = exporter.Execute(packageIds);

            var result = new BaseDataResult(new
            {
                TaskId = dataSendingResult.Task == null ? 0 : dataSendingResult.Task.Id,
                PackageSendingErrors = dataSendingResult.PackageSendingResults.Where(x => !x.Success).Select(x => new
                {
                    x.Package.Id,
                    x.Package.Name,
                    x.Message
                }).ToList(),
                State = dataSendingResult.State.ToString()
            });

            return result;
        }

        /// <summary>
        /// Удалить временные объекты
        /// </summary>
        /// <param name="baseParams">Параметры, содержащие идентификаторы всех созданных объектов в рамках работы одного мастера</param>
        /// <returns>Результат выполнения операции</returns>
        public IDataResult DeleteTempObjects(BaseParams baseParams)
        {
            var packageIds = baseParams.Params.GetAs("package_Ids", string.Empty).ToLongArray();

            var tempPackages = this.GetTempPackages(packageIds);

            this.DeletePackages(tempPackages);

            return new BaseDataResult();
        }

        /// <summary>
        /// Получить контрагента текущего пользователя
        /// </summary>
        /// <returns>Контрагент РИС текущего пользователя</returns>
        public RisContragent GetCurrentContragent()
        {
            var userManager = this.Container.Resolve<IGkhUserManager>();
            var contragentDomain = this.Container.ResolveDomain<RisContragent>();

            try
            {
                var currentContragentIds = userManager.GetContragentIds();

                if (currentContragentIds == null || currentContragentIds.Count == 0)
                {
                    throw new Exception("К вашей учетной записи не привязан контрагент, передача данных невозможна.\nОбратитесь к администратору системы.");
                }
                else if (currentContragentIds.Count > 1)
                {
                    throw new Exception("К вашей учетной записи привязано несколько контрагентов, передача данных невозможна.\nОбратитесь к администратору системы.");
                }

                return contragentDomain.GetAll().FirstOrDefault(x => x.GkhId == currentContragentIds[0]);
            }
            finally
            {
                this.Container.Release(userManager);
                this.Container.Release(contragentDomain);
            }
        }

        private long[] GetTempPackages(long[] packageIdentifiers)
        {
            var taskPackageDomain = this.Container.ResolveDomain<RisTaskPackage>();

            try
            {
                //считаем временными пакеты, не связанные ни с одной задачей

                return packageIdentifiers.Where(x => !taskPackageDomain.GetAll().Any(y => y.Package.Id == x)).ToArray();
            }
            finally
            {
                this.Container.Release(taskPackageDomain);
            }
        }

        private void DeletePackages(long[] packageIdentifiers)
        {
            var packagesDomain = this.Container.ResolveDomain<RisPackage>();

            try
            {
                foreach (var packageIdentifier in packageIdentifiers)
                {
                    packagesDomain.Delete(packageIdentifier);
                }
            }
            finally
            {
                this.Container.Release(packagesDomain);
            }
        }

        /// <summary>
        /// Получить сертификат по thumbprint
        /// </summary>
        /// <param name="thumbprint"></param>
        /// <returns></returns>
        private X509Certificate2 GetCertificateByThumbprint(string thumbprint)
        {
            if (!this.SingXmlConfig())
            {
                return null;
            }

            var storeCurrentUser = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            storeCurrentUser.Open(OpenFlags.ReadOnly);
            var certCurrentUser = storeCurrentUser.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true)[0];
            storeCurrentUser.Close();

            if (certCurrentUser != null)
            {
                return certCurrentUser;
            }

            var storeLocalMachine = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            storeLocalMachine.Open(OpenFlags.ReadOnly);
            var certLocalMachine = storeLocalMachine.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, true)[0];
            storeLocalMachine.Close();

            return certLocalMachine;
        }

        /// <summary>
        /// Метод получения настройки "подписывать сообщение"
        /// </summary>
        /// <returns>Значение настройки</returns>
        private bool SingXmlConfig()
        {
            var configProvider = this.Container.Resolve<IGkhConfigProvider>();

            try
            {
                var gisIntegrationConfig = configProvider.Get<GisIntegrationConfig>();
                return gisIntegrationConfig.SingXml;
            }
            finally
            {
                this.Container.Release(configProvider);
            }
        }

        private class CertificateProxy
        {
            public string Thumbprint { get; set; }
            public string SubjectName { get; set; }
        }

        private class MethodProxy
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Order { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public bool NeedSign { get; set; }
        }

        private class SignedDataProxy
        {
            public long Id { get; set; }

            public string SignedData { get; set; }
        }
    }
}