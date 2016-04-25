Ext.define('B4.model.integrations.houseManagement.Contract', {
    extend: 'B4.base.Model',
    idProperty: 'Id',
    proxy: {
        type: 'b4proxy',
        controllerName: 'ManOrgContractOwners',
        listAction: 'ContractsByCurrentContragent'
    },
    fields: [
        { name: 'Id', useNull: true },
        { name: 'DocNum' },
        { name: 'SigningDate' },
        { name: 'Organization' },
        { name: 'Address' }
    ]
});