define(['apps/base/base.controller'],
    function (module) {

        module.controller('base.controller.changePassword',
            function ($scope, $rootScope, $uibModalInstance, userService) {

                $scope.$watch("newPsw", function (newval, oldval) {
                    if (newval && $scope.newPsw2) {
                        if (newval != $scope.newPsw2) {
                            $scope.error1 = true;
                        }
                        else {
                            $scope.error1 = false;
                        }
                    }
                })

                $scope.$watch("newPsw2", function (newval,oldval) {
                    if (newval) {
                        if (newval != $scope.newPsw) {
                            $scope.error1 = true;
                        }
                        else {
                            $scope.error1 = false;
                        }
                    }
                })

                $scope.change = function () {
                    userService.changepassword($scope.oldPsw, $scope.newPsw).then(function (result) {
                        if (result == -1) {
                            $scope.error2 = true;
                        } else {
                            $scope.error2 = false;
                            $uibModalInstance.dismiss('cancel');
                        }
                    });
                }

                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });
    });
