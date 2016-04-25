Ext.define('B4.view.wizard.export.XmlPreviewStepFrame', {
    extend: 'B4.view.wizard.WizardBaseStepFrame',
    wizard: undefined,
    stepId: 'xmlPreview',
    title: 'Предпросмотр XML',
    layout: 'fit',
    items: [{
        xtype: 'b4grid',
        title: 'Пакеты данных для отправки',
        store: Ext.create('B4.store.RisPackage'),
        flex: 1,
        name: 'PackageGrid',
        columnLines: true,
        columns: [
            {
                xtype: 'gridcolumn',
                flex: 1,
                align: 'center',
                text: 'Наименование',
                dataIndex: 'Name'
            },
            {
                xtype: 'actioncolumn',
                flex: 1,
                align: 'center',
                text: 'Неподписанная XML',
                dataIndex: 'NotSignedData',
                renderer: function () {
                    return '<a href="javascript:void(0)" style="color: black;">Просмотр</a>';
                }
            },
            {
                xtype: 'actioncolumn',
                flex: 1,
                align: 'center',
                text: 'Подписанная XML',
                dataIndex: 'SignedData',
                renderer: function (val) {
                    if (val) {
                        return '<a href="javascript:void(0)" style="color: black;">Просмотр</a>';
                    }

                    return null;
                }
            }
        ],
        dockedItems: [
            {
                xtype: 'toolbar',
                dock: 'top',
                items: [
                    {
                        xtype: 'buttongroup',
                        columns: 2,
                        items: [
                            {
                                xtype: 'combobox',
                                emptyItem: { SubjectName: '-' },
                                displayField: 'SubjectName',
                                valueField: 'Certificate',
                                editable: false,
                                queryMode: 'local',
                                fieldLabel: 'Сертификат для подписи',
                                width: 590,
                                labelWidth: 135,
                                labelAlign: 'right',
                                name: 'Certificate',
                                disabled: true
                            },
                            {
                                xtype: 'button',
                                name: 'Sign',
                                text: 'Подписать',
                                iconCls: 'icon-accept',
                                disabled: true
                            }
                        ]
                    }
                ]
            }
        ]
    }],

    firstInit: function () {
        var me = this,
            packageGrid = me.down('b4grid[name=PackageGrid]');

        if (!me.wizard.needSign) {
            packageGrid.down('actioncolumn[dataIndex=NotSignedData]').text = 'XML для отправки';
            packageGrid.down('actioncolumn[dataIndex=SignedData]').hide();
            packageGrid.down('toolbar').hide();
        }

        packageGrid.down('actioncolumn[dataIndex=NotSignedData]').on('click', function (grid, cell, index, hz, event, rec) {
            B4.Ajax.request({
                url: B4.Url.action('GetNotSignedData', 'GisIntegration'),
                params: {
                    package_Ids: rec.get('Id'),
                    forPreview: true
                },
                timeout: 9999999
            }).next(function (response) {
                var notSignedXmlPreviewWin = Ext.create('B4.view.wizard.export.NotSignedXmlPreviewWin'),
                    data = Ext.decode(response.responseText).data;
                notSignedXmlPreviewWin.down('textareafield').setValue(data[0].NotSignedData);
                notSignedXmlPreviewWin.show();
            }, me).error(function (e) {
                Ext.Msg.alert('Ошибка!', (e.message || 'Не найдена неподписанная xml'));
            }, me);
        });

        packageGrid.down('actioncolumn[dataIndex=SignedData]').on('click', function (grid, cell, index, hz, event, rec) {
            if (rec) {
                var signedXmlPreviewWin = Ext.create('B4.view.wizard.export.SignedXmlPreviewWin');
                signedXmlPreviewWin.on('beforeshow', function () {
                    signedXmlPreviewWin.down('textareafield').setValue(rec.get('SignedData'));
                });
                signedXmlPreviewWin.show();
            }
        });

        packageGrid.down('combobox[name=Certificate]').on('expand', function () {
            me.wizard.signer.getCertificates(2, 'My', me.fillCertificatesCombo, me.showError, me);
        });

        packageGrid.down('combobox[name=Certificate]').on('change', function (combo, newVal, oldVal) {
            me.wizard.signer.setCertificate(newVal);
            Ext.each(packageGrid.getStore().data.items, function (rec) {
                rec.set('SignedData', null);
            });
        });

        // массовая подпись всех xml грида
        packageGrid.down('button[name=Sign]').on('click', function () {
            var packageStore = packageGrid.getStore(),
                certCombo = packageGrid.down('combobox[name=Certificate]'),
                packageIds = [],
                signedPackages,
                recs = [],
                count = 0;

            if (!certCombo.value) {
                Ext.Msg.alert('Ошибка!', 'Выберите сертификат для подписи');
                return;
            }

            me.wizard.mask();

            packageStore.each(function (rec) {
                packageIds.push(rec.get('Id'));
            });

            B4.Ajax.request({
                url: B4.Url.action('GetNotSignedData', 'GisIntegration'),
                params: {
                    package_Ids: packageIds
                },
                timeout: 9999999
            }).next(function (response) {
                signedPackages = Ext.decode(response.responseText).data;
                Ext.each(signedPackages, function (signedPackage) {
                    var rec = packageStore.findRecord('Id', signedPackage.Id);
                    me.wizard.signer.signXml(signedPackage.NotSignedData, function (xml) {
                        rec.set('SignedData', xml);
                        recs.push({
                            Id: rec.get('Id'),
                            SignedData: encodeURI(rec.get('SignedData'))
                        });
                        count++;

                        if (count === signedPackages.length) {
                            B4.Ajax.request({
                                url: B4.Url.action('SaveSignedData', 'GisIntegration'),
                                params: {
                                    exporter_Id: me.wizard.exporter_Id,
                                    packages: Ext.JSON.encode(recs)
                                },
                                timeout: 9999999
                            }).next(function () {
                                me.fireEvent('selectionchange', me);
                                me.wizard.unmask();
                            }, me).error(function (e) {
                                me.wizard.unmask();
                                Ext.Msg.alert('Ошибка!', (e.message || 'Не удалось сохранить подписанные xml'));
                            }, me);
                        }
                    }, me.showError, me);
                });                
            }, me).error(function (e) {
                me.wizard.unmask();
                Ext.Msg.alert('Ошибка!', (e.message || 'Не найдена неподписанная xml'));
            }, me);            
        });
    },

    init: function() {
        var me = this,
            packages = me.wizard.packages,
            hasPackages = packages && packages.length !== 0,
            packageGrid = me.down('b4grid[name=PackageGrid]'),
            packageGridStore = packageGrid.getStore(),
            certCombo = packageGrid.down('combobox[name=Certificate]');

        certCombo.setDisabled(!hasPackages);

        packageGridStore.removeAll();

        packageGrid.down('button[name=Sign]').setDisabled(!hasPackages);

        if (hasPackages) {
            Ext.each(packages, function (pack) {
                var packageRec = Ext.create('B4.model.RisPackage', {
                    Id: pack.Id,
                    Name: pack.Name,
                    NotSignedData: pack.NotSignedData
                });
                packageGridStore.add(packageRec);
            });
        }
    },

    fillCertificatesCombo: function (certificates) {
        var me = this,
            certCombo = me.down('b4grid[name=PackageGrid]').down('combobox[name=Certificate]'),
            certComboStore = certCombo.getStore();

        certCombo.clearValue();
        certComboStore.removeAll();

        Ext.each(certificates, function (cert) {
            var certificateRec = Ext.create('B4.model.RisCertificate', {
                SubjectName: cert.subjectName,
                Certificate: cert
            });

            certComboStore.add(certificateRec);
        });
    },

    showError: function (e) {
        this.wizard.unmask();
        Ext.Msg.alert('Ошибка!', (e.message || 'Не удалось подписать xml'));
    },

    allowForward: function () {

        var me = this,
            result = true;

        if (me.wizard.needSign) {
            //проверяем подписаны ли все пакеты
            var packageGrid = me.down('b4grid[name=PackageGrid]');

            Ext.each(packageGrid.getStore().data.items, function (rec) {
                var signedData = rec.get('SignedData');

                if (!signedData || signedData.length === 0) {
                    result = false;
                    return false;
                }
            });
        }

        return result;
    },

    doBackward: function () {
        var me = this,
            validateResults = me.wizard.validateResults;

        me.wizard.setCurrentStep(validateResults && validateResults.length !== 0 ? 'validationResult' : 'pageParameters');
    },

    doForward: function () {
        var me = this;

        me.wizard.mask();
        B4.Ajax.request({
            url: B4.Url.action('ExecuteExporter', 'GisIntegration'),
            params: {
                exporter_Id: me.wizard.exporter_Id,
                package_Ids: Ext.Array.pluck(me.wizard.packages, 'Id')
            },
            timeout: 9999999
        }).next(function (response) {
            me.wizard.result = me.parseResult(response);
            me.wizard.setCurrentStep('finish');
            me.wizard.unmask();
        }, me).error(function (e) {
            var errorMessage = e.message || 'Не удалось выполнить экспорт';
            me.wizard.result = {
                state: 'error',
                message: 'При выполнении экспорта произошла ошибка.'
                    + '<br><br>'
                    + errorMessage
            }
            me.wizard.setCurrentStep('finish');
            me.wizard.unmask();
        }, me);

        return;
    },

    parseResult: function(response) {
        var data = Ext.decode(response.responseText).data;

        if (data.State === 'Success') {
            return {
                state: 'success',
                message: 'Экспорт успешно запущен. Задача получения результата запланирована.'
                    + '<br><br>'
                    + 'Для получения результата перейдите в раздел \'Список запланированных задач\'',
                data: data
            };
        }

        var errorsList = '';

        for (var i = 0; i < data.PackageSendingErrors.length; i++) {
            errorsList += data.PackageSendingErrors[i].Name + " : " + data.PackageSendingErrors[i].Message + '<br>';
        }

        if (data.State === 'WithErrors') {          
            return {
                state: 'warning',
                message: 'Экспорт запущен c ошибками. При отправке части пакетов возникли ошибки: '
                    + '<br>'
                    + errorsList
                    + '<br>'
                    + 'Задача получения результата для отправленных пакетов запланирована.'
                    + '<br><br>'
                    + 'Для получения результата перейдите в раздел \'Список запланированных задач\'',
                data: data
            };
        }

        return {
            state: 'error',
            message: 'Экспорт не запущен. При отправке всех пакетов возникли ошибки: '
                + '<br>'
                + errorsList,
            data: data
        };
    }
});