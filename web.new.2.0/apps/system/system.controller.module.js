define(['apps/system/system.controller',
    'apps/system/system.service.business',
    'apps/system/system.service.module'], function (app) {

        app.controller("system.controller.module", function ($scope, system_business_service, system_module_service, permissionCheckService) {
            var permissionDelete = true;
            $scope.newModule = {};

            var permissionCheck = function () {
                permissionCheckService.check("Sys-Module-Delete").then(function (result) {
                    // 拒绝
                    if (result == 1) {
                        permissionDelete = false;
                    } else {
                        permissionDelete = true;
                    }
                })
            };
            
            var loadModules = function (system) {
                $scope.modulePanel.block();
                system_module_service.getModules(system).then(function (result) {
                    $scope.modules = paraseTreeData(result);
                    $scope.currentModule = undefined;
                    $scope.modulePanel.unblock();
                })
            };

            // 将服务端数据转换为界面tree识别的数据
            var paraseTreeData = function (nodes) {

                var newNodes = [];

                angular.forEach(nodes, function (node) {

                    node.text = node.Text;
                    node.type = node.SubModules ? "cubes" : "cube";
                    node.state = { 'opened': true, selected: false };
                    node.children = paraseTreeData(node.SubModules);

                    newNodes.push(node);
                });

                return newNodes;
            }

            // 添加模块
            $scope.createModule = function () {
                //$scope.newPanel.block();
                var moduleKey = $scope.currentModule ? $scope.currentModule.Key : "";

                system_module_service.addModule($scope.newModule, moduleKey).then(function () {
                    loadModules($scope.currentBusiness.Key);
                    $scope.newModule = {};
                    //$scope.newPanel.unblock();
                })
            }

            // 删除模块
            $scope.removeModule = function (node) {
                var module = node.original;
                if (module) {

                    var msg = "确定删除模块【" + module.Text + "】";

                    if (module.SubModules && module.SubModules.length > 0) {
                        msg += "及其所有子模块"
                    }

                    bootbox.confirm(msg + "?", function (result) {
                        if (result === true) {

                            //$scope.modulePanel.block();

                            system_module_service.removeModule(module.Key).then(function (result) {

                                loadModules($scope.currentBusiness.Key);

                                //$scope.treeApi.delete_node(node);
                                $scope.currentModule = undefined;
                                //$scope.modulePanel.unblock();
                            });
                        }
                    });
                }
            }

            // 更新模块
            $scope.updateModule = function () {
                //$scope.editPanel.block();
                $scope.currentModule.text = $scope.currentModule.Text;
                system_module_service.updateModule($scope.currentModule).then(function () {

                    loadModules($scope.currentBusiness.Key);

                    //$scope.editPanel.unblock();
                })
            }

            $scope.treeContextmenu = function (node) {

                if (permissionDelete) {

                    var deletePer = {
                        "label": "删除模块",
                        "icon": "fa fa-trash",
                        "action": function (obj) {
                            $scope.removeModule(node)
                        },
                    };

                    return { "deletePer": deletePer };
                }
            }

            $scope.modulesChanged = function (e, data) {

                if (data.node && data.node.state.selected) {

                    $scope.$safeApply(function () {

                        $scope.currentModule = data.node.original;

                    });
                }
            }

            $scope.$on('businessChanged', function (e, business) {
                
                if (business) {
                    loadModules(business.Key);
                }
            });

            $scope.$watch('$viewContentLoaded', function (e) {
                if ($scope.currentBusiness) {
                    loadModules($scope.currentBusiness.Key);
                }
            });

            $scope.$on("permissionChanged", function () {
                permissionCheck();
            });

            permissionCheck();

        });
    });
