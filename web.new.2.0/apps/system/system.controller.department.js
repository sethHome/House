define(['apps/system/system.controller',
    'apps/system/system.service.organization',
    'apps/system/system.service.business',
    'apps/system/system.service.permission',
    'apps/system/system.service.user'], function (app) {

        app.controller("system.controller.department", function ($scope, $stateParams,
            system_organization_service, system_business_service) {

            $scope.setTreeApi = function (api) {
                $scope.treeApi = api;
            }

            $scope.loadDeptTree = function () {
                $scope.deptPanel.block();
                system_organization_service.getDepartment().then(function (source) {
                    $scope.deptPanel.unblock();
                    $scope.departments = paraseTreeData(source)
                });
            }

            $scope.$watch('$viewContentLoaded', function () {

                $scope.loadDeptTree();
            });

            // 将服务端数据转换为界面tree识别的数据
            var paraseTreeData = function (nodes) {
                var newNodes = [];
                angular.forEach(nodes, function (node) {
                    var newNode = {};
                    newNode.text = node.Name;
                    newNode.key = node.Key;
                    newNode.permissionValue = node.Permission;
                    newNode.unInheritPermissionValue = node.UnInheritPermission;
                    newNode.inheritPermissionValue = node.InheritPermission;

                    newNode.permissions = node.Permissions;
                    newNode.users = node.Users;
                    newNode.state = { 'opened': true };
                    newNode.children = paraseTreeData(node.SubDepartments);
                    newNodes.push(newNode);
                });
                return newNodes;
            }

            var deptKey = "";
            $scope.changed = function (e, data) {
                deptKey = data.node.original.key;
                $scope.$broadcast("$DeptSelected", data.node.original);
            }

            // 部门右键菜单
            $scope.treeContextmenu = function (node) {

                var remove = {
                    "label": "删除部门",
                    "icon": "fa fa-trash",
                    "action": function (obj) {
                        removeDepartment(node);
                    },
                };

                var create = {
                    "label": "新建部门",
                    "icon": "fa fa-file",
                    "action": function (obj) {
                        createDepartment(node);
                    },
                };

                var rename = {
                    "label": "重命名",
                    "icon": "fa fa-edit",
                    "action": function (obj) {
                        renameDepartment(node);
                    },
                }

                return { "remove": remove, "create": create, "rename": rename };
            }

            // 保存权限
            $scope.savePermission = function (permission, callback) {
                system_organization_service.changeDeptPermission(deptKey, permission).then(function () {
                    $scope.loadDeptTree();
                    callback();
                });
            }

            // 保存用户
            $scope.saveUser = function (users, callback) {
                system_organization_service.changeDeptUsers(deptKey, users).then(function () {
                    $scope.loadDeptTree();
                    callback();
                });
            }

            // 删除部门
            var removeDepartment = function (node) {

                system_organization_service.removeDept(node.original.key).then(function () {
                    $scope.treeApi.delete_node(node);
                });
            }

            // 新建部门
            var createDepartment = function (parentNode) {

                var nodeID = $scope.treeApi.create_node(parentNode, { text: "新建部门", state: { opened: true } });

                $scope.treeApi.edit(nodeID, '新建部门', function (node, isRename, isCancelled) {

                    system_organization_service.createDept(node.text, parentNode.original.key).then(function (key) {

                        node.original.key = key;
                    })
                });

            }

            // 部门重命名
            var renameDepartment = function (node) {

                $scope.treeApi.edit(node, null, function (editNode, isRename, isCancelled) {
                    system_organization_service.renameDept(editNode.text, editNode.original.key).then(function () {

                    })
                });
            }
        });

        app.controller("system.controller.department.user", function ($scope, $stateParams,
           system_user_service, system_organization_service) {

            var waitToRole = undefined;
            var opRoleUser = function (role) {
                angular.forEach($scope.users, function (user) {
                    user.isSelected = false;
                    angular.forEach(role.Users, function (roleUser) {
                        if (user.ID == roleUser.ID) {
                            user.isSelected = true;
                        }
                    });
                });
            }

            $scope.$watch('$viewContentLoaded', function () {

                $scope.userPanel.block();

                system_user_service.getUsers(true, true, true).then(function (result) {
                    $scope.users = result;

                    if (waitToRole) {
                        opRoleUser(waitToRole);
                    }

                    $scope.userPanel.unblock();
                })
            });

            $scope.$on("$DeptSelected", function (event, dept) {

                $scope.$safeApply(function () {
                    angular.forEach($scope.users, function (user) {
                        user.isSelected = false;
                        angular.forEach(dept.users, function (deptUser) {
                            if (user.ID == deptUser.ID) {
                                user.isSelected = true;
                            }
                        });
                    });
                });

            });

            $scope.$on("$RoleSelected", function (event, role) {

                if ($scope.users) {
                    opRoleUser(role);
                } else {
                    waitToRole = role;
                }
            });

            $scope.save = function () {
                $scope.userPanel.block();

                var checkedUsers = [];

                angular.forEach($scope.users, function (user) {
                    if (user.isSelected) {
                        checkedUsers.push(user);
                    }
                });

                $scope.saveUser(checkedUsers, function () {
                    $scope.userPanel.unblock();
                })
            }
        });

        app.controller("system.controller.department.permission", function ($scope, $rootScope, $stateParams,
           system_permission_service, system_organization_service) {

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

                    var newNode = {};
                    newNode.text = node.Name;
                    newNode.state = { 'opened': deep == 0, selected: false };
                    newNode.Index = node.Index;
                    newNode.Value = node.Value;
                    newNode.StrValue = node.StrValue;
                    newNode.ID = node.ID;
                    newNode.CanInherit = node.CanInherit;
                    newNode.OrgCanInherit = node.OrgCanInherit;
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

            var treeReady = false;
            var enableContextmenu = true;
            var waitToRole = undefined;
            var waitToDept = undefined;

            var opRole = function (role, treeApi) {
                enableContextmenu = false;
                treeApi.deselect_all();
                for (var id in treeApi._model.data) {

                    if (id == "#") {
                        continue;
                    }

                    var node = treeApi._model.data[id];
                    var data = node.original;
                    var v = role.PermissionValue[data.Index];
                 
                    if (v && new BigInt(data.StrValue).permissionCheck(new BigInt(v))) {

                        treeApi.select_node(node);
                    }
                }
            }

            var opDept = function (dept, treeApi) {
                enableContextmenu = true;
                treeApi.deselect_all();

                $scope.$safeApply(function () {

                    for (var id in treeApi._model.data) {

                        if (id == "#") {
                            continue;
                        }

                        var node = treeApi._model.data[id];
                        var data = node.original;

                        if (!data.CanInherit) {
                            treeApi.rename_node(node, data.text + "<span class='inheritTag'>[禁止继承_权限]</span>");
                        } else {
                            treeApi.rename_node(node, data.text);
                        }

                        treeApi.enable_node(node);

                        if (dept.permissionValue && new BigInt(data.StrValue).permissionCheck(new BigInt(dept.permissionValue[data.Index]))) {

                            treeApi.select_node(node);

                            node.original.OrgCanInherit = !isInherit(dept.unInheritPermissionValue, data);
                            node.original.IsInherit = isInherit(dept.inheritPermissionValue, data);

                            if (node.original.IsInherit) {
                                treeApi.rename_node(node, data.text + "<span class='inheritTag'>[继承]</span>");
                                treeApi.disable_node(node);
                            } else if (!node.original.OrgCanInherit) {
                                treeApi.rename_node(node, data.text + "<span class='inheritTag'>[禁止继承_部门]</span>");
                            }
                        }
                    }
                })
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

                if (waitToRole) {
                    opRole(waitToRole, $scope.uiTreeApi);
                }

                if (waitToDept) {
                    opDept(waitToDept, $scope.uiTreeApi);
                }

                if ($scope.uiTreeScroll) {
                    $scope.uiTreeScroll.init();
                }
            }
            $scope.treeReady2 = function (e, data) {
                treeReady2 = true;

                if (waitToRole) {
                    opRole(waitToRole, $scope.dataTreeApi);
                }

                if (waitToDept) {
                    opDept(waitToDept, $scope.dataTreeApi);
                }

                if ($scope.dataTreeScroll) {
                    $scope.dataTreeScroll.init();
                }
            }
            $scope.treeReady3 = function (e, data) {
                treeReady3 = true;

                if (waitToRole) {
                    opRole(waitToRole, $scope.apiTreeApi);
                }

                if (waitToDept) {
                    opDept(waitToDept, $scope.apiTreeApi);
                }

                if ($scope.apiTreeScroll) {
                    $scope.apiTreeScroll.init();
                }
            }

            $scope.$on("$RoleSelected", function (event, role) {

                waitToRole = role;

                if (treeReady1) {
                    opRole(role, $scope.uiTreeApi);
                }
                if (treeReady2) {
                    opRole(role, $scope.dataTreeApi);
                }
                if (treeReady3) {
                    opRole(role, $scope.apiTreeApi);
                }
            });

            $scope.$on("$DeptSelected", function (event, dept) {

                waitToDept = dept;

                if (treeReady1) {
                    opDept(dept, $scope.uiTreeApi);
                }
                if (treeReady2) {
                    opDept(dept, $scope.dataTreeApi);
                }
                if (treeReady3) {
                    opDept(dept, $scope.apiTreeApi);
                }
            });

            $scope.treeContextmenu = function (node) {

                if ((node.state && node.state.disabled) || node.original.CanInherit == false || !enableContextmenu) {
                    return null;
                }

                var canInherit = {
                    "label": "允许继承",
                    "action": function (obj) {

                        node.original.OrgCanInherit = true;
                        $scope.uiTreeApi.rename_node(node, node.original.text);
                    }
                }
                var disInherit = {
                    "label": "不允许继承",
                    "action": function (obj) {

                        node.original.OrgCanInherit = false;
                        $scope.uiTreeApi.rename_node(node, node.original.text + "<span class='inheritTag'>[禁止继承_部门]</span>");

                    },
                }

                return { "canInherit": canInherit, "disInherit": disInherit };

            }

            $scope.save = function () {

                $scope.permissionPanel.block();

                var uiNodes = $scope.uiTreeApi.get_selected(true);
                var dataNodes = $scope.dataTreeApi.get_selected(true);
                var apiNodes = $scope.apiTreeApi.get_selected(true);

                var permissions = {};
                var unInheritPermissions = {};
               
                getPermission(uiNodes, permissions, unInheritPermissions);
               
                getPermission(dataNodes, permissions, unInheritPermissions);
                
                getPermission(apiNodes, permissions, unInheritPermissions);

                $scope.savePermission({
                    Permissions: permissions,
                    UnInheritPermissions: unInheritPermissions
                }, function () {

                    $rootScope.$broadcast("permissionChanged");

                    $scope.permissionPanel.unblock();
                });
            }

            var getPermission = function (nodes, permissions, unInheritPermissions) {

                angular.forEach(nodes, function (node) {
                    var data = node.original;

                    // 排除掉继承来的权限
                    if (!node.state.disabled) {
                        
                        if (permissions[data.Index]) {
                            permissions[data.Index].push(data.StrValue);
                        } else {
                            permissions[data.Index] = [data.StrValue];
                        }

                        if (!data.OrgCanInherit) {
                            if (unInheritPermissions[data.Index]) {
                                unInheritPermissions[data.Index].push(data.StrValue);
                            } else {
                                unInheritPermissions[data.Index] = [data.StrValue];
                            }
                        }
                    }
                });
            }
        });
    });
