define(['angularAMD',
    'css!apps/system3/production/css/production',
    'ngload!apps/system3/business/business',

    'apps/system3/business/engineering/engineering.directive'], function (angularAMD) {

        var controller = angular.module("module_production_controller",
            ['base.service',
            'module_business']);

        controller.controller("production.sidebar.controller", ['$scope', function ($scope) {
          
        }]);

        controller.controller("production.content.controller", function ($scope, $state, $rootScope) {
            if ($state.current.name == "production") {
                $state.go("production.engworking");
            }
        });

        controller.controller("production.header.controller", function ($scope, $rootScope) {


        });

        angularAMD.processQueue();

        return controller;
    });
