Ext.define('B4.view.integrations.gis.LogGrid', {
    extend: 'B4.ux.grid.Panel',

    alias: 'widget.gisloggrid',

    requires: [
        'B4.form.ComboBox',
        'B4.ux.button.Update',
        'B4.ux.grid.plugin.HeaderFilters',
        'B4.ux.grid.toolbar.Paging'
    ],

    closable: false,

    initComponent: function () {
        var me = this,
            store = Ext.create('B4.store.integrations.gis.Log');

        Ext.applyIf(me, {
            store: store,
            columnLines: true,
            columns: [
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'ServiceLink',
                    flex: 1,
                    text: 'Адрес сервиса',
                    filter: {
                        xtype: 'textfield'
                    }
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'UserName',
                    flex: 1,
                    text: 'Пользователь',
                    filter: {
                        xtype: 'textfield'
                    }
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'MethodName',
                    flex: 1,
                    text: 'Наименование метода',
                    filter: {
                        xtype: 'textfield'
                    }
                },
                {
                    xtype: 'datecolumn',
                    format: 'd.m.Y',
                    dataIndex: 'DateStart',
                    text: 'Дата начала',
                    flex: 1,
                    filter: {
                        xtype: 'datefield',
                        operand: CondExpr.operands.eq,
                        format: 'd.m.Y'
                    }
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'TimeStart',
                    flex: 1,
                    text: 'Время начала'
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'TimeWork',
                    flex: 1,
                    text: 'Время выполнения'
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'CountObjects',
                    flex: 1,
                    text: 'Плановое количество объектов',
                    filter: {
                        xtype: 'numberfield',
                        hideTrigger: true,
                        minValue: 0,
                        operand: CondExpr.operands.eq
                    }
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'ProcessedObjects',
                    flex: 1,
                    text: 'Количество выполненных объектов',
                    filter: {
                        xtype: 'numberfield',
                        hideTrigger: true,
                        minValue: 0,
                        operand: CondExpr.operands.eq
                    }
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'ProcessedPercent',
                    flex: 1,
                    text: 'Процент выполнения',
                    filter: {
                        xtype: 'numberfield',
                        hideTrigger: true,
                        minValue: 0,
                        operand: CondExpr.operands.eq
                    }
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'FileLog',
                    width: 100,
                    text: 'Файл лога',
                    renderer: function (v) {
                        return v ? ('<a href="' + B4.Url.action('/FileUpload/Download?id=' + v.Id) + '" target="_blank" style="color: black">Скачать</a>') : '';
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