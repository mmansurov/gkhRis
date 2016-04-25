Ext.define('B4.store.integrations.gis.DataExtractor', {
    extend: 'B4.base.Store',
    requires: ['B4.model.integrations.gis.DataExtractor'],
    autoLoad: false,
    storeId: 'gisDataExtractorStore',
    model: 'B4.model.integrations.gis.DataExtractor'
});