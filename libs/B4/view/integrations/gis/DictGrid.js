Ext.define('B4.view.integrations.gis.DictGrid', {
    extend: 'B4.ux.grid.Panel',

    alias: 'widget.gisdictgrid',

    requires: [
        'B4.form.ComboBox',
        'B4.ux.button.Add',
        'B4.ux.button.Save',
        'B4.ux.button.Update',
        'B4.ux.grid.column.Edit',
        'B4.ux.grid.column.Delete',
        'B4.ux.grid.plugin.HeaderFilters',
        'B4.ux.grid.toolbar.Paging'
    ],

    closable: false,

    initComponent: function () {
        var me = this,
            store = Ext.create('B4.store.integrations.gis.Dict');

        Ext.applyIf(me, {
            store: store,
            columnLines: true,
            columns: [
                {
                    xtype: 'b4editcolumn',
                    scope: me
                },
                {
                    xtype: 'actioncolumn',
                    align: 'center',
                    width: 20,
                    icon: B4.Url.content('content/img/icons/page_copy.png'),
                    handler: function (gridView, rowIndex, colIndex, el, e, rec) {
                        var me = this;
                        var scope = me.origScope;
                        scope.fireEvent('rowaction', scope, 'refrecord', rec);
                    },
                    scope: me
                },
                {
                    text: 'Номер справочника',
                    minWidth: 100,
                    maxWidth: 120,
                    dataIndex: 'NsiRegistryNumber',
                    filter: {
                        xtype: 'textfield'
                    }
                },
                {
                    text: 'Внешний справочник',
                    flex: 1,
                    dataIndex: 'Name',
                    filter: {
                        xtype: 'textfield'
                    }
                },
                {
                    text: 'Сопоставляемые данные',
                    flex: 1,
                    dataIndex: 'ActionCode',
                    filter: {
                        xtype: 'textfield'
                    }
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'CountRefRecords',
                    flex: 1,
                    text: 'Кол-во записей',
                    filter: {
                        xtype: 'numberfield',
                        hideTrigger: true,
                        minValue: 0,
                        operand: CondExpr.operands.eq,
                        allowDecimals: false
                    }
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'CountNotRefRecords',
                    flex: 1,
                    text: 'Кол-во несопоставленных',
                    filter: {
                        xtype: 'numberfield',
                        hideTrigger: true,
                        minValue: 0,
                        operand: CondExpr.operands.eq,
                        allowDecimals: false
                    }
                },
                {
                    xtype: 'b4deletecolumn',
                    scope: me
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
                                { xtype: 'b4addbutton' }
                            ]
                        },
                        {
                            xtype: 'buttongroup',
                            items: [
                                { xtype: 'b4updatebutton' }
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