define(['apps/system3/office/office'], function (app) {

    app.module.factory("carService", function ($rootScope,Restangular, stdApiUrl, stdApiVersion) {
        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getCarInfo: function (id) {
                return restSrv.one("car", id).get();
            },
            getUseInfo: function (id) {
                return restSrv.one("car/use", id).get();
            },
            getMyUse: function (filter) {
                filter.myapply = $rootScope.currentUser.Account.ID;
                return restSrv.all("car").customGET("use", filter);
            },
            getUsedCar: function () {
                var id = $rootScope.currentUser.Account.ID;
                return restSrv.all("car").customGET("used", { user: id });
            },
            getCarList: function (filter) {
                return restSrv.one("car").get(filter);
            },
            create: function (carInfo) {
                return restSrv.all("car").customPOST(carInfo);
            },
            update: function (carInfo) {
                return restSrv.one("car", carInfo.ID).customPUT(carInfo);
            },
            remove: function (id) {
                return restSrv.one("car", id).remove();
            },
            maintain: function (id) {
                // 保养维修
                return restSrv.one("car", id).customPUT({}, "maintain");
            },
            scrap: function (id) {
                // 报废
                return restSrv.one("car", id).customPUT({}, "scrap");
            },
            normal: function (id) {
                // 报废
                return restSrv.one("car", id).customPUT({}, "normal");
            },
            applyCar: function (id,applyInfo) {
                // 车辆申请
                return restSrv.one("car", id).customPOST(applyInfo, "apply");
            }
        }
    });

});
