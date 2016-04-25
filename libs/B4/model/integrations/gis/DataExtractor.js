Ext.define('B4.model.integrations.gis.DataExtractor', {
    extend: 'B4.base.Model',
    idProperty: 'Id',
    proxy: {
        type: 'b4proxy',
        controllerName: 'GisIntegration',
        listAction: 'DataExtractorList'
    },
    fields: [
        { name: 'Id', useNull: true },
        { name: 'Code' }
    ]
});