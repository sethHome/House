define(['apps/system3/production/production.controller'], function (app) {

    app.factory("resourceService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            loadEngSource: function (filter) {
                return restSrv.all("engineering").customGET('source', filter);
            },
            getSource: function (filter) {
                return restSrv.one("resource").get(filter);
            },
            update: function (info) {
                return restSrv.one("resource", info.ID).customPUT(info);
            },
            create: function (info) {
                return restSrv.all("resource").customPOST(info);
            },
            remove: function (id) {
                return restSrv.one("resource", id).remove();
            }
        }
    });
});