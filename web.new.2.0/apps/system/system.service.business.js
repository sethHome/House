define(['apps/system/system.service'], function (app) {

    app.factory("system_business_service", function (Restangular, userApiUrl, userApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(userApiUrl + userApiVersion);
        })

        return {
            getBusiness: function (option) {
                return restSrv.all("business").getList(option);
            },
            setUsers: function (key,users) {
                return restSrv.one("business", key).customPUT(users,"user");
            }
        }
    });
});
