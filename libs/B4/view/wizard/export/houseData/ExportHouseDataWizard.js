Ext.define('B4.view.wizard.export.houseData.ExportHouseDataWizard', {
    extend: 'B4.view.wizard.export.ExportWizard',

    getParametersStepFrames: function () {
        return [
            Ext.create('B4.view.wizard.export.houseData.HouseDataParametersStepFrame', { wizard: this })
        ];
    }
});
