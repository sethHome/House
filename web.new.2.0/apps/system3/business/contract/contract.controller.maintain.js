define(['apps/system3/business/business',
        'apps/system3/business/contract/contract.service',
        'apps/system3/business/engineering/engineering.service',
        'apps/system3/business/customer/customer.directive'], function (app) {

            app.module.controller("business.controller.contract.maintain", function ($scope, contractService, engineeringService,$uibModalInstance, maintainParams) {

                $scope.conInfo = maintainParams.entityInfo ? maintainParams.entityInfo : { Engineerings: []};

                $scope.engIDs = [];

                $scope.$watch("chooseEngineeringInfo", function (newval, oldval) {
                    if (newval && newval.ID) {

                        if ($scope.engIDs.indexOf(newval.ID) == -1) {
                            $scope.conInfo.Engineerings.push(newval);
                            $scope.engIDs.push(newval.ID);
                        }

                        $scope.chooseEngineeringInfo = undefined;
                    }
                });

                // 加载工程
                $scope.loadEngineering = function (textFilter) {

                    return engineeringService.loadSource({ txtfilter: textFilter, exceptids: $scope.engIDs.join(',') });
                };

                // 移除选择的工程
                $scope.removeEngineering = function (e) {
                    $scope.conInfo.Engineerings.removeObj(e);
                    $scope.engIDs.removeObj(e.ID);
                };

                // 页面重置
                $scope.reset = function () {
                    $scope.conInfo.Engineerings = [];
                    $scope.engIDs = [];
                };

                // 保存项目信息
                $scope.save = function () {

                    $scope.conInfo.CustomerID = $scope.conInfo.Customer.ID;

                    if ($scope.conInfo.ID > 0) {

                        contractService.edit($scope.conInfo).then(function () {
                            $uibModalInstance.close($scope.conInfo);
                        });
                    } else {

                        contractService.create($scope.conInfo).then(function (id) {
                            $scope.conInfo.ID = id;
                            $uibModalInstance.close($scope.conInfo);
                        });
                    }
                };

                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });

        });
