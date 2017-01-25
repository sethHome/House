define(['apps/system/system.controller',
'apps/system/system.service.business',
'apps/system/system.service.user'], function (app) {

    app.controller("system.controller.business", function ($scope, system_business_service, system_user_service) {

        var loadBusiness = function () {
            $scope.sysPanel.blockUI();
            system_business_service.getBusiness({withuser:true}).then(function (result) {

                $scope.business = result;
                $scope.currentBusiness = result[0];
                $scope.sysPanel.unblockUI();
            })
        }

        var loadUser = function () {
            $scope.userPanel.blockUI();
            system_user_service.getUsers(false,false,false).then(function (result) {
                
                $scope.users = result;
                if ($scope.currentBusiness) {
                    opUser($scope.currentBusiness);
                }
                $scope.userPanel.unblockUI();
            });
        }

        var opUser = function (business) {
            angular.forEach($scope.users, function (user) {
                user.isSelected = false;
                for (var u in business.Users) {

                    if (user.ID == business.Users[u].ID) {
                        user.isSelected = true;
                        break;
                    }
                }
            })
        }

        $scope.businessChanged = function (b) {
            $scope.currentBusiness = b;
        }

        $scope.$watch('currentBusiness', function (newVal,oldVal) {
           
            if (newVal) {

                if ($scope.users) {
                    opUser(newVal)
                }

                $scope.$broadcast("$BusinessSelected", newVal);
            }
        });

        $scope.$watch('$viewContentLoaded', function () {

            loadBusiness();

            loadUser();
        });

        $scope.save = function () {

            $scope.userPanel.blockUI();

            var checkedUsers = [];

            angular.forEach($scope.users, function (user) {
                if (user.isSelected) {
                    checkedUsers.push(user);
                }
            });

            system_business_service.setUsers($scope.currentBusiness.Key, checkedUsers).then(function () {
                loadBusiness();

                $scope.userPanel.unblockUI();
            })
        }
    });

    app.controller("system.controller.business.maintain", ['$scope', function ($scope) {
        $scope.msg = "maintain"
    }]);
});
