define(['apps/system/system.controller',
    'apps/system/system.service.user',
    'apps/system/system.service.business'], function (app) {

        app.controller("system.controller.user.maintain", function ($scope, system_organization_service, system_user_service, system_business_service) {

            $scope.roles = system_organization_service.getRole().$object;
            $scope.allBusiness = system_business_service.getBusiness().$object;

            $scope.hasRole = function (r) {

                if ($scope.currentUser && $scope.currentUser.Roles) {
                    return $scope.currentUser.Roles.contains(function (role) {
                        return role.Key == r.Key;
                    });
                }
                return false;
            }

            $scope.setRole = function (r) {

                if ($scope.hasRole(r)) {
                    $scope.currentUser.Roles.custRemove(function (role) { return role.Key == r.Key; })
                } else {
                    if ($scope.currentUser.Roles == undefined) {
                        $scope.currentUser.Roles = [];
                    }
                    $scope.currentUser.Roles.push(r);
                }
            }

            $scope.hasBusiness = function (b) {
                if ($scope.currentUser && $scope.currentUser.Businesses) {
                    return $scope.currentUser.Businesses.contains(function (bus) {
                        return bus.Key == b.Key;
                    });
                }
                return false;
            }

            $scope.setBusiness = function (b) {
                if ($scope.hasBusiness(b)) {
                    $scope.currentUser.Businesses.custRemove(function (bus) { return bus.Key == b.Key; })
                } else {
                    if ($scope.currentUser.Businesses == undefined) {
                        $scope.currentUser.Businesses = [];
                    }

                    $scope.currentUser.Businesses.push(b);
                }
            }

            $scope.$watch("currentUser.Dept.Key", function (newval,oldval) {
                if (newval) {
                    var dept = $scope.department.find(function (d) { return d.key == newval; });

                    $scope.currentUser.Dept.Name = dept.name;
                }
            })

            $scope.save = function () {
                $scope.maintainPanel.block();
                system_user_service.update($scope.currentUser).then(function () {
                    $scope.maintainPanel.unblock();
                });
            }

            $scope.resetPsw = function () {
                $scope.maintainPanel.block();
                system_user_service.resetPsw($scope.currentUser.ID).then(function () {
                    $scope.maintainPanel.unblock();
                });
            }

            $scope.enable = function () {
                $scope.maintainPanel.block();

                if ($scope.currentUser.Visiable) {
                    system_user_service.disable($scope.currentUser.ID).then(function () {
                        $scope.currentUser.Visiable = false;
                        $scope.maintainPanel.unblock();
                    });
                } else {
                    system_user_service.enable($scope.currentUser.ID).then(function () {
                        $scope.currentUser.Visiable = true;
                        $scope.maintainPanel.unblock();
                    });
                }
            }
        });

        app.controller("system.controller.user.create", function ($scope, system_organization_service, system_user_service, system_business_service) {

            $scope.roles = system_organization_service.getRole().$object;
            $scope.allBusiness = system_business_service.getBusiness({ withuser: true }).$object;

            $scope.newUser = { Visiable : true,Roles: [], Businesses: [] };
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
               
                $scope.createPanel.block();

                system_user_service.checkAccount($scope.newUser.Account).then(function (result) {
                    if (result) {
                        system_user_service.create($scope.newUser).then(function (id) {
                            $scope.newUser.ID = id;
                            $scope.users.push($scope.newUser);

                            $scope.createPanel.unblock();
                        });
                    } else {
                        $scope.accountError = true;

                        $scope.createPanel.unblock();
                    }
                });
            }
        });
    });
