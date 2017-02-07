define(['apps/system/system.controller',
    'apps/system/system.service.enum'], function (app) {

        app.controller("system.controller.enum", ['$scope', 'system_enum_service', function ($scope, system_enum_service) {

            $scope.newEnum = {};
            $scope.newItem = {};
            $scope.newTag = {};

            var loadEnum = function () {
                system_enum_service.all().then(function (data) {
                    $scope.enums = data;
                });
            }

            $scope.$watch('$viewContentLoaded', function () {

                loadEnum();
            });

            $scope.enumChanged = function (e) {
                $scope.currentEnum = e;
                $scope.currentItem = e.Items[0];
            }

            $scope.itemChanged = function (i) {
                $scope.currentItem = i;
            }

            $scope.tagChanged = function (t) {
                $scope.currentTag = t;
            }

            // enum
            $scope.addEnum = function () {

                system_enum_service.addEnum($scope.newEnum).then(function (key) {
                    $scope.enums.push({
                        Key: key,
                        Name: $scope.newEnum.Name,
                        Text: $scope.newEnum.Text,
                        Items: []
                    });
                    $scope.newEnum = {};
                    bootbox.alert("保存成功");
                });
            }

            $scope.updateEnum = function () {
                system_enum_service.updateEnum($scope.currentEnum).then(function () {
                    bootbox.alert("更新成功");
                });
            }

            $scope.deleteEnum = function () {
                system_enum_service.deleteEnum($scope.currentEnum.Name).then(function () {
                    $scope.enums.removeObj($scope.currentEnum);
                    $scope.currentEnum = undefined;
                    bootbox.alert("删除成功");
                });
            }

            // item
            $scope.addItem = function () {

                system_enum_service.addItem($scope.currentEnum.Name, $scope.newItem, $scope.add2).then(function (value) {

                    $scope.currentEnum.Items.push({
                        Key: value,
                        Value: value,
                        Text: $scope.newItem.Text,
                        Tags: { index: value }
                    });

                    $scope.newItem = {};
                    bootbox.alert("保存成功");
                });
            }

            $scope.updateItem = function () {

                system_enum_service.updateItem($scope.currentEnum.Name, $scope.currentItem).then(function () {

                    bootbox.alert("更新成功");
                });
            }

            $scope.deleteItem = function () {

                system_enum_service.deleteItem($scope.currentEnum.Name, $scope.currentItem.Value).then(function () {
                    $scope.currentEnum.Items.removeObj($scope.currentItem);

                    $scope.currentItem = undefined;
                    bootbox.alert("删除成功");
                });
            }

            // tag
            $scope.addTag = function () {

                system_enum_service.addTag($scope.currentEnum.Name, $scope.currentItem.Value, $scope.newTag).then(function () {

                    $scope.currentItem.Tags[$scope.newTag.Key] = $scope.newTag.Value;

                    $scope.Tags.push({
                        Key: $scope.newTag.Key,
                        Value: $scope.newTag.Value,
                    });
                   
                    $scope.newTag = {};
                    bootbox.alert("保存成功");
                });
            }

            $scope.updateTag = function () {

                system_enum_service.updateTag($scope.currentEnum.Name, $scope.currentItem.Value, $scope.currentTag).then(function () {

                    bootbox.alert("更新成功");
                });
            }

            $scope.deleteTag = function () {

                system_enum_service.deleteTag($scope.currentEnum.Name, $scope.currentItem.Value, $scope.currentTag.Key).then(function () {

                    $scope.currentItem.Tags[$scope.currentTag.Key] = undefined;
                    $scope.Tags.removeObj($scope.currentTag);

                    $scope.currentTag = undefined;
                    bootbox.alert("删除成功");
                });
            }

            $scope.$watch('currentItem', function (newVal, oldVal) {
                if (newVal) {

                    $scope.Tags = [];
                    for (var t in newVal.Tags) {
                        if (newVal.Tags[t]) {
                            $scope.Tags.push({
                                Key: t,
                                Value: newVal.Tags[t]
                            });
                        }
                    }

                    $scope.currentTag = $scope.Tags[0];

                }
            });

            $scope.$watch('currentTag.Value', function (newVal, oldVal) {
                if (newVal) {
                    $scope.currentItem.Tags[$scope.currentTag.Key] = newVal;
                    
                }
            });

        }]);

    });
