Ext.define('B4.view.wizard.export.houseData.HouseDataParametersStepFrame', {
    extend: 'B4.view.wizard.export.ParametersStepFrame',
    requires: [
        'B4.form.SelectField',
        'B4.store.integrations.houseManagement.House',
        'B4.model.integrations.houseManagement.House'
    ],
    layout: 'hbox',
    items: [
    {
        xtype: 'b4selectfield',
        itemId: 'housesSelect',
        flex: 1,
        padding: '10 5 0 10',
        labelWidth: 130,
        labelAlign: 'right',
        textProperty: 'Address',
        selectionMode: 'MULTI',
        fieldLabel: 'Дома',
        store: 'B4.store.integrations.houseManagement.House',
        model: 'B4.model.integrations.houseManagement.House',
        columns: [
            {
                text: 'Адрес',
                dataIndex: 'Address',
                flex: 1,
                filter: { xtype: 'textfield' }
            },
            {
                text: 'Тип дома',
                dataIndex: 'HouseType',
                flex: 1,
                filter: { xtype: 'textfield' }
            }
        ],
        listeners: {
            beforeload: function (store, operation) {

                var me = this,
                    parametersStepFrame = me.up();
                    exporter_Id = parametersStepFrame.wizard.exporter_Id;

                operation.params = operation.params || {};

                if (exporter_Id === 'HouseUODataExporter') {
                    operation.params.forUO = true;
                }

                if (exporter_Id === 'HouseOMSDataExporter') {
                    operation.params.forOMS = true;
                }

                if (exporter_Id === 'HouseRSODataExporter') {
                    operation.params.forRSO = true;
                }
            }
        }
    }],

    firstInit: function () {
        var me = this;
        me.wizard.down('#housesSelect').on('change', function (field, newValue, oldValue, eOpts) {
            me.fireEvent('selectionchange', me);
        }, me);
    },

    getParams: function () {
        var me = this;
        return {
            selectedHouses: me.wizard.down('#housesSelect').getValue()
        };
    }
});