define(['apps/base/base.service'],
    function (module) {

        module.factory("settingsService", function (Restangular, userApiUrl, userApiVersion) {

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            });

            return {
                getSettings: function (filter, objectKey) {
                    return restSrv.all("settings").customGET("dic");
                },
                
            }
        });
    });
