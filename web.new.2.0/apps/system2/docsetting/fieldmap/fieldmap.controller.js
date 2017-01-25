define([
'apps/system2/docsetting/docsetting',
'apps/system2/docsetting/docsetting.service'], function (app) {

    app.module.controller("docsetting.controller.fieldmap", function ($scope, $state, docsettingService, $uibModal) {

        app.module.filter('fieldmap', function () {
            var filterfun = function (data) {

                if ($scope.fileFileds) {
                    var fieldInfo = $scope.fileFileds.find(function (item) { return item.ID == data; });
                    return fieldInfo ? fieldInfo.Name : ""
                }

                return ""

            };
            return filterfun;
        });

        var loadArchive = function () {
            docsettingService.archive.getList().then(function (result) {

                angular.forEach(result, function (item) {

                    angular.forEach(item.Archives, function (a) {

                        a.text = a.Name;
                        a.type = "database";
                        a.state = { 'opened': true };
                        a.FondsNumber = item.Number;
                        a.src = "docsetting.archive.op";

                        if (a.Disabled) {
                            a.type = "disabled";
                            a.text = "<span class='bg-orange'>" + a.Name + "&nbsp;[禁用]</span>"
                        } else {
                            a.children = [];

                            if (a.HasVolume) {
                                a.children.push({
                                    text: "案卷",
                                    parentText: a.Name,
                                    type: 'file',
                                    fondsNumber: item.Number,
                                    archiveKey: a.Key,
                                    key: 'Volume',
                                    src: "docsetting.archive.field"
                                });
                            }

                            a.children.push({
                                text: "文件",
                                parentText: a.Name,
                                type: 'file',
                                fondsNumber: item.Number,
                                archiveKey: a.Key,
                                key: 'File',
                                src: "docsetting.archive.field"
                            });
                        }
                    })

                    item.children = item.Archives;
                    item.text = item.Name;
                    item.type = "cloud"
                    item.state = { 'opened': true };
                    item.src = "docsetting.archive.op";
                    //$scope.archives.push(item);
                })

                $scope.archiveNodes = result;
            });
        }

        var loadFileLibrarys = function () {

            docsettingService.file.getList().then(function (result) {

                angular.forEach(result, function (item) {

                    item.text = item.Name;
                    item.type = "cloud"
                    item.state = { 'opened': true };
                    angular.forEach(item.Children, function (item2) {

                        item2.text = item2.Name;
                        item2.type = "folder"
                    })

                    item.children = item.Children;
                })

                $scope.fileNodes = result;
            });
        }

        var loadArchiveFields = function () {
            if ($scope.currentArchive.key) {
                docsettingService.archive.getFields($scope.currentArchive.fondsNumber, $scope.currentArchive.archiveKey, $scope.currentArchive.key,true).then(function (result) {
                   
                    $scope.archiveFields = result;

                    setArchiveFieldMap();
                });
            }
            else {
                $scope.archiveFields = [];
            }
        }

        var loadFileFields = function () {

            if ($scope.currentFile.Number) {
                docsettingService.file.getFields($scope.currentFile.FondsNumber, $scope.currentFile.Number).then(function (result) {
                    $scope.fileFileds = result;

                    setArchiveFieldMap();
                });
            }
            else {
                $scope.fileFileds = [];

            }
        }

        var setArchiveFieldMap = function () {
            if($scope.currentFile){
                $scope.archiveFields = $scope.archiveFields.map(function (item) {
                    if (item.Mappings) {
                        var file = item.Mappings.find(function (a) { return a.FileNumber == $scope.currentFile.Number });
                        item.FileFieldID = file ? file.FileFieldID : 0;
                        item.MappingType = file ? file.MappingType : 0;
                    } else {
                        item.FileFieldID = 0;
                        item.MappingType = 0;
                    }

                    return item;
                });

                $scope.gridApi.core.refresh();
                $scope.gridApi.core.refreshRows();
                //$scope.gridApi.core.notifyDataChange();
            }
        }

        // 表格配置
        $scope.mapGridOptions = {
            data: "archiveFields",
            enableGridMenu: false,
            columnDefs: [
                {
                    name: 'Name', displayName: "档案库字段", width: 150, enableColumnMenu: false,
                    cellEditableCondition: false
                },
                {
                    name: 'MappingType', displayName: "对应规则", width: 100, enableColumnMenu: false,
                    cellFilter: "enumMap:'MappingType'",
                    getSource: function () {

                        return $scope.getBaseData("MappingType");
                    },
                    editableCellTemplate: 'uiMappingSelect',

                },
                {
                    name: 'FileFieldID', displayName: "文件库字段", width: 150, enableColumnMenu: false,
                    cellFilter: 'fieldmap',
                    getSource: function () {
                        return $scope.fileFileds;
                    },
                    editableCellTemplate: 'uiSelect',
                },
                {
                    name: 'IsRemove', displayName: "删除映射", width: 100, enableColumnMenu: false,
                    cellEditableCondition: false,
                    cellTemplate: '<div class="ui-grid-cell-contents" ><a class="btn btn-sm  " ng-click="grid.appScope.removeMapping(row)">移除</a></div>',
                },
            ],
            onRegisterApi: function (gridApi) {

                $scope.gridApi = gridApi;
            }
        };

        $scope.removeMapping = function (row) {

            if (row.entity.FileFieldID > 0) {
                row.entity.IsRemove = true;
            }
            
            row.entity.FileFieldIDBak = row.entity.FileFieldID;
            row.entity.FileFieldID = undefined;
            row.entity.MappingType = undefined;
            
        }

        $scope.archive_changed = function (e, data) {

            $scope.$safeApply(function () {
                $scope.currentArchive = data.node.original;
            })
        }

        $scope.file_changed = function (e, data) {

            $scope.$safeApply(function () {
                $scope.currentFile = data.node.original;
            })
        }

        $scope.$watch("currentArchive", function (newval, oldval) {
            if (newval) {
                loadArchiveFields();
            }
        });

        $scope.$watch("currentFile", function (newval, oldval) {
            if (newval) {
                loadFileFields();
            }
        });

        $scope.saveMapping = function () {

            var maps = $scope.archiveFields.where(function (item) {
                
                return item.FileFieldID > 0 || item.IsRemove;
            });

            var maps = maps.map(function (item) {
                 return {
                        FondsNumber: $scope.currentArchive.fondsNumber,
                        ArchiveNumber: $scope.currentArchive.archiveKey,
                        ArchiveKey: $scope.currentArchive.key,
                        ArchiveFieldID: item.ID,
                        FileNumber: $scope.currentFile.Number,
                        FileFieldID: item.FileFieldID ? item.FileFieldID : item.FileFieldIDBak,
                        MappingType : item.MappingType,
                        IsRemove: item.IsRemove && item.FileFieldID == undefined
                    };
            });

            docsettingService.argument.saveMapping(maps).then(function () {

                bootbox.alert("保存成功");

                loadArchiveFields();
        });
        }

        loadArchive();
        loadFileLibrarys();

    });

});
