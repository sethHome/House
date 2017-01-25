define(['apps/system3/production/production.controller',
        'apps/system3/production/volume/volume.service'], function (app) {

            app.factory("volumePlanScope", function () {
                return {
                    init: function ($scope, $uibModalInstance, volumeService, volPlanParams) {

                        if (volPlanParams) {
                            $scope.planVolume = volPlanParams.volumeInfo;

                            $scope.Process = $scope.ProcessModel_enum[volPlanParams.volumeInfo.Specialty.ProcessModel];

                        } else {
                            $scope.$watch("currentVolume", function (newval, oldval) {
                                if (newval) {
                                    $scope.planVolume = newval;

                                    $scope.Process = $scope.ProcessModel_enum[newval.Specialty.ProcessModel];
                                    
                                    $scope.addColDef();

                                    $scope.loadVolumes();
                                }
                            })
                        }

                        // 加载卷册列表
                        $scope.loadVolumes = function () {

                            if ($scope.planVolume == undefined) {
                                return;
                            }

                            volumeService.getSpecVolumes($scope.planVolume.Engineering.ID,
                                $scope.planVolume.Specialty.SpecialtyID).then(function (result) {

                                    // 获取卷册流程各个节点的信息
                                    angular.forEach(result, function (volume) {
                                        volume.CanDelete = true;

                                        angular.forEach($scope.Process.Tasks, function (task) {

                                            var item = volume.Tasks.get(function (t) { return t.SourceID == task.ID });

                                            volume[task.Owner] = item.UserID;
                                            volume[task.Owner + "_Status"] = item.Status;

                                            if (item.Status == 2) {
                                                volume.CanDelete = false;
                                            }
                                        });
                                    });

                                    $scope.Volumes = result;
                                });
                        }

                        // 卷册表格参数
                        $scope.volOptions = {
                            data: "Volumes",
                            enableCellEditOnFocus: true,
                            enableRowHeaderSelection: true,
                            cellEditableCondition: true,
                            onRegisterApi: function (gridApi) {

                                //gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
                                //    gridApi.selection.toggleRowSelection(newRowCol.row.entity);
                                //});
                                gridApi.edit.on.afterCellEdit($scope, function (rowEntity, colDef, newValue, oldValue) {
                                    $scope.$safeApply(function () {
                                        rowEntity.IsModified = true;
                                    });
                                });

                                $scope.volGridApi = gridApi;
                            }
                        }

                        // 添加卷册表格列
                        $scope.addColDef = function () {
                            var index = 0;
                            $scope.volOptions.columnDefs = [
                                { name: 'Number', displayName: $scope.local.volume.number, width: '120', enableColumnMenu: false },
                                { name: 'Name', displayName: $scope.local.volume.name, width: '120', enableColumnMenu: false },
                                { name: "StartDate", displayName: $scope.local.specil.startDate, width: '120', editableCellTemplate: 'datepicker', cellFilter: 'TDate', enableColumnMenu: false },
                                { name: "EndDate", displayName: $scope.local.specil.endDate, width: '120', editableCellTemplate: 'datepicker', cellFilter: 'TDate', enableColumnMenu: false },
                                { name: "Note", displayName: $scope.local.specil.note, width: '120', enableColumnMenu: false }
                            ];


                            angular.forEach($scope.Process.Tasks, function (task) {

                                var colDef = {
                                    name: task.Owner, displayName: task.Name, width: '120', cellFilter: 'enumMap:"user"', enableColumnMenu: false,
                                    editDropdownValueLabel: 'Name', editDropdownOptionsArray: $scope.user_item,
                                    editableCellTemplate: 'uiSelect',

                                    SpecialtyID: $scope.planVolume.Specialty.SpecialtyID,
                                    ProcessID : $scope.Process.Key,
                                    TaskID: task.ID,

                                    cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                                        if (row.entity[col.field + "_Status"] == 2) {
                                            return 'bg-blue';
                                        }
                                    },
                                    cellEditableCondition: function ($scope) {
                                        return $scope.row.entity[$scope.col.field + "_Status"] != 2;
                                    },
                                };

                                $scope.volOptions.columnDefs.insertAt(2 + index++, colDef);
                            });
                        }

                        // 添加卷册
                        $scope.addVolume = function (vol) {

                            var newVol = { Name: '新建卷册' };

                            var index = $scope.Volumes.length;
                            if (index == 0) {
                                angular.forEach($scope.Process.Tasks, function (task) {
                                    newVol[task.Owner] = 1;
                                    newVol[task.Owner + "_Status"] = 0;
                                });
                            } else {

                                var lastVolume = $scope.Volumes[index - 1];

                                angular.forEach($scope.Process.Tasks, function (task) {
                                    newVol[task.Owner] = lastVolume[task.Owner];
                                    newVol[task.Owner + "_Status"] = 0;
                                });
                            }
                            $scope.Volumes.push(newVol);
                        }

                        // 删除卷册
                        $scope.removeVolume = function () {

                            var items = $scope.volGridApi.selection.getSelectedRows();
                            if (items.length > 0) {
                                bootbox.confirm("共选中" + items.length + "个卷册，确定删除？", function (result) {
                                    if (result) {

                                        for (var i = 0; i < items.length; i++) {
                                            if (items[i].CanDelete != undefined && !items[i].CanDelete) {
                                                bootbox.alert(items[i].Name + '已有完成的任务，无法删除！');
                                                return;
                                            }
                                        }

                                        $scope.$safeApply(function () {
                                            angular.forEach(items, function (item) {

                                                $scope.Volumes.custRemove(function (v) {

                                                    return v.$$hashKey == item.$$hashKey
                                                });
                                            });
                                        });
                                    }
                                });
                            } else {
                                bootbox.alert('请选择卷册！');
                            }
                        }

                        // 保存卷册信息
                        $scope.updateVolumes = function () {

                            //$scope.thisPanel.block();
                            angular.forEach($scope.Volumes, function (vol) {
                                vol.TaskUsers = [];
                                angular.forEach($scope.Process.Tasks, function (task) {
                                    vol.TaskUsers.push({
                                        ID: task.ID,
                                        Owner: task.Owner,
                                        User: vol[task.Owner]
                                    });
                                });
                            })

                            volumeService.updateVolumes($scope.planVolume.Engineering.ID,
                                $scope.planVolume.Specialty.SpecialtyID,
                                $scope.Volumes).then(function (newItems) {
                                   
                                    if ($scope.maintainModel == 2) {
                                        $uibModalInstance.close();
                                    } else {
                                        $scope.loadSource();
                                    }
                                    
                                })
                        }

                        // 关闭编辑模式
                        $scope.closeMaintain = function () {
                            $uibModalInstance.dismiss('cancel');
                        }
                    }
                }
            });

            app.controller('production.controller.volume.plan', function ($scope, $uibModalInstance, volumeService, volumePlanScope, volPlanParams) {

                volumePlanScope.init($scope, $uibModalInstance, volumeService, volPlanParams);

                $scope.addColDef();

                $scope.loadVolumes();
            });
        });