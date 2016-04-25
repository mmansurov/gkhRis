namespace Bars.Gkh.Ris
{
    using B4;
    using B4.IoC;
    using B4.Windsor;
    
    using Bars.Gkh.Entities;
    using BillsAsync;
    using Bars.Gkh.Ris.ViewModel.Task;
    using Castle.MicroKernel.Registration;
    using ConfigSections;
    using Controllers;
    using DeviceMetering;
    using DomainService.GisIntegration;
    using DomainService.GisIntegration.Impl;
    using Entities;
    using Entities.GisIntegration;
    using Entities.GisIntegration.Ref;
    using Entities.HouseManagement;
    using ExecutionAction;
    using GisServiceProvider;
    using GisServiceProvider.Bills;
    using GisServiceProvider.DeviceMetering;
    using GisServiceProvider.HouseManagement;
    using GisServiceProvider.Infrastructure;
    using GisServiceProvider.Inspection;
    using GisServiceProvider.Nsi;
    using GisServiceProvider.OrgRegistry;
    using GisServiceProvider.OrgRegistryCommon;
    using GisServiceProvider.Payment;
    using GisServiceProvider.Services;
    using Gkh.ExecutionAction;
    using HouseManagementAsync;
    using Infrastructure;
    using Inspection;
    using Integration;
    using Integration.Bills.Exporters;
    using Integration.DeviceMetering.Methods;
    using Integration.FileService;
    using Integration.FileService.Impl;
    using Integration.HouseManagement.DataExtractors;
    using Integration.HouseManagement.Exporters;
    using Integration.HouseManagement.Methods;
    using Integration.Infrastructure.Methods;
    using Integration.Inspection.DataExtractors;
    using Integration.Inspection.Methods;
    using Integration.Nsi;
    using Integration.Nsi.DataExtractors;
    using Integration.Nsi.DictionaryAction;
    using Integration.Nsi.DictionaryAction.HouseManagement;
    using Integration.Nsi.DictionaryAction.Services;
    using Integration.Nsi.Exporters;
    using Integration.Nsi.Methods;
    using Integration.OrgRegistryCommon.Exporters;
    using Integration.OrgRegistryCommon.Methods;
    using Integration.Payment.Methods;
    using Integration.Services.DataExtractors;
    using Integration.Services.Methods;
    using Interceptors;
    using Interceptors.GisIntegration;
    using NsiAsync;
    using PaymentAsync;
    using Quartz.Scheduler;
    using Services;
    using Tasks.Bills;
    using Tasks.HouseManagement;
    using Tasks.Nsi;
    using Tasks.OrgRegistryCommon;
    using Utils;
    using ViewModel;
    using ViewModel.GisIntegration;
    using ViewModel.HouseManagement;

    public partial class Module : AssemblyDefinedModule
    {        
        public override void Install()
        {
            // маршруты
            this.Container.RegisterTransient<IClientRouteMapRegistrar, ClientRouteMapRegistrar>();

            // ресурсы
            this.Container.RegisterTransient<IResourceManifest, ResourceManifest>();

            // настройки ограничений
            this.Container.Register(Component.For<IPermissionSource>().ImplementedBy<RisPermissionMap>());

            this.Container.RegisterTransient<INavigationProvider, NavigationProvider>();

            this.Container.RegisterGkhConfig<GisIntegrationConfig>();
            this.Container.RegisterTransient<IExecutionAction, GisIntegrationConfigCreateAction>(GisIntegrationConfigCreateAction.Code);

            this.RegisterControllers();

            this.RegisterInterceptors();

            this.RegisterViewModels();

            this.RegisterServices();

            this.RegisterExtractors();

            this.RegisterBundlers();

            this.RegisterTasks();

            this.RegisterServiceProviders();

            this.RegisterExporters();
        }

        public void RegisterControllers()
        {
            this.Container.RegisterAltDataController<GisDict>();
            this.Container.RegisterAltDataController<GisDictRef>();
            this.Container.RegisterAltDataController<GisLog>();
            this.Container.RegisterController<GisIntegrationController>();
            this.Container.RegisterController<RisSettingsController>();
            this.Container.RegisterController<TaskTreeController>();
            this.Container.RegisterController<HouseManagementController>();
            this.Container.RegisterController<NsiController>();

            this.Container.RegisterAltDataController<RisContract>();
        }

        public void RegisterViewModels()
        {
            this.Container.RegisterViewModel<GisDict, GisDictViewModel>();
            this.Container.RegisterViewModel<GisDictRef, GisDictRefViewModel>();
            this.Container.RegisterViewModel<GisLog, GisLogViewModel>();
            this.Container.RegisterTransient<ITreeViewModel, TaskTreeViewModel>("TaskTreeViewModel");
            this.Container.RegisterViewModel<RisContract, RisContractViewModel>();
        }

        public void RegisterServices()
        {
            this.Container.RegisterTransient<IGisIntegrationService, GisIntegrationService>();
            this.Container.RegisterTransient<IFileUploadService, FileUploadService>();
            this.Container.RegisterTransient<IHouseManagementService, HouseManagementService>();
            this.Container.RegisterTransient<INsiService, NsiService>();

            // регистрация словарей
            this.Container.RegisterTransient<IGisIntegrDictAction, TypeContractManOrgDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, ConditionHouseDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, TypeBaseCheckJurPersonDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, CheckFormDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, KindCheckDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, ContractBaseDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, ServiceTypeDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, MunicipalServiceDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, MunicipalResourceDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, RoomsNumDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, PremisesCharacteristicDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, PositionDictAction>();
            this.Container.RegisterTransient<IGisIntegrDictAction, PurposeDictAction>();

            // регистрация методов
            this.Container.RegisterTransient<IGisIntegrationMethod, ExportOrgRegistryMethod>("ExportOrgRegistryMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportInspectionPlanMethod>("ImportInspectionPlanMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportExaminationsMethod>("ImportExaminationsMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportAccountDataMethod>("ImportAccountDataMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportCharterDataMethod>("ImportCharterDataMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportShareEncbrDataMethod>("ImportShareEncbrDataMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportPublicPropertyContractRequestMethod>("ImportPublicPropertyContractRequestMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportMeteringDeviceDataMethod>("ImportMeteringDeviceDataMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportVotingProtocolMethod>("ImportVotingProtocolMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportNotificationDataMethod>("ImportNotificationDataMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportMeteringDeviceCurrentValuesMethod>("ImportMeteringDeviceCurrentValuesMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportMeteringDeviceControlValuesMethod>("ImportMeteringDeviceControlValuesMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportMeteringDeviceVerificationValueMethod>("ImportMeteringDeviceVerificationValueMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportNotificationsOfOrderExecutionMethod>("ImportNotificationsOfOrderExecutionMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportHouseOMSMethod>("ImportHouseOMSMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportContractDataMethod>("ImportContractDataMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportHouseUODataMethod>("ImportHouseUODataMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportHouseRSODataMethod>("ImportHouseRSODataMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportMSRSOMethod>("ImportMSRSOMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportOkiMethod>("ImportOkiMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportMunicipalServiceMethod>("ImportMunicipalServiceMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportWorkingListMethod>("ImportWorkingListMethod");
            this.Container.RegisterTransient<IGisIntegrationMethod, ImportAcknowledgmentMethod>("ImportAcknowledgmentMethod");
        }

        public void RegisterExtractors()
        {
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, ImportExaminationsDataExtractor>("ImportExaminationsDataExtractor");
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, ImportInspectionPlanDataExtractor>("ImportInspectionPlanDataExtractor");
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, CharterDataExtractor>("CharterDataExtractor");
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, ImportPublicPropertyContractDataExtractor>("ImportPublicPropertyContractDataExtractor");
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, ImportVotingProtocolDataExtractor>("ImportVotingProtocolDataExtractor");           
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, ContractDataExtractor>("ContractDataExtractor");
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, ImportMSRSODataExtractor>("ImportMSRSODataExtractor");
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, ImportWorkingListDataExtractor>("ImportWorkingListDataExtractor");
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, AdditionalServicesDataExtractor>("AdditionalServicesDataExtractor");
            this.Container.RegisterTransient<IGisIntegrationDataExtractor, ImportMunicipalServiceDataExtractor>("ImportMunicipalServiceDataExtractor");

            this.Container.RegisterTransient<IDataExtractor<RisHouse, RealityObject>, RisHouseDataExtractor>("RisHouseDataExtractor");
            this.Container.RegisterTransient<IDataExtractor<RisEntrance, Entrance>, EntranceDataExtractor>("EntranceDataExtractor");
            this.Container.RegisterTransient<IDataExtractor<NonResidentialPremises, Room>, NonResidentialPremisesDataExtractor>("NonResidentialPremisesDataExtractor");
            this.Container.RegisterTransient<IDataExtractor<ResidentialPremises, Room>, ResidentialPremisesDataExtractor>("ResidentialPremisesDataExtractor");
        }

        public void RegisterInterceptors()
        {
            this.Container.RegisterDomainInterceptor<GisDict, GisDictInterceptor>();
            this.Container.RegisterDomainInterceptor<GisLog, UserEntityInterceptor<GisLog>>();
            this.Container.RegisterDomainInterceptor<RisPackage, UserEntityInterceptor<RisPackage>>();
            this.Container.RegisterDomainInterceptor<RisTask, UserEntityInterceptor<RisTask>>();
        }

        public void RegisterTasks()
        {
            this.Container.RegisterTask<ExportAcknowledgmentTask>();
            this.Container.RegisterTask<ExportDataProviderTask>();
            this.Container.RegisterTask<ExportCharterDataTask>();
            this.Container.RegisterTask<ExportContractDataTask>();
            this.Container.RegisterTask<ExportHouseDataTask>();
            this.Container.RegisterTask<ExportAdditionalServicesTask>();
        }

        public void RegisterServiceProviders()
        {
            this.Container.RegisterTransient<IGisServiceProvider<BillsPortsTypeAsyncClient>, BillsAsyncServiceProvider>("BillsAsyncServiceProvider");
            this.Container.RegisterTransient<IGisServiceProvider<DeviceMeteringPortTypesClient>, DeviceMeteringServiceProvider>("DeviceMeteringServiceProvider");
            this.Container.RegisterTransient<IGisServiceProvider<HouseManagementPortsTypeAsyncClient>, HouseManagementServiceProvider>("HouseManagementServiceProvider");
            this.Container.RegisterTransient<IGisServiceProvider<InfrastructurePortsTypeClient>, InfrastructureServiceProvider>("InfrastructureServiceProvider");
            this.Container.RegisterTransient<IGisServiceProvider<InspectionPortsTypeClient>, InspectionServiceProvider>("InspectionServiceProvider");
            this.Container.RegisterTransient<IGisServiceProvider<NsiPortsTypeAsyncClient>, NsiServiceProvider>("NsiServiceProvider");
            this.Container.RegisterTransient<IGisServiceProvider<OrgRegistryAsync.RegOrgPortsTypeAsyncClient>, OrgRegistryServiceProvider>("OrgRegistryServiceProvider");
            this.Container.RegisterTransient<IGisServiceProvider<OrgRegistryCommonAsync.RegOrgPortsTypeAsyncClient>, OrgRegistryCommonServiceProvider>("OrgRegistryCommonServiceProvider");
            this.Container.RegisterTransient<IGisServiceProvider<PaymentPortsTypeAsyncClient>, PaymentServiceProvider>("PaymentServiceProvider");
            this.Container.RegisterTransient<IGisServiceProvider<ServicesPortsTypeClient>, ServicesServiceProvider>("ServicesServiceProvider");
        }

        public void RegisterExporters()
        {
            this.Container.RegisterTransient<IDataExporter, ContractDataExporter>("ContractDataExporter");
            this.Container.RegisterTransient<IDataExporter, AcknowledgmentExporter>("AcknowledgmentExporter");
            this.Container.RegisterTransient<IDataExporter, DataProviderExporter>("DataProviderExporter");
            this.Container.RegisterTransient<IDataExporter, CharterDataExporter>("CharterDataExporter");
            this.Container.RegisterTransient<IDataExporter, HouseUODataExporter>("HouseUODataExporter");
            this.Container.RegisterTransient<IDataExporter, HouseOMSDataExporter>("HouseOMSDataExporter");
            this.Container.RegisterTransient<IDataExporter, AdditionalServicesExporter>("AdditionalServicesExporter");
            this.Container.RegisterTransient<IDataExporter, HouseRSODataExporter>("HouseRSODataExporter");
        }
    }
}