Ext.define('B4.view.wizard.export.dataprovider.ExportDataProviderXmlPreviewStepFrame', {
    extend: 'B4.view.wizard.export.XmlPreviewStepFrame',

    doBackward: function () {
        this.wizard.setCurrentStep('start');
    }
});