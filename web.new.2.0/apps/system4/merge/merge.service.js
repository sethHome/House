define([
'apps/system4/merge/merge'], function (app) {

    app.module.factory("mergeService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + $rootScope.currentBusiness.Key + '/' + stdApiVersion);
        })

        var restAttachSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getProjects: function (filter) {
                return restSrv.one("project").get(filter);
            },
            createProject: function (info) {
                return restSrv.all("project").post(info);
            },
            updateProject: function (id,info) {
                return restSrv.one("project", id).customPUT(info);
            },
            deleteProject: function (ids) {
                return restSrv.one("project", ids).remove();
            },
            getMyTask: function () {
                return restSrv.all("project").customGET("mytask");
            },
            taskFinish: function (id,attachID) {
                return restSrv.one("task", id).customPUT({}, 'finish/' + attachID);
            },
            merge: function (id,optinos) {
                return restSrv.one("task", id).customPUT(optinos, 'merge');
            },
            getMergeHistory: function (id) {
                return restAttachSrv.all("attach").getList({ objkey: 'MergeProjectDocResult', objid: id });
            },
        }
    });
});
