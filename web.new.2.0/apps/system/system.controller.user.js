define(['apps/system/system.controller',
    'apps/system/system.service.user',
    'apps/system/system.service.organization',
    'apps/system/system.service.business'], function (app) {

        app.filter('deptFilter', function () {
            return function (input, dept) {
                if (input != undefined) {
                    return input.where(function (item) {
                        return dept == null || dept.key == 'Origanization.Dept.D1' || item.Dept.Key == dept.key;
                    });
                }
            };
        });

        app.controller("system.controller.user", function ($scope, $uibModal, system_user_service, system_organization_service, system_business_service) {

            $scope.userChanged = function (user) {
                $scope.currentUser = user;
            }

            $scope.setPage = function () {
                //$scope.userScroller.init();

                var tiles = $(".tile, .tile-small, .tile-sqaure, .tile-wide, .tile-large, .tile-big, .tile-super");

                $.each(tiles, function () {
                    var tile = $(this);
                    setTimeout(function () {
                        tile.css({
                            opacity: 1,
                            "-webkit-transform": "scale(1)",
                            "transform": "scale(1)",
                            "-webkit-transition": ".3s",
                            "transition": ".3s"
                        });
                    }, Math.floor(Math.random() * 500));
                });

                $(".tile-group").animate({
                    left: 0
                });
            }

            var loadUser = function () {

                $scope.userPanel.block();
                system_user_service.getUsersEx(true, true, true, true).then(function (result) {
                    $scope.userPanel.unblock();
                    $scope.users = result;
                    $scope.currentUser = result[0];
                });
            }

            $scope.setTreeApi = function (api) {
                $scope.treeApi = api;
            }

            var loadDeptTree = function () {

                system_organization_service.getDepartment().then(function (source) {
                    $scope.departments = paraseTreeData(source)
                });
            }

            $scope.$watch('$viewContentLoaded', function () {

                loadDeptTree();

                loadUser();
            });

            // 将服务端数据转换为界面tree识别的数据
            var paraseTreeData = function (nodes) {
                var newNodes = [];
                angular.forEach(nodes, function (node) {
                    var newNode = {};
                    newNode.ID = node.ID;
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
                $scope.$safeApply(function () {
                    $scope.currentDept = data.node.original;
                });
            }

            // 部门右键菜单
            $scope.treeContextmenu = function (node) {

                var newUser = {
                    "label": "新建用户",
                    "icon": "fa fa-user",
                    "action": function (obj) {
                        createUser(node);
                    },
                }

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

                return { "newUser": newUser, "remove": remove, "create": create, "rename": rename };
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
            };

            // 新建用户
            var createUser = function (node) {
                $uibModal.open({
                    animation: false,
                    size: 'md',
                    templateUrl: 'apps/system/view/user.new.html',
                    controller: 'system.controller.user.create',
                    resolve: {
                        maintainParams: function () {
                            return {
                                DeptName: node.original.text,
                                DeptKey: node.original.key
                            }
                        }
                    }
                }).result.then(function (newuser) {
                    bootbox.alert("创建成功");
                    
                    $scope.users.push(newuser);

                });

            }
        });

        app.controller("system.controller.user.create", function ($scope, maintainParams, $uibModalInstance, system_organization_service, system_user_service, system_business_service) {

            $scope.roles = system_organization_service.getRole().$object;
            $scope.allBusiness = system_business_service.getBusiness({ withuser: true }).$object;

            $scope.newUser = {
                Visiable: true,
                Roles: [],
                Businesses: [],
                Dept: {
                    Key: maintainParams.DeptKey,
                    Name: maintainParams.DeptName
                }
            };
            $scope.$watch("newUser.Account", function () {
                $scope.accountError = false;
            });
            $scope.setRole = function (r) {

                r.selected = !r.selected;

                if (r.selected) {
                    $scope.newUser.Roles.push(r);
                } else {
                    $scope.newUser.Roles.removeObj(r);
                }
            }

            $scope.setBusiness = function (b) {

                b.selected = !b.selected;

                if (b.selected) {
                    $scope.newUser.Businesses.push(b);
                } else {
                    $scope.newUser.Businesses.removeObj(b);
                }
            }

            $scope.save = function () {

                system_user_service.checkAccount($scope.newUser.Account).then(function (result) {
                    if (result) {
                        system_user_service.create($scope.newUser).then(function (id) {

                            $scope.newUser.ID = id;
                            //$scope.newUser.ID = 0;
                            //$scope.newUser.Name = undefined;
                            //$scope.newUser.Account = undefined;
                            $scope.newUser.PhotoImgLarge = "1x.jpg";
                            
                            $uibModalInstance.close($scope.newUser);
                        });
                    } else {
                        $scope.accountError = true;
                    }
                });
            }

            // 关闭弹出对话框(仅在弹框模式下有用)
            $scope.close = function () {
                $uibModalInstance.dismiss('cancel');
            }
        });
    });
