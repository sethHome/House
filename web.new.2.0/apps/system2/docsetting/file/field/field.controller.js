define([
'apps/system2/docsetting/docsetting',
'apps/system2/docsetting/docsetting.service'], function (app) {

    app.module.controller("docsetting.controller.file.field", function ($scope, docsettingService, $uibModal) {

        $scope.$watch("currentFile", function (newval, oldval) {
            if (newval) {
               
                loadFields();
            }
        });

        var loadFields = function () {
            $scope.fieldList = [];
            if ($scope.currentFile.fileNumber) {
                docsettingService.file.getFields($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber).then(function (result) {
                    $scope.fieldList = result;
                });
            }
        }

        $scope.addField = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/field/view/field-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.file.field.maintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: false,
                            fieldInfo: {
                                Fonds: $scope.currentFile.FondsNumber,
                                ParentKey: $scope.currentFile.fileNumber
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
            field.Fonds = $scope.currentFile.FondsNumber;

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/field/view/field-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.file.field.maintain",
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
                    docsettingService.file.removeField($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber, field.ID).then(function () {
                        $scope.fieldList.removeObj(field);
                    });
                }
            });
        }


    });

    app.module.controller("docsetting.controller.file.field.maintain", function ($scope, docsettingService, $uibModal, $uibModalInstance, maintainInfo) {
        $scope.update = maintainInfo.update;
        $scope.field = maintainInfo.fieldInfo;

        $scope.save = function () {
            if (maintainInfo.update) {
                docsettingService.file.updateField($scope.field).then(function () {
                    $uibModalInstance.close($scope.field);
                });
            } else {

                docsettingService.file.checkField($scope.field).then(function (result) {
                    if (!result) {
                        docsettingService.file.addField($scope.field).then(function () {
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
