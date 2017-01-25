define(['apps/system/system.service'], function (app) {

    app.factory("system_menu_service", function (Restangular, userApiUrl, userApiVersion, $rootScope) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(userApiUrl + userApiVersion);
        })

        return {
            getMenus: function () {
                return restSrv.all("menu").getList({ business: $rootScope.currentBusiness.Key });
            },
            
            addMenu: function (module, parentKey) {
                module.BusinessKey = $rootScope.currentBusiness.Key;
                module.ParentKey = parentKey;
                return restSrv.all("menu").customPOST(module);
            },
            removeMenu: function (key) {
                return restSrv.all("menu").customDELETE("", { Key: key });
            },
            updateMenu: function (module) {
                return restSrv.all("menu").customPUT(module);
            },
            favorites: function (key) {
                return restSrv.one("menu", key).customPUT(undefined, 'favorites');
            },
            removeFavorites: function (key) {
                return restSrv.one("menu", key).customDELETE('favorites');
            }
        }
    });
});
