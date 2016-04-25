Ext.define('B4.view.integrations.gis.Panel', {
    extend: 'Ext.panel.Panel',

    closable: true,
    alias: 'widget.gisintegrationpanel',
    title: 'Интеграция с ГИС ЖКХ',
    requires: [
        'B4.view.integrations.gis.DataExtractorGrid',
        'B4.view.integrations.gis.MethodGrid',
        'B4.view.integrations.gis.DictGrid',
        'B4.view.integrations.gis.LogGrid',
        'B4.store.integrations.gis.DataExtractor',
        'B4.store.integrations.gis.Method',
        'B4.store.integrations.gis.Dict',
        'B4.store.integrations.gis.Log',
        'B4.view.integrations.gis.TaskTree'
    ],

    bodyStyle: B4.getBgStyle(),
    layout: {
        type: 'vbox',
        align: 'stretch'
    },

    items: [
        {
            xtype: 'tabpanel',
            flex: 1,
            layout: {
                align: 'stretch'
            },
            enableTabScroll: true,
            defaults:
            {
                layout: {
                    type: 'vbox',
                    align: 'stretch'
                },
                flex: 1,
                margins: -1,
                border: false
            },
            items: [
                {
                    xtype: 'panel',
                    title: 'Методы',
                    name: 'methodpanel',
                    items: [
                        {
                            xtype: 'gismethodgrid',
                            flex: 1
                        }
                    ]
                },
                {
                    xtype: 'panel',
                    name: 'dictpanel',
                    title: 'Справочники',
                    items: [
                        {
                            xtype: 'gisdictgrid',
                            flex: 1
                        }
                    ]
                },
                {
                    xtype: 'panel',
                    name: 'logpanel',
                    title: 'Логи интеграции',
                    items: [
                        {
                            xtype: 'gisloggrid',
                            flex: 1
                        }
                    ]
                },
                {
                    xtype: 'panel',
                    name: 'taskpanel',
                    title: 'Задачи',
                    items: [
                        {
                            xtype: 'gistasktree',
                            flex: 1
                        }
                    ]
                }
            ]
        }
    ]
});