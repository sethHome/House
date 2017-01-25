define(['apps/system3/production/production.controller',
        'apps/system3/production/specialty/specialty.service',
        'apps/system3/production/provide/provide.service'], function (app) {


            app.controller('production.controller.provide.maintain', function ($scope, maintainParams, $uibModalInstance) {

                $scope.windowProvide = maintainParams.provideInfo;

                if (maintainParams.task) {
                    $scope.taskID = maintainParams.task.ID;
                }

                $scope.receiveUsers = [];
                $scope.volumeFiles = [];
                $scope.currentVolume = {};

                // 新建提资
                $scope.newProvide = function (provide) {

                    $scope.readOnly = false;

                    $scope.provideInfo = {
                        Engineering: provide.Engineering,
                        EngineeringID: provide.Engineering.ID,
                        SendSpecialtyID: provide.Specialty.SpecialtyID,
                        ReceiveUserIDs: "",
                        DocName: "",
                        DocContent: "",
                        AttachIDs: [],
                        VolumeFiles: []
                    };

                    $scope.flowData = {
                        EngineeringID: provide.Engineering.ID,
                        SpecialtyID: provide.Specialty.SpecialtyID
                    };

                    // 新增加载专业的卷册文件
                    $scope.loadVolumeFiles();
                }

                // 编辑提资
                $scope.maintainProvide = function (provide, task) {

                    $scope.readOnly = !$scope.taskID;

                    $scope.provideInfo = provide;

                    if (!$scope.readOnly) {

                        if (provide.ReceiveUserIDs.length > 0) {

                            var str = ',' + provide.ReceiveUserIDs + ',';

                            angular.forEach($rootScope.user_item, function (user) {

                                if (str.indexOf(',' + user.ID + ',') >= 0) {
                                    $scope.receiveUsers.push({
                                        ID: user.ID,
                                        Name: user.Name,
                                        Dept: user.Dept,
                                        PhotoImg: user.PhotoImg
                                    });
                                }
                            });
                        }

                        // 新增加载专业的卷册文件
                        $scope.loadVolumeFiles();
                    }
                }

                // 获取卷册文件
                $scope.loadVolumeFiles = function () {
                    console.log(111)
                    //$scope.providePanel.block();
                    specialtyService.getFiles($scope.provideInfo.EngineeringID, $scope.provideInfo.SendSpecialtyID).then(function (data) {
                        $scope.volumeFiles = data;
                        $scope.currentVolume = data[0];
                        //$scope.providePanel.unblock();
                    });
                }

                // 选择卷册
                $scope.volChanged = function (v) {
                    $scope.currentVolume = v;
                }

                // 全选卷册文件
                $scope.selectAll = function (vol) {
                    angular.forEach(vol.Files, function (f) {
                        f.selected = vol.selected;
                    });
                }

                // 获取选中卷册文件的数量
                $scope.getSelectedFileCount = function (files) {
                    var count = 0;
                    angular.forEach(files, function (f) {
                        if (f.selected) {
                            count++;
                        }
                    });
                    return count;
                }

                // 保存
                $scope.save = function (flow) {

                    $scope.providePanel.block();

                    if ($scope.provideInfo.ID > 0) {
                        // 编辑
                        provideService.update($scope.provideInfo).then(function () {
                            flow.callBack(function () {
                                $scope.providePanel.unblock();

                                if ($scope.afterSave) {
                                    $scope.afterSave();
                                } else {
                                    $scope.loadSource();
                                }
                            });
                        });
                    } else {

                        // 新增
                        $scope.provideInfo.ApproveUser = flow.taskInfo.user;

                        provideService.create($scope.provideInfo).then(function () {

                            // 通知附件预览控件更新附件列表
                            if ($scope.attachChangedCB) {
                                $scope.attachChangedCB();
                            }

                            flow.callBack(function () {
                                $scope.providePanel.unblock();

                                if ($scope.afterSave) {
                                    $scope.afterSave();
                                } else {
                                    $scope.loadSource();
                                }
                            });
                        });
                    }

                    return true;
                }


                $scope.$watchCollection("receiveUsers", function (newval, oldval) {
                    if (newval && $scope.provideInfo) {

                        if (newval.length == 0) {
                            $scope.provideInfo.ReceiveUserIDs = undefined;
                        } else {
                            var ids = [];
                            angular.forEach(newval, function (user) {
                                ids.push(user.ID);
                            })
                            $scope.provideInfo.ReceiveUserIDs = ids.join(',');
                        }
                    }
                });

                $scope.$watch("volumeFiles", function (newval, oldval) {
                    if (newval && $scope.provideInfo) {
                        $scope.provideInfo.VolumeFiles = [];
                        angular.forEach(newval, function (vol) {
                            angular.forEach(vol.Files, function (f) {
                                if (f.selected) {
                                    $scope.provideInfo.VolumeFiles.push(f.ID);
                                }
                            });
                        });
                    }
                }, true);

                $scope.$watch("currentProvide", function (newval, oldval) {
                    if (newval) {
                        if (newval.ID > 0) {
                            $scope.maintainProvide(newval);
                        } else {
                            $scope.newProvide(newval);
                        }
                    }
                });

                $scope.$on("newProvide", function (e, data) {
                    $scope.newProvide(data);
                })

                // 附件上传完成后回调
                $scope.attachUploaded = function (id, attachChangedCB) {

                    // 新建的模式下，先保存上传的附件ID，当信息被保存后，在保存该附件ID
                    $scope.provideInfo.AttachIDs.push(id);

                    $scope.attachChangedCB = attachChangedCB;
                };

                // 变更信息
                if ($scope.windowProvide) {
                    if ($scope.windowProvide.ID > 0) {
                        $scope.maintainProvide($scope.windowProvide)
                    } else {
                        $scope.newProvide($scope.windowProvide)
                    }

                }

                
                
                // 关闭编辑模式
                $scope.closeModal = function () {
                    $uibModalInstance.dismiss('cancel');
                }

            });
        });