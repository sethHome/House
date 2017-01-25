define(['apps/system3/production/production.controller',
    'apps/system3/production/note/note.controller.maintain',
    'apps/system3/production/production.directive',
    'apps/system3/production/work/work.service'], function (app) {

        app.controller("production.controller.work", function ($scope, $state, $stateParams, workService, $uibModal) {

            $scope.currentTaskID = $stateParams.id;
            $scope.engUsers = [];

            workService.getTask($stateParams.id).then(function (result) {
                $scope.currentTask = result;
                $scope.isDesignModel = $scope.currentTask.TaskModelID == '_3';
            });

            workService.getVolumeInfo($stateParams.id).then(function (result) {
                $scope.volumeInfo = result;

                loadVolumeChecks(result.ID);

                loadEngNote(result.EngineeringID);

                loadEngPlan(result.EngineeringID);

                $scope.volumeInfo.Engineering.IsFollow = $scope.userConfig.engFollows.contains(function (f) { return f.ConfigKey == result.EngineeringID.toString(); });
            });

            // 取消、关注工程
            $scope.followEng = function () {
                if ($scope.volumeInfo.Engineering.IsFollow) {
                    workService.unfollow($scope.volumeInfo.Engineering.ID).then(function (result) {
                        $scope.volumeInfo.Engineering.IsFollow = false;
                    })
                } else {
                    workService.follow($scope.volumeInfo.Engineering.ID).then(function (result) {
                        $scope.volumeInfo.Engineering.IsFollow = true;
                    })
                }
               
            }

            // 加载工程要求
            var loadEngPlan = function (id) {
                workService.getEngPlan(id).then(function (result) {
                    $scope.engPlan = result;
                })
            }

            // 加载工程记事
            var loadEngNote = function (id) {
                workService.getEngNote(id).then(function (result) {
                    $scope.engNotes = result;
                })
            }

            // 加载卷册校审意见
            var loadVolumeChecks = function (id) {

                workService.getVolumeChecks(id).then(function (result) {
                    $scope.checkItems = result.Source;
                })
            }


            $scope.addEngNote = function () {
                $uibModal.open({
                    animation: false,
                    size: 'lg',
                    templateUrl: 'apps/system3/production/note/view/note-maintain.html',
                    controller: 'production.controller.note.maintain',
                    resolve: {
                        maintainParam: function () {
                            return {
                                engID: $scope.volumeInfo.EngineeringID
                            }
                        }
                    }
                }).result.then(function (info) {
                    $scope.engNotes.push(info);
                });
            }

            // 添加校审意见
            $scope.addCheckInfo = function (info) {

                workService.addVolumeCheck($scope.volumeInfo.ID, info).then(function (checkInfo) {
                    $scope.checkItems.push(checkInfo);
                })
            }

            // 修改校审意见
            $scope.updateCheckInfo = function (info) {

                workService.updateVolumeCheck(info.ID, info).then(function () {
                    info.edit = false;
                });
            }

            // 删除校审意见
            $scope.deleteCheckInfo = function (info) {

                workService.deleteVolumeCheck(info.ID).then(function () {
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
                    if ($scope.isDesignModel) {
                        if ($scope.checkItems.count(function (item) { return !item.IsCorrect; }) > 0) {
                            bootbox.alert("所有的校审意见必须修改完成");
                            return false;
                        }
                    } else {
                        if ($scope.checkItems.count(function (item) { return !item.IsPass; }) > 0) {
                            bootbox.alert("所有的校审意见必须已通过");
                            return false;
                        }
                    }
                }

                flow.callBack(function () {

                    //$rootScope.$broadcast("$ReloadUserTask");

                    $scope.closeCurrentTab();

                });

                return true;
            }
        });

    });
