Ext.define('B4.view.wizard.WizardFinishStepFrame', {
    extend: 'B4.view.wizard.WizardBaseStepFrame',
    stepId: 'finish',
    title: 'Результаты работы мастера',
    description: 'Работа мастера была завершена.',

    initComponent: function () {
        var me = this,
            config = {
                layout: 'border',
                defaults: {
                    bodyStyle: 'padding:15px',
                    margins: '5 5 5 5'
                },
                items: [
                    {
                        ref: '../finishStepImage',
                        region: 'west',
                        width: 150,
                        baseCls: 'icon_apply'
                    },
                    {
                        region: 'center',
                        html: me.description,
                        ref: '../finishDescription'
                    }
                ]
            };

        Ext.applyIf(me, Ext.apply(me.initialConfig, config));

        me.callParent(arguments);
    },

    allowBackward: function () {
        return false;
    },

    allowForward: function () {
        return false;
    }
});