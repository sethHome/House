define(['angularAMD'], function (angularAMD) {

    var service = angular.module("module_system_service", ["base.service", "restangular", "LocalStorageModule"]);

    service.factory("system_service", function () {

    });
    
    angularAMD.processQueue();

    return service;
});
