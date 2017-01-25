define([
'apps/system2/docsetting/docsetting',
'apps/system2/docsetting/docsetting.service'], function (app) {

    app.module.controller("docsetting.controller.field", function ($scope, docsettingService, $uibModal) {

        $scope.items = [
            { Text: '项目',   Value: 'Project' },
            { Text: '案卷',   Value: 'Volume' },
            { Text: '盒',    Value: 'Box' },
            { Text: '文件',   Value: 'File' }];

        $scope.currentItem = { Key: "Project" };

        var loadFields = function () {
            $scope.fieldList = docsettingService.field.getList($scope.currentItem.Key).$object;
        }

        $scope.createTable = function () {

            docsettingService.archive.generateProject().then(function (result) {
                bootbox.alert("创建成功");
            });
        }

        $scope.addField = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/field/view/field-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.field.maintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: false,
                            fieldInfo: {
                                ParentKey: $scope.currentItem.Key
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                loadFields();
            }, function () {
                //dismissed
            });
        }

        $scope.update = function (field) {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/field/view/field-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.field.maintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: true,
                            fieldInfo: field
                        };
                    }
                }
            });
        }

        $scope.remove = function (field) {
            bootbox.confirm("确定删除？", function (result) {
                if (result === true) {
                    docsettingService.field.remove(field.ParentKey, field.ID).then(function () {
                        $scope.fieldList.removeObj(field);
                    });
                }
            });
        }

        $scope.$watch("currentItem.Key", function (newval, old) {
            if (newval) {
                loadFields();
            }
        })


    });

    app.module.controller("docsetting.controller.field.maintain", function ($scope, docsettingService, $uibModal, $uibModalInstance, maintainInfo) {
        $scope.update = maintainInfo.update;
        $scope.field = maintainInfo.fieldInfo;

        $scope.save = function () {
            if (maintainInfo.update) {
                docsettingService.field.update($scope.field).then(function () {
                    $uibModalInstance.close($scope.field);
                });
            } else {

                docsettingService.field.check($scope.field).then(function (result) {
                    if (!result) {
                        docsettingService.field.add($scope.field).then(function () {
                            $uibModalInstance.close($scope.field);
                        });
                    } else {
                        bootbox.alert("字段名称重复");
                    }
                })
            }
        };

        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    });
});
