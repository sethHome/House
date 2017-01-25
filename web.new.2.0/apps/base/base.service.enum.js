define(['apps/base/base.service'],
    function (module) {

        module.factory("enumService", function ($rootScope, Restangular, userApiUrl, userApiVersion) {

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            })

            return {
                get: function (name) {
                    return restSrv.all("enum").customGET(name, { system: $rootScope.currentBusiness.Key });
                },
                all: function () {
                    return restSrv.all("enum").getList();
                }
            }
        });

    });
