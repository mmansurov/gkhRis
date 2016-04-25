Ext.define('B4.view.integrations.gis.SignatureWindow', {
    extend: 'Ext.window.Window',
    alias: 'widget.signaturewindow',
    requires: [
        'B4.form.ComboBox',
        'B4.mixins.MaskBody',
        'B4.ux.button.Close'
    ],
    mixins: ['B4.mixins.window.ModalMask'],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    bodyPadding: 5,
    width: 600,
    height: 130,
    title: 'Параметры выполнения',
    maximizable: true,

    initComponent: function () {
        var me = this;
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'checkbox',
                    name: 'OnlyMethod',
                    labelWidth: 150,
                    labelAlign: 'right',
                    fieldLabel: 'Без извлечения данных'
                },
                //{
                //    xtype: 'b4combobox',
                //    url: '/GisIntegration/GetCertificates',
                //    emptyItem: { SubjectName: '-' },
                //    displayField: 'SubjectName',
                //    valueField: 'Thumbprint',
                //    editable: false,
                //    storeAutoLoad: false,
                //    fieldLabel: 'Сертификат',
                //    labelWidth: 150,
                //    labelAlign: 'right',
                //    padding: '5 0 0 0',
                //    name: 'SignatureList'
                //}
            ],
            dockedItems: [
                {
                    xtype: 'toolbar',
                    dock: 'top',
                    items: [
                        {
                            xtype: 'buttongroup',
                            items: [
                                {
                                    xtype: 'button',
                                    text: 'Выполнить',
                                    name: 'Sign',
                                    iconCls: 'icon-accept'
                                }
                            ]
                        },
                        { xtype: 'tbfill' },
                        {
                            xtype: 'buttongroup',
                            items: [
                                { xtype: 'b4closebutton' }
                            ]
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }
});