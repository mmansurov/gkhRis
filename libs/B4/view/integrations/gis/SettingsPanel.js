Ext.define('B4.view.integrations.gis.SettingsPanel', {
    extend: 'Ext.form.Panel',

    requires: [
        'B4.ux.button.Save'
    ],

    title: 'Настройки интеграции с ГИС',
    alias: 'widget.gissettingspanel',
    layout: 'vbox',
    bodyStyle: B4.getBgStyle(),
    bodyPadding: 5,
    closable: true,

    initComponent: function () {
        var me = this;
        Ext.applyIf(me, {
            defaults: {
                labelAlign: 'right',
                width: 500
            },
            items: [
                {
                    xtype: 'fieldset',
                    title: 'Подключение',
                    anchor: '100%',
                    layout: {
                        type: 'vbox',
                        align: 'stretch'
                    },
                    defaults: {
                        padding: '0 0 5 0',
                        xtype: 'container',
                        layout: 'hbox'
                    },
                    items: [
                        {
                            xtype: 'textfield',
                            name: 'ServiceName',
                            fieldLabel: 'Ссылка на Nsi сервис'
                        },
                        {
                            xtype: 'textfield',
                            name: 'Login',
                            fieldLabel: 'Логин'
                        },
                        {
                            xtype: 'textfield',
                            name: 'Password',
                            fieldLabel: 'Пароль'
                        }
                    ]
                }
            ],

            dockedItems: [
                {
                    xtype: 'buttongroup',
                    dock: 'top',
                    items: [
                        {
                            xtype: 'b4savebutton'
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }
});