define(['apps/system3/production/production.controller'], function (app) {

    app.factory("volumeService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getVolumePlan: function (filter) {
                return restSrv.all("volume").customGET('plan', filter);
            },
            getVolumeProcess: function (filter) {
                return restSrv.all("volume").customGET('process', filter);
            },
            getSpecVolumes: function (engineeringID, specialtyID) {
                return restSrv.all("volume").customGET('engineering/' + engineeringID + "/specialty/" + specialtyID);
            },
            updateVolumes: function (engineeringID,specialtyID ,volumes) {
                return restSrv.all("volume").customPUT(volumes, 'engineering/' + engineeringID + "/specialty/" + specialtyID);
            },
            getVolumeStatisticsInfo: function (id) {
                return restSrv.one("volume", id).customGET('statistics');
            },
        }
    });

});