Ext.define('B4.model.integrations.houseManagement.House', {
    extend: 'B4.base.Model',
    idProperty: 'Id',
    proxy: {
        type: 'b4proxy',
        controllerName: 'HouseManagement',
        listAction: 'GetHouseList'
    },
    fields: [
        { name: 'Id'},
        { name: 'Address' },
        { name: 'HouseType' }
    ]
});
