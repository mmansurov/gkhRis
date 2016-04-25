Ext.define('B4.view.integrations.gis.TaskTree', {
    extend: 'Ext.tree.Panel',

    alias: 'widget.gistasktree',

    requires: [
        'B4.store.integrations.gis.TaskTreeStore'
    ],

    region: 'west',
    rootVisible: false,
    //animate: true,
    autoScroll: true,
    useArrows: true,
    loadMask: true,
    viewConfig: {
        loadMask: true
    },

    filterByStartTime: true,

    title: 'Выполнение задач',
    columns: [
        {
            xtype: 'treecolumn',
            text: 'Наименование',
            flex: 1,
            sortable: true,
            dataIndex: 'Name'
        }, {
            text: 'Автор',
            flex: 1,
            sortable: true,
            dataIndex: 'Author',
            align: 'center'
        }, {
            text: 'Время начала',
            flex: 1,
            dataIndex: 'StartTime',
            sortable: true
        }, {
            text: 'Время окончания',
            flex: 1,
            dataIndex: 'EndTime',
            sortable: true
        }, {
            text: 'Статус',
            flex: 1,
            dataIndex: 'State',
            sortable: true
        },
        {
            text: 'Сообщение',
            flex: 1,
            dataIndex: 'Message',
            sortable: true
        },
        {
            xtype: 'gridcolumn',
            dataIndex: 'ResultLog',
            width: 100,
            text: 'Результат',
            renderer: function (v) {
                return v ? ('<a href="' + B4.Url.action('/FileUpload/Download?id=' + v.Id) + '" target="_blank" style="color: black">Скачать</a>') : '';
            }
        }
    ],

    listeners: {
        beforeload: function (store, operation, eOpts) {
            var params = operation.params,
                nodeId = params.node,
                node = store.getById(nodeId),                   
                me = this;

            if (me.filterByStartTime === true) {

                var dateFromCmp = me.down('[name="DateFrom"]'),
                    timeFromCmp = me.down('[name="TimeFrom"]'),
                    dateTimeFrom = me.getDateTime(dateFromCmp, timeFromCmp),
                        
                    dateToCmp = me.down('[name="DateTo"]'),
                    timeToCmp = me.down('[name="TimeTo"]'),
                    dateTimeTo = me.getDateTime(dateToCmp, timeToCmp);
                
                Ext.apply(params, {
                    dateTimeFrom: dateTimeFrom,
                    dateTimeTo: dateTimeTo
                });
            }

            if (nodeId === 'root') {
                Ext.apply(params, {
                    nodeType: 'root',
                    nodeId: '0'
                });
            } else {
                Ext.apply(params, {
                    nodeType: node.data.Type,
                    nodeId: node.data.Id
                });
            }            
        }
    },

    initComponent: function() {
        var me = this,
        store = Ext.create('B4.store.integrations.gis.TaskTreeStore');

        Ext.applyIf(me, {
            store: store
        });

        if (me.filterByStartTime === true) {
            me.dockedItems[0].items.push({
                xtype: 'panel',
                padding: '0 0 5 0',
                bodyStyle: B4.getBgStyle(),
                border: false,
                layout: 'hbox',
                defaults: {                    
                    labelAlign: 'right',
                    labelWidth: 200,
                    flex: 1
                },
                items: [
                    {
                        xtype: 'datefield',
                        format: 'd.m.Y',
                        name: 'DateFrom',
                        fieldLabel: 'Время начала выполнения задачи с',
                        value: new Date(),
                        maxValue: new Date()
                    },
                    {
                        xtype: 'timefield',
                        name: 'TimeFrom',
                        fieldLabel: '',
                        format: 'H:i',
                        value: '00:00',
                        increment: 30
                    },
                    {
                        xtype: 'datefield',
                        format: 'd.m.Y',
                        name: 'DateTo',
                        fieldLabel: 'по',
                        labelWidth: 50,
                        value: new Date(),
                        maxValue: new Date()
                    },
                    {
                        xtype: 'timefield',
                        name: 'TimeTo',
                        fieldLabel: '',
                        format: 'H:i',
                        value: '23:30',
                        increment: 30
                    }
                ]
            });
        }

        me.callParent(arguments);
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
        }
    ],

    getDateTime: function (dateCmp, timeCmp) {

        var dateValue = dateCmp.getValue(),
            dateStr = Ext.Date.format(dateValue, 'd.m.Y'),
            timeValue = timeCmp.getValue(),
            timeStr = Ext.Date.format(timeValue, 'H:i'),
            dateTime = Ext.Date.parse(dateStr + ' ' + timeStr, 'd.m.Y H:i');

        return dateTime;
    }
});
