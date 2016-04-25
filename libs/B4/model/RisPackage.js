Ext.define('B4.model.RisPackage', {
    extend: 'B4.base.Model',
    fields: [
        { name: 'Id', useNull: true },
        { name: 'Name' },
        { name: 'NotSignedData' },
        { name: 'SignedData' }
    ]
});