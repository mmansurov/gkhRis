Ext.define('B4.view.wizard.export.contractData.CharterDataParametersStepFrame', {
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
        name: 'CharterParameter',
        flex: 1,
        padding: '10 5 0 10',
        labelWidth: 130,
        labelAlign: 'right',
        textProperty: 'DocNum',
        selectionMode: 'MULTI',
        fieldLabel: 'Уставы',
        store: 'B4.store.integrations.houseManagement.Charter',
        model: 'B4.model.integrations.houseManagement.Charter',
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

    allowForward: function () {
        var charterGrid = this.down('b4selectfield[name=CharterParameter]');

        return charterGrid.getStore().getValue();
    },

    firstInit: function () {
        var me = this;
        me.wizard.down('b4selectfield[name=CharterParameter]').on('change', function () {
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