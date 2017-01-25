define([
'apps/system2/docsetting/docsetting',
'apps/system2/docsetting/docsetting.service'], function (app) {

    app.module.controller("docsetting.controller.fonds", function ($scope, docsettingService, $uibModal) {

        $scope.fondsList = docsettingService.fonds.getList().$object;

        $scope.add = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/fonds/view/fonds-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.fonds.maintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update : false,
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                $scope.fondsList.push(info);
            }, function () {
                //dismissed
            });
        };

        $scope.update = function (f) {
            $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/fonds/view/fonds-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.fonds.maintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: true,
                            fondsInfo: f
                        };
                    }
                }
            });
        };

        $scope.delete = function (f) {
            bootbox.confirm("确定删除？", function (result) {
                if (result === true) {
                    docsettingService.fonds.remove(f.Number).then(function () {
                        $scope.fondsList.removeObj(f);
                    });
                }
            });
        }
    });

    app.module.controller("docsetting.controller.fonds.maintain", function ($scope, docsettingService, $uibModal, $uibModalInstance, maintainInfo) {
        $scope.update = maintainInfo.update;
        $scope.fonds = maintainInfo.fondsInfo;

        $scope.save = function () {
            if (maintainInfo.update) {
                docsettingService.fonds.update($scope.fonds).then(function () {
                    $uibModalInstance.close($scope.fonds);
                });
            } else {

                docsettingService.fonds.check($scope.fonds.Number).then(function (result) {
                    if (!result) {
                        docsettingService.fonds.add($scope.fonds).then(function () {
                            $uibModalInstance.close($scope.fonds);
                        });
                    } else {
                        bootbox.alert("全宗号重复");
                    }
                })
            }
        };

        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    });
});
