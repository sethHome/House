define(['apps/system/system.service'], function (app) {

    app.factory("system_enum_service", function (Restangular, userApiUrl, userApiVersion,$rootScope) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(userApiUrl + userApiVersion);
        })

        return {
            all: function () {
                return restSrv.all("enum").getList({"system" : $rootScope.currentBusiness.Key});
            },
            // enum
            addEnum: function (data) {
                return restSrv.all("enum").customPOST(data, $rootScope.currentBusiness.Key);
            },
            updateEnum: function (data) {
                return restSrv.all("enum").customPUT(data, $rootScope.currentBusiness.Key);
            },
            deleteEnum: function (name) {
                return restSrv.one("enum", name).customDELETE($rootScope.currentBusiness.Key);
            },

            // item
            addItem: function (name, data) {
                return restSrv.all("enum/" + name + '/item').customPOST(data, $rootScope.currentBusiness.Key);
            },
            updateItem: function (name, data) {
                return restSrv.all("enum/" + name + '/item').customPUT(data, $rootScope.currentBusiness.Key);
            },
            deleteItem: function (name, value) {
                return restSrv.one("enum/" + name + '/item', value).customDELETE($rootScope.currentBusiness.Key);
            },

            // tag
            addTag: function (name, value, data) {
                return restSrv.all("enum/" + name + '/item/' + value + '/tag').customPOST(data, $rootScope.currentBusiness.Key);
            },
            updateTag: function (name, value, data) {
                return restSrv.all("enum/" + name + '/item/' + value + '/tag').customPUT(data, $rootScope.currentBusiness.Key);
            },
            deleteTag: function (name, value, key) {
                return restSrv.one("enum/" + name + '/item/' + value + '/tag', key).customDELETE($rootScope.currentBusiness.Key);
            }
        }
    });
});
