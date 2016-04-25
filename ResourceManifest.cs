namespace Bars.Gkh.Ris
{    
    using Bars.B4;

    public partial class ResourceManifest : ResourceManifestBase
    {

		protected override void BaseInit(IResourceManifestContainer container)
        {  

            AddResource(container, "libs/B4/controller/integrations/GisIntegration.js");
            AddResource(container, "libs/B4/controller/integrations/GisIntegrationSettings.js");
            AddResource(container, "libs/B4/cryptopro/jsxmlsigner.js");
            AddResource(container, "libs/B4/cryptopro/xadessigner.js");
            AddResource(container, "libs/B4/model/ExporterValidateResult.js");
            AddResource(container, "libs/B4/model/RisCertificate.js");
            AddResource(container, "libs/B4/model/RisPackage.js");
            AddResource(container, "libs/B4/model/TaskTreeNode.js");
            AddResource(container, "libs/B4/model/integrations/gis/DataExtractor.js");
            AddResource(container, "libs/B4/model/integrations/gis/Dict.js");
            AddResource(container, "libs/B4/model/integrations/gis/DictRef.js");
            AddResource(container, "libs/B4/model/integrations/gis/Log.js");
            AddResource(container, "libs/B4/model/integrations/gis/Method.js");
            AddResource(container, "libs/B4/model/integrations/houseManagement/Charter.js");
            AddResource(container, "libs/B4/model/integrations/houseManagement/Contract.js");
            AddResource(container, "libs/B4/model/integrations/houseManagement/House.js");
            AddResource(container, "libs/B4/model/integrations/nsi/AdditionalService.js");
            AddResource(container, "libs/B4/store/ExporterValidateResult.js");
            AddResource(container, "libs/B4/store/RisCertificate.js");
            AddResource(container, "libs/B4/store/RisPackage.js");
            AddResource(container, "libs/B4/store/integrations/gis/DataExtractor.js");
            AddResource(container, "libs/B4/store/integrations/gis/Dict.js");
            AddResource(container, "libs/B4/store/integrations/gis/DictList.js");
            AddResource(container, "libs/B4/store/integrations/gis/DictRecordList.js");
            AddResource(container, "libs/B4/store/integrations/gis/DictRef.js");
            AddResource(container, "libs/B4/store/integrations/gis/Log.js");
            AddResource(container, "libs/B4/store/integrations/gis/Method.js");
            AddResource(container, "libs/B4/store/integrations/gis/TaskTreeStore.js");
            AddResource(container, "libs/B4/store/integrations/houseManagement/Charter.js");
            AddResource(container, "libs/B4/store/integrations/houseManagement/Contract.js");
            AddResource(container, "libs/B4/store/integrations/houseManagement/House.js");
            AddResource(container, "libs/B4/store/integrations/nsi/AdditionalService.js");
            AddResource(container, "libs/B4/view/integrations/gis/DataExtractorGrid.js");
            AddResource(container, "libs/B4/view/integrations/gis/DictGrid.js");
            AddResource(container, "libs/B4/view/integrations/gis/DictRefGrid.js");
            AddResource(container, "libs/B4/view/integrations/gis/DictRefSelectWindow.js");
            AddResource(container, "libs/B4/view/integrations/gis/DictRefWindow.js");
            AddResource(container, "libs/B4/view/integrations/gis/DictWindow.js");
            AddResource(container, "libs/B4/view/integrations/gis/LogGrid.js");
            AddResource(container, "libs/B4/view/integrations/gis/MethodGrid.js");
            AddResource(container, "libs/B4/view/integrations/gis/Panel.js");
            AddResource(container, "libs/B4/view/integrations/gis/SettingsPanel.js");
            AddResource(container, "libs/B4/view/integrations/gis/SignatureWindow.js");
            AddResource(container, "libs/B4/view/integrations/gis/TaskTree.js");
            AddResource(container, "libs/B4/view/wizard/WizardBaseStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/WizardFinishStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/WizardStartStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/WizardWindow.js");
            AddResource(container, "libs/B4/view/wizard/export/ExportWizard.js");
            AddResource(container, "libs/B4/view/wizard/export/NotSignedXmlPreviewWin.js");
            AddResource(container, "libs/B4/view/wizard/export/ParametersStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/export/SignedXmlPreviewWin.js");
            AddResource(container, "libs/B4/view/wizard/export/ValidationResultStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/export/XmlPreviewStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/export/additionalservices/AdditionalServicesParametersStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/export/additionalservices/ExportAdditionalServicesWizard.js");
            AddResource(container, "libs/B4/view/wizard/export/charterData/CharterDataParametersStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/export/charterData/ExportCharterDataWizard.js");
            AddResource(container, "libs/B4/view/wizard/export/contractData/ContractDataParametersStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/export/contractData/ExportContractDataWizard.js");
            AddResource(container, "libs/B4/view/wizard/export/dataprovider/ExportDataProviderWizard.js");
            AddResource(container, "libs/B4/view/wizard/export/dataprovider/ExportDataProviderXmlPreviewStepFrame.js");
            AddResource(container, "libs/B4/view/wizard/export/houseData/ExportHouseDataWizard.js");
            AddResource(container, "libs/B4/view/wizard/export/houseData/HouseDataParametersStepFrame.js");

            AddResource(container, "content/css/risMain.css");
            AddResource(container, "content/icon/arrow-180.png");
            AddResource(container, "content/icon/arrow.png");
            AddResource(container, "content/icon/cross.png");
            AddResource(container, "content/icon/task.png");
            AddResource(container, "content/icon/wand.png");
            AddResource(container, "content/img/apply.png");
            AddResource(container, "content/img/error.png");
            AddResource(container, "content/img/warning.png");
            AddResource(container, "content/img/wizard.png");
        }

        private void AddResource(IResourceManifestContainer container, string path)
		{

            container.Add(path, string.Format("Bars.Gkh.Ris.dll/Bars.Gkh.Ris.{0}", path.Replace("/", ".")));
        }
    }
}
