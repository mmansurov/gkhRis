Ext.define('B4.view.wizard.export.ParametersStepFrame', {
    extend: 'B4.view.wizard.WizardBaseStepFrame',
    stepId: 'pageParameters',
    wizard: undefined,
    title: 'Параметры экспорта',
    layout: 'vbox',
    items: [{
        xtype: 'checkbox',
        itemId: 'onlyMethodCheckbox',
        boxLabel: 'Выполнить метод без извлечения данных',
        padding: '15 5 15 5'
    }],
    firstInit: function () {
        var me = this;
        me.wizard.down('#onlyMethodCheckbox').on('change', function (field, newValue, oldValue, eOpts) {
            me.wizard.onlyMethod = newValue;
            me.fireEvent('selectionchange', me);
        }, me);
    },

    allowBackward: function () {
        return true;
    },

    allowForward: function () {
        return true;
    },

    doBackward: function () {
        this.wizard.setCurrentStep('start');
    },
   
    getPrepareDataParams: function() {
        var me = this,
            result = {
            exporter_Id: me.wizard.exporter_Id
        };

        Ext.apply(result, me.getParams());

        return result;
    },

    //virtual
    getParams: function () {
        var me = this;
        return {
            onlyMethod: me.wizard.onlyMethod
        };
    },

    doForward: function () {
        var me = this;

        me.wizard.mask();
        B4.Ajax.request({
            url: B4.Url.action('PrepareDataForExport', 'GisIntegration'),
            params: me.getPrepareDataParams(),
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
});