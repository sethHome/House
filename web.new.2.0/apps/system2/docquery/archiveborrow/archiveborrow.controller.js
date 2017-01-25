define(['apps/system2/docquery/docquery', 'apps/system2/docquery/docquery.service'], function (app) {

    app.module.controller("docquery.controller.archiveborrow", function ($scope,$rootScope, $sce, $uibModal, $filter, docqueryService) {

        $scope.borrowInfo = {};

        $scope.removeArchiveItem = function (item) {
            $rootScope.archiveBorrowList.removeObj(item);
        }
      
        // 保存
        $scope.save = function (flow) {

            if ($rootScope.archiveBorrowList == undefined || $rootScope.archiveBorrowList.length == 0) {
                bootbox.alert('借阅列表没有档案！');
                return false;
            }

            // 借阅
            $scope.borrowInfo.FlowData = flow.flowInfo;

            $scope.borrowInfo.Items = $rootScope.archiveBorrowList.map(function (a) {
                return {
                    Fonds: a.fonds,
                    ArchiveID: a.archiveID,
                    ArchiveType: a.type,
                }
            });

            docqueryService.archiveBorrow($scope.borrowInfo).then(function () {

                flow.callBack(function () {

                    // 清空借阅列表
                    $rootScope.archiveBorrowList = [];

                    bootbox.alert('借阅成功！');
                });
            });

            return true;
        }
    });

});
