define(['apps/system3/business/business',
        'apps/system3/business/customer/customer.service',
        'apps/system3/business/engineering/engineering.service',
        'apps/system3/business/customer/customer.directive'], function (app) {

            app.module.controller("business.controller.customer.maintain",
                function ($scope, $stateParams, customerService, engineeringService,$uibModalInstance, maintainParams) {

                    $scope.custInfo = maintainParams.entityInfo ? maintainParams.entityInfo : { Persons :[]};

                // 添加联系人
                $scope.addPerson = function () {
                    if ($scope.custInfo.ID > 0) {
                        customerService.addPerson($scope.custInfo.ID,$scope.personInfo).then(function (p) {
                            $scope.custInfo.Persons.push(p);
                        });
                    } else {
                        $scope.custInfo.Persons.push(angular.copy($scope.personInfo));
                    }
                }

                // 更新联系人
                $scope.updatePerson = function (person) {
                    if ($scope.custInfo.ID > 0) {
                        customerService.updatePerson(person);
                    }
                    person.edit = false;
                }

                // 删除联系人
                $scope.deletePerson = function (person) {
                    if ($scope.custInfo.ID > 0) {
                        customerService.deletePerson(person.ID);
                    }
                    $scope.custInfo.Persons.removeObj(person);
                }

                // 页面重置
                $scope.reset = function () {
                    $scope.custInfo.Persons = [];
                }

                // 保存
                $scope.save = function () {

                    if ($scope.custInfo.ID > 0) {
                        customerService.edit($scope.custInfo).then(function () {
                            $uibModalInstance.close($scope.custInfo);
                        });
                    } else {

                        $scope.custInfo.Type = $stateParams.type;
                        customerService.addCustomer($scope.custInfo).then(function (id) {
                            $scope.custInfo.ID = id;
                            $uibModalInstance.close($scope.custInfo);
                        });;
                    }
                };


                // 关闭弹出对话框(仅在弹框模式下有用)
                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });
        });
