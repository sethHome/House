define(['apps/base/base.service'],
    function (module) {

        module.factory("permissionCheckService", function ($rootScope, Restangular, userApiUrl, userApiVersion) {

            var _modules = undefined;

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            });

            return {
               
                check: function (key) {
                    return restSrv.one("permission", key).customGET("check", { business: $rootScope.currentBusiness.Key });
                },
            }
        });
    });
