Ext.define('B4.model.ExporterValidateResult', {
    extend: 'B4.base.Model',
    fields: [
        { name: 'Id', useNull: true },
        { name: 'Description' },
        { name: 'State' },
        { name: 'Message' }
    ]
});