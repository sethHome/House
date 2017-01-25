define(['apps/system3/production/production.controller',
        'apps/system3/production/resource/resource.service'], function (app) {

            app.controller('production.controller.resource.maintain', function ($scope, resourceService, $uibModalInstance, maintainParam) {

                $scope.resourceInfo = maintainParam.entityInfo ? maintainParam.entityInfo : {};

                // 加载工程
                $scope.loadEngineering = function (textFilter) {

                    return resourceService.loadEngSource({ txtfilter: textFilter });
                };


                $scope.save = function () {

                    if ($scope.resourceInfo.ID > 0) {
                        resourceService.update($scope.resourceInfo);

                        $uibModalInstance.dismiss('cancel');

                    } else {
                        $scope.resourceInfo.EngineeringID = $scope.resourceInfo.Engineering.ID;
                        resourceService.create($scope.resourceInfo).then(function (id) {
                            $scope.resourceInfo.ID = id;
                            $uibModalInstance.close($scope.noteInfo);
                        });
                    }
                }

                // 关闭编辑模式
                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });
        });