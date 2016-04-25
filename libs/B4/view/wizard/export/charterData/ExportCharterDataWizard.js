Ext.define('B4.view.wizard.export.charterData.ExportCharterDataWizard', {
    extend: 'B4.view.wizard.export.ExportWizard',

    getParametersStepFrames: function () {
        return [Ext.create('B4.view.wizard.export.contractData.CharterDataParametersStepFrame', { wizard: this })];
    }
});