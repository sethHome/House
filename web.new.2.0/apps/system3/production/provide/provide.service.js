define(['apps/system3/production/production.controller'], function (app) {

    app.factory("provideService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getSource: function (filter) {
                return restSrv.all("specialty").customGET("provide", filter);
            },
            update: function (info) {
                return restSrv.one("specialty/provide", info.ID).customPUT(info);
            },
            create: function (info) {
                return restSrv.all("specialty").customPOST(info, "provide");
            },
            remove: function (id) {
                return restSrv.one("specialty/provide", id).remove();
            }
        }
    });
});