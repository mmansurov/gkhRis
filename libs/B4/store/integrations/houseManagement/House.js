Ext.define('B4.store.integrations.houseManagement.House', {
    extend: 'B4.base.Store',
    requires: ['B4.model.integrations.houseManagement.House'],
    autoLoad: false,
    storeId: 'houseStore',
    model: 'B4.model.integrations.houseManagement.House'
});
