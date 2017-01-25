define([
'apps/system2/docsetting/docsetting',
'apps/system2/docsetting/docsetting.service'], function (app) {

    app.module.controller("docsetting.controller.archive.field", function ($scope, docsettingService, $uibModal) {

        $scope.$watch("currentArchive", function (newval, oldval) {
            if (newval) {
                loadFields();
            }
        });

        var loadFields = function () {
            if ($scope.currentArchive.key) {
                docsettingService.archive.getFields($scope.currentArchive.fondsNumber, $scope.currentArchive.archiveKey, $scope.currentArchive.key).then(function (result) {
                    $scope.fieldList = result;
                });
            }
            else {
                $scope.fieldList = [];
            }

        }

        $scope.addField = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/field/view/field-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.archive.field.maintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: false,
                            fieldInfo: {
                                Fonds: $scope.currentArchive.fondsNumber,
                                Archive: $scope.currentArchive.archiveKey,
                                ParentKey: $scope.currentArchive.key
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
            field.Fonds = $scope.currentArchive.fondsNumber;
            field.Archive = $scope.currentArchive.archiveKey;

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/field/view/field-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.archive.field.maintain",
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
                    docsettingService.archive.removeField($scope.currentArchive.fondsNumber, $scope.currentArchive.archiveKey, $scope.currentArchive.key, field.ID).then(function () {
                        $scope.fieldList.removeObj(field);
                    });
                }
            });
        }


    });

    app.module.controller("docsetting.controller.archive.field.maintain", function ($scope, docsettingService, $uibModal, $uibModalInstance, maintainInfo) {
        $scope.update = maintainInfo.update;
        $scope.field = maintainInfo.fieldInfo;

        $scope.save = function () {
           
            if (maintainInfo.update) {
                docsettingService.archive.updateField($scope.field).then(function () {
                    $uibModalInstance.close($scope.field);
                });
            } else {

                docsettingService.archive.checkField($scope.field).then(function (result) {
                    if (!result) {
                        docsettingService.archive.addField($scope.field).then(function () {
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
