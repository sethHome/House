define(['apps/system3/document/document'], function (app) {

    app.module.factory("documentService", function (Restangular, stdApiUrl, stdApiVersion) {
        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getTree: function (filter) {
                return restSrv.all("object").customGET('tree', filter);
            }
        }
    });

});
