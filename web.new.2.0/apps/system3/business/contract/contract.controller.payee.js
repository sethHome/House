define(['apps/system3/business/business',
        'apps/system3/business/contract/contract.service',
        'apps/system3/business/customer/customer.directive'], function (app) {

            app.module.controller("business.controller.contract.payee", function ($scope, contractService,$uibModalInstance, maintainParams, attachService) {

                $scope.conInfo = maintainParams.entityInfo;

                $scope.rePayees = [];
                $scope.acPayees = [];
                $scope.blPayees = [];

                contractService.getPayees($scope.conInfo.ID).then(function (data) {
                    angular.forEach(data, function (item) {

                        for (var i = 0; i < $scope.conInfo.Engineerings.length; i++) {
                            if ($scope.conInfo.Engineerings[i].ID == item.ObjectID) {
                                item.Engineering = $scope.conInfo.Engineerings[i];
                                break;
                            }
                        }


                        if (item.Type == 1) {
                            $scope.rePayees.push(item);
                        } else if (item.Type == 2) {
                            $scope.acPayees.push(item);
                        } else if (item.Type == 3) {
                            $scope.blPayees.push(item);
                        }
                    });
                });
                

                // 预收款
                $scope.reTotalFee = 0.00;
                $scope.rePayee = {};

                $scope.addRePayee = function () {

                    $scope.maintainPanel.block();

                    $scope.rePayee.Type = 1; // 1:预收款
                    $scope.rePayee.ObjectKey = 'Engineering'; 
                    $scope.rePayee.ObjectID = $scope.rePayee.Engineering.ID;

                    contractService.addPayee($scope.conInfo.ID, $scope.rePayee).then(function (id) {
                  
                        var obj = angular.copy($scope.rePayee);

                        obj.ID = id;
                        $scope.rePayees.push(obj);

                        $scope.maintainPanel.unblock();
                    });
                }

                $scope.delRePayee = function (item) {
                    $scope.maintainPanel.block();
                    contractService.deletePayee(item.ID).then(function () {
                        $scope.rePayees.removeObj(item);
                        $scope.maintainPanel.unblock();
                    });
                }

                $scope.$watch("rePayees", function (newval, oldval) {
                    $scope.reTotalFee = 0.00;

                    angular.forEach(newval, function (item) {
                        $scope.reTotalFee += new Number(item.Fee);
                    });
                   
                    $scope.reTotalFee = $scope.reTotalFee.toFixed(2);
                }, true);


                // 实际收费
                $scope.acTotalFee = 0.00;
                $scope.acPayee = {};

                $scope.addAcPayee = function () {
                    
                    $scope.maintainPanel.block();

                    $scope.acPayee.Type = 2; // 1:预收款
                    $scope.acPayee.ObjectKey = 'Engineering';
                    $scope.acPayee.ObjectID = $scope.acPayee.Engineering.ID;

                    contractService.addPayee($scope.conInfo.ID, $scope.acPayee).then(function (id) {
                        var obj = angular.copy($scope.acPayee);
                        obj.ID = id;
                        $scope.acPayees.push(obj);

                        $scope.maintainPanel.unblock();
                    });
                }

                $scope.delAcPayee = function (item) {
                    $scope.maintainPanel.block();
                    contractService.deletePayee(item.ID).then(function () {
                        $scope.acPayees.removeObj(item);
                        $scope.maintainPanel.unblock();
                    });
                }

                $scope.$watch("acPayees", function (newval, oldval) {
                    $scope.acTotalFee = 0.00;

                    angular.forEach(newval, function (item) {
                        $scope.acTotalFee += new Number(item.Fee);
                    });

                    $scope.acTotalFee = $scope.acTotalFee.toFixed(2);
                }, true);

                // 开票Billing
                $scope.blTotalFee = 0.00;
                $scope.blPayee = {};

                $scope.addBlPayee = function () {

                    $scope.maintainPanel.block();

                    $scope.blPayee.Type = 3; // 1:预收款
                    $scope.blPayee.ObjectKey = 'Engineering';
                    $scope.blPayee.ObjectID = $scope.blPayee.Engineering.ID;

                    contractService.addPayee($scope.conInfo.ID, $scope.blPayee).then(function (id) {
                        var obj = angular.copy($scope.blPayee);
                        obj.ID = id;
                        $scope.blPayees.push(obj);

                        $scope.maintainPanel.unblock();
                    });
                }

                $scope.delBlPayee = function (item) {
                    $scope.maintainPanel.block();
                    contractService.deletePayee(item.ID).then(function () {
                        $scope.blPayees.removeObj(item);
                        $scope.maintainPanel.unblock();
                    });
                }

                $scope.$watch("blPayees", function (newval, oldval) {
                    $scope.blTotalFee = 0.00;

                    angular.forEach(newval, function (item) {
                        $scope.blTotalFee += new Number(item.Fee);
                    });

                    $scope.blTotalFee = $scope.blTotalFee.toFixed(2);
                }, true);

                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }

            });

        });
