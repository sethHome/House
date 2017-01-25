define(['apps/system3/production/production.controller',
        'apps/system3/production/note/note.controller.maintain',
        'apps/system3/production/note/note.service'], function (app) {

            app.controller('production.controller.note', function ($scope, noteService, $uibModal) {

                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
                    orderby: 'ID',
                };

                $scope.gridOptions = {
                    multiSelect: true,
                    enableGridMenu: false,
                    enableColumnResizing: true,
                    paginationPageSizes: $scope.pageSizes,
                    paginationPageSize: $scope.pageSize,
                    useExternalPagination: true,
                    useExternalSorting: false,
                    enableRowSelection: true,
                    enableRowHeaderSelection: true,
                    rowTemplate: "<div " +
                       "ng-click=\"grid.appScope.gridApi.selection.toggleRowSelection(row.entity)\" ng-dblclick=\"grid.appScope.update(row.entity)\" " +
                       "ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" " +
                       "ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
                    columnDefs: [
                        {
                            name: 'Engineering.Number', displayName: $scope.local.eng.number, enableColumnMenu: false, width: 120
                        },
                        {
                            name: 'Engineering.Name', displayName: $scope.local.eng.name, enableColumnMenu: false, width: 200
                        },
                       {
                           name: 'NoteDate', displayName: $scope.local.note.date, enableColumnMenu: false, cellFilter: 'TDate', width: 120
                       },
                        {
                            name: 'UserID', displayName: $scope.local.note.user, enableColumnMenu: false, cellFilter: 'enumMap:"user"', width: 100
                        },
                        {
                            name: 'Content', displayName: $scope.local.note.content, enableColumnMenu: false,
                            cellTemplate: '<div class="ui-grid-cell-contents" > ' +
                                    '<span><span ng-class="{\'label-info\' : row.entity.NoteType == 1,\'label-danger\' : row.entity.NoteType == 2,\'label-primary\' : row.entity.NoteType == 3,\'label-warning\': row.entity.NoteType == 4}" class="label">{{row.entity.NoteType | enumMap:"NoteType"}}</span>&nbsp;{{row.entity.Content | subStr:20}}</span>' +
                                '</div>',
                        },
                    ],
                    onRegisterApi: function (gridApi) {

                        $scope.gridApi = gridApi;

                        $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                            $scope.filter.orderby = sortColumns;

                            if (sortColumns.length == 0) {
                                $scope.filter.orderby = null;
                            } else {
                                $scope.filter.orderby = sortColumns[0].field;
                                $scope.filter.orderdirection = sortColumns[0].sort.direction;
                            }
                        });

                        gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                            $scope.filter.pagesize = pageSize,
                            $scope.filter.pageindex = newPage;
                        });

                        gridApi.selection.on.rowSelectionChanged($scope, function (row, e) {
                            $scope.selectedRows = gridApi.selection.getSelectedRows();

                            if (row.isSelected) {
                                $scope.currentEngineering = row.entity;
                            }
                        });
                    }
                };

                // 加载页面数据
                $scope.loadSource = function () {

                    noteService.getSource($scope.filter).then(function (result) {
                        $scope.gridOptions.data = result.Source;
                        $scope.gridOptions.totalItems = result.TotalCount;
                    })
                };

                // 新建记事
                $scope.create = function () {

                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/production/note/view/note-maintain.html',
                        controller: 'production.controller.note.maintain',
                        resolve: {
                            maintainParam: function () {
                                return {
                                }
                            }
                        }
                    }).result.then(function (info) {
                        //$scope.gridOptions.data.push(info);
                        $scope.loadSource();
                    });
                };

                // 更新记事
                $scope.update = function (entity) {

                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/production/note/view/note-maintain.html',
                        controller: 'production.controller.note.maintain',
                        resolve: {
                            maintainParam: function () {
                                return {
                                    entityInfo: entity
                                }
                            }
                        }
                    }).result.then(function (info) {
                        //$scope.gridOptions.data.push(info);
                        //$scope.loadSource();
                    });
                };

                // 删除
                $scope.delete = function () {

                    if ($scope.selectedRows == undefined) {
                        bootbox.alert($scope.local.msg.noRowSelected);
                    }

                    var ids = $scope.selectedRows.map(function (obj) {
                        return obj.ID;
                    });

                    if ($scope.filter.trash) {
                        // 回收站状态，恢复选中对象
                        bootbox.confirm($scope.local.msg.confirmBackup, function (result) {
                            if (result === true) {
                                resourceService.backup(ids).then(function () {
                                    angular.forEach($scope.selectedRows, function (proj) {
                                        $scope.gridOptions.data.removeObj(proj);
                                    })
                                    bootbox.alert("恢复成功");
                                });
                            }
                        });
                    } else {
                        bootbox.confirm($scope.local.msg.confirmBatchDelete, function (result) {
                            if (result === true) {
                                
                                noteService.remove($scope.currentNote.ID).then(function () {
                                    angular.forEach($scope.selectedRows, function (proj) {
                                        $scope.gridOptions.data.removeObj(proj);
                                    })
                                    bootbox.alert("删除成功");
                                });
                            }
                        });
                    }
                };

                // 重置查询条件
                $scope.clearFilter = function () {
                    $scope.filter = {
                        pagesize: $scope.pageSize,
                        pageindex: 1,
                        orderby: 'ID',
                    };
                };

                // 监听筛选条件查询数据
                $scope.$watch("filter", function (newval, oldval) {

                    if (newval.txtfilter != undefined && newval.txtfilter != "") {
                        return;
                    }

                    $scope.loadSource();
                }, true);


            });
        });