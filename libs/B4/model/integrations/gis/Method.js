Ext.define('B4.model.integrations.gis.Method', {
    extend: 'B4.base.Model',
    idProperty: 'Id',
    proxy: {
        type: 'b4proxy',
        controllerName: 'GisIntegration',
        listAction: 'MethodList'
    },
    fields: [
        { name: 'Id', useNull: true },
        { name: 'Order' },
        { name: 'Name' },
        { name: 'Description' },
        { name: 'Type' },
        { name: 'NeedSign' }
    ]
});