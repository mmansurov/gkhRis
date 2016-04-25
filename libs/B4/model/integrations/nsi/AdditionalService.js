Ext.define('B4.model.integrations.nsi.AdditionalService', {
    extend: 'B4.base.Model',
    idProperty: 'Id',
    proxy: {
        type: 'b4proxy',
        controllerName: 'Nsi',
        listAction: 'GetAdditionalServices'
    },
    fields: [
        { name: 'Id', useNull: true },
        { name: 'Name' },
        { name: 'UnitMeasure' }
    ]
});