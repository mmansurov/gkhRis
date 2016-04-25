Ext.define('B4.controller.integrations.GisIntegration', {
    extend: 'B4.base.Controller',

    requires: [
        'B4.Ajax',
        'B4.Url',
        'B4.aspects.GkhInlineGrid',
        'B4.aspects.GridEditCtxWindow',
        'B4.aspects.permission.GkhPermissionAspect'
    ],

    models: [
        'integrations.gis.DataExtractor',
        'integrations.gis.Method',
        'integrations.gis.Dict',
        'integrations.gis.DictRef',
        'integrations.gis.Log'
    ],

    views: [
        'integrations.gis.Panel',
        'integrations.gis.DataExtractorGrid',
        'integrations.gis.MethodGrid',
        'integrations.gis.DictGrid',
        'integrations.gis.DictWindow',
        'integrations.gis.DictRefWindow',
        'integrations.gis.DictRefSelectWindow',
        'integrations.gis.LogGrid',
        'integrations.gis.SignatureWindow'
    ],

    mixins: {
        context: 'B4.mixins.Context',
        mask: 'B4.mixins.MaskBody'
    },

    refs: [
        { ref: 'mainPanel', selector: 'gisintegrationpanel' },
        { ref: 'methodGrid', selector: 'gismethodgrid' },
        { ref: 'dictGrid', selector: 'gisidictgrid' },
        { ref: 'logGrid', selector: 'gisloggrid' }
    ],

    mainView: 'integrations.gis.Panel',
    mainViewSelector: 'gisintegrationpanel',

    aspects: [
        {
            xtype: 'grideditctxwindowaspect',
            name: 'gisDictGridWindowAspect',
            gridSelector: 'gisdictgrid',
            editFormSelector: 'gisdictwindow',
            modelName: 'integrations.gis.Dict',
            editWindowView: 'integrations.gis.DictWindow',
            otherActions: function (actions) {
                var me = this;
                actions['gisdictrefgrid'] = {
                    'rowaction': {
                        fn: me.refRowAction,
                        scope: me
                    }
                };
                actions['gisdictrefgrid b4savebutton'] = {
                    'click': {
                        fn: me.saveRefGrid,
                        scope: me
                    }
                };
                actions['gisdictrefgrid b4updatebutton'] = {
                    'click': {
                        fn: me.updateRefGrid,
                        scope: me
                    }
                };
                actions['gisdictrefgrid b4closebutton'] = {
                    'click': {
                        fn: me.closeRefWin,
                        scope: me
                    }
                };
                actions['gisdictrefselectwindow b4savebutton'] = {
                    'click': {
                        fn: me.onGisGkhDictCompareWinSaveBtnClick,
                        scope: me
                    }
                };
                actions['gisdictrefselectwindow b4closebutton'] = {
                    'click': {
                        fn: me.onGisGkhDictCompareWinCloseBtnClick,
                        scope: me
                    }
                };
            },
            getRecordBeforeSave: function (record) {
                var selField = this.getForm().down('b4selectfield[name=DictProxy]');
                record.set('NsiRegistryNumber', selField.getValue());
                record.set('Name', selField.getText());
                return record;
            },
            rowAction: function (grid, action, record) {
                if (this.fireEvent('beforerowaction', this, grid, action, record) !== false) {
                    switch (action.toLowerCase()) {
                        case 'edit':
                            this.editRecord(record);
                            break;
                        case 'delete':
                            this.deleteRecord(record);
                            break;
                        case 'refrecord':
                            this.refRecord(record);
                            break;
                    }
                }
            },
            refRowAction: function (grid, action, record) {
                switch (action.toLowerCase()) {
                    case 'selectref':
                        this.selectRefRecord(record);
                        break;
                }
            },
            selectRefRecord: function (record) {
                var me = this,
                    win,
                    store;

                win = me.controller.getView('integrations.gis.DictRefSelectWindow').create({
                    constrain: true,
                    modal: false,
                    gisRecord: record,
                    closeAction: 'destroy',
                    renderTo: B4.getBody().getActiveTab().getEl()
                });

                win.show();

                store = win.down('grid').getStore();
                store.clearFilter(true);
                store.filter('dictId', record.get('Dict').Id);
            },
            onGisGkhDictCompareWinSaveBtnClick: function (btn) {
                var wnd = btn.up('gisdictrefselectwindow'),
                    grid = wnd.down('grid'),
                    selRec;

                if (grid) {
                    selRec = grid.getSelectionModel().getSelection()[0];

                    if (selRec) {

                        wnd.gisRecord.set('GisId', selRec.get('Id'));
                        wnd.gisRecord.set('GisName', selRec.get('Name'));
                        wnd.gisRecord.set('GisGuid', selRec.get('Guid'));

                        wnd.close();
                    } else {
                        Ext.Msg.alert('Внимание', 'Необходимо выбрать запись');
                    }
                }
            },
            onGisGkhDictCompareWinCloseBtnClick: function (btn) {
                btn.up('gisdictrefselectwindow').close();
            },
            saveRefGrid: function (btn) {
                var me = this,
                    grid = btn.up('grid'),
                    store = grid.getStore();

                var modifiedRecs = store.getModifiedRecords();
                var removedRecs = store.getRemovedRecords();
                if (modifiedRecs.length > 0 || removedRecs.length > 0) {

                    me.mask('Сохранение', grid);
                    store.sync({
                        callback: function () {
                            me.unmask();
                            store.load();
                            me.getGrid().getStore().load();
                        },
                        failure: me.handleDataSyncError,
                        scope: me
                    });
                }
            },
            updateRefGrid: function (btn) {
                btn.up('grid').getStore().load();
            },
            closeRefWin: function (btn) {
                btn.up('gisdictrefwindow').close();
            },
            refRecord: function (record) {
                var me = this,
                    id = record ? record.getId() : null,
                    win = me.getRefWindow(),
                    store;

                win.show();

                store = win.down('gisdictrefgrid').getStore();
                store.clearFilter(true);
                store.filter('dictId', id);

            },
            getRefWindow: function () {
                var me = this,
                    win;

                if (me.componentQuery) {
                    win = me.componentQuery('gisdictrefwindow');
                }

                if (win && !win.getBox().width) {
                    win = win.destroy();
                }

                if (!win) {
                    win = me.controller.getView('integrations.gis.DictRefWindow').create({ constrain: true, autoDestroy: true });
                    if (B4.getBody().getActiveTab()) {
                        B4.getBody().getActiveTab().add(win);
                    } else {
                        B4.getBody().add(win);
                    }
                }
                return win;
            }
        },
        {
            xtype: 'gkhpermissionaspect',
            permissions: [
                {
                    name: 'Administration.OutsideSystemIntegrations.Gis.Methods.View',
                    applyTo: '[name=methodpanel]',
                    selector: 'gisintegrationpanel',
                    applyBy: function (component, allowed) {
                        if (allowed) {
                            component.tab.show();
                        } else {
                            component.tab.hide();
                        }
                    }
                },
                {
                    name: 'Administration.OutsideSystemIntegrations.Gis.Dictions.View',
                    applyTo: '[name=dictpanel]',
                    selector: 'gisintegrationpanel',
                    applyBy: function (component, allowed) {
                        if (allowed) {
                            component.tab.show();
                        } else {
                            component.tab.hide();
                        }
                    }
                },
                {
                    name: 'Administration.OutsideSystemIntegrations.Gis.Logs.View',
                    applyTo: '[name=logpanel]',
                    selector: 'gisintegrationpanel',
                    applyBy: function (component, allowed) {
                        if (allowed) {
                            component.tab.show();
                        } else {
                            component.tab.hide();
                        }
                    }
                },
                {
                    name: 'Administration.OutsideSystemIntegrations.Gis.Methods.ExecuteMethod',
                    applyTo: '[name=ExecuteMethod]',
                    selector: 'gisintegrationpanel'
                },
                {
                    name: 'Administration.OutsideSystemIntegrations.Gis.Tasks.View',
                    applyTo: '[name=taskpanel]',
                    selector: 'gisintegrationpanel',
                    applyBy: function (component, allowed) {
                        if (allowed) {
                            component.tab.show();
                        } else {
                            component.tab.hide();
                        }
                    }
                }
            ]
        }
    ],

    init: function () {
        var me = this;

        me.control({
            'gismethodgrid b4updatebutton': { 'click': { fn: me.updateMethodGrid, scope: me } },
            'gismethodgrid button[name=ExecuteMethod]': { 'click': { fn: me.executeMethod, scope: me } },
            'gisloggrid b4updatebutton': { 'click': { fn: me.updateLogGrid, scope: me } },
            'gistasktree b4updatebutton': { 'click': { fn: me.updateTaskTree, scope: me } },
            'signaturewindow button[name=Sign]': { 'click': { fn: me.onSignButtonClick, scope: me } },
            'signaturewindow b4closebutton': { 'click': { fn: me.onSignWinCloseClick, scope: me } }
        });

        me.callParent(arguments);
    },

    index: function () {
        var me = this,
            view = me.getMainPanel() || Ext.widget('gisintegrationpanel');

        me.bindContext(view);
        me.application.deployView(view);

        view.down('gismethodgrid').getStore().load();
        view.down('gisdictgrid').getStore().load();
        view.down('gisloggrid').getStore().load();
        view.down('gistasktree').getStore().load();
    },

    updateMethodGrid: function (btn) {
        btn.up('gismethodgrid').getStore().load();
    },

    updateLogGrid: function (btn) {
        btn.up('gisloggrid').getStore().load();
    },

    updateTaskTree: function (btn) {
        btn.up('gistasktree').getStore().load();
    },

    getExpoterClass: function (exporterId) {
        var result;

        switch (exporterId) {
            case 'DataProviderExporter':
                result = 'B4.view.wizard.export.dataprovider.ExportDataProviderWizard';
                break;

            case 'ContractDataExporter':
                result = 'B4.view.wizard.export.contractData.ExportContractDataWizard';
                break;

            case 'CharterDataExporter':
                result = 'B4.view.wizard.export.charterData.ExportCharterDataWizard';
                break;

            case 'HouseUODataExporter':
            case 'HouseOMSDataExporter':
            case 'HouseRSODataExporter':
                result = 'B4.view.wizard.export.houseData.ExportHouseDataWizard';
                break;

            case 'AdditionalServicesExporter':
                result = 'B4.view.wizard.export.additionalservices.ExportAdditionalServicesWizard';
                break;

            default:
                result = 'B4.view.wizard.export.ExportWizard';
                break;
        }

        return result;
    },

    executeMethod: function (btn) {
        var me = this,
            methodsGrid = btn.up('gismethodgrid'),
            sm = methodsGrid.getSelectionModel(),
            selected = sm.getSelection(),
            method_Id = selected[0].get('Id'),
            method_Name = selected[0].get('Name'),
            method_Description = selected[0].get('Description'),
            method_Type = selected[0].get('Type'),
            method_NeedSign = selected[0].get('NeedSign'),
            tabPanel = me.getMainPanel().down('tabpanel');

        if (!selected || !method_Id) {
            B4.QuickMsg.msg('', 'Не выбран метод!', 'warning');
            return;
        }

        if (method_Type === 'exporter') {
            var vizardWindow = Ext.create(this.getExpoterClass(method_Id), {
                exporter_Id: method_Id,
                methodName: method_Name,
                methodDescription: method_Description,
                needSign: method_NeedSign
            });
            B4.getBody().getActiveTab().add(vizardWindow);

            vizardWindow.show();

            vizardWindow.on('close', function () {
                if (vizardWindow.down('#openScheduledTasks') && vizardWindow.down('#openScheduledTasks').getValue()) {
                    var taskTab = tabPanel.down('panel[name=taskpanel]');
                    taskTab.down('gistasktree').getStore().load();
                    tabPanel.setActiveTab(taskTab);
                }
            });
        } else {
            var signatureWindow = Ext.create('widget.signaturewindow');
            B4.getBody().getActiveTab().add(signatureWindow);
            me.setContextValue(signatureWindow, 'method_Id', method_Id);
            signatureWindow.show();
        }
    },

    onSignButtonClick: function (btn) {
        var me = this,
            signatureWin = btn.up('signaturewindow'),
            onlyMethod = signatureWin.down('checkbox[name=OnlyMethod]').getValue(),
            method_Id = me.getContextValue(me.getMainView(), 'method_Id'),
            certificate = null;

        signatureWin.close();
        me.doMethod(method_Id, onlyMethod, certificate);
    },

    onSignWinCloseClick: function (btn) {
        btn.up('signaturewindow').close();
    },

    doMethod: function (method_Id, onlyMethod, certificate) {
        var me = this,
            methodsGrid = me.getMethodGrid(),
            logGrid = me.getLogGrid();

        me.mask('Выполнение метода...', methodsGrid);
        B4.Ajax.request({
            url: B4.Url.action('ExecuteMethod', 'GisIntegration'),
            params: {
                method_Id: method_Id,
                certificate: certificate,
                onlyMethod: onlyMethod
            },
            timeout: 9999999
        }).next(function () {

            methodsGrid.getStore().load();
            logGrid.getStore().load();
            me.unmask();

            B4.QuickMsg.msg('Успешно', 'Метод выполнен!', 'success');
        }).error(function (e) {
            me.unmask();
            Ext.Msg.alert('Ошибка!', (e.message || 'Не удалось выполнить метод'));
        });
    }
});