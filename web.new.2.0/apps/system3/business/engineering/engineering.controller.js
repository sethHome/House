define(['apps/system3/business/business',
        'apps/system3/business/engineering/engineering.controller.maintain',
        'apps/system3/business/engineering/engineering.service'], function (app) {

            app.module.controller("business.controller.engineering", function ($scope,$stateParams, $timeout, $uibModal, engineeringService, attachService, uiGridGroupingConstants) {

                // 查询条件
                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
                    orderby: 'ID',
                    trash: false,
                    phase: $stateParams.phase
                };

                // 表格配置
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
                            name: 'Follow', displayName: '关注', enableColumnMenu: false, cellClass: 't-center',
                            width: 60, pinnedRight: true,
                            cellTemplate: '<div class="ui-grid-cell-contents" > <button type="button" ng-click="grid.appScope.follow(row.entity)" class="btn btn-sm btn-primary btn-square"><i class="fa " ng-class="{\'fa-heart\' : row.entity.isFollow, \'fa-heart-o\' : !row.entity.isFollow }"></i></button> </div>',
                        },
                         {
                             name: 'Number', displayName: $scope.local.eng.number, enableColumnMenu: false,
                             width: 130, maxWidth: 200, minWidth: 120, pinnedLeft: true,
                         },
                        {
                            name: 'Name', displayName: $scope.local.eng.name, enableColumnMenu: false,
                            minWidth: 200, pinnedLeft: true,
                        },
                        {
                            name: 'Type', displayName: $scope.local.eng.type, width: '100', enableColumnMenu: false,
                            cellFilter: 'enumMap:"EngineeringType"' 
                        },
                        {
                            name: 'Manager', displayName: $scope.local.eng.manager, enableColumnMenu: false, width: '100',
                            cellFilter: 'enumMap:"user"',
                        },
                        {
                            name: 'Status', displayName: $scope.local.eng.status, enableColumnMenu: false, width: '100',
                            cellFilter: 'enumMap:"EngineeringStatus"'
                        },
                        {
                            name: 'CreateDate', displayName: $scope.local.eng.createdate, enableColumnMenu: false, width: '150',
                            cellFilter: 'TDate',
                        },
                        {
                            name: 'DeliveryDate', displayName: $scope.local.eng.deliverydate, enableColumnMenu: false, width: '150',
                            cellFilter: 'TDate',
                        }
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
                    $scope.gridPanel.block();
                    engineeringService.getEngineerings($scope.filter).then(function (result) {
                        
                        $scope.gridOptions.data = result.Source.map(function (item) {
                            item.isFollow = $scope.userConfig.engFollows.contains(function (f) { return f.ConfigKey == item.ID.toString(); });
                            return item;
                        });

                        $scope.gridOptions.totalItems = result.TotalCount;

                        $scope.gridPanel.unblock();
                    })
                };

                // 重置查询条件
                $scope.clearFilter = function () {
                    $scope.filter = {
                        pagesize: $scope.pageSize,
                        pageindex: 1,
                        orderby: 'ID',
                        phase: $stateParams.phase
                    };
                };

                // 关注
                $scope.follow = function (info) {

                    if (info.isFollow) {
                        engineeringService.unfollow(info.ID).then(function () {
                            bootbox.alert("取消成功");

                            info.isFollow = false;

                            $scope.userConfig.engFollows.custRemove(function (item) {
                                return item.ConfigKey == info.ID.toString();
                            })
                        })
                    } else {
                        engineeringService.follow(info.ID).then(function () {
                            bootbox.alert("关注成功");

                            info.isFollow = true;

                            $scope.userConfig.engFollows.push({
                                ConfigKey: info.ID.toString()
                            });
                        })
                    }
                }

                // 新建
                $scope.create = function () {
                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/business/engineering/view/engineering-maintain.html',
                        controller: 'business.controller.engineering.maintain',
                        resolve: {
                            maintainParams: function () {
                                return {
                                    phase: parseInt($stateParams.phase)
                                }
                            }
                        }
                    }).result.then(function (info) {
                        bootbox.alert("创建成功");
                        $scope.gridOptions.data.push(info);
                        
                    });
                };

                // 更新
                $scope.update = function (info) {
                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/business/engineering/view/engineering-maintain.html',
                        controller: 'business.controller.engineering.maintain',
                        resolve: {
                            maintainParams: function () {
                                return {
                                    entityInfo: info,
                                }
                            }
                        }
                    }).result.then(function (info) {
                        bootbox.alert("更新成功");
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
                                engineeringService.backup(ids).then(function () {
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
                                engineeringService.batchRemove(ids).then(function () {
                                    angular.forEach($scope.selectedRows, function (proj) {
                                        $scope.gridOptions.data.removeObj(proj);
                                    })
                                    bootbox.alert("删除成功");
                                });
                            }
                        });
                    }
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
