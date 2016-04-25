Ext.define('B4.store.integrations.gis.DictList', {
    extend: 'B4.base.Store',
    fields: ['Id', 'Name'],
    autoLoad: false,
    proxy: {
        timeout: 9999999,
        type: 'b4proxy',
        controllerName: 'GisIntegration',
        listAction: 'GetDictList'
    }
});