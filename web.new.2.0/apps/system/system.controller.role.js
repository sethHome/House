define(['apps/system/system.controller',
    'apps/system/system.service.business',
    'apps/system/system.service.organization'], function (app) {

    app.controller("system.controller.role", function ($scope, $stateParams,
        system_organization_service, system_business_service) {

        var loadRole = function () {
            $scope.rolePanel.block();
            system_organization_service.getRole().then(function (source) {
                $scope.rolePanel.unblock();
                $scope.roles = source;
                if (!$scope.currentRole) {
                    $scope.currentRole = $scope.roles[0];
                }
            });
        }

        $scope.$watch('$viewContentLoaded', function () {

            loadRole();
        });

        $scope.$watch("currentRole", function (newVal, oldVal) {
            if (newVal) {
                $scope.$broadcast("$RoleSelected", newVal);
            }
        });

        $scope.currentIndex = 0
       
        $scope.roleChanged = function (role,index) {
            $scope.currentRole = role;
            $scope.currentRole.index = index;
        }

        // 保存权限
        $scope.savePermission = function (permission, callback) {
            system_organization_service.changeRolePermission($scope.currentRole.Key, permission.Permissions).then(function () {
                loadRole();
                callback();
            });
        }

        // 保存用户
        $scope.saveUser = function (users, callback) {
            system_organization_service.changeRoleUsers($scope.currentRole.Key, users).then(function () {
                loadRole();
                callback();
            });
        }

        // 添加角色
        $scope.add = function () {
            var newRole = { Key: "newKey" };
            $scope.currentRole = newRole;
            $scope.editModel = true;
            $scope.roles.push(newRole);
        }

        $scope.rename = function () {
            $scope.editModel = !$scope.editModel;
        }

        $scope.remove = function () {
            system_organization_service.removeRole($scope.currentRole.Key).then(function (key) {
                $scope.roles.splice($scope.currentRole.index, 1);
            });
        }

        $scope.saveRole = function () {
            if ($scope.currentRole.Key != "newKey") {
                // rename
                system_organization_service.renameRole($scope.currentRole.Name, $scope.currentRole.Key).then(function (key) {
                    $scope.editModel = false;
                });
                
            } else {
               
                system_organization_service.createRole($scope.currentRole.Name).then(function (key) {
                    $scope.currentRole.Key = key;
                    $scope.editModel = false;
                });
            }
        }
    });
});
