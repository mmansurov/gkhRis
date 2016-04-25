Ext.define('B4.model.integrations.gis.DictRef', {
    extend: 'B4.base.Model',
    idProperty: 'Id',
    proxy: {
        type: 'b4proxy',
        controllerName: 'GisDictRef'
    },
    fields: [
        { name: 'Id', useNull: true },
        { name: 'ClassName' },
        { name: 'GkhId' },
        { name: 'GkhName' },
        { name: 'GisId' },
        { name: 'GisName' },
        { name: 'GisGuid' },
        { name: 'Dict' }
    ]
});