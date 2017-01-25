define(['apps/system/system.service'], function (app) {

    app.factory("system_setting_service", function (Restangular, userApiUrl, userApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(userApiUrl + userApiVersion);
        })

        return {
            all: function () {
                return restSrv.all("settings").getList();
            },
            updateSettings: function (settings) {
                return restSrv.all("settings").customPUT(settings);
            }
        }
    });
});
