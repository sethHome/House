define(['apps/base/base.directive'],
    function (app) {

        app.directive("processInfo", function () {
            return {
                restrict: 'EA',
                templateUrl:'apps/base/view/process-btn.html',
                scope: {
                    key: '@',
                    id: '@'
                },
                controller: function ($scope, processService, $uibModal) {

                    $scope.$watch("id", function (newval, oldval) {
                        if (newval && newval > 0) {
                            $scope.processInfo = processService.getCurrentTaskName($scope.key, newval).$object;
                        }
                    });

                    $scope.openDetail = function () {
                        $uibModal.open({
                            animation: false,
                            size: 'lg',
                            templateUrl: 'apps/base/view/process-detail.html',
                            controller: 'process.controller.detail',
                            resolve: {
                                ProcessID: function () {
                                    return $scope.processInfo.ProcessID
                                },
                            }
                        });
                    }
                },
                link: function ($scope, element, attrs) {

                }
            };
        });

        app.directive("processControl", function () {
            return {
                restrict: 'EA',
                templateUrl: 'apps/base/view/process-control.html',
                scope: {
                    flowName:'@',
                    taskId: '=',
                    flowData: '=',
                    taskInfo:'=?',
                    next: '&',
                    form: '=',
                    simple : '@'
                },
                controller: function ($scope,$rootScope, processService, $uibModal, $validation) {
                    
                    // 流程信息
                    $scope.taskInfo = {
                        joinUsers_obj: []
                    };

                    // 加载流程节点信息
                    $scope.load = function () {
                       
                        if ($scope.taskId) {
                            
                            processService.getFlowTaskInfo($scope.taskId, $scope.flowData).then(function (nodeInfo) {

                                $scope.flowNodeInfo = nodeInfo;
                               
                                if (nodeInfo.NextUsers && nodeInfo.NextUsers.length > 0) {
                                    if ($scope.taskInfo == undefined) {
                                        $scope.taskInfo = {
                                            joinUsers_obj: [],
                                            user: nodeInfo.NextUsers[0]
                                        };
                                    } else  {
                                        $scope.taskInfo.user = nodeInfo.NextUsers[0];
                                    }
                                    
                                }
                                else if ($scope.flowNodeInfo.NextIsJoinSign) {
                                    $scope.$watchCollection("taskInfo.joinUsers_obj", function (newval, oldval) {
                                        if (newval) {

                                            var array = [];
                                            angular.forEach(newval, function (u) {
                                                array.push(u.id);
                                            });
                                            $scope.taskInfo[$scope.flowNodeInfo.NextOwner] = array.join(',')
                                        }
                                    });
                                }

                                angular.forEach($scope.flowNodeInfo.Params, function (p) {
                                    $scope.canBack = $scope.canBack | p.indexOf('bool_') >= 0;
                                });
                            });
                        } else if ($scope.flowName) {
                            processService.getFlowInitInfo($scope.flowName, $scope.flowData).then(function (nodeInfo) {

                                $scope.flowNodeInfo = nodeInfo;

                                if (nodeInfo.NextUsers && nodeInfo.NextUsers.length > 0) {
                                    if ($scope.taskInfo == undefined) {
                                        $scope.taskInfo = {
                                            joinUsers_obj: [],
                                            user: nodeInfo.NextUsers[0]
                                        };
                                    } else {
                                        $scope.taskInfo.user = nodeInfo.NextUsers[0];
                                    }
                                }
                                else if ($scope.flowNodeInfo.NextIsJoinSign) {

                                    $scope.$watchCollection("taskInfo.joinUsers_obj", function (newval, oldval) {
                                        if (newval) {

                                            var array = [];
                                            angular.forEach(newval, function (u) {
                                                array.push(u.id);
                                            });
                                            $scope.taskInfo[$scope.flowNodeInfo.NextOwner] = array.join(',')
                                        }
                                    });
                                }

                                $scope.canBack = false;
                            });
                        }
                    }

                   

                    // 流程下一步
                    $scope.flowNext = function (result) {

                        if ($scope.taskInfo == undefined) {
                            $scope.taskInfo = {};
                        }

                        $scope.taskInfo.isBack = !result;   //  是否回退

                        // 提交流程
                        var next = function (callBackForm) {

                            if ($scope.taskId > 0) {

                                var data = {
                                    note: $scope.taskInfo.note
                                };

                                // 需要提交的参数内容
                                if ($scope.flowNodeInfo.Params ) {
                                    angular.forEach($scope.flowNodeInfo.Params,function (para) {
                                        if (para.indexOf('bool_') >= 0) {
                                            data[para] = result;
                                        } else if (para.indexOf('#') >= 0) {
                                            var ps = para.split('#');
                                            data[ps[0]] = ps[1];
                                            data[ps[0] + "Result"] = result;
                                        }
                                    });
                                }

                                if ($scope.flowNodeInfo.NextIsJoinSign) {

                                    if ($scope.flowNodeInfo.NextJoinSignUsers.length > 0) {
                                        // 如果下一步已经有了负责人，则直接提交到负责人
                                        data[$scope.flowNodeInfo.NextOwner] = $scope.flowNodeInfo.NextJoinSignUsers;
                                    } else {
                                        // 选择的下一步会签人员
                                        data[$scope.flowNodeInfo.NextOwner] = $scope.taskInfo[$scope.flowNodeInfo.NextOwner]
                                    }
                                    
                                } else {
                                    if ($scope.flowNodeInfo.NextUser > 0) {
                                        // 如果下一步已经有了负责人，则直接提交到负责人
                                        data[$scope.flowNodeInfo.NextOwner] = $scope.flowNodeInfo.NextUser;
                                    } else {
                                        // 选择的下一步负责人
                                        data[$scope.flowNodeInfo.NextOwner] = $scope.taskInfo.user;
                                    }
                                }

                                // 提交下一步
                                processService.next($scope.taskId, data).then(function () {
                                    if (callBackForm) {
                                        callBackForm();
                                    }
                                });
                            } else {
                                if (callBackForm) {
                                    callBackForm();
                                }
                            }
                        }

                        // 验证表单
                        if ($scope.form != undefined) {
                            $validation.validate($scope.form).success(function () {
                                
                                if ($scope.next({ callBack: next, taskInfo: $scope.taskInfo }) == undefined) {
                                    next();
                                } 

                            }).error(function () {

                            });
                        } else {
                            if ($scope.next({ callBack: next, taskInfo: $scope.taskInfo }) == undefined) {
                                next();
                            }
                        }
                    }

                    // 查看流程进度信息
                    $scope.openFlow = function () {
                        if ($scope.flowNodeInfo.ProcessID) {
                            $uibModal.open({
                                animation: false,
                                size: 'lg',
                                templateUrl: 'apps/base/view/process-detail.html',
                                controller: 'process.controller.detail',
                                resolve: {
                                    ProcessID: function () {
                                        return $scope.flowNodeInfo.ProcessID
                                    },
                                }
                            });
                        }
                    }

                    $scope.getJoinSignUsers = function (filter) {
                        return $rootScope.filterUsers(filter);
                    }

                    $scope.$watch('flowData', function (newval, oldval) {
                        $scope.load();
                    }, true);

                    $scope.$watch('taskId', function (newval, oldval) {
                        $scope.load();
                    }, true);
                },
                link: function ($scope, element, attrs) {
                    
                }
            };
        });

        app.controller("process.controller.detail", function ($scope, $uibModalInstance, processService, ProcessID) {

            $scope.flowInfo = processService.getFlowDetailByID(ProcessID).$object;

            // 关闭编辑模式
            $scope.closeModal = function () {
                $uibModalInstance.dismiss('cancel');
            }
        });
    });
