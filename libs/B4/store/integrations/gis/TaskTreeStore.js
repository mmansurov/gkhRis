Ext.define('B4.store.integrations.gis.TaskTreeStore', {
    extend: 'Ext.data.TreeStore',
    requires: ['B4.model.TaskTreeNode'],
    model: 'B4.model.TaskTreeNode',
    root: {
        children: [],
        expanded: true,
        loaded: true
    },
    autoLoad: false
});
