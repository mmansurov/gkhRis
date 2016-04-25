Ext.define('B4.view.integrations.gis.DictRefGrid', {
    extend: 'B4.ux.grid.Panel',

    alias: 'widget.gisdictrefgrid',

    requires: [
        'B4.ux.button.Close',
        'B4.ux.button.Save',
        'B4.ux.button.Update',
        'B4.ux.grid.plugin.HeaderFilters',
        'B4.ux.grid.toolbar.Paging'
    ],

    title: '',

    initComponent: function () {
        var me = this,
            store = Ext.create('B4.store.integrations.gis.DictRef');

        Ext.applyIf(me, {
            store: store,
            columnLines: true,
            columns: [
                {
                    xtype: 'actioncolumn',
                    align: 'center',
                    width: 25,
                    icon: B4.Url.content('content/img/icons/page_copy.png'),
                    handler: function (gridView, rowIndex, colIndex, el, e, rec) {
                        var me = this;
                        var scope = me.origScope;
                        scope.fireEvent('rowaction', scope, 'selectref', rec);
                    },
                    scope: me
                },
                {
                    text: 'Запись ЖКХ',
                    flex: 1,
                    dataIndex: 'GkhName',
                    filter: {
                        xtype: 'textfield'
                    }
                },
                {
                    text: 'Запись ГИС',
                    flex: 1,
                    dataIndex: 'GisName',
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
                                    xtype:
                                      'b4savebutton'
                                },
                                {
                                    xtype:
                                      'b4updatebutton'
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