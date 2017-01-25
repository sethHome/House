define(['apps/system3/business/business',
        'apps/system3/business/engineering/engineering.service',
        'apps/system3/business/project/project.service',
        'apps/system3/business/customer/customer.directive'], function (app) {

            app.module.controller("business.controller.engineering.maintain", function ($scope, engineeringService, projectService, $uibModalInstance, maintainParams) {
                
                $scope.engInfo = maintainParams.entityInfo ? maintainParams.entityInfo : {};

                // 设置工程阶段
                if ($scope.engInfo.ID == undefined) {
                    $scope.engInfo.Phase = maintainParams.phase;
                } else {
                    $scope.chooseProjectInfo = $scope.engInfo.ProjectInfo;
                }

                // 加载项目信息
                $scope.loadProjectByNumber = function (textFilter) {
                    return projectService.loadSource({ number: textFilter });
                }

                $scope.loadProjectByName = function (textFilter) {
                    return projectService.loadSource({ name: textFilter });
                }

                $scope.$watch("chooseProjectInfo", function (newval, oldval) {
                    
                    if (newval && $scope.engInfo.ID == undefined) {

                        if (!$scope.engInfo.Number) {
                            $scope.engInfo.Number = newval.Number;
                        }
                        if (!$scope.engInfo.Name) {
                            $scope.engInfo.Name = newval.Name;
                        }
                        if (!$scope.engInfo.VolLevel) {
                            $scope.engInfo.VolLevel = newval.VolLevel;
                        }
                        if (!$scope.engInfo.Manager) {
                            $scope.engInfo.Manager = newval.Manager;
                        }
                        if (!$scope.engInfo.CreateDate) {
                            $scope.engInfo.CreateDate = newval.CreateDate;
                        }
                        if (!$scope.engInfo.DeliveryDate) {
                            $scope.engInfo.DeliveryDate = newval.DeliveryDate;
                        }
                    }
                });

                // 保存信息
                $scope.save = function () {

                    if ($scope.engInfo.ID > 0) {

                        engineeringService.edit($scope.engInfo).then(function () {

                            $uibModalInstance.close($scope.engInfo);
                        });
                    } else {

                        $scope.engInfo.ProjectID = $scope.chooseProjectInfo.ID;

                        engineeringService.create($scope.engInfo).then(function (id) {

                            $scope.engInfo.ID = id;
                           
                            $uibModalInstance.close($scope.engInfo);
                        });
                    }
                };

                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });

        });
