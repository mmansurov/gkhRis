Ext.define('B4.view.integrations.gis.DictWindow', {
    extend: 'B4.form.Window',

    alias: 'widget.gisdictwindow',

    mixins: ['B4.mixins.window.ModalMask'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    width: 650,
    maximizable: true,
    resizable: true,
    title: 'Справочник ГИС',
    closeAction: 'hide',
    trackResetOnLoad: true,
    defaults: {
        labelAlign: 'right',
        labelWidth: 150
    },

    requires: [
        'B4.form.ComboBox',
        'B4.form.SelectField',
        'B4.ux.button.Close',
        'B4.ux.button.Save'
    ],

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'b4selectfield',
                    store: 'B4.store.integrations.gis.DictList',
                    isGetOnlyIdProperty: true,
                    name: 'DictProxy',
                    fieldLabel: 'Справочник',
                    padding: '10 5 0 0',
                    allowBlank: true,
                    editable: false,
                    columns: [
                        { text: 'Наименование', dataIndex: 'Name', flex: 1, filter: { xtype: 'textfield' } }
                    ],
                    textProperty: 'Name'
                },
                {
                    xtype: 'b4combobox',
                    name: 'ActionCode',
                    fieldLabel: 'Метод интеграции',
                    padding: '0 5 5 0',
                    operand: CondExpr.operands.eq,
                    storeAutoLoad: false,
                    editable: false,
                    valueField: 'Name',
                    url: '/GisIntegration/GetDictActions'
                }
            ],
            dockedItems: [
                {
                    xtype: 'toolbar',
                    dock: 'top',
                    items: [
                        {
                            xtype: 'buttongroup',
                            items: [
                                {
                                    xtype: 'b4savebutton'
                                }
                            ]
                        },
                        {
                            xtype: 'tbfill'
                        },
                        {
                            xtype: 'buttongroup',
                            items: [
                                {
                                    xtype: 'b4closebutton'
                                }
                            ]
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }
});