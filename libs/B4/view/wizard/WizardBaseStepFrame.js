Ext.define('B4.view.wizard.WizardBaseStepFrame', {
    extend: 'Ext.Panel',
    bubbleEvents: ['selectionchange'],

    initComponent: function () {
        var me = this;
        // Установка обработчика рендера
        me.on('afterrender',
            function () {
                if (me.firstInit) {
                    me.firstInit();
                }
            },
            me);
        me.addEvents('selectionchange');
        me.callParent(arguments);
    },

    // virtual. Инициализация страницы (при первом открытии)
    firstInit: function () { return true; },

    // virtual. Инициализация страницы (каждый раз при открытии страницы)
    init: function () { return true; },

    // virtual. Проверить возможноть шага в направлении 'назад'
    allowBackward: function () { return true; },

    // virtual. Проверить возможноть шага в направлении 'вперед'
    allowForward: function () { return true; },

    // virtual. Выполнить шаг в направлении 'назад'
    doBackward: function() { return; },

    // virtual. Выполнить шаг в направлении 'вперед'
    doForward: function() { return; }
});