define(['apps/system3/production/production.controller'], function (app) {

    app.factory("noteService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            get: function (id) {
                return restSrv.one("note",id).get();
            },
            getSource: function (filter) {
                return restSrv.one("note").get(filter);
            },
            update: function (info) {
                return restSrv.one("note",info.ID).customPUT(info);
            },
            create: function (info) {
                return restSrv.all("note").customPOST(info);
            },
            remove: function (id) {
                return restSrv.one("note", id).remove();
            },
            loadEngSource: function (filter) {
                return restSrv.all("engineering").customGET('source', filter);
            },
        }
    });
});