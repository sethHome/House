define([
    'apps/system3/office/office',
    'apps/system3/office/car/car.service'], function (app) {

        app.module.controller("office.controller.car.maintain", function ($scope, $sce, $stateParams, $uibModal, $timeout, carService) {

            $scope.showAllCar();

            $scope.$watch("currentCar", function (newval, oldval) {
                if (newval) {
                    $scope.carInfo = newval;
                }
            });

            $scope.new = function () {
                $scope.carInfo = {};
            }

            $scope.save = function () {
                $scope.viewPanel.block();
                if ($scope.carInfo.ID > 0) {
                    carService.update($scope.carInfo).then(function (id) {

                        $scope.viewPanel.unblock();
                    });
                } else {
                    carService.create($scope.carInfo).then(function (id) {
                        $scope.carInfo.ID = id;
                        $scope.carInfo.Status = 1;
                        $scope.addCar($scope.carInfo);
                        $scope.viewPanel.unblock();
                    });
                }

            }

            $scope.delete = function () {
                $scope.viewPanel.block();
                if ($scope.carInfo.ID > 0) {
                    carService.remove($scope.carInfo.ID).then(function (id) {

                        $scope.removeCar($scope.carInfo);
                        $scope.carInfo = undefined;
                        $scope.viewPanel.unblock();
                    });
                }
            }

            // 维修保养
            $scope.maintain = function () {
                $scope.viewPanel.block();
                carService.maintain($scope.carInfo.ID).then(function (status) {
                    $scope.carInfo.Status = status;
                    $scope.viewPanel.unblock();
                });
            }

            // 报废
            $scope.scrap = function () {
                $scope.viewPanel.block();
                carService.scrap($scope.carInfo.ID).then(function (status) {
                    $scope.carInfo.Status = status;
                    $scope.viewPanel.unblock();
                });
            }

            // 正常
            $scope.normal = function () {
                $scope.viewPanel.block();
                carService.normal($scope.carInfo.ID).then(function (status) {
                    $scope.carInfo.Status = status;
                    $scope.viewPanel.unblock();
                });
            }

        });
    });
