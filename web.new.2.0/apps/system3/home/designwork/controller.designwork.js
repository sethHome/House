define(['apps/system3/home/home.controller',
    'apps/system3/home/desktop/desktop.service',
    'apps/system3/home/designwork/designwork.service'], function (app) {

        app.controller("home.controller.designwork", function ($scope, $rootScope, $state, $stateParams,desktopService, processService, volumeCheckService) {

            $scope.isDesignModel = false;
            $scope.checkItems = [];

            $scope.$watch('currentTask', function (newval, oldval) {
                if (newval) {
                    $scope.isDesignModel = newval.TaskModelID == '_3';

                    $scope.loadVolumeChecks(newval.Volume.ID);

                    $scope.flowInfo = processService.getFlowDetailByObj('Volume', newval.Volume.ID).$object;
                }
            });
            
            // 设置当前任务
            $scope.setCurrentTask = function (task) {
                $scope.currentTask = task;
            }

            // 加载任务
            $scope.loadTasks = function () {

                desktopService.getMyProductionTasks().then(function (result) {
                    $scope.productionTasks = result;

                    $scope.currentTask = $scope.productionTasks.Source.get(function (task) { return task.ID == $stateParams.id });

                });
            }

            // 加载卷册校审意见
            $scope.loadVolumeChecks = function (id) {

                volumeCheckService.getVolumeChecks(id).then(function (result) {
                    $scope.checkItems = result.Source;
                })
            }

            // 添加校审意见
            $scope.addCheckInfo = function (info) {

                volumeCheckService.addVolumeCheck($scope.currentTask.Volume.ID, info).then(function (checkInfo) {
                    $scope.checkItems.push(checkInfo);
                })
            }

            // 修改校审意见
            $scope.updateCheckInfo = function (info) {

                volumeCheckService.updateVolumeCheck(info.ID, info).then(function () {
                    info.edit = false;
                });
            }

            // 删除校审意见
            $scope.deleteCheckInfo = function (info) {

                volumeCheckService.deleteVolumeCheck(info.ID).then(function () {
                    $scope.checkItems.removeObj(info);
                });
            }

            // 校审通过
            $scope.checkPass = function (info) {

                if (info.IsPass && info.IsCorrect) {
                    updateCheckInfo(info);
                } else {
                    info.IsPass = false;
                }
            }

            // 提交下一步回调
            $scope.check = function (flow) {

                // 回退不检查校审意见
                if (!flow.flowInfo.isBack) {

                    var result = true;

                    angular.forEach($scope.checkItems, function (item) {

                        //  设计检查校审意见
                        if ($scope.isDesignModel) {
                            if (!item.IsCorrect) {
                                result = false;
                                bootbox.alert("所有的校审意见必须修改完成");
                                return false;
                            }
                        } else {
                            if (!item.IsPass) {
                                result = false;
                                bootbox.alert("所有的校审意见必须已通过");
                                return false;
                            }
                        }
                    });

                    if (!result) {
                        return false;
                    }
                }

                flow.callBack(function () {
                    $scope.productionTasks.Source.removeObj($scope.currentTask);

                    $rootScope.$broadcast("$ReloadUserTask");

                    if ($scope.productionTasks.Source.length == 0) {
                        $state.go("home.desktop");
                    } else {
                        $scope.currentTask = $scope.productionTasks.Source[0];
                    }
                });

                return true;
            }

            $scope.loadTasks();
        });

    });
