Ext.define('B4.view.wizard.export.NotSignedXmlPreviewWin', {
    extend: 'B4.form.Window',
    alias: 'widget.notsignedxmlpreviewwin',
    modal: true,
    width: 550,
    height: 405,
    bodyPadding: 5,
    requires: [
        'B4.ux.button.Close'
    ],
    layout: {
        type: 'vbox',
        align: 'stretch'
    },
    title: 'Просмотр',

    initComponent: function () {
        var me = this;

        Ext.apply(me, {
            items: [
                {
                    xtype: 'tabpanel',
                    flex: 1,
                    border: false,
                    items: [
                        {
                            xtype: 'panel',
                            border: false,
                            title: 'Неподписанная XML',
                            bodyStyle: B4.getBgStyle(),
                            items: [
                                {
                                    xtype: 'container',
                                    layout: 'fit',
                                    padding: '5 0 0 0',
                                    items: [
                                        {
                                            xtype: 'textareafield',
                                            grow: true,
                                            name: 'NotSignedXml',
                                            height: 300,
                                            flex: 1,
                                            readOnly: true
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ],
            dockedItems: [
                {
                    xtype: 'toolbar',
                    dock: 'top',
                    items: [
                        {
                            xtype: 'tbfill'
                        },
                        {
                            xtype: 'buttongroup',
                            items: [
                                {
                                    xtype: 'b4closebutton',
                                    text: 'Закрыть',
                                    listeners: {
                                        click: function() {
                                            this.up('notsignedxmlpreviewwin').close();
                                        }
                                    }
                                }
                            ]
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }
});