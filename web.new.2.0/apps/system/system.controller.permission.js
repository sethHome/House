define(['apps/system/system.controller',
'apps/system/system.service.permission',
'apps/system/system.service.business'], function (app) {

    app.controller("system.controller.permission", function ($scope, $stateParams, system_permission_service, system_business_service) {

        $scope.newPermission = {};
        $scope.currentPermission = {};

        var loadPermission = function (businessKey) {
            
            //$scope.permissionPanel.block();
            system_permission_service.all($stateParams.type, businessKey).then(function (source) {

                $scope.permissions = paraseTreeData(source);
                $scope.currentPermission = undefined;

                //$scope.permissionPanel.unblock();
            });
        }

        $scope.treeReady = function () {
            $scope.permissionScroller.init();
        }

        $scope.$on('businessChanged', function (e, business) {

            if (business) {
                loadPermission(business.Key);
            }
        });

        $scope.$watch('$viewContentLoaded', function (e) {
            if ($scope.currentBusiness) {
                loadPermission($scope.currentBusiness.Key);
            }
        });

        // 将服务端数据转换为界面tree识别的数据
        var paraseTreeData = function (nodes) {

            var newNodes = [];

            angular.forEach(nodes, function (node) {

                node.text = node.Name;
                node.state = { 'opened': node.Key.split('_').length < 3, selected: false };
                node.children = paraseTreeData(node.Children);

                newNodes.push(node);
            });

            return newNodes;
        }

        $scope.permissionChanged = function (e, data) {
            
            if (data.node && data.node.state.selected) {

                $scope.$safeApply(function () {
                    
                    $scope.currentPermission = data.node.original;
                    $scope.currentPermission.CannotInherit = !data.node.original.CanInherit;
                });
            }
        }

        $scope.treeContextmenu = function (node) {

            var deletePer = {
                "label": "删除权限",
                "icon": "fa fa-trash",
                "action": function (obj) {
                    $scope.removePermission(node)
                },
            }

            return { "deletePer": deletePer };
        }

        $scope.removePermission = function (node) {
            var permission = node.original;
            if (permission) {

                var msg = "确定删除权限【" + permission.Name + "】";

                if (permission.Children.length > 0) {
                    msg += "及其所有子级权限"
                }

                bootbox.confirm(msg + "?", function (result) {
                    if (result === true) {

                        //$scope.permissionPanel.block();

                        system_permission_service.remove(permission.ID).then(function (result) {

                            $scope.uiTreeApi.delete_node(node);

                            //$scope.permissionPanel.unblock();
                        });
                    }
                });
            }
        }

        $scope.createPermission = function () {
            //$scope.newPanel.block();

            $scope.newPermission.Type = $stateParams.type;
            $scope.newPermission.PID = 0;
            $scope.newPermission.BusinessName = $scope.currentBusiness.Key;
            $scope.newPermission.CanInherit = !$scope.newPermission.CannotInherit;

            if ($scope.currentPermission) {
                $scope.newPermission.PID = $scope.currentPermission.ID;
            }

            system_permission_service.create($scope.newPermission).then(function (result) {
                //$scope.newPanel.unblock();
                $scope.newPermission = {
                    CanInherit : true
                };

                loadPermission();
            });
        }

        $scope.updatePermission = function () {
            //$scope.editPanel.block();
            
            $scope.currentPermission.CanInherit = !$scope.currentPermission.CannotInherit;
            system_permission_service.update($scope.currentPermission).then(function () {
                //$scope.editPanel.unblock();

                loadPermission();
            });
        }
    });

});
