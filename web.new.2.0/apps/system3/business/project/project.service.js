define(['apps/system3/business/business'], function (app) {

    app.module.factory("projectService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            loadSource: function (filter) {
                return restSrv.all("project").customGET('source',filter);
            },
            getProjects: function (filter) {
                return restSrv.one("project").get(filter);
            },
            create: function (proj) {
                return restSrv.all("project").customPOST(proj);

            },
            edit: function (proj) {
                return restSrv.one("project", proj.ID).customPUT(proj);
            },
            backup: function (id) {
                return restSrv.one("project", id).customDELETE("backup");
            },
            remove: function (ID) {
                return restSrv.one("project", ID).remove();
            },
            batchRemove: function (IDs) {
                return restSrv.one("project", IDs.join(',')).remove();
            }
        }
    });


});
