define(['apps/system2/docquery/docquery', 'apps/system2/docquery/docquery.service'], function (app) {

    app.module.controller("docquery.controller.archiveapprove", function ($scope,  $uibModal, $filter, docqueryService) {

        $scope.borrowInfo = {};

        var loadTask = function () {
            docqueryService.getMyArchiveArrpve().then(function (data) {
                $scope.tasks = data;
            })
        }

        $scope.setCurrentItem = function (item) {

            $scope.borrowInfo = item.BorrowInfo;

            $scope.taskID = item.ID;

            docqueryService.getMyArchiveItems(item.BorrowInfo.ID).then(function (result) {
                $scope.borrowItem = result;
            })
        }


        // 保存
        $scope.save = function (flow) {

            // 借阅
            $scope.borrowInfo.FlowData = flow.flowInfo;

            flow.callBack(function () {

                $scope.borrowInfo = {};

                $scope.taskID = 0;

                loadTask();
            });

            return true;
        }

        loadTask();
    });

});
