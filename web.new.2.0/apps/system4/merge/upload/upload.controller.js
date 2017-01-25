define([
'apps/system4/merge/merge',
'apps/system4/merge/merge.service'], function (app) {

    app.module.controller("merge.controller.upload", function ($scope, mergeService, $uibModal) {

        $scope.filters = [{ title: "OfficeWord", extensions: "docx" }];

        $scope.selected_changed = function (e, data) {

            if (data.node.type == 'file') {
                $scope.$safeApply(function () {
                    $scope.currentTask = data.node.original;
                })
            }
        }

        $scope.beforeUpload = function (file) {
            $scope.upPanel.block();
        }

        $scope.attachUploaded = function (attachID) {
            mergeService.taskFinish($scope.currentTask.ID, attachID).then(function () {
                //loadData();
                bootbox.alert("上传成功");

                $scope.upPanel.unblock();
            })
        }

        var loadData = function () {

            mergeService.getMyTask().then(function (result) {

                var specs = $scope.getBaseEnum("Specialty");

                angular.forEach(result, function (proj) {

                    proj.text = proj.Name;
                    proj.type = 'folder';
                    angular.forEach(proj.ProjSpecils, function (s) {

                        s.text = specs[s.SpecilID];
                        s.type = 'file';
                    })

                    proj.children = proj.ProjSpecils;
                })

                $scope.tasks = result;
            });
        }

        loadData();
    });

});
