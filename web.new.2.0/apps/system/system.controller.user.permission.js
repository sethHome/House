define(['apps/system/system.controller',
    'apps/system/system.service.user',
    'apps/system/system.service.business'], function (app) {
        app.controller("system.controller.user.permission", function ($scope, $rootScope, $stateParams,
           system_permission_service, system_organization_service, system_user_service) {

            var loadPermission = function (business) {
                $scope.permissionPanel.block();

                system_permission_service.all(1, business.Key).then(function (source) {
                    $scope.uiPermissions = paraseTreeData(source,0);
                    $scope.permissionPanel.unblock();
                });
                system_permission_service.all(2, business.Key).then(function (source) {
                    $scope.dataPermissions = paraseTreeData(source,0);
                });
                system_permission_service.all(3, business.Key).then(function (source) {
                    $scope.apiPermissions = paraseTreeData(source,0);
                });
            }

            // 将服务端数据转换为界面tree识别的数据
            var paraseTreeData = function (nodes,deep) {

                var newNodes = [];

                angular.forEach(nodes, function (node) {

                    var newNode = angular.copy(node);

                    newNode.text = node.Name;
                    newNode.state = { 'opened': deep == 0, selected: false };
                    newNode.children = paraseTreeData(node.Children, deep+1);

                    newNodes.push(newNode);
                });

                return newNodes;
            }

            var isInherit = function (permission, data) {

                if (permission && permission[data.Index] && new BigInt(permission[data.Index]).permissionCheck(new BigInt(data.StrValue))) {
                    return true;
                } else {
                    return false;
                }
            }
           
            var opUser = function (user, treeApi) {
                enableContextmenu = false;
                treeApi.deselect_all();

                for (var id in treeApi._model.data) {

                    if (id == "#") {
                        continue;
                    }

                    var node = treeApi._model.data[id];
                    var data = node.original;

                    treeApi.enable_node(node);
                    treeApi.rename_node(node, data.text);

                    var hasFlage = false;
                   
                    if (user.DeptPermissionsValue && user.DeptPermissionsValue[data.Index] && new BigInt(data.StrValue).permissionCheck(new BigInt(user.DeptPermissionsValue[data.Index]))) {
                        hasFlage = true;
                        treeApi.select_node(node);

                        treeApi.rename_node(node, data.text + "<span class='inheritTag'>[部门]</span>");
                        treeApi.disable_node(node);
                    }

                    if (user.RolePermissionsValue && user.RolePermissionsValue[data.Index] && new BigInt(data.StrValue).permissionCheck(new BigInt(user.RolePermissionsValue[data.Index]))) {
                        if (hasFlage) {
                            treeApi.rename_node(node, data.text + "<span class='inheritTag'>[部门][角色]</span>");

                        } else {
                            treeApi.select_node(node);

                            treeApi.rename_node(node, data.text + "<span class='inheritTag'>[角色]</span>");
                            treeApi.disable_node(node);
                        }
                    } else if (!hasFlage && user.PermissionsValue && user.PermissionsValue[data.Index] && new BigInt(data.StrValue).permissionCheck(new BigInt(user.PermissionsValue[data.Index]))) {

                        treeApi.select_node(node);
                    }
                }
            }
           
            $scope.$on('businessChanged', function (e, business) {

                if (business) {
                    loadPermission(business);
                } else {
                    $scope.uiPermissions = [];
                    $scope.dataPermissions = [];
                    $scope.apiPermissions = [];
                }
            });

            $scope.$watch('$viewContentLoaded', function (e) {
                if ($scope.currentBusiness) {
                    loadPermission($scope.currentBusiness);
                }
            });

            $scope.$watch('currentIndex', function (newVal, oldVal) {

                if ($scope.permissionScroller) {
                    $scope.permissionScroller.init();
                }
            });

            $scope.permissionChanged = function (e, data) {

                if (data.node && !data.node.state.selected && data.node.original.CanInherit) {
                    $scope.uiTreeApi.rename_node(data.node, data.node.original.text);
                }
            }

            $scope.dataPermissionChanged = function (e, data) {

                if (data.node && !data.node.state.selected && data.node.original.CanInherit) {
                    $scope.dataTreeApi.rename_node(data.node, data.node.original.text);
                }
            }

            $scope.apiPermissionChanged = function (e, data) {

                if (data.node && !data.node.state.selected && data.node.original.CanInherit) {
                    $scope.apiTreeApi.rename_node(data.node, data.node.original.text);
                }
            }

            var treeReady1 = false, treeReady2 = false, treeReady3 = false;
            $scope.treeReady1 = function (e, data) {
                treeReady1 = true;

                if ($scope.currentUser) {
                    opUser($scope.currentUser, $scope.uiTreeApi);
                }

                if ($scope.uiTreeScroll) {
                    $scope.uiTreeScroll.init();
                }
            }
            $scope.treeReady2 = function (e, data) {
                treeReady2 = true;

                if ($scope.currentUser) {
                    opUser($scope.currentUser, $scope.dataTreeApi);
                }

                if ($scope.dataTreeScroll) {
                    $scope.dataTreeScroll.init();
                }
            }
            $scope.treeReady3 = function (e, data) {
                treeReady3 = true;

                if ($scope.currentUser) {
                    opUser($scope.currentUser, $scope.apiTreeApi);
                }

                if ($scope.apiTreeScroll) {
                    $scope.apiTreeScroll.init();
                }
            }
            
            $scope.$watch("currentUser", function (newval, old) {
                if (newval) {
                    if (treeReady1) {
                        opUser(newval, $scope.uiTreeApi);
                    }
                    if (treeReady2) {
                        opUser(newval, $scope.dataTreeApi);
                    }
                    if (treeReady3) {
                        opUser(newval, $scope.apiTreeApi);
                    }
                }
            });

            $scope.save = function () {

                $scope.permissionPanel.block();

                var uiNodes = $scope.uiTreeApi.get_selected(true);
                var dataNodes = $scope.dataTreeApi.get_selected(true);
                var apiNodes = $scope.apiTreeApi.get_selected(true);

                var permissions = {};
             
                getPermission(uiNodes, permissions);

                getPermission(dataNodes, permissions);

                getPermission(apiNodes, permissions);

                system_user_service.setPermission($scope.currentUser.ID, permissions).then(function () {

                    $scope.currentUser.PermissionsValue = permissions;

                    $scope.permissionPanel.unblock();

                });
            }

            var getPermission = function (nodes, permissions) {

                angular.forEach(nodes, function (node) {
                    var data = node.original;

                    // 排除掉继承来的权限
                    if (!node.state.disabled) {

                        if (permissions[data.Index]) {
                            permissions[data.Index].push(data.StrValue);
                        } else {
                            permissions[data.Index] = [data.StrValue];
                        }
                    }
                });
            }
        });
    });
