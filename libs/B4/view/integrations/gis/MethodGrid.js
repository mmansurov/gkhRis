Ext.define('B4.view.integrations.gis.MethodGrid', {
    extend: 'B4.ux.grid.Panel',

    alias: 'widget.gismethodgrid',

    requires: [
        'B4.form.ComboBox',
        'B4.ux.button.Update',
        'B4.ux.grid.plugin.HeaderFilters',
        'B4.ux.grid.toolbar.Paging'
    ],

    closable: false,
    enableColumnHide: true,

    initComponent: function () {
        var me = this,
            store = Ext.create('B4.store.integrations.gis.Method');

        Ext.applyIf(me, {
            store: store,
            selModel: Ext.create('Ext.selection.CheckboxModel', {
                mode: 'SINGLE',
                allowDeselect: true,
                showHeaderCheckbox: false,
                ignoreRightMouseSelection: true
            }),
            columns: [
                {
                    dataIndex: 'Order',
                    text: 'Порядок выполнения',
                    hidden: true,
                    filter: {
                        xtype: 'numberfield',
                        hideTrigger: true,
                        operand: CondExpr.operands.eq
                    }
                },
                {
                    text: 'Наименование метода',
                    flex: 1,
                    sortable: false,
                    dataIndex: 'Name',
                    filter: {
                        xtype: 'textfield'
                    }
                },
                {
                    text: 'Описание метода',
                    flex: 2,
                    sortable: false,
                    dataIndex: 'Description',
                    filter: {
                        xtype: 'textfield'
                    }
                }
            ],
            plugins: [
                Ext.create('B4.ux.grid.plugin.HeaderFilters')
            ],
            viewConfig: {
                loadMask: true
            },
            dockedItems: [
                {
                    xtype: 'toolbar',
                    dock: 'top',
                    items: [
                        {
                            xtype: 'buttongroup',
                            items: [
                                {
                                    xtype: 'b4updatebutton'
                                }
                            ]
                        },
                        {
                            xtype: 'buttongroup',
                            items: [
                                {
                                    xtype: 'button',
                                    name: 'ExecuteMethod',
                                    text: 'Выполнить метод',
                                    textAlign: 'left'
                                }
                            ]
                        }
                    ]
                },
                {
                    xtype: 'b4pagingtoolbar',
                    displayInfo: true,
                    store: store,
                    dock: 'bottom'
                }
            ]
        });

        me.callParent(arguments);
    }
});