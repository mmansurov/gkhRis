Ext.define('B4.view.wizard.export.additionalservices.AdditionalServicesParametersStepFrame', {
    extend: 'B4.view.wizard.export.ParametersStepFrame',
    requires: [
        'B4.form.SelectField',
        'B4.ux.grid.plugin.HeaderFilters'
    ],
    layout: 'hbox',
    mixins: ['B4.mixins.window.ModalMask'],
    items: [
    {
        xtype: 'b4selectfield',
        name: 'AdditionalServices',
        flex: 1,
        padding: '10 5 0 10',
        labelWidth: 130,
        labelAlign: 'right',
        textProperty: 'Name',
        selectionMode: 'MULTI',
        fieldLabel: 'Дополнительные услуги',
        store: 'B4.store.integrations.nsi.AdditionalService',
        model: 'B4.model.integrations.nsi.AdditionalService',
        columns: [
            {
                text: 'Наименование',
                dataIndex: 'Name',
                flex: 1,
                filter: { xtype: 'textfield' }
            },
            {
                text: 'Ед. измерения',
                dataIndex: 'UnitMeasure',
                flex: 1,
                filter: { xtype: 'textfield' }
            }
        ]
    }],

    firstInit: function () {
        var me = this;
        me.wizard.down('b4selectfield[name=AdditionalServices]').on('change', function () {
            me.fireEvent('selectionchange', me);
        }, me);
    },

    getParams: function () {
        var me = this;
        return {
            selectedList: me.wizard.down('b4selectfield[name=AdditionalServices]').getValue()
        };
    }
});