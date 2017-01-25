define(['apps/system/system.service'],
    function (module) {

        module.factory("system_module_service", function ($rootScope, Restangular, userApiUrl, userApiVersion) {

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            });

            return {
                getModules: function (key) {
                    return restSrv.all("module").getList({ business: (key ? key : $rootScope.currentBusiness.Key) });
                },
                addModule: function (module, parentKey) {
                    module.BusinessKey = $rootScope.currentBusiness.Key;
                    module.ParentKey = parentKey;
                    return restSrv.all("module").customPOST(module);
                },
                removeModule: function (key) {
                    return restSrv.all("module").customDELETE("", { Key: key });
                },
                updateModule: function (module) {
                    return restSrv.all("module").customPUT(module);
                },
            }
        });
    });
