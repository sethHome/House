define(['apps/system3/production/production.controller',
    'apps/system3/production/engworking/engworking.service'], function (app) {

        app.controller("production.controller.engworking.plan", function ($scope, engworkingService, $uibModal,$uibModalInstance, maintainParams) {

            $scope.engInfo = maintainParams.entityInfo ? maintainParams.entityInfo : {};

            $scope.planInfo = engworkingService.getEngPlan($scope.engInfo.ID).$object;

            // 拷贝专业信息
            var Specialtys = $scope.getBaseData("Specialty");
            $scope.SpecialtysCopy = angular.copy(Specialtys);

            var loadSpecils = function () {
                // 获取工程已策划专业
                engworkingService.getEngineeringSpecils($scope.engInfo.ID).then(function (data) {

                    $scope.Specialtys = data;

                    angular.forEach($scope.SpecialtysCopy, function (item) {
                        item.Manager = undefined;
                        item.ProcessModel = undefined;
                        item.StartDate = undefined;
                        item.EndDate = undefined;
                        item.Note = undefined;

                        var obj = data.find(function (s) {
                            return s.SpecialtyID == item.Value
                        });

                        if (obj) {
                            item.Manager = obj.Manager;
                            item.ProcessModel = obj.ProcessModel;
                            item.StartDate = obj.StartDate;
                            item.EndDate = obj.EndDate;
                            item.Note = obj.Note;
                        }

                        item.isSelected = obj != undefined;

                    });

                    angular.forEach($scope.Specialtys, function (spec) {

                        var Process = $scope.ProcessModel_enum[spec.ProcessModel];

                        // 卷册表格参数
                        spec.Tasks = Process.Tasks;
                        spec.ProcessID = Process.Key;
                        spec.newVolumeCount = 0;
                        spec.newVolumes = [];

                        // 获取专业的卷册
                        loadVolumes(spec);

                    });
                });

            }

            // 获取专业的卷册
            var loadVolumes = function (spec,hander) {

                engworkingService.getSpecVolumes($scope.engInfo.ID, spec.SpecialtyID).then(function (result) {

                    // 获取卷册流程各个节点的信息
                    angular.forEach(result, function (volume) {
                        volume.CanDelete = volume.Tasks.count(function (task) { return task.Status == 2; }) == 0;
                    });

                    spec.Volumes = result;

                    if (hander) {
                        hander();
                    }
                });
            }

            // 添加卷册
            $scope.addVolume = function (s) {
                $uibModal.open({
                    animation: false,
                    size: 'md',
                    templateUrl: 'apps/system3/production/engworking/view/vol-maintain.html',
                    controller: 'production.controller.engworking.plan.volume',
                    resolve: {
                        maintainParams: function () {
                            return {
                                eng: $scope.engInfo,
                                spec: s,
                                addVolume: function (info) {

                                    s.newVolumeCount++;
                                    s.saved = undefined;
                                    s.beforeSaved = undefined;

                                    info.flage = true;
                                    info.CanDelete = true;
                                    info.Designer = info.TaskUsers[0].User;
                                    info.Checker = info.TaskUsers[1].User;

                                    s.newVolumes.push(info);

                                    
                                }
                            }
                        }
                    }
                });
            }

            // 更新卷册
            $scope.updateVolume = function (s,v) {
                $uibModal.open({
                    animation: false,
                    size: 'md',
                    templateUrl: 'apps/system3/production/engworking/view/vol-maintain.html',
                    controller: 'production.controller.engworking.plan.volume',
                    resolve: {
                        maintainParams: function () {
                            return {
                                eng: $scope.engInfo,
                                spec: s,
                                vol : v,
                            }
                        }
                    }
                }).result.then(function (info) {
                    bootbox.alert("卷册更新成功");

                    s.saved = undefined;
                    s.beforeSaved = undefined;

                    loadVolumes(s);
                });
            }

            // 删除卷册
            $scope.deleteVolume = function (s,v) {

                if (v.flage) {
                    s.Volumes.removeObj(v);
                } else {
                    bootbox.confirm($scope.local.msg.confirmDelete, function (result) {
                        if (result === true) {
                            engworkingService.deleteVolumes(v.ID).then(function () {

                                s.Volumes.removeObj(v);

                                bootbox.alert("卷册删除成功");

                                s.saved = undefined;
                                s.beforeSaved = undefined;
                            });
                        }
                    });
                }
            }

            $scope.close = function () {
                $uibModalInstance.dismiss('cancel');
            }

            // 保存专业信息
            var saveSpecialtys = function () {
                var result = [];

                angular.forEach($scope.SpecialtysCopy, function (item) {
                    if (item.isSelected) {
                        item.SpecialtyID = item.Value;
                        result.push(item);
                    }
                });

                engworkingService.saveEngineeringSpecils($scope.engInfo.ID, result).then(function () {
                   
                    

                    loadSpecils();

                    bootbox.alert("工程策划保存成功！");
                })
            };

            // 批量添加每个专业卷册
            var saveVolumes = function () {

                angular.forEach($scope.Specialtys, function (spec) {
                    spec.isOpen = false;

                    if (spec.newVolumeCount > 0) {

                        spec.beforeSaved = true;

                        engworkingService.batchAddVolumes($scope.engInfo.ID, spec.SpecialtyID, spec.newVolumes).then(function () {
                            spec.saved = true;
                            spec.beforeSaved = false;

                            loadVolumes(spec, function () {
                                spec.newVolumeCount = 0;
                                spec.newVolumes = [];
                            });
                        })
                    }
                });
            };

            // 保存工程要求
            var saveEngPlan = function () {
                $scope.planInfo.EngineeringID = $scope.engInfo.ID;

                if ($scope.planInfo.ID > 0) {
                    engworkingService.updateEngPlan($scope.planInfo).then(function (result) {
                        bootbox.alert("更新成功");
                    })
                } else {
                    engworkingService.addEngPlan($scope.planInfo).then(function (result) {
                        $scope.planInfo.ID = result;
                        bootbox.alert("保存成功");
                    })
                }
            }

            $scope.save = function () {
                switch ($scope.currentIndex) {
                    case 1: {
                        saveEngPlan();
                        break;
                    }
                    case 2: {
                        saveSpecialtys();
                        break;
                    }
                    case 3: {
                        saveVolumes();
                        break;
                    }
                }
            }


            loadSpecils();
        });
        
        app.controller("production.controller.engworking.plan.volume", function ($scope, engworkingService, $uibModalInstance, maintainParams) {
            
            $scope.Process = $scope.ProcessModel_enum[maintainParams.spec.ProcessModel];
            $scope.spec = maintainParams.spec;
            $scope.spec.ProcessID = $scope.Process.Key;

            $scope.volInfo = maintainParams.vol ? maintainParams.vol : {};
            $scope.volInfo.EngineeringID = $scope.spec.EngineeringID;
            $scope.volInfo.SpecialtyID = $scope.spec.SpecialtyID;
           
            if ($scope.volInfo.TaskUsers == undefined) {
                if ($scope.volInfo.Tasks) {
                    $scope.volInfo.TaskUsers = $scope.volInfo.Tasks.map(function (task) {
                        return {
                            Name: task.Name,
                            ID: task.ID,
                            Owner: task.Owner,
                            User: task.UserID,
                            Status: task.Status
                        }
                    })
                } else {
                    $scope.volInfo.TaskUsers = $scope.Process.Tasks.map(function (task) {
                        return {
                            Name: task.Name,
                            ID: task.ID,
                            Owner: task.Owner,
                        }
                    })
                }
            }
            
            $scope.addAndClose = function () {

                maintainParams.addVolume(angular.copy($scope.volInfo));

                $scope.volInfo.Name = undefined;
                $scope.volInfo.Number = undefined;
                $scope.volInfo.Note = undefined;

                $uibModalInstance.dismiss('cancel');
            }

            // 保存卷册信息
            $scope.save = function () {

                //$scope.thisPanel.block();

                if ($scope.volInfo.ID > 0) {
                    $scope.modalPanel.block();
                    engworkingService.updateVolumes($scope.volInfo.ID, $scope.volInfo).then(function (id) {
                        $scope.modalPanel.unblock();
                        $uibModalInstance.close();
                    })

                } else {
                    
                    maintainParams.addVolume(angular.copy($scope.volInfo));

                    $scope.volInfo.Name = undefined;
                    $scope.volInfo.Number = undefined;
                    $scope.volInfo.Note = undefined;

                    //engworkingService.createVolumes($scope.volInfo).then(function (id) {
                    //    $scope.volInfo.ID = id;

                    //    $uibModalInstance.close();
                    //})
                }

            }

            $scope.close = function () {
                $uibModalInstance.dismiss('cancel');
            }
        });

        app.controller("production.controller.engworking.stop", function ($scope, engworkingService, $uibModal, $uibModalInstance, maintainParams) {

            $scope.close = function () {
                $uibModalInstance.dismiss('cancel');
            }
           
            $scope.stop = function () {
                engworkingService.stopEng(maintainParams.entityInfo.ID, $scope.noteInfo).then(function () {
                    $uibModalInstance.close(maintainParams.entityInfo);
                });
            }
        });

    });
