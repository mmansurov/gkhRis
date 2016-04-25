Ext.define('B4.model.integrations.gis.Log', {
    extend: 'B4.base.Model',
    idProperty: 'Id',
    proxy: {
        type: 'b4proxy',
        controllerName: 'GisLog'
    },
    fields: [
        { name: 'Id', useNull: true },
        { name: 'ServiceLink' },
        { name: 'UserName' },
        { name: 'MethodName' },
        { name: 'DateStart' },
        { name: 'TimeStart' },
        { name: 'TimeWork' },
        { name: 'CountObjects' },
        { name: 'ProcessedObjects' },
        { name: 'ProcessedPercent' },
        { name: 'FileLog' }
    ]
});