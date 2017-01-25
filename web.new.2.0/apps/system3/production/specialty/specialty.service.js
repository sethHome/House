define(['apps/system3/production/production.controller'], function (app) {

    app.factory("specialtyService", function (Restangular, stdApiUrl, stdApiVersion,  $rootScope) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getSpecialtys: function (filter) {
                return restSrv.all("specialty").customGET('engineering', filter);
            },
            getSpecialtysByEngID: function (id) {
                return restSrv.one("engineering", id).customGET('specialty');
            },
            updateSpecialtys: function (engineeringID, specialtys) {
                return restSrv.all("specialty").customPUT(specialtys, 'engineering/' + engineeringID);
            },
            getFiles: function (engineeringID, specialtyID) {
                return restSrv.one("engineering", engineeringID).customGET("specialty/" + specialtyID + '/file', {
                    user: $rootScope.currentUser.Account.ID
                });
            },
            getVolumes: function (engineeringID, specialtyID) {
                return restSrv.one("engineering", engineeringID).customGET("specialty/" + specialtyID + '/volume');
            }
        }
    });

});