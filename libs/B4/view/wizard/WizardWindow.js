Ext.define('B4.view.wizard.WizardWindow', {
    extend: 'Ext.window.Window',
    requires: [
        'B4.view.wizard.WizardBaseStepFrame',
        'B4.view.wizard.WizardStartStepFrame',
        'B4.view.wizard.WizardFinishStepFrame'
    ],
    iconCls:'icon_wand',
    modal:true,
    closable: false,
    width:700,
    height:400,
    minWidth:300,
    minHeight: 200,
    bodyMask: null,
    layout:'card',
    defaults:{
        border:false,
        hideMode:'offsets'
    },
    title: 'Мастер выполнения операций',

    // virtual. Получить конфигурацию шагов
    getStepsFrames:[],

    initComponent: function () {
        var me = this;
        me.buildActions();

        me.addEvents('wizardComplete');

        var config = {
            items: me.buildStepsFrames()
        };

        me.buttons = [
            me.actPrev,
            me.actNext,
            '-',
            me.actClose
        ];

        Ext.applyIf(me, Ext.applyIf(me.initialConfig, config));

        // Установка стартовой страницы
        me.on('afterrender',
            function () {
                me.setCurrentStep('start');
            },
            me);

        me.callParent(arguments);
    },

    fireWizardComplete: function(){
        this.fireEvent('wizardComplete');
    },

    // private. Получить номер страницы шага по id
    findStepFrameIndexById:function (stepId) {
        var i = -1;

        Ext.each(this.stepsFrames,
            function (step, idx) {
                if (step.stepId === stepId) {
                    i = idx;
                }
            }
        );

        return i;
    },

    // private. Перейти на шаг с указанным id
    setCurrentStep: function (stepId) {
        var me = this,
            idx = me.findStepFrameIndexById(stepId);

        if (idx >= 0) {
            me.currentStep = me.stepsFrames[idx];

            if (me.currentStep.init() !== false) {
                me.actPrev.setDisabled(!me.currentStep.allowBackward());
                me.actNext.setDisabled(!me.currentStep.allowForward());

                me.getLayout().setActiveItem(idx);
            }
        }
    },

    buildActions: function () {
        var me = this;
        me.actClose = new Ext.Action({
            itemId: 'wizardCloseBtn',
            text: 'Закрыть',
            iconCls:'icon_cross',
            tooltip: 'Закрыть форму',
            iconAlign:'left',
            handler: me.doClose,
            scope: me
        });

        me.actPrev = new Ext.Action({
            text: 'Назад',
            iconCls:'icon_arrow-180',
            tooltip: 'Назад',
            iconAlign:'left',
            disabled:true,
            handler: function () {
                me.currentStep.doBackward();
            },
            scope: me
        });

        me.actNext = new Ext.Action({
            text: 'Далее',
            iconCls:'icon_arrow',
            tooltip: 'Далее',
            iconAlign:'left',
            disabled:true,
            handler: function () {
                me.currentStep.doForward();
            },
            scope: me
        });
    },

    // Создание страниц мастера
    buildStepsFrames: function () {
        var me = this;
        var addWizardRef = Ext.bind(function (stepCfg) {
            return Ext.apply(stepCfg, { wizard: me });
        }, me);

        me.stepsFrames = [Ext.create('B4.view.wizard.WizardStartStepFrame', addWizardRef(me.startStep))];

        Ext.each(me.getStepsFrames(), function (step) {
            me.stepsFrames.push(step);
        });

        me.stepsFrames.push(Ext.create('B4.view.wizard.WizardFinishStepFrame', addWizardRef(me.finishStep)));

        // Вешаем обработчик selectionchange
        Ext.each(me.stepsFrames,
            function (stepObj) {
                stepObj.on('selectionchange',
                    function (step) {
                        me.actPrev.setDisabled(!step.allowBackward());
                        me.actNext.setDisabled(!step.allowForward());
                    },
                    me);
            },
            me);

        return me.stepsFrames;
    },

    mask: function (msg) {
        var me = this;
        if (Ext.isEmpty(msg)) {
            msg = 'Пожалуйста, подождите';
        }

        if (!Ext.isObject(msg)) {
            msg = { msg: msg };
        }

        me.unmask();

        me.bodyMask = Ext.create('Ext.LoadMask', me, msg);
        me.bodyMask.show();

        return me.bodyMask;
    },

    unmask: function () {
        var me = this;
        if (!Ext.isEmpty(me.bodyMask)) {
            try {
                me.bodyMask.hide();
                me.bodyMask.destroy();
                me.bodyMask = null;
            } catch (e) {

            }
        }
    }
});