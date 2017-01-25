define([
'apps/system3/statistics/statistics'], function (app) {

    app.module.factory("statisticsEngService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getCarInfo: function (id) {
                return restSrv.one("car", id).get();
            },
        }
    });
});
