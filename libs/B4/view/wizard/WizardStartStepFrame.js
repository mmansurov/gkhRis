Ext.define('B4.view.wizard.WizardStartStepFrame', {
    extend: 'B4.view.wizard.WizardBaseStepFrame',
    stepId: 'start',
    title: 'Начало работы с мастером',
    description: 'Вас приветствует мастер.',

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
                        region: 'west',
                        width: 150,
                        baseCls: 'icon_wizard'
                    },
                    {
                        ref: '../startStepDescription',
                        region: 'center',
                        html: me.description
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
        return true;
    },

    doForward: function () {
        this.wizard.setCurrentStep('pageParameters');
    }
});