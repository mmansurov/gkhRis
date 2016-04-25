Ext.define('B4.view.wizard.export.contractData.ExportContractDataWizard', {
    extend: 'B4.view.wizard.export.ExportWizard',
    //exporter_Id: 'className',
    //methodName: 'contract data',
    //methodDescription: 'contract data',

    getParametersStepFrames: function () {
        return [Ext.create('B4.view.wizard.export.contractData.ContractDataParametersStepFrame', { wizard: this })];
    }
});