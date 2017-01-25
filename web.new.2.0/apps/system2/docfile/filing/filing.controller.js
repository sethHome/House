define([
'apps/system2/docfile/docfile',
'apps/system2/docfile/docfile.controller',
'apps/system2/docfile/docfile.service'], function (app) {

    app.module.controller("docfile.controller.filing", function ($scope, $rootScope, $uibModal, docfileService) {

        // 查询条件
        $scope.filter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
            status: 1,
            dept: '0'
        };

        // 案卷查询条件
        $scope.archiveVolumeFilter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
            status: 1,
        };

        // 案卷内文件查询条件
        $scope.archiveFileFilter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
            status: 1,
        };

        // 文件数据
        $scope.fileSource = [];

        // 筛选条件
        $scope.fileConditions = [];

        // 最后一次筛选条件
        $scope.lastConditions = [];

        // 部门列表
        $scope.depts = [{ key: '0', name: '全部部门' }];

        $scope.$on("UsersExLoaded", function () {
            angular.forEach($rootScope.department, function (d) {
                $scope.depts.push({ key: d.key, name: d.name });
            })
        });

        // 表格配置
        $scope.fileGridOptions = {
            data: "fileSource",
            multiSelect: true,
            enableGridMenu: false,
            enableColumnResizing: true,
            paginationPageSizes: $scope.pageSizes,
            paginationPageSize: $scope.pageSize,
            useExternalPagination: true,
            useExternalSorting: false,
            enableRowSelection: true,
            enableRowHeaderSelection: true,
            columnDefs: [
               {
                   name: "ROWNUMBER", displayName: "序号", width: 50, enableColumnMenu: false
               },
               {
                   name: "FondsNumber", displayName: "全宗号", width: 80, enableColumnMenu: false
               }
            ],
            rowTemplate: "<div ng-click=\"grid.appScope.fileGridApi.selection.toggleRowSelection(row.entity)\"  " +
                   "ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" " +
                   "ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
            onRegisterApi: function (gridApi) {

                $scope.fileGridApi = gridApi;

                gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                    //$scope.filter.orderby = sortColumns;

                    if (sortColumns.length == 0) {
                        $scope.filter.orderby = null;
                    } else {
                        $scope.filter.orderdirection = sortColumns[0].sort.direction;
                        $scope.filter.orderby = sortColumns[0].field;

                    }
                });

                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                    $scope.filter.pagesize = pageSize,
                    $scope.filter.pageindex = newPage;

                });
            }
        };

        // 表格配置
        $scope.archiveGridOptions = {
            data: "archiveVolumeSource",
            multiSelect: false,
            enableGridMenu: false,
            enableColumnResizing: true,
            paginationPageSizes: $scope.pageSizes,
            paginationPageSize: $scope.pageSize,
            useExternalPagination: true,
            useExternalSorting: false,
            enableRowSelection: true,
            enableRowHeaderSelection: true,
            columnDefs: [
                {
                    name: "ROWNUMBER", displayName: "序号", width: 50, enableColumnMenu: false
                },
                {
                    name: "FondsNumber", displayName: "全宗号", width: 80, enableColumnMenu: false
                }
            ],
            rowTemplate: "<div ng-click=\"grid.appScope.archiveGridApi.selection.toggleRowSelection(row.entity)\" ng-dblclick=\"grid.appScope.updateArchiveVolume(row.entity)\" " +
                   "ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" " +
                   "ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
            onRegisterApi: function (gridApi) {

                $scope.archiveGridApi = gridApi;

                gridApi.selection.on.rowSelectionChanged($scope, function (selected) {
                    if (selected.isSelected) {
                        $scope.archiveFileFilter.volume = selected.entity.ID;
                    } else {
                        $scope.archiveFileFilter.volume = undefined;
                    }

                });


                gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                    //$scope.filter.orderby = sortColumns;

                    if (sortColumns.length == 0) {
                        $scope.filter.orderby = null;
                    } else {
                        $scope.filter.orderdirection = sortColumns[0].sort.direction;
                        $scope.filter.orderby = sortColumns[0].field;

                    }
                });

                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                    $scope.filter.pagesize = pageSize,
                    $scope.filter.pageindex = newPage;

                });
            }
        };

        // 表格配置
        $scope.archiveFileGridOptions = {
            data: "archiveFileSource",
            multiSelect: true,
            enableGridMenu: false,
            enableColumnResizing: true,
            paginationPageSizes: $scope.pageSizes,
            paginationPageSize: $scope.pageSize,
            useExternalPagination: true,
            useExternalSorting: false,
            enableRowSelection: true,
            enableRowHeaderSelection: true,
            columnDefs: [
               {
                   name: "ROWNUMBER", displayName: "序号", width: 50, enableColumnMenu: false
               },
               {
                   name: "FondsNumber", displayName: "全宗号", width: 80, enableColumnMenu: false
               }
            ],
            rowTemplate: "<div ng-click=\"grid.appScope.archiveFileGridApi.selection.toggleRowSelection(row.entity)\" ng-dblclick=\"grid.appScope.updateArchiveFile(row.entity)\" " +
                   "ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" " +
                   "ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
            onRegisterApi: function (gridApi) {

                $scope.archiveFileGridApi = gridApi;

                gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                    //$scope.filter.orderby = sortColumns;

                    if (sortColumns.length == 0) {
                        $scope.filter.orderby = null;
                    } else {
                        $scope.filter.orderdirection = sortColumns[0].sort.direction;
                        $scope.filter.orderby = sortColumns[0].field;

                    }
                });

                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                    $scope.filter.pagesize = pageSize,
                    $scope.filter.pageindex = newPage;

                });
            }
        };

        // 文件查询条件
        $scope.addCondition = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/view/field-condition.html',
                size: 'md',
                controller: "docfile.controller.fieldcondition",
                resolve: {
                    conditionService: function () {
                        return {
                            fieldList: $scope.fileFileds,
                            conditions: $scope.fileConditions,
                            clearConditions: function () {
                                $scope.fileConditions = [];
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (condition) {

                loadFileData(condition);

            }, function () {
                //dismissed
            });
        };

        $scope.file_changed = function (e, data) {

            $scope.$safeApply(function () {
                $scope.currentFile = data.node.original;
                if ($scope.currentFile.NodeType > 0) {
                    $scope.filter.nodeid = $scope.currentFile.ID;
                }
            })
        }

        $scope.archive_changed = function (e, data) {

            $scope.$safeApply(function () {
                $scope.currentArchive = data.node.original;
            })
        }

        // 自动组卷/自动组盒
        $scope.autoCreateArchive = function () {

            if (!$scope.currentFile) {
                bootbox.alert("未选择文件库节点");
                return;
            }

            if (!$scope.currentArchive) {
                bootbox.alert("未选择档案库节点");
                return;
            }

            var createInfo = {
                FondsNumber: $scope.currentFile.FondsNumber,
                FileNumber: $scope.currentFile.fileNumber,
                ArchiveNode: $scope.currentArchive.ParentFullKey ? $scope.currentArchive.ParentFullKey + '.Node.' + $scope.currentArchive.Number : $scope.currentArchive.Number,
                Conditions: $scope.fileConditions
            };

            var rows = $scope.fileGridApi.selection.getSelectedRows();

            if (rows.length > 0) {

                bootbox.confirm("选中" + rows.length + "个文件,确认自动组卷？", function (result) {
                    if (result === true) {

                        createInfo.IDs = rows.map(function (item) { return item.ID }).join(',');

                        docfileService.autoCreateArchive(createInfo, $scope.filter).then(function (result) {

                            loadFileData($scope.lastConditions);

                            loadArchiveVolumeData($scope.lastArchiveVolumeConditions);

                            loadArchiveFileData($scope.lastArchiveFileConditions);
                        })
                    }
                });
            } else {
                bootbox.confirm("确定将所有文件自动组卷?", function (result) {
                    if (result === true) {

                        docfileService.autoCreateArchive(createInfo, $scope.filter).then(function (result) {

                            loadFileData($scope.lastConditions);

                            loadArchiveVolumeData($scope.lastArchiveVolumeConditions);

                            loadArchiveFileData($scope.lastArchiveFileConditions);
                        })
                    }
                });
            }
        }

        // 添加档案案卷信息
        $scope.addArchiveVolume = function () {

            if (!$scope.currentArchive.Number) {
                bootbox.alert("未选择档案节点");
                return;
            }

            var nodeid = $scope.currentArchive.ParentFullKey ? $scope.currentArchive.ParentFullKey + '.Node.' + $scope.currentArchive.Number : $scope.currentArchive.Number;

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/filing/view/archive-maintain.html',
                size: 'lg',
                controller: "docfile.controller.filing.archiveMaintain",
                resolve: {
                    maintainService: function () {
                        return {
                            update: false,
                            fieldList: $scope.archiveFileds,
                            fondsNumber: $scope.currentArchive.FondsNumber,
                            archiveType: $scope.currentArchive.ArchiveType,
                            archiveName: $scope.currentArchive.HasVolume ? 'Volume' : 'Box',
                            nodeID: nodeid,
                            allCount: $scope.archiveVolumeSource.length,
                            addArchiveInfo: function (info) {

                                $scope.archiveVolumeSource.push(info);
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                $scope.archiveVolumeSource.push(info);
            }, function () {
                //dismissed
            });
        };

        // 更新档案案卷信息
        $scope.updateArchiveVolume = function (info) {

            $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/filing/view/archive-maintain.html',
                size: 'lg',
                controller: "docfile.controller.filing.archiveMaintain",
                resolve: {
                    maintainService: function () {
                        return {
                            update: true,
                            fieldList: $scope.archiveFileds,
                            fondsNumber: $scope.currentArchive.FondsNumber,
                            archiveType: $scope.currentArchive.ArchiveType,
                            archiveName: $scope.currentArchive.HasVolume ? 'Volume' : 'Box',
                            nodeID: info.NodeID,
                            allCount: $scope.archiveVolumeSource.length,
                            archiveInfo: info,
                            get: function (index) {
                                if (index >= 0 && index < $scope.archiveVolumeSource.length) {
                                    return $scope.archiveVolumeSource[index];
                                }
                            }
                        };
                    }
                }
            });
        };

        // 更新档案文件信息
        $scope.updateArchiveFile = function (info) {

            $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/filing/view/archive-maintain.html',
                size: 'lg',
                controller: "docfile.controller.filing.archiveMaintain",
                resolve: {
                    maintainService: function () {
                        return {
                            update: true,
                            fieldList: $scope.archiveFileFileds,
                            fondsNumber: $scope.currentArchive.FondsNumber,
                            archiveType: $scope.currentArchive.ArchiveType,
                            archiveName: 'File',
                            nodeID: info.NodeID,
                            allCount: $scope.archiveFileSource.length,
                            archiveInfo: info,
                            get: function (index) {
                                if (index >= 0 && index < $scope.archiveFileSource.length) {
                                    return $scope.archiveFileSource[index];
                                }
                            }
                        };
                    }
                }
            });
        };

        // 删除档案案卷
        $scope.deleteArchiveVolume = function () {

            var rows = $scope.archiveGridApi.selection.getSelectedRows();

            if (rows.length > 0) {

                bootbox.confirm("选中" + rows.length + "行,确认删除？", function (result) {
                    if (result === true) {
                        var ids = rows.map(function (item) { return item.ID }).join(',')
                        var arg = {
                            fonds: $scope.currentArchive.FondsNumber,
                            archive: $scope.currentArchive.ArchiveType,
                            name: $scope.currentArchive.HasVolume ? 1 : 4 // Volume
                        };
                        docfileService.deleteArchive(ids, arg).then(function (result) {

                            loadArchiveVolumeData($scope.lastArchiveVolumeConditions);
                        })
                    }
                });

            } else {
                bootbox.alert("未选中案卷");
            }
        }

        // 拆离案卷
        $scope.removeArchiveFile = function () {
            var rows = $scope.archiveFileGridApi.selection.getSelectedRows();

            if (rows.length == 0) {
                bootbox.alert("未选中文件");
                return;
            }

            var ids = rows.map(function (item) { return item.ID }).join(',')

            docfileService.removeArchiveFile(ids, $scope.currentArchive.FondsNumber, $scope.currentArchive.ArchiveType).then(function (result) {

                loadFileData($scope.lastConditions);
                loadArchiveFileData($scope.lastArchiveFileConditions);
            })
        }

        // 加入案卷
        $scope.addToArchive = function () {

            var rowsFile = $scope.fileGridApi.selection.getSelectedRows();
            var rowsArchive = $scope.archiveGridApi.selection.getSelectedRows();

            if (rowsFile.length == 0) {
                bootbox.alert("未选中文件");
                return;
            }

            if (rowsArchive.length == 0) {
                bootbox.alert("未选中案卷");
                return;
            }

            var ids = rowsFile.map(function (item) { return item.ID }).join(',');

            var info = {
                FondsNumber: $scope.currentArchive.FondsNumber,
                ArchiveType: $scope.currentArchive.ArchiveType,
                FileNumber: $scope.currentFile.fileNumber,
                FileIDs: ids,
                ArchiveVolumeID: rowsArchive[0].ID,
                ArchiveNode: rowsArchive[0].NodeID
            };

            docfileService.addArchiveFile(info).then(function (result) {
                loadFileData($scope.lastConditions);
                loadArchiveFileData($scope.lastArchiveFileConditions);
            })
        }

        $scope.$watch("currentFile", function (newval, oldval) {
            if (newval) {

                $scope.fileSource = [];

                if (newval.NodeType > 0 && (oldval == undefined || newval.FondsNumber != oldval.FondsNumber || newval.fileNumber != oldval.fileNumber)) {
                    loadFileFields();
                }
            }
        });

        $scope.$watch("currentArchive", function (newval, oldval) {
            if (newval) {

                loadArchiveFields();

                var nodeid = newval.ParentFullKey ? newval.ParentFullKey + '.Node.' + newval.Number : newval.Number;

                var a = angular.copy($scope.archiveVolumeFilter);
                a.nodeid = nodeid;
                a.fonds = $scope.currentArchive.FondsNumber;
                a.archive = $scope.currentArchive.ArchiveType;
                $scope.archiveVolumeFilter = a;

                var b = angular.copy($scope.archiveVolumeFilter);
                b.nodeid = nodeid;
                b.fonds = $scope.currentArchive.FondsNumber;
                b.archive = $scope.currentArchive.ArchiveType;
                b.volume = undefined;
                $scope.archiveFileFilter = b;
            }
        });

        // 监听筛选条件查询数据
        $scope.$watch("filter", function (newval, oldval) {
            if (newval && $scope.currentFile) {
                loadFileData();
            }
        }, true);

        // 监听筛选条件查询数据
        $scope.$watch("archiveVolumeFilter", function (newval, oldval) {
            if (newval && $scope.currentArchive) {

                loadArchiveVolumeData();
            }
        }, true);

        // 监听筛选条件查询数据
        $scope.$watch("archiveFileFilter", function (newval, oldval) {

            if (newval && $scope.currentArchive) {

                loadArchiveFileData();
            }
        }, true);

        // 加载文件库字段
        var loadFileFields = function () {
            $scope.fileFileds = [];
            var columns = [];

            docfileService.getFileFields($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber).then(function (result) {
                $scope.fileFileds = result;

                columns.push({
                    name: "ROWNUMBER", displayName: "序号", width: 50, enableColumnMenu: false
                });
                columns.push({
                    name: "FondsNumber", displayName: "全宗号", width: 80, enableColumnMenu: false
                });

                angular.forEach(result, function (item) {
                    var col = {
                        name: "_f" + item.ID, displayName: item.Name, minWidth: 80, enableColumnMenu: false,
                        filter: item.DataType == 3 ? "cellFilter: 'TDate'" : ""
                    };

                    if (item.DataType == 3) {
                        col.cellFilter = 'TDate';
                    } else if (item.DataType == 4) {
                        col.cellFilter = 'enumMap:"' + item.BaseData + '"';
                    }

                    columns.push(col);
                });

                columns.push({
                    name: "CreateUser", displayName: "登记人", width: 100, enableColumnMenu: false, cellFilter: 'enumMap:"user"'
                });

                $scope.fileGridOptions.columnDefs = columns;

                // loadFileData();
            });
        }

        // 加载档案库字段
        var loadArchiveFields = function () {

            var key = $scope.currentArchive.HasVolume ? "Volume" : "Box";

            docfileService.getArchiveFields($scope.currentArchive.FondsNumber, $scope.currentArchive.ArchiveType, key).then(function (result) {
                $scope.archiveFileds = result;
                var columns = [];
                columns.push({
                    name: "ROWNUMBER", displayName: "序号", width: 50, enableColumnMenu: false
                });
                columns.push({
                    name: "FondsNumber", displayName: "全宗号", width: 80, enableColumnMenu: false
                });

                angular.forEach(result, function (item) {
                    var col = {
                        name: "_f" + item.ID, displayName: item.Name, minWidth: 80, enableColumnMenu: false,
                        filter: item.DataType == 3 ? "cellFilter: 'TDate'" : ""
                    };

                    if (item.DataType == 3) {
                        col.cellFilter = 'TDate';
                    } else if (item.DataType == 4) {
                        col.cellFilter = 'enumMap:"' + item.BaseData + '"';
                    }

                    columns.push(col);
                });

                columns.push({
                    name: "CreateUser", displayName: "登记人", width: 100, enableColumnMenu: false, cellFilter: 'enumMap:"user"'
                });

                $scope.archiveGridOptions.columnDefs = columns;
            });

            docfileService.getArchiveFields($scope.currentArchive.FondsNumber, $scope.currentArchive.ArchiveType, "File").then(function (result) {
                $scope.archiveFileFileds = result;
                var columns = [];
                columns.push({
                    name: "ROWNUMBER", displayName: "序号", width: 50, enableColumnMenu: false
                });
                columns.push({
                    name: "FondsNumber", displayName: "全宗号", width: 80, enableColumnMenu: false
                });

                angular.forEach(result, function (item) {
                    var col = {
                        name: "_f" + item.ID, displayName: item.Name, minWidth: 80, enableColumnMenu: false,
                        filter: item.DataType == 3 ? "cellFilter: 'TDate'" : ""
                    };

                    if (item.DataType == 3) {
                        col.cellFilter = 'TDate';
                    } else if (item.DataType == 4) {
                        col.cellFilter = 'enumMap:"' + item.BaseData + '"';
                    }

                    columns.push(col);
                });

                columns.push({
                    name: "CreateUser", displayName: "登记人", width: 100, enableColumnMenu: false, cellFilter: 'enumMap:"user"'
                });

                $scope.archiveFileGridOptions.columnDefs = columns;
            });
        }

        // 加载文件数据
        var loadFileData = function (condition) {

            if ($scope.currentFile == undefined) {
                return;
            }

            $scope.lastConditions = condition;

            docfileService.getFileData($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber, $scope.filter, condition).then(function (result) {
                $scope.fileSource = result.Source;
                $scope.fileGridOptions.totalItems = result.TotalCount;
            });
        }

        var loadFileLibrarys = function () {

            var nodeTypes = {
                0: "cloud",
                1: "folder",
                2: "circle"
            }

            var convertTreeNode = function (fileNumber, nodes) {

                angular.forEach(nodes, function (item) {

                    item.text = item.Name;
                    item.type = nodeTypes[item.NodeType];
                    item.state = { 'opened': true };
                    item.fileNumber = item.NodeType == 1 ? item.Number : fileNumber;

                    item.children = convertTreeNode(item.fileNumber, item.Children);
                })

                return nodes;
            }

            docfileService.getFileLibraryList().then(function (result) {

                $scope.documentsFile = convertTreeNode("", result);
            });
        }

        // 加载档案节点
        var loadArchive = function () {

            var nodeType = {
                "0": 'cloud',
                "1": 'database',
                "2": 'folder',
                '3': 'filter'
            }

            var convertTreeNode = function (nodes) {
                angular.forEach(nodes, function (item) {

                    if (item.Disabled) {
                        item.type = "disabled";
                        item.text = "<span class='bg-orange'>" + item.Name + "&nbsp;[禁用]</span>"
                    } else {
                        item.text = item.Name;
                        item.type = nodeType[item.NodeType]
                    }

                    item.state = { 'opened': true };

                    item.children = convertTreeNode(item.Children ? item.Children.where(function (cn) { return cn.NodeType != 3; }) : undefined);
                })

                return nodes;
            }

            docfileService.getArchiveList().then(function (result) {

                $scope.documentsArchive = convertTreeNode(result);
            });

        }

        // 加载案卷
        var loadArchiveVolumeData = function (condition) {

            if ($scope.currentArchive == undefined) {
                return;
            }

            $scope.lastArchiveVolumeConditions = condition;

            $scope.archiveVolumeSource = [];
            $scope.archiveFileSource = [];

            docfileService.getArchiveVolumeData($scope.archiveVolumeFilter, condition).then(function (result) {
                $scope.archiveVolumeSource = result.Source;
                $scope.archiveGridOptions.totalItems = result.TotalCount;
            });
        }

        // 加载案卷内文件
        var loadArchiveFileData = function (condition) {

            if ($scope.currentArchive == undefined) {
                return;
            }

            $scope.lastArchiveFileConditions = condition;

            $scope.archiveFileSource = [];

            if ($scope.currentArchive.HasVolume && !$scope.archiveFileFilter.volume) {
                return;
            }

            docfileService.getArchiveFileData($scope.archiveFileFilter, condition).then(function (result) {

                $scope.archiveFileSource = result.Source;
                $scope.archiveFileGridOptions.totalItems = result.TotalCount;
            });
        }

        loadFileLibrarys();

        loadArchive();
    });

    app.module.controller("docfile.controller.filing.archiveMaintain", function ($scope, $validation, docfileService, $uibModal, $uibModalInstance, maintainService) {

        $scope.maintainService = maintainService;
        $scope.archiveInfo = maintainService.archiveInfo;
        $scope.attachIDs = [];

        // 字段重组
        var resetFiels = function () {
            $scope.fields = [];

            var temp = { fields: [] };

            angular.forEach(maintainService.fieldList, function (item) {

                if (maintainService.update) {
                    item.Value = maintainService.archiveInfo["_f" + item.ID];
                } else {
                    item.Value = undefined;
                }

                if (temp.fields.length == 2) {
                    $scope.fields.push(temp);

                    temp = { fields: [] };
                }

                temp.fields.push(item);
            });

            if (temp.fields.length > 0) {
                $scope.fields.push(temp);
            }
        }

        // 保存
        $scope.save = function (close, form) {

            $scope.blockHander.block();

            var newFiles = [];
            var info = {
                FondsNumber: maintainService.fondsNumber,
                CreateUser: $scope.currentUser.Account.ID,
                NodeID: maintainService.nodeID
            };

            angular.forEach($scope.fields, function (item) {
                angular.forEach(item.fields, function (subitem) {

                    newFiles.push(subitem);

                    info["_f" + subitem.ID] = subitem.Value;

                    if (maintainService.update) {
                        maintainService.archiveInfo["_f" + subitem.ID] = subitem.Value;
                    }
                });
            });

            if (maintainService.update) {

                var updateInfo = {
                    FondsNumber: maintainService.fondsNumber,
                    ArchiveType: maintainService.archiveType,
                    ArchiveName: maintainService.archiveName,
                    ArchiveNode: maintainService.nodeID,
                    Fields: newFiles,
                };

                docfileService.updateArchive($scope.archiveInfo.ID, updateInfo).then(function (result) {
                    $scope.blockHander.unblock();
                    if (close) {
                        $uibModalInstance.dismiss('cancel');
                    }
                })

            } else {

                var addInfo = {
                    FondsNumber: maintainService.fondsNumber,
                    ArchiveType: maintainService.archiveType,
                    ArchiveName: maintainService.archiveName,
                    ArchiveNode: maintainService.nodeID,
                    Fields: newFiles,
                };

                docfileService.createArchive(addInfo).then(function (result) {
                    $scope.blockHander.unblock();

                    info.ID = result;
                    info.ROWNUMBER = maintainService.allCount + 1;

                    if (close) {
                        $uibModalInstance.close(info);
                    } else {

                        maintainService.addArchiveInfo(info);
                        maintainService.allCount++;

                        $validation.reset(form);
                    }
                })
            }
        }

        // 第一条
        $scope.first = function () {

            maintainService.archiveInfo = maintainService.get(0);
            $scope.archiveInfo = maintainService.archiveInfo;
            resetFiels();
        }

        // 上一条
        $scope.sub = function () {

            var index = maintainService.archiveInfo.ROWNUMBER - 2;

            maintainService.archiveInfo = maintainService.get(index);
            $scope.archiveInfo = maintainService.archiveInfo;
            resetFiels();
        }

        // 下一条
        $scope.next = function () {
            var index = maintainService.archiveInfo.ROWNUMBER;

            maintainService.archiveInfo = maintainService.get(index);
            $scope.archiveInfo = maintainService.archiveInfo;
            resetFiels();
        }

        // 最后一条
        $scope.last = function () {

            maintainService.archiveInfo = maintainService.get(maintainService.allCount - 1);
            $scope.archiveInfo = maintainService.archiveInfo;
            resetFiels();
        }

        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }

        resetFiels();
    });
});
