define(['apps/system3/production/production.controller',
        'apps/system3/production/form/change/change.service',
        'apps/system3/production/specialty/specialty.service',
        'apps/system3/business/engineering/engineering.service',
        'apps/system3/business/customer/customer.service'], function (app) {

            app.controller('production.controller.change.maintain', function ($scope, modelParam, $uibModalInstance) {

                $scope.taskID = modelParam.taskID;
                $scope.objectID = modelParam.objID
                $scope.modelType = 'window';

                // 关闭编辑模式
                $scope.closeModal = function () {
                    $uibModalInstance.dismiss('cancel');
                }

                $scope.afterSave = function () {
                    $uibModalInstance.close();
                }
            });

            app.controller('formChangeCtl', function ($scope, specialtyService, 
                engineeringService, customerService, changeService,stdApiUrl,stdApiVersion) {

                // 变更信息
                if ($scope.objectID > 0) {
                    $scope.formExoprtUrl = stdApiUrl + stdApiVersion + "/form/change/" + $scope.objectID + "/download";
                    $scope.readOnly = !$scope.taskID;
                    changeService.getChangeInfo($scope.objectID).then(function (changeInfo) {
                        $scope.changeInfo = changeInfo;
                        $scope.chooseEngineeringInfo = $scope.changeInfo.Engineering;
                        $scope.chooseMainCustInfo = $scope.changeInfo.MainCustomer;
                        $scope.chooseCopyCustInfo = $scope.changeInfo.CopyCustomer;
                    });
                } else {
                    $scope.changeInfo = {
                        AttachIDs: []
                    };
                }


                // 加载卷册
                $scope.loadVolume = function (engID, specID) {

                    specialtyService.getVolumes(engID, specID).then(function (data) {
                        $scope.volumes = data;
                        if (data.length > 0 && !$scope.changeInfo.VolumeID ) {
                            $scope.changeInfo.VolumeID = data[0].ID;
                        } 
                    });
                }

                // 加载专业
                $scope.loadSpecialty = function (engID) {

                    specialtyService.getSpecialtysByEngID(engID).then(function (data) {
                        $scope.specialtys = data;
                        $scope.changeInfo.SpecialtyID = data[0];
                    });
                }

                // 加载工程
                $scope.loadEngineeringByNumber = function (textFilter) {
                    return engineeringService.loadSource({ number: textFilter });
                }

                // 加载工程
                $scope.loadEngineeringByName = function (textFilter) {
                    return engineeringService.loadSource({ name: textFilter });
                }

                // 加载客户
                $scope.loadCustomer = function (textFilter) {
                    return customerService.loadSource({ name: textFilter });
                }

                // 保存
                $scope.save = function (flow) {

                    $scope.panel.block();

                    if ($scope.changeInfo.ID > 0) {
                        // 编辑
                        changeService.update($scope.changeInfo).then(function () {
                            flow.callBack(function () {
                                $scope.panel.unblock();

                                $scope.afterSave();
                              
                            });
                        });
                    } else {
                        
                        // 新增
                        $scope.changeInfo.FlowData = flow.flowInfo;
                        
                        changeService.create($scope.changeInfo).then(function () {

                            // 通知附件预览控件更新附件列表
                            if ($scope.attachChangedCB) {
                                $scope.attachChangedCB();
                            }

                            flow.callBack(function () {
                                $scope.panel.unblock();
                                //bootbox.alert('新增成功！');
                                $scope.afterSave();
                                //if ($uibModalInstance) {
                                //    $uibModalInstance.close();
                                //} else {
                                //    $scope.loadSource();
                                //}
                            });
                        });
                    }

                    return true;
                }

                // 导出
                $scope.export = function () {

                }

                $scope.$watch("chooseEngineeringInfo", function (newval, oldval) {
                    if (newval && newval.ID > 0) {
                        $scope.changeInfo.EngineeringID = newval.ID;
                        $scope.loadSpecialty(newval.ID);
                    }
                });
                $scope.$watch("chooseMainCustInfo", function (newval, oldval) {
                    if (newval && newval.ID > 0) {
                        $scope.changeInfo.MainCustomerID = newval.ID;
                    }
                });
                $scope.$watch("chooseCopyCustInfo", function (newval, oldval) {
                    if (newval && newval.ID > 0) {
                        $scope.changeInfo.CopyCustomerID = newval.ID;
                    }
                });

                $scope.$watch("changeInfo.SpecialtyID", function (newval, oldval) {
                    if (newval > 0) {
                        $scope.loadVolume($scope.changeInfo.EngineeringID, newval);
                    }
                });
                $scope.$watch("changeInfo.VolumeID", function (newval, oldval) {
                    if (newval > 0) {
                        $scope.flowData = {
                            VolumeID: newval
                        };
                    }
                });

                // 附件上传完成后回调
                $scope.attachUploaded = function (id, attachChangedCB) {

                    // 新建的模式下，先保存上传的附件ID，当信息被保存后，在保存该附件ID
                    $scope.changeInfo.AttachIDs.push(id);

                    $scope.attachChangedCB = attachChangedCB;
                };
            });
        });