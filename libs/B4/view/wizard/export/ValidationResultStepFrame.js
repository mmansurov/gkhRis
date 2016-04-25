Ext.define('B4.view.wizard.export.ValidationResultStepFrame', {
    extend: 'B4.view.wizard.WizardBaseStepFrame',
    wizard: undefined,
    stepId: 'validationResult',
    title: 'Результаты валидации',
    layout: 'fit',
    requires: [
        'B4.enums.ObjectValidateState',
        'B4.ux.grid.column.Enum'
    ],
    items: [{
        xtype: 'b4grid',
        store: Ext.create('B4.store.ExporterValidateResult'),
        flex: 1,
        name: 'ValidateResultGrid',
        columnLines: true,
        cls: 'x-large-head',
        columns: [
            {
                xtype: 'gridcolumn',
                flex: 1,
                align: 'center',
                text: 'Идентификатор',
                dataIndex: 'Id'
            },
            {
                xtype: 'gridcolumn',
                flex: 3,
                align: 'center',
                text: 'Объект',
                dataIndex: 'Description'
            },
            {
                xtype: 'b4enumcolumn',
                enumName: 'B4.enums.ObjectValidateState',
                flex: 3,
                align: 'center',
                text: 'Статус',
                dataIndex: 'State'
            },
            {
                xtype: 'gridcolumn',
                flex: 7,
                align: 'center',
                text: 'Сообщение',
                dataIndex: 'Message'
            }
        ]
    }],

    init: function() {
        var me = this,
           validateResults = me.wizard.validateResults,
           hasValidateResults = validateResults && validateResults.length !== 0,
           validateResultGrid = me.down('b4grid[name=ValidateResultGrid]'),
           validateResultGridStore = validateResultGrid.getStore();

        validateResultGridStore.removeAll();

        if (hasValidateResults) {
            Ext.each(validateResults, function(validateResult) {
                var validateResultRec = Ext.create('B4.model.ExporterValidateResult', {
                    Id: validateResult.Id,
                    Description: validateResult.Description,
                    State: validateResult.State,
                    Message: validateResult.Message
                });
                validateResultGridStore.add(validateResultRec);
            });
        }
    },

    doBackward: function () {
        this.wizard.setCurrentStep('pageParameters');
    },

    doForward: function () {
        var me = this;
        if (me.wizard.packages && me.wizard.packages.length > 0) {
            me.wizard.setCurrentStep('xmlPreview');
        } else {
            me.wizard.result = {
                success: false,
                message: 'Нет данных для экспорта'
            };
            me.wizard.setCurrentStep('finish');
        }
    }
});