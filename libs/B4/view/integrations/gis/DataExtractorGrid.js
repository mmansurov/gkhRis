Ext.define('B4.view.integrations.gis.DataExtractorGrid', {
    extend: 'B4.ux.grid.Panel',

    alias: 'widget.gisdataextractorgrid',

    requires: [
        'B4.form.ComboBox',
        'B4.ux.button.Update',
        'B4.ux.grid.plugin.HeaderFilters',
        'B4.ux.grid.toolbar.Paging'
    ],

    title: 'Извлечение данных',
    closable: false,

    initComponent: function () {
        var me = this,
            store = Ext.create('B4.store.integrations.gis.DataExtractor');

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
                    text: 'Код',
                    flex: 1,
                    dataIndex: 'Code',
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
                                    name: 'ExtractData',
                                    text: 'Выполнить',
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