define([
    'apps/system3/office/office',
    'apps/system3/office/car/car.service'], function (app) {

        app.module.controller("office.controller.car.apply", function ($scope, $stateParams, $uibModal, $timeout, carService) {
           
            $scope.carUseInfo = {};
            $scope.showNormalCar();
        });

        app.module.controller("carUseModalCtl", function ($scope, carService, modelParam, $uibModalInstance) {

            $scope.taskID = modelParam.taskID;
            $scope.objectID = modelParam.objID
            $scope.modelType = 'window';

            $scope.afterApply = function () {
                $uibModalInstance.close();
            }
        });

        app.module.controller("carUseCtl", function ($scope, $stateParams, $uibModal, $timeout, carService) {
           
            if ($scope.objectID) {
                //$scope.readOnly = true;
                carService.getUseInfo($scope.objectID).then(function (result) {
                    $scope.carUseInfo = result;
                    $scope.carInfo = carService.getCarInfo(result.CarID).$object;
                });
            }
            

            $scope.$watch("currentCar", function (newval, oldval) {
                if (newval) {
                    $scope.carInfo = newval;
                }
            });

            $scope.$watchCollection("PeerStaff_Users", function (newval, oldval) {
                if (newval) {

                    var ids = [];
                    angular.forEach(newval, function (user) {
                        ids.push(user.ID);
                    });
                    
                    $scope.carUseInfo.PeerStaff = ids.join(',');
                }
            });

            $scope.save = function (flow) {

                if ($scope.viewPanel == undefined) {
                    $scope.viewPanel = $scope.$parent.panel;
                }

                $scope.viewPanel.block();

                if ($scope.objectID) {
                    flow.callBack(function () {
                        $scope.viewPanel.unblock();
                        $scope.afterApply();
                    });
                } else {

                    // 新增
                    $scope.carUseInfo.FlowData = flow.flowInfo;

                    carService.applyCar($scope.carInfo.ID,$scope.carUseInfo).then(function () {

                        flow.callBack(function () {
                            $scope.viewPanel.unblock();
                           
                            $scope.afterApply();
                            
                        });
                    });
                }

                return true;
            }
            
        });
        
    });
