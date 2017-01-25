define(['apps/system/system.controller',
    'apps/system/system.service.menu'], function (app) {

    app.controller("system.controller.menu", function ($scope, system_menu_service, permissionCheckService) {
            var permissionDelete = true;
            $scope.newMenu = {};
            var permissionCheck = function () {
                //permissionCheckService.check("Sys-Menu-Delete").then(function (result) {
                //    // 拒绝
                //    if (result == 1) {
                //        permissionDelete = false;
                //    } else {
                //        permissionDelete = true;
                //    }
                //})
            };

            var loadMenus = function (system) {
                $scope.menuPanel.block();
                system_menu_service.getMenus(system).then(function (result) {
                    $scope.menus = paraseTreeData(result);
                    $scope.currentMenu = undefined;
                    $scope.menuPanel.unblock();
                })
            };

            // 将服务端数据转换为界面tree识别的数据
            var paraseTreeData = function (nodes) {

                var newNodes = [];

                angular.forEach(nodes, function (node) {

                    node.text = node.Text;
                    node.type = "folder";
                    node.type = node.SubMenus ? "folder" : "file";
                    node.state = { 'opened': true, selected: false };
                    node.children = paraseTreeData(node.SubMenus);

                    newNodes.push(node);
                });

                return newNodes;
            }

            // 添加菜单
            $scope.createMenu = function () {
                //$scope.newPanel.block();
                var menuKey = $scope.currentMenu ? $scope.currentMenu.Key : "";

                system_menu_service.addMenu($scope.newMenu,menuKey).then(function () {
                    loadMenus($scope.currentBusiness.Key);
                    $scope.newMenu = {};
                    //$scope.newPanel.unblock();
                    bootbox.alert("添加成功");
                })
            }

            // 删除菜单
            $scope.removeMenu = function (node) {
                var menu = node.original;
                if (menu) {

                    var msg = "确定删除菜单【" + menu.Text + "】";

                    if (menu.SubMenus && menu.SubMenus.length > 0) {
                        msg += "及其所有子菜单"
                    }

                    bootbox.confirm(msg + "?", function (result) {
                        if (result === true) {

                            $scope.menuPanel.block();

                            system_menu_service.removeMenu(menu.Key).then(function (result) {

                                $scope.treeApi.delete_node(node);
                                $scope.currentMenu = undefined;
                                //$scope.menuPanel.unblock();
                                bootbox.alert("更新成功");
                            });
                        }
                    });
                }
            }

            // 更新菜单
            $scope.updateMenu = function () {
                //$scope.editPanel.block();
                $scope.currentMenu.text = $scope.currentMenu.Text;
                system_menu_service.updateMenu($scope.currentMenu).then(function () {

                    loadMenus($scope.currentBusiness.Key);

                    bootbox.alert("更新成功");
                    //$scope.editPanel.unblock();
                })
            }

            $scope.treeContextmenu = function (node) {

                if (permissionDelete) {

                    var deletePer = {
                        "label": "删除菜单",
                        "icon": "fa fa-trash",
                        "action": function (obj) {
                            $scope.removeMenu(node)
                        },
                    };

                    return { "deletePer": deletePer };
                }
            }

            $scope.menusChanged = function (e, data) {

                if (data.node && data.node.state.selected) {

                    $scope.$safeApply(function () {

                        $scope.currentMenu = data.node.original;

                    });
                }
            }

            $scope.$on('businessChanged', function (e, business) {

                if (business) {
                    loadMenus(business.Key);
                }
            });

            $scope.$watch('$viewContentLoaded', function (e) {
                if ($scope.currentBusiness) {
                    loadMenus($scope.currentBusiness.Key);
                }
            });

            $scope.$on("permissionChanged", function () {
                permissionCheck();
            });

            permissionCheck();

        });
    });
