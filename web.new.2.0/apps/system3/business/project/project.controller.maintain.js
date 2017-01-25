define(['apps/system3/business/business',
        'apps/system3/business/project/project.service',
        'apps/system3/business/customer/customer.directive'], function (app) {

            app.module.controller("business.controller.project.maintain", function ($scope, projectService, $uibModalInstance, maintainParams) {
              
                $scope.projInfo = maintainParams.entityInfo ? maintainParams.entityInfo : {};
                
                // 保存项目信息
                $scope.save = function () {

                    $scope.projInfo.CustomerID = $scope.projInfo.Customer.ID;

                    if ($scope.projInfo.ID > 0) {

                        projectService.edit($scope.projInfo).then(function () {
                            $uibModalInstance.close($scope.projInfo);
                        });

                    } else {

                        projectService.create($scope.projInfo).then(function (id) {

                            $scope.projInfo.ID = id;

                            $uibModalInstance.close($scope.projInfo);
                        });;
                    }
                };

                // 关闭窗口
                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });
        });
