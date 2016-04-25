Ext.define('B4.controller.integrations.GisIntegrationSettings', {
    extend: 'B4.base.Controller',

    requires: [
        'B4.Ajax',
        'B4.Url',
        'B4.view.integrations.gis.SettingsPanel'
    ],

    mixins: {
        context: 'B4.mixins.Context',
        mask: 'B4.mixins.MaskBody'
    },

    refs: [
        {
            ref: 'mainPanel',
            selector: 'gissettingspanel'
        }
    ],

    init: function () {
        var me = this;

        me.control({
            'gissettingspanel b4savebutton': { 'click': { fn: me.saveParams, scope: me } }
        });

        me.callParent(arguments);
    },

    index: function () {
        var me = this,
            view = me.getMainPanel() || Ext.widget('gissettingspanel');

        me.setParams(view);
        me.bindContext(view);
        me.application.deployView(view);
    },

    setParams: function (panel) {
        var me = this,
            form = panel.getForm();

        B4.Ajax.request({
            url: B4.Url.action('GetParams', 'RisSettings', {
                settingsCode: 'gisIntegration'
            }),
            timeout: 9999999
        }).next(function (resp) {
            var response = Ext.decode(resp.responseText);
            form.setValues(response.data);
        }).error(function () {
            me.unmask();
            Ext.Msg.alert('Ошибка!', 'При получении параметров произошла ошибка!');
        });
    },

    saveParams: function (btn) {
        var me = this,
            panel = btn.up('gissettingspanel'),
            form = panel.getForm(),
            values = form.getValues(false, false, false, true);

        if (form.isValid()) {
            me.mask('Сохранение', panel);
            form.submit({
                url: B4.Url.action('SaveParams', 'RisSettings'),
                timeout: 9999999,
                params: {
                    settingsCode: 'gisIntegration',
                    values: values
                },
                success: function () {
                    me.unmask();
                    B4.QuickMsg.msg('Успешно', 'Изменения сохранены', 'success');
                },
                failure: function () {
                    me.unmask();
                    B4.QuickMsg.msg('Ошибка!', 'При сохранении параметров произошла ошибка!', 'error');
                }
            });
        } else {
            B4.QuickMsg.msg('Ошибка!', 'Проверьте правильность заполнения формы!', 'error');
        }
    }
});