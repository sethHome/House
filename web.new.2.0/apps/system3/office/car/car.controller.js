define([
'apps/system3/office/office',
'apps/system3/office/car/car.service'], function (app) {

    app.module.controller("office.controller.car", function ($scope, $sce, $state, $stateParams, $uibModal, $timeout, carService) {
        
        // 所有的车辆
        $scope.allCars = undefined;

        // 申请的车辆
        $scope.applyCars = undefined;

        $scope.currentStateName = $state.current.name;

        $scope.$watch("currentCar", function (newval, oldval) {
            if (newval) {
                $scope.viewScroll.init();
            }
        });

        // 显示正常可用的车辆
        $scope.showNormalCar = function () {
         
            $scope.showAllCar(true)
        }

        // 显示所有车辆
        $scope.showAllCar = function (isNormal) {
            $scope.normal = isNormal == true;

            if ($scope.allCars == undefined) {
                carService.getCarList().then(function (result) {

                    $scope.allCars = result.Source;
                    $scope.carList = result.Source;

                    if ($stateParams.id > 0) {
                        angular.forEach(result.Source, function (n) {
                            if (n.ID == $stateParams.id) {
                                $scope.setcurrentCar(n);
                            }
                        });
                    }
                });
            } else {
                $scope.carList = $scope.allCars;
            }
        }

        // 显示我申请的车辆
        $scope.showMyApplyCar = function () {

            $scope.normal = false;

            if ($scope.applyCars == undefined) {
                carService.getUsedCar().then(function (result) {
                    $scope.applyCars = result;
                    $scope.carList = result;

                    if ($stateParams.id > 0) {
                        angular.forEach($scope.carList, function (n) {
                            if (n.ID == $stateParams.id) {
                                $scope.setcurrentCar(n);
                            }
                        });
                    }
                });
            } else {
                $scope.carList = $scope.applyCars;
            }
        }


        $scope.setCurrentCar = function (car) {
            $scope.currentCar = car;
            $scope.currentCar.HtmlContent = $sce.trustAsHtml($scope.currentCar.Content);
            $scope.viewScroll.init();
        }

        $scope.addCar = function (n) {
            $scope.allCars.push(n);
        }

        $scope.removeCar = function (n) {
            $scope.allCars.removeObj(n);
        }

        $scope.afterApply = function () {
            
            $scope.currentCar.Status = 5;
            console.log($scope.currentCar)
        }

        

    });
});
