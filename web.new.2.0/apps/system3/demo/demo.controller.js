define(['angularAMD'], function (angularAMD) {

    var controller = angular.module("module_demo_controller", []);

    controller.controller("demo.sidebar.controller", ['$scope', function ($scope) {

    }]);
    controller.controller("demo.content.controller", ['$scope', function ($scope) {

    }]);
    controller.controller("demo.header.controller", ['$scope', function ($scope) {

    }]);
    angularAMD.processQueue();

    return controller;
});
