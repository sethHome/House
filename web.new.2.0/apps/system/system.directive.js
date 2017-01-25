define(['angularAMD'], function (angularAMD) {

    var directive = angular.module("module_system_directive", []);

    angularAMD.processQueue();

    return directive;
});
