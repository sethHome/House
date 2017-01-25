define(['apps/base/base.service'], function (app) {

    app.factory("messageService", function ($rootScope,Restangular, userApiUrl, userApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(userApiUrl + userApiVersion);
        })

        return {
            getMyMessage: function (filter) {
                filter.user = $rootScope.currentUser.Account.ID;
                return restSrv.one("message").get(filter);
            },
            createChatGroup: function (info) {
                return restSrv.all("group").post(info);
            },
            updateGroup: function (info) {
                return restSrv.one("group", info.GroupID.replace('G', '')).customPUT(info);
            },
            removeGroup: function (id) {
                return restSrv.one("group", id.replace('G', '')).remove();
            },
            exitGroup: function (id) {
                return restSrv.one("group", id.replace('G', '')).customDELETE('exit');
            }

        }
    });
});
