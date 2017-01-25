define([
'apps/system3/admin/admin'], function (app) {

    app.module.factory("adminService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getProjectList: function (filter) {
                //filter.deep = 2;
                filter.task = 1;
                return restSrv.all("object").customGET('tree', filter);
            },
            getEngineeringList: function (filter) {
                filter.task = 1;
                return restSrv.all("object").customGET('engineering/tree', filter);
            },
            getEngineeringNotes: function (id) {
                return restSrv.one("engineering",id).customGET('note');
            },
            getGanntSource: function (id) {
                return restSrv.one("engineering", id).customGET('gantt');
            }
        }
    });
});
