Ext.define('B4.store.integrations.gis.Log', {
    extend: 'B4.base.Store',
    requires: ['B4.model.integrations.gis.Log'],
    autoLoad: false,
    storeId: 'gisLogStore',
    model: 'B4.model.integrations.gis.Log'
});