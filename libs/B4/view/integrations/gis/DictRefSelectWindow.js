Ext.define('B4.view.integrations.gis.DictRefSelectWindow', {
    extend: 'Ext.window.Window',

    alias: 'widget.gisdictrefselectwindow',

    requires: [
        'B4.ux.button.Save',
        'B4.ux.button.Close',
        'B4.ux.grid.Panel',
        'B4.ux.grid.toolbar.Paging',
        'B4.ux.grid.plugin.HeaderFilters',
        'B4.store.integrations.gis.DictRecordList'
    ],

    closeAction: 'destroy',
    height: 400,
    width: 600,
    layout: 'fit',
    mixins: ['B4.mixins.window.ModalMask'],
    maximizable: true,
    trackResetOnLoad: true,
    title: 'Выбор записи из ГИС',

    initComponent: function () {
        var me = this,
            store = Ext.create('B4.store.integrations.gis.DictRecordList');

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'panel',
                    border: false,
                    layout: {
                        type: 'hbox',
                        align: 'stretch'
                    },
                    items: [
                        {
                            xtype: 'b4grid',
                            flex: 1,
                            allowDeselect: true,
                            rowLines: false,
                            title: '',
                            style: 'border-right: solid 1px #99bce8;',
                            border: false,
                            // необходимо для того чтобы неработали восстановления для грида посколкьу колонки показываются и скрываются динамически
                            provideStateId: Ext.emptyFn,
                            stateful: false,
                            store: store,
                            selModel: Ext.create('Ext.selection.CheckboxModel', {
                                mode: 'SINGLE',
                                ignoreRightMouseSelection: true
                            }),
                            columns: [
                                {
                                    text: 'Id записи ГИС',
                                    width: 100,
                                    dataIndex: 'Id',
                                    filter: {
                                        xtype: 'numberfield',
                                        hideTrigger: true,
                                        minValue: 0,
                                        operand: CondExpr.operands.eq
                                    }
                                },
                                {
                                    text: 'Наименование',
                                    flex: 1,
                                    dataIndex: 'Name',
                                    filter: {
                                        xtype: 'textfield'
                                    }
                                }
                            ],
                            plugins: [Ext.create('B4.ux.grid.plugin.HeaderFilters')],
                            viewConfig: {
                                loadMask: true
                            },
                            dockedItems: [
                                {
                                    xtype: 'b4pagingtoolbar',
                                    displayInfo: true,
                                    store: store,
                                    dock: 'bottom'
                                }
                            ]
                        }
                    ]
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
                                    xtype: 'b4savebutton',
                                    text: 'Применить'
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