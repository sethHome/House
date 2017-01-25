define(['angularAMD',
    'angular-ui-grid','apps/system/system.service'], function (angularAMD) {

        var controller = angular.module("module_system_controller",
            ['ui.grid',
            'ui.grid.resizeColumns',
            'ui.grid.selection',
            'ui.grid.treeView',
            'ui.grid.edit',
            'ui.grid.cellNav',
            'module_system_service',
            'base.service']);

        controller.controller("system.controller.sidebar", ['$scope', function ($scope) {
           
        }]);

        controller.controller("system.controller.content", function ($scope,$rootScope) {
            
           
        });

        controller.controller("system.controller.header", function ($scope, $rootScope, system_business_service, system_user_service) {

            var loadBusiness = function (userID) {
                if (userID) {
                    system_user_service.getBusiness(userID).then(function (result) {

                        $scope.business = result;
                        $rootScope.currentBusiness = result[0];
                    });
                    
                }
                else {
                    system_business_service.getBusiness({ withuser: false }).then(function (result) {

                        $scope.business = result;
                        $rootScope.currentBusiness = result[0];
                    });
                }
                
            }

            $scope.changeBusiness = function (b) {
                $rootScope.currentBusiness = b;
            }

            $scope.$watch("currentBusiness", function (newVal, oldVal) {
                $rootScope.$broadcast("businessChanged", newVal);
            });

            //$rootScope.$on("$UserSelected", function (e, val) {
                
            //    loadBusiness(val.ID);
            //});

            loadBusiness();

        });

        angularAMD.processQueue();

        return controller;
    });
