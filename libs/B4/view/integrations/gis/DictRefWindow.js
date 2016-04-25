Ext.define('B4.view.integrations.gis.DictRefWindow', {
    extend: 'B4.form.Window',

    alias: 'widget.gisdictrefwindow',

    mixins: [
        'B4.mixins.window.ModalMask'
    ],
    layout: {
        type: 'hbox',
        align: 'stretch',
        pack: 'start'
    },
    width: 800,
    height: 450,
    maxWidth: 900,
    minWidth: 500,
    minHeight: 500,
    autoScroll: true,
    bodyPadding: 5,
    title: 'Сопоставление записей БАРС-ЖКХ и ГИС',
    closeAction: 'destroy',
    trackResetOnLoad: true,
    requires: [
        'B4.form.SelectField',
        'B4.view.integrations.gis.DictRefGrid',
        'B4.ux.button.Close',
        'B4.ux.button.Save'
    ],

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'gisdictrefgrid',
                    flex: 1
                }
            ],
            dockedItems: [

            ]
        });

        me.callParent(arguments);
    }
});