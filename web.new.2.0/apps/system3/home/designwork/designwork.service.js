define(['apps/system3/home/home.controller'], function (app) {

    app.factory("volumeCheckService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getVolumeChecks: function (volumeID) {
                return restSrv.one("volume", volumeID).customGET('check');
            },
            addVolumeCheck: function (volumeID, check) {
                return restSrv.one("volume", volumeID).customPOST(check, 'check');
            },
            updateVolumeCheck: function (id, check) {
                return restSrv.all("volume").customPUT(check, 'check/' + id);
            },
            deleteVolumeCheck: function (id) {
                return restSrv.all("volume").customDELETE('check/' + id);
            },
            
        }
    });
});