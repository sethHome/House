define([
'apps/system2/docfile/docfile',
'apps/system2/docfile/docfile.controller',
'apps/system2/docfile/docfile.service'], function (app) {

    app.module.controller("docfile.controller.arrange", function ($scope, $rootScope, $uibModal, docfileService) {

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

        // 筛选条件
        $scope.fileConditions = [];

        // 最后一次筛选条件
        $scope.lastConditions = [];

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
                        $scope.archiveVolumeFilter.orderby = null;
                    } else {
                        $scope.archiveVolumeFilter.orderdirection = sortColumns[0].sort.direction;
                        $scope.archiveVolumeFilter.orderby = sortColumns[0].field;

                    }
                });

                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                    $scope.archiveVolumeFilter.pagesize = pageSize,
                    $scope.archiveVolumeFilter.pageindex = newPage;

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
                    name: 'Delete', displayName: '拆离', enableColumnMenu: false, cellClass: 't-center',
                    width: 60, pinnedLeft: true,
                    cellTemplate: '<div class="ui-grid-cell-contents" > <button type="button" ng-click="grid.appScope.follow(row.entity)" class="btn btn-sm btn-primary btn-square"><i class="fa fa-sign-out"></i>拆离</button> </div>',
                },
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
                        $scope.archiveFileFilter.orderby = null;
                    } else {
                        $scope.archiveFileFilter.orderdirection = sortColumns[0].sort.direction;
                        $scope.archiveFileFilter.orderby = sortColumns[0].field;

                    }
                });

                //gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                //    $scope.archiveFileFilter.pagesize = pageSize,
                //    $scope.archiveFileFilter.pageindex = newPage;

                //});
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

        $scope.archive_changed = function (e, data) {
            if (data.node.original.NodeType > 0) {
                $scope.$safeApply(function () {
                    $scope.currentArchive = data.node.original;
                })
            }
        }

        $scope.category_changed = function (e, data) {
            $scope.$safeApply(function () {
                $scope.currentArchiveCategory = data.node.original;

                if (data.node.original.ParentFullKey == "") {
                    $scope.archiveVolumeFilter.category = data.node.original.Number;
                } else {
                    $scope.archiveVolumeFilter.category = data.node.original.ParentFullKey + "." + data.node.original.Number;
                }
            })
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
                templateUrl: 'apps/system2/docfile/arrange/view/archive-maintain.html',
                size: 'lg',
                controller: "docfile.controller.arrange.archiveMaintain",
                resolve: {
                    maintainService: function () {

                        var c = "";

                        if ($scope.currentArchiveCategory) {

                            if ($scope.currentArchiveCategory.ParentFullKey == "") {
                                c = $scope.currentArchiveCategory.Number;
                            } else {
                                c = $scope.currentArchiveCategory.ParentFullKey + "." + $scope.currentArchiveCategory.Number
                            }
                        }
                         
                        return {
                            update: false,
                            fieldList: $scope.archiveFileds,
                            projectFields : $scope.projectFields,
                            fondsNumber: $scope.currentArchive.FondsNumber,
                            archiveType: $scope.currentArchive.ArchiveType,
                            archiveName: $scope.currentArchive.HasVolume ? 'Volume' : 'Box',
                            hasProject: $scope.currentArchive.HasProject,
                            hasCategory: $scope.currentArchive.HasCategory,
                            category : c,
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
                //$scope.archiveVolumeSource.push(info);
                loadArchiveVolumeData();
            }, function () {
                //dismissed
            });
        };

        // 更新档案案卷信息
        $scope.updateArchiveVolume = function (info) {

            $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/arrange/view/archive-maintain.html',
                size: 'lg',
                controller: "docfile.controller.arrange.archiveMaintain",
                resolve: {
                    maintainService: function () {
                        return {
                            update: true,
                            fieldList: $scope.archiveFileds,
                            projectFields: $scope.projectFields,
                            fondsNumber: $scope.currentArchive.FondsNumber,
                            archiveType: $scope.currentArchive.ArchiveType,
                            archiveName: $scope.currentArchive.HasVolume ? 'Volume' : 'Box',
                            hasProject: $scope.currentArchive.HasProject,
                            hasCategory: $scope.currentArchive.HasCategory,
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
                templateUrl: 'apps/system2/docfile/arrange/view/archive-maintain.html',
                size: 'lg',
                controller: "docfile.controller.arrange.archiveMaintain",
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

        // 正式归档
        $scope.setArchiveStatus = function () {

            var rows = $scope.archiveGridApi.selection.getSelectedRows();

            if (rows.length > 0) {

                bootbox.confirm("确认正式归档？", function (result) {
                    if (result === true) {
                        var ids = rows.map(function (item) { return item.ID }).join(',')
                        var info = {
                            FondsNumber: $scope.currentArchive.FondsNumber,
                            ArchiveType: $scope.currentArchive.ArchiveType,
                            ArchiveName: $scope.currentArchive.HasVolume ? 1 : 4, // Volume
                            ArchiveStatus : 2 // 正式
                        };
                        docfileService.setArchiveStatus(ids, info).then(function (result) {

                            loadArchiveVolumeData($scope.lastArchiveVolumeConditions);
                        })
                    }
                });

            } else {
                bootbox.alert("未选中案卷");
            }
        }

        // 新建案卷文件
        $scope.addArchiveFile = function () {
            
            if (!$scope.currentArchive.Number) {
                bootbox.alert("未选择档案节点");
                return;
            }

            var nodeid = $scope.currentArchive.ParentFullKey ? $scope.currentArchive.ParentFullKey + '.Node.' + $scope.currentArchive.Number : $scope.currentArchive.Number;

            $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/arrange/view/archive-maintain.html',
                size: 'lg',
                controller: "docfile.controller.arrange.archiveMaintain",
                resolve: {
                    maintainService: function () {
                        return {
                            update: false,
                            fieldList: $scope.archiveFileFileds,
                            volumeID: $scope.archiveFileFilter.volume,
                            fondsNumber: $scope.currentArchive.FondsNumber,
                            archiveType: $scope.currentArchive.ArchiveType,
                            hasProject : $scope.currentArchive.HasProject,
                            archiveName: 'File',
                            nodeID: nodeid,
                            allCount: $scope.archiveFileSource.length,
                        };
                    }
                }
            }).result.then(function (info) {
                $scope.archiveFileSource.push(info);
            });
        }

        // 拆离案卷
        $scope.removeArchiveFile = function (entity) {

            bootbox.confirm("确定从档案中" + (entity.FileID > 0 ? "拆离" : "移除")+ "此文件？", function (result) {
                if (result === true) {

                    if (entity.FileID > 0) {

                        docfileService.removeArchiveFile(entity.ID, $scope.currentArchive.FondsNumber, $scope.currentArchive.ArchiveType).then(function (result) {

                            $scope.archiveFileSource.removeObj(entity);

                            bootbox.alert("拆离成功");

                        })
                    } else {
                        var arg = {
                            fonds: $scope.currentArchive.FondsNumber,
                            archive: $scope.currentArchive.ArchiveType,
                            name: 2 // File
                        };

                        docfileService.deleteArchive(entity.ID, arg).then(function (result) {

                            $scope.archiveFileSource.removeObj(entity);

                            bootbox.alert("移除成功");
                        })
                    }
                }
            });
        }

        $scope.$watch("currentArchive", function (newval, oldval) {
            if (newval) {

                loadArchiveFields();

                var nodeid = newval.ParentFullKey ? newval.ParentFullKey + '.Node.' + newval.Number : newval.Number;

                var a = angular.copy($scope.archiveVolumeFilter);
                a.nodeid = nodeid;
                a.fonds = $scope.currentArchive.FondsNumber;
                a.archive = $scope.currentArchive.ArchiveType;
                a.category = undefined;
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

        // 查询条件
        $scope.addCondition = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/view/field-condition.html',
                size: 'md',
                controller: "docmgr.controller.fieldcondition",
                resolve: {
                    conditionService: function () {

                        return {

                            fieldList: $scope.archiveFileds,
                            conditions: $scope.conditions,
                            clearConditions: function () {
                                $scope.conditions = [];
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (condition) {
                loadArchiveVolumeData(condition);
            }, function () {
                //dismissed
            });
        };

        // 表格列的显示
        $scope.displayColumns = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/view/column-display.html',
                size: 'md',
                controller: "docmgr.controller.columns",
                resolve: {
                    columnService: function () {
                        return {
                            columns: $scope.archiveGridOptions.columnDefs,
                        };
                    }
                }
            });

            modalInstance.result.then(function (columns) {
                $scope.archiveGridOptions.columnDefs = columns;
                $scope.archiveGridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
            }, function () {
                //dismissed
            });


        };

        // 批量修改
        $scope.batchUpdate = function () {

            // 更新选择行
            var rows = $scope.archiveGridApi.selection.getSelectedRows();

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/view/batch-update.html',
                size: 'md',
                controller: "docmgr.controller.batchupdate",
                resolve: {
                    batchService: function () {
                        return {
                            fieldList: $scope.archiveFileds,
                            updateRegion: rows.length > 0 ? 2 : 1
                        };
                    }
                }
            });

            modalInstance.result.then(function (batchUpdateInfo) {

                if (batchUpdateInfo.UpdateRegion == 1) {
                    // 筛选数据集
                    batchUpdateInfo.Conditions = $scope.lastArchiveVolumeConditions;

                } else {

                    if (rows.length == 0) {
                        bootbox.alert("未选中行，无法更新");
                        return;
                    } else {
                        batchUpdateInfo.UpdateIDs = rows.map(function (item) { return item.ID }).join(',');
                    }
                }

                //docfileService.batchUpdate($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber, batchUpdateInfo, { nodeid: $scope.currentFile.ID }).then(function () {
                //    loadFileData();
                //});

            }, function () {
                //dismissed
            });
        };

        $scope.currentTabIndex = 1;

        // 加载档案库字段
        var loadArchiveFields = function () {

            var fond = $scope.archiveTypes.find(a => a.Number == $scope.currentArchive.FondsNumber);

            if (fond){
                var archiveType = fond.Archives.find(a => a.Key == $scope.currentArchive.ArchiveType);
               
                if (archiveType) {

                    if (archiveType.HasCategory) {
                        loadCategoryTreeNode(archiveType.Categorys,"");

                        $scope.archiveCategoryTreeNode = archiveType.Categorys;

                    } else {
                        $scope.archiveCategoryTreeNode = [];
                    }

                    $scope.archiveFileds = archiveType.HasVolume ? archiveType.VolumeFields : archiveType.BoxFields;

                    var columns = [];
                    columns.push({
                        name: "ROWNUMBER", displayName: "序号", width: 50, enableColumnMenu: false
                    });
                    columns.push({
                        name: "FondsNumber", displayName: "全宗号", width: 80, enableColumnMenu: false
                    });
                    columns.push({
                        name: "Status", displayName: "状态", width: 80, enableColumnMenu: false, cellFilter: "enumMap:'ArchiveStatus'"
                    });
                    columns.push({
                        name: "AccessLevel", displayName: "访问级别", width: 80, enableColumnMenu: false, cellFilter: "enumMap:'AccessLevel'"
                    });

                    angular.forEach($scope.archiveFileds, function (item) {


                        var col = {
                            name: "_f" + item.ID, displayName: item.Name, minWidth: 80, enableColumnMenu: false,

                        };

                        if (item.DataType == 3) {
                            col.cellFilter = 'TDate';
                        } else if (item.DataType == 4) {
                            col.cellFilter = 'enumMap:"' + item.BaseData + '"';
                        }
                        if (item.Length > 500) {
                            col.width = 400;
                        } else if (item.Length > 200) {
                            col.width = 200;
                        } else {
                            col.width = 100;
                        }
                        columns.push(col);
                    });

                    columns.push({
                        name: "CreateUser", displayName: "登记人", width: 100, enableColumnMenu: false, cellFilter: 'enumMap:"user"'
                    });

                    $scope.archiveGridOptions.columnDefs = columns;

                    // File

                    $scope.archiveFileFileds = archiveType.FileFields;
                    var columns = [];
                    columns.push({
                        name: 'Delete', displayName: '拆离', enableColumnMenu: false, cellClass: 't-center',
                        width: 70, pinnedLeft: true,
                        cellTemplate: '<div class="ui-grid-cell-contents" > <button type="button" ng-click="grid.appScope.removeArchiveFile(row.entity)" class="btn btn-sm btn-primary btn-square"><i class="fa fa-sign-out"></i>{{row.entity.FileID > 0 ? "拆离" : "移除"}}</button> </div>',
                    });

                    columns.push({
                        name: "ROWNUMBER", displayName: "序号", width: 50, pinnedLeft: true, cellClass: 't-center', enableColumnMenu: false
                    });
                    columns.push({
                        name: "FondsNumber", displayName: "全宗号", width: 80, cellClass: 't-center', enableColumnMenu: false
                    });

                    angular.forEach($scope.archiveFileFileds, function (item) {
                        var col = {
                            name: "_f" + item.ID, displayName: item.Name, minWidth: 80, enableColumnMenu: false,
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
                }
            }
        }

        var loadCategoryTreeNode = function (categorys, parents) {

            angular.forEach(categorys, function (a) {

                a.text = "<span class='c-blue m-r-5'>[" + a.Number + "]</span>" + a.Name;
                a.type = 'folder';
                a.state = { opened: true };
                a.ParentFullKey = parents ;

                loadCategoryTreeNode(a.Children, parents != "" ? parents + "." + a.Number : a.Number);

                a.children = a.Children;
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

            docfileService.getArchiveTypes().then(function (result) {

                $scope.archiveTypes = result;

                docfileService.getArchiveList().then(function (result) {

                    $scope.documentsArchive = convertTreeNode(result);
                });
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

        var loadProjectFields = function () {
           $scope.projectFields = docfileService.getProjectFields().$object;
        }

        loadArchive();

        loadProjectFields();
    });

    app.module.controller("docfile.controller.arrange.archiveMaintain", function ($scope, $validation, docfileService, $uibModal, $uibModalInstance, maintainService) {

        $scope.maintainService = maintainService;
        $scope.archiveInfo = maintainService.update ? maintainService.archiveInfo : { AccessLevel: 1, Copies: 1, Category: maintainService.category};
        $scope.chooseProj = maintainService.update ? true : false;

        // 字段重组
        var resetFiels = function () {
            $scope.fields = [];

            var temp = { fields: [] };

            angular.forEach(maintainService.fieldList, function (item) {

                if (maintainService.update) {
                    item.Value = maintainService.archiveInfo["_f" + item.ID];
                } else {
                   
                    item.Value = (item.DataType == 4 || item.DataType == 1) ? parseInt(item.Default) : item.Default;

                }

                if (temp.fields.length == 2) {
                    $scope.fields.push(temp);

                    temp = { fields: [] };
                }

                temp.fields.push(item);

                // 多行文本输入独占一行
                if (item.Length >= 200) {
                    
                    $scope.fields.push(temp);
                    temp = { fields: [] };
                } 
            });

            if (temp.fields.length > 0) {
                $scope.fields.push(temp);
            }

            // 如果存在项目信息，加载项目数据
            if (maintainService.archiveName == "Volume") {
                // 先清空原来项目字段的值
                maintainService.projectFields = maintainService.projectFields.map(function (f) {
                    f.Value = undefined;
                    return f;
                });

                if (maintainService.hasProject && maintainService.update && maintainService.archiveInfo.ProjectID > 0) {
                    docfileService.getProjInfo(maintainService.archiveInfo.ProjectID).then(function (result) {
                        angular.forEach(maintainService.projectFields, function (field) {
                            field.Value = result["_f" + field.ID];
                        });
                    });
                }
            }
        }

        // 设置验证规则
        $scope.setValidator = function (field) {
           
            var items = [];

            if (field.NotNull) {
                items.push("required");
            }

            if (field.Length > 0) {
                items.push("maxlength=" + field.Length);
            }

            if (items.length > 0) {
                return items.join(',');
            }
            return "empty";
        }

        // 加载项目信息
        $scope.loadProject = function (textFilter) {
            return docfileService.loadProjSource({ filter: textFilter });
        }

        $scope.$watch("chooseProj", function (newval, oldval) {
            if (!newval) {
                angular.forEach(maintainService.projectFields, function (field) {
                    field.Value = undefined;
                });

                $scope.chooseProjectInfo = null;
            }
        });

        $scope.$watch("chooseProjectInfo", function (newval, oldval) {

            if (newval && maintainService.hasProject) {

                angular.forEach(maintainService.projectFields, function (field) {
                    field.Value = newval["_f" + field.ID];
                });
            }
        });

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
                    AccessLevel: $scope.archiveInfo.AccessLevel,
                    Copies: $scope.archiveInfo.Copies,
                    Category: $scope.archiveInfo.Category,
                    Fields: newFiles,
                };

                updateInfo.ProjectFields = maintainService.projectFields;

                // 选择的已有项目
                if ($scope.chooseProjectInfo && $scope.chooseProjectInfo.ID > 0) {
                    updateInfo.ProjectID = $scope.chooseProjectInfo.ID;
                } else {
                    updateInfo.ProjectID = 0;
                }

                docfileService.updateArchive($scope.archiveInfo.ID, updateInfo).then(function (result) {
                    $scope.blockHander.unblock();
                    if (close) {
                        $uibModalInstance.dismiss('cancel');
                    }
                })

            } else {
                
                var addInfo = {
                    ArchiveVolumeID: maintainService.volumeID,
                    FondsNumber: maintainService.fondsNumber,
                    ArchiveType: maintainService.archiveType,
                    ArchiveName: maintainService.archiveName,
                    ArchiveNode: maintainService.nodeID,
                    AccessLevel: $scope.archiveInfo.AccessLevel,
                    Copies: $scope.archiveInfo.Copies,
                    Category: $scope.archiveInfo.Category,
                    Fields: newFiles
                };
                
                // 选择的已有项目
                if ($scope.chooseProjectInfo && $scope.chooseProjectInfo.ID > 0) {
                    addInfo.ProjectID = $scope.chooseProjectInfo.ID;
                } else {
                    addInfo.ProjectFields = maintainService.projectFields;
                }

                docfileService.createArchive(addInfo).then(function (result) {
                    $scope.blockHander.unblock();
                    $scope.archiveInfo = { ID: result };
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

    app.module.controller("docmgr.controller.fieldcondition", function ($scope, conditionService, $uibModalInstance) {
        $scope.fields = conditionService.fieldList;

        if (!conditionService.conditions) {
            conditionService.conditions = [];
        }

        $scope.conditions = conditionService.conditions;
        $scope.condition = {};


        $scope.addCondition = function () {

            $scope.conditions.push(angular.copy($scope.condition));
        }

        var getCondtions1 = function () {

            return
        }

        $scope.query = function () {

            if ($scope.currentIndex == 1) {

                var items = $scope.fields.where(function (f) {
                    return f.Operator != undefined;
                });

                var conditins = items.map(function (f) {
                    return {
                        Field: f,
                        Value: f.FilterValue,
                        Operator: f.Operator,
                        LogicOperation: 'AND'
                    }
                });

                $uibModalInstance.close(conditins);
            }
            else {
                $uibModalInstance.close($scope.conditions);
            }
        }
        // 清空查询条件
        $scope.clear = function () {
            conditionService.fieldList = conditionService.fieldList.map(function (f) {
                f.FilterValue = undefined;
                f.Operator = undefined;
            });
            $scope.condition = {};
            $scope.conditions = [];
            conditionService.clearConditions();
        };
        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    })

    app.module.controller("docmgr.controller.columns", function ($scope, columnService, $uibModalInstance) {

        $scope.columns = columnService.columns.map(function (c) {
            if (c.visible == undefined) {
                c.visible = true;
            }
            return c;
        });

        $scope.ok = function () {
            $uibModalInstance.close($scope.columns);
        }

        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    })

    app.module.controller("docmgr.controller.batchupdate", function ($scope, batchService, $uibModalInstance) {

        $scope.fields = batchService.fieldList;
        $scope.info = {};
        $scope.expressionStr = "";
        $scope.result = {
            UpdateRegion: batchService.updateRegion,
            Expressions: []
        };

        // 添加字符串
        $scope.addValue = function () {
            $scope.expressionStr += "[" + $scope.info.Value + "]  ";
            $scope.result.Expressions.push({
                Value: $scope.info.Value
            });
        }

        // 添加数据字段
        $scope.addDataField = function () {
            $scope.expressionStr += "[" + $scope.info.DataField.Name + "]  ";

            $scope.result.Expressions.push({
                Field: $scope.info.DataField
            });
        }

        // 添加自动编号
        $scope.addAutoNumber = function () {

            var a = $scope.info.autoNumber.start.PadLeft($scope.info.autoNumber.length, $scope.info.autoNumber.fill);

            $scope.expressionStr += "[" + a + "]  ";

            $scope.result.Expressions.push({
                IdentityStart: $scope.info.autoNumber.start,
                IdentityLength: $scope.info.autoNumber.length,
                IdentityFill: $scope.info.autoNumber.fill,
            });
        }

        // 清除
        $scope.clear = function () {
            $scope.expressionStr = "";
            $scope.result.Expressions = [];
        }

        // 返回
        $scope.ok = function () {
            $uibModalInstance.close($scope.result);
        }

        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    })

});
