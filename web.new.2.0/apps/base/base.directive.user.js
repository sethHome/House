define(['apps/base/base.directive'],
    function (app) {

        app.directive('userChoose', function () {

            return {
                restrict: 'EA',
                template: '<button type="button" ng-click="openUserWindow()" class="btn btn-primary btn-square "><i  class="icon-users "></i>&nbsp;{{text}}</button>',
                scope: {
                    text: '@',
                    users: '=',
                    max: '=',
                    fixedUser: '='
                },
                controller: function ($scope, $rootScope, $uibModal) {
                    //var maxUserLength = $scope.max;
                    var parentScope = $scope;

                    $scope.openUserWindow = function () {
                        var modalInstance = $uibModal.open({
                            animation: false,
                            templateUrl: 'apps/base/view/select-user.html',
                            controller: function ($scope, $rootScope, $uibModalInstance, users) {

                                $scope.chooseUsers = users;
                                $scope.depts = $rootScope.department;

                                if ($scope.chooseUsers == undefined) {
                                    $scope.chooseUsers = [];
                                }

                                angular.forEach($scope.depts, function (d) {
                                    angular.forEach(d.users, function (u) {
                                        if ($scope.chooseUsers.contains(function (_u) { return _u.ID == u.ID })) {
                                            u.hide = true;
                                        } else if (u.hide) {
                                            u.hide = false;
                                        }
                                    })
                                })

                                // 已选择的用户从左边选择框内隐藏
                                angular.forEach($scope.chooseUsers, function (user) {
                                    user.hide = true;
                                })

                                // 判断用户是否指定为只读
                                $scope.isFixed = function (user) {
                                    if (parentScope.fixedUser) {
                                        if (angular.isArray(parentScope.fixedUser)) {
                                            return parentScope.fixedUser.contains(function (u) { return (u.ID ? u.ID : u) == user.ID })
                                        } else {
                                            return (parentScope.fixedUser.ID ? parentScope.fixedUser.ID : parentScope.fixedUser) == user.ID;
                                        }
                                    } else {
                                        return false;
                                    }
                                }

                                $scope.showUser = function (dept) {
                                    dept.show = !dept.show;
                                    $scope.userScroll.init();
                                }

                                $scope.chooseUser = function (user) {
                                    if (!parentScope.max || $scope.chooseUsers.length < parentScope.max) {
                                        $scope.chooseUsers.push(user);
                                        user.hide = true;
                                    }
                                }

                                $scope.removeUser = function (user) {

                                    if (!$scope.isFixed(user)) {
                                        user.hide = false;
                                        $scope.chooseUsers.removeObj(user);
                                    }
                                }

                                // 取消
                                $scope.cancel = function () {
                                    angular.forEach($scope.chooseUsers, function (user) {
                                        user.hide = false;
                                    });
                                    $scope.chooseUsers = [];
                                    $uibModalInstance.close([]);
                                }

                                // 关闭
                                $scope.closeModal = function () {

                                    $uibModalInstance.dismiss('cancel');
                                }

                                // 确定
                                $scope.ok = function () {
                                    $uibModalInstance.close($scope.chooseUsers);
                                };

                            },
                            resolve: {
                                users: function () {
                                    return $scope.users;
                                }
                            }
                        });

                        modalInstance.result.then(function (chooseUser) {
                            $scope.users = chooseUser;
                        }, function () {
                            //$scope.users = [];
                        });
                    }

                },
                link: function ($scope, element, attrs) {

                }
            };

        });
    });
