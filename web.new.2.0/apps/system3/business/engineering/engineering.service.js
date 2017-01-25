define(['apps/system3/business/business'], function (app) {

    app.module.factory("engineeringService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            get: function (id) {
                return restSrv.one("engineering",id).get();
            },
            getEngineerings: function (filter) {
                return restSrv.one("engineering").get(filter);
            },
            create: function (eng) {
                return restSrv.all("engineering").customPOST(eng);
            },
            edit: function (eng) {
                return restSrv.one("engineering", eng.ID).customPUT(eng);
            },
            remove: function (ID) {
                return restSrv.one("engineering", ID).remove();
            },
            backup: function (id) {
                return restSrv.one("engineering", id).customDELETE("backup");
            },
            batchRemove: function (IDs) {
                return restSrv.one("engineering", IDs.join(',')).remove();
            },
            loadSource: function (filter) {
                return restSrv.all("engineering").customGET('source', filter);
            },
            follow: function (id) {
                return restSrv.one("engineering", id).customPUT(undefined, 'follow');
            },
            unfollow: function (id) {
                return restSrv.one("engineering", id).customPUT(undefined, 'unfollow');
            }
        }
    });
});
