define(['apps/system3/business/business',
        'apps/system3/business/bid/bid.service',
        'apps/system3/business/engineering/engineering.service',
        'apps/system3/business/customer/customer.directive'], function (app) {

            app.module.controller("business.controller.bid.maintain", function ($scope, $stateParams, bidService, engineeringService,$uibModalInstance, maintainParams) {

                $scope.bidInfo = maintainParams.entityInfo ? maintainParams.entityInfo : { };

                $scope.bidInfo.ServicePercent = 0.05;

                // 修改客户后，联系人需要重新选择
                $scope.$watch('bidInfo.Customer.ID', function (newval, oldval) {
                    if (newval) {
                        if ($scope.bidInfo.Customer.Persons != undefined && $scope.bidInfo.Customer.Persons.length > 0) {
                            $scope.bidInfo.PersonID = $scope.bidInfo.Customer.Persons[0].ID;
                        } else {
                            $scope.bidInfo.PersonID = undefined;
                        }
                    }
                });

                // 自动计算代理服务费
                $scope.$watch('bidInfo.ServicePercent', function (newval, oldval) {
                    if (newval) {
                        var val = (new Number( $scope.bidInfo.BidFee) * new Number( newval)).toFixed(2);

                        $scope.bidInfo.ServiceFee = isNaN(val) ? 0 : val ;
                    }
                });
                $scope.$watch('bidInfo.BidFee', function (newval, oldval) {
                    if (newval && $scope.bidInfo.ServicePercent != undefined) {

                        var val = (new Number(newval) * new Number($scope.bidInfo.ServicePercent)).toFixed(2);

                        $scope.bidInfo.ServiceFee = isNaN(val) ? 0 : val;
                    }
                });
                $scope.$watch('bidInfo.ServiceFee', function (newval, oldval) {
                    if (newval) {
                        
                        var val = (new Number($scope.bidInfo.DepositFee) - new Number(newval)).toFixed(2);
                        $scope.bidInfo.ReturnFee = isNaN(val) ? 0 : val;
                    }
                });
                $scope.$watch('bidInfo.DepositFee', function (newval, oldval) {
                    if (newval) {

                        var val = (new Number(newval) - new Number($scope.bidInfo.ServiceFee)).toFixed(2);
                        $scope.bidInfo.ReturnFee = isNaN(val) ? 0 : val;
                    }
                });

                // 保存项目信息
                $scope.save = function () {

                    $scope.bidInfo.CustomerID = $scope.bidInfo.Customer.ID;

                    if ($scope.bidInfo.ID > 0) {
                        bidService.edit($scope.bidInfo).then(function () {
                            $uibModalInstance.close($scope.bidInfo);
                        });
                    } else {
                       
                        bidService.addBid($scope.bidInfo).then(function (id) {

                            $scope.bidInfo.ID = id;
                            $uibModalInstance.close($scope.bidInfo);
                        });
                    }
                };
               
                // 关闭弹出对话框(仅在弹框模式下有用)
                $scope.close = function () {
                    $uibModalInstance.dismiss('cancel');
                }
            });
        });
