define(['apps/system3/home/home.controller'], function (app) {

    app.factory("desktopService", function ($rootScope,Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getMyTaskCount: function () {
                return restSrv.all("task").customGET('count');
            },
            getMyProductionTasks: function (filter) {
                filter = filter ? filter : {};
                filter.user = $rootScope.currentUser.Account.ID;
                
                return restSrv.all("task").customGET('production', filter);
            },
            getMyProvideTasks: function (filter) {
                filter = filter ? filter : {};
                filter.user = $rootScope.currentUser.Account.ID;

                return restSrv.all("task").customGET('provide', filter);
            },
            getMyFormTasks: function (filter) {
                filter = filter ? filter : {};
                filter.user = $rootScope.currentUser.Account.ID;

                return restSrv.all("task").customGET('form', filter);
            },
            getProductionTasks: function (filter) {
                return restSrv.all("task").customGET('production', filter);
            },
            getMyStatistics: function () {
                return restSrv.all("statistics").customGET('my');
            },
            getTask: function (id) {
                return restSrv.one("task",id).get();
            },
            getFollowEngs: function () {

                var ids = $rootScope.userConfig.engFollows.map(function (item) { return item.ConfigKey; });

                return restSrv.one("engineering").get({
                    pagesize: 4,
                    pageindex: 1,
                    orderby: 'ID',
                    ids: ids.join(',')
                });
            },
            getFollowEngNotes: function (id) {

                return restSrv.one("engineering", id).customGET('note');
            },
            getVolumeInfo: function (id) {
                return restSrv.one("task", id).customGET('volume');
            }
        }
    });


});
