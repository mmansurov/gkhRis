Ext.define('B4.model.TaskTreeNode', {
    extend: 'Ext.data.Model',
    idgen: {
        type: 'sequential',
        prefix: 'ID_'
    },
    fields: [
        'Id',
        'Type',
        'Name',
        'Author',
        'StartTime',
        'EndTime',
        'State',
        'Message',
        'ResultLog'
    ],
    proxy: {
        url: 'TaskTree/GetTaskTreeNodes',
        type: 'ajax',
        reader: {
            type: 'json'
        }
    }
});