define(['apps/system3/production/production.controller'], function (app) {

    app.factory("changeService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            // 变更单
            create: function (info) {
                return restSrv.all("form").customPOST(info,'change');
            },
            // 更新
            update: function (info) {
                return restSrv.one("form/change", info.ID).customPUT(info);
            },
            // 获取变更单信息
            getChangeInfo: function (id) {
                return restSrv.one("form/change",id).get();
            },
            
        }
    });
});