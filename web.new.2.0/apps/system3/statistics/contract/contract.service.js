define([
'apps/system3/statistics/statistics'], function (app) {

    app.module.factory("statisticsConService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            
        }
    });
});
