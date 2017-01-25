define(['apps/base/base.service'],
    function (module) {

        module.factory("processService", function ($rootScope,Restangular, userApiUrl, userApiVersion) {

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            });

            return {


                create: function (name, data) {
                    return restSrv.all("process").customPOST(data, 'create/' + name);
                },
                next: function (taskID, data) {

                    return restSrv.all("process/" + $rootScope.currentBusiness.Key).customPUT(data, 'next/' + taskID);
                },
                getModels: function () {
                    return restSrv.all("process").customGET('model');
                },
             
                getFlowInitInfo: function (name, params) {
                    return restSrv.one("flow", name).customPOST(params, 'init');
                },
                getFlowTaskInfo: function (ID, params) {
                    return restSrv.one("flow/" + $rootScope.currentBusiness.Key + "/task", ID).customPOST(params);
                },
                getCurrentTaskName: function (key, id) {
                    return restSrv.one("flow" , $rootScope.currentBusiness.Key).customGET('info', { key: key, id: id });
                },
                getFlowDetailByID: function (id) {
                    return restSrv.one("flow", $rootScope.currentBusiness.Key).customGET('detail', {
                        id: id,
                    });
                },
                getFlowDetailByObj: function (objkey, objid) {
                    return restSrv.one("flow", $rootScope.currentBusiness.Key).customGET('detail', {
                        objkey: objkey,
                        objid: objid
                    });
                },
                getFlowUsers: function () {
                    return restSrv.all("flow").customGET('user');
                }
            }
        });
    });
