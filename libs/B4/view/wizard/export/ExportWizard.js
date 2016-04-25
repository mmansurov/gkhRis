Ext.define('B4.view.wizard.export.ExportWizard', {
    extend: 'B4.view.wizard.WizardWindow',
    title: 'Мастер экспорта',
    exporter_Id: undefined,
    methodName: undefined,
    methodDescription: undefined,
    certificate: undefined,
    onlyMethod: false,
    needSign: true,

    //массив для хранения актуальных пакетов - полученных при последней подготовке данных (в т.ч. создании пакетов)
    packages: [],

    //массив для хранения идентификаторов всех созданных в текущем визарде пакетов
    //с учетом возможных изменений входных параметров и повторных подготовок данных (в т.ч. созданий пакетов)
    allCreatedPackages: [],

    //массив для хранения актуальных результатов валидации - полученных при последней подготовке данных (в т.ч. валидации)
    validateResults: [],

    result: undefined,
    signer: undefined,
    requires: [
        'B4.enums.ObjectValidateState',
        'B4.form.ComboBox',
        'B4.model.RisPackage',
        'B4.store.RisPackage',
        'B4.ux.button.Update',
        'B4.ux.grid.Panel',
        'B4.ux.grid.plugin.HeaderFilters',
        'B4.ux.grid.toolbar.Paging',
        'B4.view.wizard.export.NotSignedXmlPreviewWin',
        'B4.view.wizard.export.SignedXmlPreviewWin',
        'B4.view.wizard.export.ParametersStepFrame',
        'B4.view.wizard.export.ValidationResultStepFrame',
        'B4.view.wizard.export.XmlPreviewStepFrame'
    ],

    initComponent: function () {
        var me = this;
        me.signer = new XadesSigner();
        me.startStep = me.getStartStepCfg();
        me.finishStep = me.getFinishStepCfg();

        me.on('close', function (panel, eOpts) {
            var me = this;

            if (me.allCreatedPackages && me.allCreatedPackages.length > 0) {
                B4.Ajax.request({
                    url: B4.Url.action('DeleteTempObjects', 'GisIntegration'),
                    params: {
                        package_Ids: me.allCreatedPackages
                    },
                    timeout: 9999999
                }).next(function (response) {

                }, me).error(function (e) {

                }, me);
            }
        }, me);

        me.callParent(arguments);
    },

    getStartStepCfg: function () {
        return {
            description: 'Вас приветствует мастер экспорта.' + '<br><br>' + this.methodName + '<br><br>' + this.methodDescription
        }
    },

    getFinishStepCfg: function () {
        return {
            layout: 'border',
            defaults: {
                bodyStyle: 'padding:15px',
                margins: '5 5 5 5'
            },
            items: [
                {
                    itemId: 'finishStepImage',
                    region: 'west',
                    width: 150,
                    baseCls: 'icon_apply'
                },
                {
                    region: 'center',
                    layout: 'border',
                    defaults: {
                        bodyStyle: 'padding:15px',
                        margins: '5 5 5 5'
                    },
                    items: [
                        {
                            region: 'center',
                            html: this.description,
                            itemId: 'finishDescription'
                        },
                        {
                            region: 'south',
                            xtype: 'checkbox',
                            height: 20,
                            boxLabel: 'Открыть список запланированных задач',
                            itemId: 'openScheduledTasks'
                        }
                    ]

                }
            ],
            init: function () {
                var me = this;
                me.description = 'Работа мастера была завершена.' + '<br><br>';

                var openScheduledTasks = false;

                var finishStepImageEl = me.wizard.down('#finishStepImage');
                var finishDescriptionEl = me.wizard.down('#finishDescription');
                var openScheduledTasksEl = me.wizard.down('#openScheduledTasks');

                if (me.wizard.result) {

                    me.description += me.wizard.result.message;

                    if (me.wizard.result.state === 'success') {
                        finishStepImageEl.removeCls('icon_error');
                        finishStepImageEl.removeCls('icon_warning');
                        finishStepImageEl.addCls('icon_apply');

                        openScheduledTasks = true;
                    }
                    else if (me.wizard.result.state === 'warning') {
                        finishStepImageEl.removeCls('icon_apply');
                        finishStepImageEl.removeCls('icon_error');
                        finishStepImageEl.addCls('icon_warning');

                        openScheduledTasks = true;
                    }
                    else {
                        finishStepImageEl.removeCls('icon_apply');
                        finishStepImageEl.removeCls('icon_warning');
                        finishStepImageEl.addCls('icon_error');
                    }

                    finishDescriptionEl.update(me.description);
                }

                openScheduledTasksEl.setValue(openScheduledTasks);
                openScheduledTasksEl.setDisabled(!openScheduledTasks);

                return true;
            }
        }
    },

    getStepsFrames: function () {
        var me = this;

        var result = me.getParametersStepFrames();

        result.push(
            Ext.create('B4.view.wizard.export.ValidationResultStepFrame', { wizard: me }),
            Ext.create('B4.view.wizard.export.XmlPreviewStepFrame', { wizard: me }));

        return result;
    },

    getParametersStepFrames: function () {
        return [Ext.create('B4.view.wizard.export.ParametersStepFrame', { wizard: this })];
    }
});