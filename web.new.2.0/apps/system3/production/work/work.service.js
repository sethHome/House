define(['apps/system3/production/production.controller'], function (app) {

    app.factory("workService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getTask: function (id) {
                return restSrv.one("task", id).get();
            },
            getVolumeInfo: function (id) {
                return restSrv.one("task", id).customGET('volume');
            },
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
            getEngNote: function (id) {
                return restSrv.one("engineering", id).customGET('note');
            },
            getEngPlan: function (id) {
                return restSrv.one("engineeringplan", id).get();
            },
            follow: function (id) {
                return restSrv.one("engineering", id).customPUT(undefined, 'follow');
            },
            unfollow: function (id) {
                return restSrv.one("engineering", id).customPUT(undefined, 'unfollow');
            }
        }
    });


});
