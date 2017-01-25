define(['apps/system3/production/production.controller'], function (app) {

    app.factory("engworkingService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            // 获取启动工程
            getEngineerings: function (filter) {
                return restSrv.one("engineering").get(filter);
            },
            // 获取工程已策划的专业
            getEngineeringSpecils: function (id) {
                return restSrv.one("engineering", id).customGET('specialty');
            },
            // 获取专业卷册
            getSpecVolumes: function (engineeringID, specialtyID) {
                return restSrv.all("volume").customGET('engineering/' + engineeringID + "/specialty/" + specialtyID);
            },
            // 保存策划的工程专业
            saveEngineeringSpecils: function (engineeringID, specialtys) {
                return restSrv.all("specialty").customPUT(specialtys, 'engineering/' + engineeringID);
            },
            // 保存工程要求
            addEngPlan: function (plan) {
                return restSrv.all("engineeringplan").customPOST(plan);
            },
            // 获取工程要求
            getEngPlan: function (id) {
                return restSrv.one("engineeringplan", id).get();
            },
            updateEngPlan: function (plan) {
                return restSrv.one("engineeringplan", plan.ID).customPUT(plan);
            },
            createVolumes: function (volume) {
                return restSrv.all("volume").customPOST(volume);
            },
            updateVolumes: function (id, volume) {
                return restSrv.one("volume", id).customPUT(volume);
            },
            deleteVolumes: function (id) {
                return restSrv.one("volume", id).customDELETE();
            },
            batchAddVolumes: function (engineeringID, specialtyID, volumes) {
                return restSrv.all("volume").customPOST(volumes, 'batch');
            },
            stopEng: function (id,note) {
                return restSrv.one("engineering", id).customPUT(note, 'stop');
            },
            startEng: function (id, note) {
                return restSrv.one("engineering", id).customPUT(note, 'start');
            },
            follow: function (id) {
                return restSrv.one("engineering", id).customPUT(undefined,'follow');
            },
            unfollow: function (id) {
                return restSrv.one("engineering", id).customPUT(undefined, 'unfollow');
            }
        }
    });
});