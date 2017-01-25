define(['apps/base/base.service'], function (app) {

    app.factory("menuService", function (Restangular, userApiUrl, userApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(userApiUrl + userApiVersion);
        })

        return {
            getAllMenus: function () {
                return restSrv.all("menu/all").getList();
            },
            getModules: function () {
                return restSrv.all("module").getList();
            }
        }
    });
});
