define(['apps/system2/docmgr/docmgr', 'apps/system2/docmgr/docmgr.service'], function (app) {

    app.module.controller("docmgr.controller.archive", function ($scope, $filter, docmgrService, uiGridConstants, $uibModal) {

        // 最后一次筛选条件
        $scope.lastArchiveVolumeConditions = [];

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

                    item.children = convertTreeNode(item.Children);
                })

                return nodes;
            }

            docmgrService.getArchiveTypes().then(function (result) {

                docmgrService.getArchiveList().then(function (result) {

                    $scope.documents = convertTreeNode(result);
                });

                $scope.archiveTypes = {};

                angular.forEach(result, function (item) {
                    $scope.archiveTypes[item.Number] = item.Archives;
                })
            });
        }

        var addArvhice = function (node) {

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docmgr/archive/view/add-archive.html',
                size: 'sm',
                controller: "docmgr.controller.archive.archivemaintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: false,
                            archiveType: $scope.archiveTypes[node.FondsNumber],
                            node: {
                                NodeType: 1,
                                FondsNumber: node.FondsNumber,
                                FondsName: node.Name
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                loadArchive();
            }, function () {
                //dismissed
            });
        }

        var updateArvhice = function (node) {

            var archiveType = node.ArchiveType.trimEnd();

            var archiveTypeItem = $scope.archiveTypes[node.FondsNumber].find(function (item) { return item.Key == archiveType });

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docmgr/archive/view/add-archive.html',
                size: 'sm',
                controller: "docmgr.controller.archive.archivemaintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: true,
                            archiveTypeName: archiveTypeItem.Name,
                            node: node
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                loadArchive();
            }, function () {
                //dismissed
            });
        }

        var addCategory = function (node) {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docmgr/archive/view/add-category.html',
                size: 'sm',
                controller: "docmgr.controller.archive.categorymaintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: false,
                            node: {
                                NodeType: 2,
                                ArchiveType: node.ArchiveType.trimEnd(),
                                FondsNumber: node.FondsNumber,
                                ParentFullKey: node.ParentFullKey + node.Number + ".Node."
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                loadArchive();
            }, function () {
                //dismissed
            });
        }

        var updateCategory = function (node) {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docmgr/archive/view/add-category.html',
                size: 'sm',
                controller: "docmgr.controller.archive.categorymaintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: true,
                            node: node
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                loadArchive();
            }, function () {
                //dismissed
            });
        }

        var addDataFilter = function (node) {

            var nodeid = node.ParentFullKey ? node.ParentFullKey + '.Node.' + node.Number : node.Number;

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docmgr/archive/view/add-datafilter.html',
                size: 'md',
                controller: "docmgr.controller.archive.datafiltermaintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: false,
                            fieldList: $scope.archiveFileds,
                            node: {
                                NodeType: 3,
                                nodeid : nodeid,
                                ArchiveType: node.ArchiveType.trimEnd(),
                                FondsNumber: node.FondsNumber,
                                ParentFullKey: node.ParentFullKey + node.Number + ".Node."
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                loadArchive();
            }, function () {
                //dismissed
            });
        }

        var deleteNode = function (node) {
            bootbox.confirm("确定删除此节点？将同时删除该节点下的所有节点！", function (result) {
                if (result === true) {
                    docmgrService.deleteNode(node.FondsNumber, node.ParentFullKey + node.Number).then(function () {
                        loadArchive();
                    });
                }
            });
        }

        var visiableNode = function (node) {
            bootbox.confirm("确定启用此节点？", function (result) {
                if (result === true) {
                    docmgrService.visiableNode(node.FondsNumber, node.ParentFullKey + node.Number).then(function () {
                        loadArchive();
                    });
                }
            });
        }

        var disableNode = function (node) {
            bootbox.confirm("确定禁用此节点？将同时禁用该节点下的所有节点！", function (result) {
                if (result === true) {
                    docmgrService.disableNode(node.FondsNumber, node.ParentFullKey + node.Number).then(function () {
                        loadArchive();
                    });
                }
            });
        }

        $scope.treeContextmenu = function (node) {

            var addArvhiceMenu = {
                "label": "添加档案节点",
                "icon": "fa fa-database",
                "action": function (obj) {
                    addArvhice(node.original);
                },
            };

            var addCategoryMenu = {
                "label": "分类节点",
                "icon": "fa fa-folder",
                "action": function (obj) {
                    addCategory(node.original);
                },
            };

            var addDataFilterMenu = {
                "label": "数据节点",
                "icon": "fa fa-filter",
                "action": function (obj) {
                    addDataFilter(node.original);
                },
            };

            var updateNodeMenu = {
                "label": "更新节点",
                "icon": "fa fa-edit",
                "action": function (obj) {
                    if (node.type == "database") {
                        updateArvhice(node.original)
                    } else if (node.type == "folder") {
                        updateCategory(node.original)
                    }

                },
            };

            var deleteNodeMenu = {
                "label": "删除节点",
                "icon": "fa fa-close",
                "action": function (obj) {
                    deleteNode(node.original)
                },
            };

            var disableNodeMenu = {
                "label": "禁用节点",
                "icon": "fa fa-ban",
                "action": function (obj) {
                    disableNode(node.original);
                },
            };

            var visiableNodeMenu = {
                "label": "启用节点",
                "icon": "fa fa-check-circle",
                "action": function (obj) {
                    visiableNode(node.original);
                },
            };

            if (node.type == "cloud") {
                return {
                    "add": addArvhiceMenu,
                };
            } else if (node.type == "database" || node.type == "folder") {
                return {
                    "addn": {
                        "label": "添加节点",
                        "icon": "fa fa-plus",
                        "submenu": {
                            "add2": addCategoryMenu,
                            "add3": addDataFilterMenu
                        }
                    },
                    "updateNodeMenu": updateNodeMenu,
                    "deleteNodeMenu": deleteNodeMenu,
                    "disableNodeMenu": disableNodeMenu,

                }
            } else if (node.type == "filter") {
                return {
                    "deleteNodeMenu": deleteNodeMenu,
                }
            } else if (node.type == "disabled") {
                return {
                    "visiableNodeMenu": visiableNodeMenu,
                    "deleteNodeMenu": deleteNodeMenu,
                };
            }
        }

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

        // 案卷查询条件
        $scope.archiveVolumeFilter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
            status: 2,
        };

        // 案卷内文件查询条件
        $scope.archiveFileFilter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
            status: 1,
        };

        // 档案配置
        $scope.archiveGridOptions = {
            data: "archiveVolumeSource",
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
            rowTemplate: "<div ng-click=\"grid.appScope.archiveGridApi.selection.toggleRowSelection(row.entity)\" ng-dblclick=\"grid.appScope.updateArchiveVolume(row.entity)\" " +
                   "ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" " +
                   "ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
            onRegisterApi: function (gridApi) {

                $scope.archiveGridApi = gridApi;

                gridApi.selection.on.rowSelectionChanged($scope, function (selected) {
                    if (selected.isSelected) {

                        $scope.currentArchiveRow = selected.entity;

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

        // 档案文件配置
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
                        $scope.filter.orderby = null;
                    } else {
                        $scope.filter.orderdirection = sortColumns[0].sort.direction;
                        $scope.filter.orderby = sortColumns[0].field;

                    }
                });
            }
        };

        $scope.$watch("currentArchive", function (newval, oldval) {
            if (newval) {

                loadArchiveFields();
                
                var nodeid = newval.ParentFullKey ? newval.ParentFullKey + '.Node.' + newval.Number : newval.Number;

                if (newval.NodeType == 3) {
                    // 数据节点
                    nodeid = newval.ParentFullKey;
                }

                var a = angular.copy($scope.archiveVolumeFilter);
                a.nodeid = nodeid;
                a.fonds = $scope.currentArchive.FondsNumber;
                a.archive = $scope.currentArchive.ArchiveType;
                a.category = undefined;
                a.now = new Date();
                $scope.archiveVolumeFilter = a;

                var b = angular.copy($scope.archiveVolumeFilter);
                b.nodeid = nodeid;
                b.fonds = $scope.currentArchive.FondsNumber;
                b.archive = $scope.currentArchive.ArchiveType;
                b.volume = undefined;
                b.status = 1;
                b.now = new Date();
                $scope.archiveFileFilter = b;

                $scope.currentArchiveRow = {};
            }
        });

        $scope.$watch("currentArchiveRow", function (newval, oldval) {
            if (newval) {
                loadArchiveLogs($scope.currentArchive.FondsNumber, $scope.currentArchive.ArchiveType, newval.ID);
            }
        });

        $scope.$watch("currentArchiveRow.ProjectID", function (newval, oldval) {
            if (newval) {
                loadProjectInfo();
            }
        });

        // 加载案卷日志
        var loadArchiveLogs = function (fonds, type, id) {
            docmgrService.getArchiveLog(fonds, type, id).then(function (datas) {
                $scope.currentArchiveLogs = datas;
            });
        }

        // 加载档案库字段

        // 加载档案库字段
        var loadArchiveFields = function () {

            var archiveTypes = $scope.archiveTypes[$scope.currentArchive.FondsNumber];

            if (archiveTypes) {
                var archiveType = archiveTypes.find(a => a.Key == $scope.currentArchive.ArchiveType);

                if (archiveType) {

                    if (archiveType.HasCategory) {
                        loadCategoryTreeNode(archiveType.Categorys, "");

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
                    //var columns = [];
                    //columns.push({
                    //    name: 'Delete', displayName: '拆离', enableColumnMenu: false, cellClass: 't-center',
                    //    width: 70, pinnedLeft: true,
                    //    cellTemplate: '<div class="ui-grid-cell-contents" > <button type="button" ng-click="grid.appScope.removeArchiveFile(row.entity)" class="btn btn-sm btn-primary btn-square"><i class="fa fa-sign-out"></i>{{row.entity.FileID > 0 ? "拆离" : "移除"}}</button> </div>',
                    //});

                    //columns.push({
                    //    name: "ROWNUMBER", displayName: "序号", width: 50, pinnedLeft: true, cellClass: 't-center', enableColumnMenu: false
                    //});
                    //columns.push({
                    //    name: "FondsNumber", displayName: "全宗号", width: 80, cellClass: 't-center', enableColumnMenu: false
                    //});

                    //angular.forEach($scope.archiveFileFileds, function (item) {
                    //    var col = {
                    //        name: "_f" + item.ID, displayName: item.Name, minWidth: 80, enableColumnMenu: false,
                    //    };

                    //    if (item.DataType == 3) {
                    //        col.cellFilter = 'TDate';
                    //    } else if (item.DataType == 4) {
                    //        col.cellFilter = 'enumMap:"' + item.BaseData + '"';
                    //    }

                    //    columns.push(col);
                    //});

                    //columns.push({
                    //    name: "CreateUser", displayName: "登记人", width: 100, enableColumnMenu: false, cellFilter: 'enumMap:"user"'
                    //});

                    //$scope.archiveFileGridOptions.columnDefs = columns;
                }
            }
        }

        var loadCategoryTreeNode = function (categorys, parents) {

            angular.forEach(categorys, function (a) {

                a.text = "<span class='c-blue m-r-5'>[" + a.Number + "]</span>" + a.Name;
                a.type = 'folder';
                a.state = { opened: true };
                a.ParentFullKey = parents;

                loadCategoryTreeNode(a.Children, parents != "" ? parents + "." + a.Number : a.Number);

                a.children = a.Children;
            });
        }

        var loadProjectInfo = function () {
            $scope.projInfo = docmgrService.getProjInfo($scope.currentArchiveRow.ProjectID).$object;
        }

        // 加载案卷
        var loadArchiveVolumeData = function (condition) {

            if ($scope.currentArchive == undefined) {
                return;
            }

            $scope.lastArchiveVolumeConditions = condition;

            $scope.archiveVolumeSource = [];
            $scope.archiveFileSource = [];

            docmgrService.getArchiveVolumeData($scope.archiveVolumeFilter, condition, $scope.currentArchive.ConditionsSqlStr).then(function (result) {
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

            docmgrService.getArchiveFileData($scope.archiveFileFilter, condition).then(function (result) {

                $scope.archiveFileSource = result.Source;
                $scope.archiveFileGridOptions.totalItems = result.TotalCount;
            });
        }

        // 加载项目字段
        var loadProjectFields = function () {
            $scope.projectFields = docmgrService.getProjectFields().$object;
        }

        $scope.getFiledValue = function (source, field) {
            if (source) {
                switch (field.DataType) {
                    case 3: return $filter('TDate')(source["_f" + field.ID]);
                    case 4: return $filter('enumMap')(source["_f" + field.ID], field.BaseData);
                    default: return source["_f" + field.ID];
                }
            }
        }

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

        $scope.createArchiveIndex = function () {
            var rows = $scope.archiveGridApi.selection.getSelectedRows();

            if (rows.length > 0) {

                bootbox.confirm("确认上架？", function (result) {
                    if (result === true) {
                        var ids = rows.map(function (item) { return item.ID }).join(',')
                        var info = {
                            FondsNumber: $scope.currentArchive.FondsNumber,
                            ArchiveType: $scope.currentArchive.ArchiveType,
                            ArchiveName: $scope.currentArchive.HasVolume ? 1 : 4, // Volume
                            ArchiveStatus: 2 // 正式
                        };
                        docmgrService.setArchiveStatus(ids, info).then(function (result) {

                            bootbox.alert("成功");
                        })
                    }
                });

            } else {
                bootbox.alert("未选中案卷");
            }
        };

        loadArchive();

        loadProjectFields();
    });

    app.module.controller("docmgr.controller.archive.archivemaintain", function ($scope, docmgrService, $uibModal, $uibModalInstance, maintainInfo) {
        $scope.update = maintainInfo.update;
        $scope.nodeInfo = maintainInfo.node;
        $scope.archiveType = maintainInfo.archiveType;
        $scope.archiveTypeName = maintainInfo.archiveTypeName;

        $scope.save = function () {
            if (maintainInfo.update) {
                docmgrService.updateNode($scope.nodeInfo).then(function () {
                    $uibModalInstance.close($scope.nodeInfo);
                });
            } else {
                docmgrService.addNode($scope.nodeInfo).then(function () {
                    $uibModalInstance.close($scope.nodeInfo);
                });
            }
        };

        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    });

    app.module.controller("docmgr.controller.archive.categorymaintain", function ($scope, docmgrService, $uibModal, $uibModalInstance, maintainInfo) {
        $scope.update = maintainInfo.update;
        $scope.nodeInfo = maintainInfo.node;



        $scope.save = function () {
            if (maintainInfo.update) {
                docmgrService.updateNode($scope.nodeInfo).then(function () {
                    $uibModalInstance.close($scope.nodeInfo);
                });
            } else {
                docmgrService.addNode($scope.nodeInfo).then(function () {
                    $uibModalInstance.close($scope.nodeInfo);
                });
            }
        };

        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    });

    app.module.controller("docmgr.controller.archive.datafiltermaintain", function ($scope, docmgrService, $uibModal, $uibModalInstance, maintainInfo) {
        $scope.update = maintainInfo.update;
        $scope.nodeInfo = maintainInfo.node;
        $scope.nodeInfo.Conditions = [];
        $scope.condition = {};

        // 表格配置
        $scope.gridOptions = {
            multiSelect: false,
            enableGridMenu: false,
            enableRowHeaderSelection: false,
            columnDefs: []
        };

        $scope.fields = maintainInfo.fieldList;

        angular.forEach(maintainInfo.fieldList, function (item) {

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
            $scope.gridOptions.columnDefs.push(col);
        });
       
        $scope.addCondition = function () {

            $scope.nodeInfo.Conditions.push(angular.copy($scope.condition));
        }

        $scope.onFinish = function () {

            docmgrService.addNode($scope.nodeInfo).then(function () {
                $uibModalInstance.close($scope.nodeInfo);
            });
        };

        $scope.next = function () {

            if ($scope.currentTabIndex <= 2) {
                $scope.currentTabIndex++;
            }
        }

        $scope.save = function () {
            if (maintainInfo.update) {
                docmgrService.updateNode($scope.nodeInfo).then(function () {
                    $uibModalInstance.close($scope.nodeInfo);
                });
            } else {
                docmgrService.addNode($scope.nodeInfo).then(function () {
                    $uibModalInstance.close($scope.nodeInfo);
                });
            }
        }

        $scope.$watch("currentTabIndex", function (newval, oldval) {

            if (newval == 3) {
                loadArchiveData();
            }
        });

        var loadArchiveData = function () {

            docmgrService.getArchiveVolumeData({

                nodeid: $scope.nodeInfo.nodeid,
                fonds: $scope.nodeInfo.FondsNumber,
                archive: $scope.nodeInfo.ArchiveType,

                pagesize: $scope.pageSize,
                pageindex: 1,
                orderby: 'ID',
                status: 2,

            }, $scope.nodeInfo.Conditions).then(function (result) {
                $scope.gridOptions.data = result.Source;
                $scope.totalItems = result.TotalCount;
            });
        }

        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
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


});
