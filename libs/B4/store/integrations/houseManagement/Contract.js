Ext.define('B4.store.integrations.houseManagement.Contract', {
    extend: 'B4.base.Store',
    requires: ['B4.model.integrations.houseManagement.Contract'],
    autoLoad: false,
    storeId: 'contractStore',
    model: 'B4.model.integrations.houseManagement.Contract'
});