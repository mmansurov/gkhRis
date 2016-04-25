Ext.define('B4.view.wizard.export.contractData.ContractDataParametersStepFrame', {
    extend: 'B4.view.wizard.export.ParametersStepFrame',
    requires: [
        'B4.form.SelectField',
        'B4.store.integrations.houseManagement.Contract',
        'B4.model.integrations.houseManagement.Contract'
    ],
    layout: 'hbox',
    items: [
    {
        xtype: 'b4selectfield',
        itemId: 'contractsSelect',
        flex:1,
        padding: '10 5 0 10',
        labelWidth: 130,
        labelAlign: 'right',
        textProperty: 'DocNum',
        selectionMode: 'MULTI',
        fieldLabel: 'Договоры управления',
        store: 'B4.store.integrations.houseManagement.Contract',
        model: 'B4.model.integrations.houseManagement.Contract',
        columns: [
            {
                text: 'Номер',
                dataIndex: 'DocNum',
                flex: 1,
                filter: { xtype: 'textfield' }
            },
            {
                xtype: 'datecolumn',
                text: 'Дата заключения',
                dataIndex: 'SigningDate',
                format: 'd.m.Y',
                flex: 1,
                filter: {
                    xtype: 'datefield',
                    operand: CondExpr.operands.eq
                }
            },
            {
                text: 'Организация',
                dataIndex: 'Organization',
                flex: 1,
                filter: { xtype: 'textfield' }
            },
            {
                text: 'Адрес',
                dataIndex: 'Address',
                flex: 1,
                filter: { xtype: 'textfield' }
            }
        ]
    }],

    firstInit: function () {
        var me = this;
        me.wizard.down('#contractsSelect').on('change', function (field, newValue, oldValue, eOpts) {
            me.fireEvent('selectionchange', me);
        }, me);
    },

    getParams: function () {
        var me = this;
        return {
            selectedList: me.wizard.down('#contractsSelect').getValue()
        };
    }
});