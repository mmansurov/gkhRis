Ext.define('B4.view.wizard.export.dataprovider.ExportDataProviderWizard', {
    extend: 'B4.view.wizard.export.ExportWizard',

    getStepsFrames: function () {
        var me = this,
            result = [];

        result.push(
            Ext.create('B4.view.wizard.export.ValidationResultStepFrame', { wizard: me }),
            Ext.create('B4.view.wizard.export.dataprovider.ExportDataProviderXmlPreviewStepFrame', { wizard: me }));

        return result;
    },

    getStartStepCfg: function () {
        return {
            description: 'Вас приветствует мастер экспорта.' + '<br><br>' + this.methodName + '<br><br>' + this.methodDescription,
            doForward: function () {
                var me = this;

                me.wizard.mask();
                B4.Ajax.request({
                    url: B4.Url.action('PrepareDataForExport', 'GisIntegration'),
                    params: {
                        exporter_Id: me.wizard.exporter_Id
                    },
                    timeout: 9999999
                }).next(function (response) {
                    var json = Ext.JSON.decode(response.responseText),
                        validateResults = json.data.ValidateResult,
                        packages = json.data.Packages,
                        hasValidateResults = validateResults && validateResults.length !== 0,
                        hasPackages = packages && packages.length !== 0;

                    me.wizard.validateResults = validateResults;
                    me.wizard.allCreatedPackages = me.wizard.allCreatedPackages.concat(Ext.Array.pluck(packages, 'Id'));
                    me.wizard.packages = packages;

                    if (hasValidateResults) {
                        me.wizard.setCurrentStep('validationResult');
                    } else if (hasPackages) {
                        me.wizard.setCurrentStep('xmlPreview');
                    } else {
                        me.wizard.result = {
                            success: false,
                            message: 'Нет данных для экспорта'
                        };
                        me.wizard.setCurrentStep('finish');
                    }

                    me.wizard.unmask();

                }, me).error(function (e) {
                    me.wizard.result = {
                        success: false,
                        message: e.message || 'Не удалось выполнить экспорт'
                    }
                    me.wizard.setCurrentStep('finish');
                    me.wizard.unmask();
                }, me);

                return;
            }
        }
    }
});