define(['apps/system3/home/home.controller',
'apps/system3/home/desktop/desktop.service'], function (app) {

    app.controller("home.controller.productiontask", function ($scope, $stateParams, desktopService) {

        $scope.currentTaskID = $stateParams.id;

        $scope.volumeInfo = desktopService.getVolumeInfo($stateParams.id).$object;

        // 提交下一步回调
        $scope.check = function (flow) {

            // 回退不检查校审意见
            if (!flow.flowInfo.isBack) {

                var result = true;

                angular.forEach($scope.checkItems, function (item) {

                    //  设计检查校审意见
                    if ($scope.isDesignModel) {
                        if (!item.IsCorrect) {
                            result = false;
                            bootbox.alert("所有的校审意见必须修改完成");
                            return false;
                        }
                    } else {
                        if (!item.IsPass) {
                            result = false;
                            bootbox.alert("所有的校审意见必须已通过");
                            return false;
                        }
                    }
                });

                if (!result) {
                    return false;
                }
            }

            flow.callBack(function () {
                
                //$rootScope.$broadcast("$ReloadUserTask");
                
            });

            return true;
        }
    });

});
