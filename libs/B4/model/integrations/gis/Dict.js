Ext.define('B4.model.integrations.gis.Dict', {
    extend: 'B4.base.Model',
    idProperty: 'Id',
    proxy: {
        type: 'b4proxy',
        controllerName: 'GisDict'
    },
    fields: [
        { name: 'Id', useNull: true },
        { name: 'NsiRegistryNumber' },
        { name: 'Name' },
        { name: 'ActionCode' },
        { name: 'CountRefRecords' },
        { name: 'CountNotRefRecords' },
        { name: 'DictProxy' }
    ]
});