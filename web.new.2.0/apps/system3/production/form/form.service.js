define(['apps/system3/production/production.controller'], function (app) {

    app.factory("formService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            // 变更单
            getChangeList: function (filter) {
                return restSrv.all("form").customGET('change', filter);
            }
        }
    });
});