define(['apps/system3/production/production.controller'], function (app) {

    app.factory("ganttService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getSource: function (filter) {
                return restSrv.all("engineering").customGET('gantt', filter);
            }
        }
    });
});