define(['apps/system/system.service'], function (app) {

    app.factory("system_permission_service", function (Restangular, userApiUrl, userApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(userApiUrl + userApiVersion);
        })

        return {
            all: function (type, business) {
                
                return restSrv.all("permission").getList({ type: type, business: business });
            },
            create: function (permission) {
                return restSrv.all("permission").customPOST(permission);
            },
            remove: function (id) {
                return restSrv.one("permission", id).customDELETE();
            },
            update: function (permission) {
                return restSrv.all("permission").customPUT(permission);
            }
        }
    });
});
