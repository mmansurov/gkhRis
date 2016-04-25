Ext.define('B4.store.integrations.gis.DictRecordList', {
    extend: 'B4.base.Store',
    fields: ['Id', 'Name', 'Guid'],
    autoLoad: false,
    proxy: {
        timeout: 9999999,
        type: 'b4proxy',
        controllerName: 'GisIntegration',
        listAction: 'GetDictRecordList'
    }
});