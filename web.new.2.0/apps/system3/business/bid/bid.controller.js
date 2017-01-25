define(['apps/system3/business/business',
        'apps/system3/business/bid/bid.controller.maintain',
        'apps/system3/business/bid/bid.service'], function (app) {


            app.module.controller("business.controller.bid", function ($scope, $stateParams, $uibModal, $timeout, bidService, attachService) {

                // 查询条件
                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
                    orderby: 'ID',
                    trash:false,
                    type: $stateParams.type
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
                            name: 'Name', displayName: $scope.local.bid.name,minWidth:200, enableColumnMenu: false
                        },
                        {
                            name: 'Manager', displayName: $scope.local.bid.manager, width: 100, enableColumnMenu: false, cellFilter: 'enumMap:"user"'
                        },
                        {
                            name: 'Customer.Name', displayName: $scope.local.cust.name, width: 150, enableColumnMenu: false,
                        },
                        {
                            name: 'Person.Name', displayName: $scope.local.business.person, width: 100, enableColumnMenu: false,
                        },
                        {
                            name: 'Person.Phone', displayName: $scope.local.person.phone, width: 100, enableColumnMenu: false,
                        },
                        {
                            name: 'Agency', displayName: $scope.local.bid.agency, width: 150, enableColumnMenu: false,
                        },
                        {
                            name: 'BidStatus', displayName: $scope.local.bid.bidStatus, width: 100, enableColumnMenu: false, cellFilter: 'enumMap:"BidStatus"'
                        },
                        {
                            name: 'BidFee', displayName: $scope.local.bid.bidFee, width: 100, enableColumnMenu: false, cellFilter: 'money'
                        },
                        {
                            name: 'ServiceFee', displayName: $scope.local.bid.serviceFee, width: 100, enableColumnMenu: false, cellFilter: 'money'
                        },
                        {
                            name: 'DepositFee', displayName: $scope.local.bid.deposit, width: 100, enableColumnMenu: false, cellFilter: 'money'
                        },
                        {
                            name: 'LimitFee', displayName: $scope.local.bid.limitFee, width: 100, enableColumnMenu: false, cellFilter: 'money'
                        },
                        {
                            name: 'IsTentative', displayName: $scope.local.bid.isTentative, width: 80, enableColumnMenu: false, cellFilter: 'bool'
                        },
                        {
                            name: 'BidDate', displayName: $scope.local.bid.bidDate, width: 150, enableColumnMenu: false, cellFilter: 'TDate'
                        },
                        {
                            name: 'SuccessfulBidDate', displayName: $scope.local.bid.successfulBidDate, width: 150, enableColumnMenu: false, cellFilter: 'TDate'
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
                                $scope.currentBid = row.entity;
                            }

                        });
                    }
                };

                // 加载页面数据
                $scope.loadSource = function () {
                    
                    bidService.getBids($scope.filter).then(function (result) {
                        $scope.gridOptions.data = result.Source;
                        $scope.gridOptions.totalItems = result.TotalCount;
                    })
                };

                // 重置查询条件
                $scope.clearFilter = function () {
                    $scope.filter = {
                        pagesize: $scope.pageSize,
                        pageindex: 1,
                        orderby: 'ID'
                    };
                };

                // 新建模式
                $scope.create = function () {
                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/business/bid/view/bid-maintain.html',
                        controller: 'business.controller.bid.maintain',
                        resolve: {
                            maintainParams: function () { return {} }
                        }
                    }).result.then(function (id) {
                        bootbox.alert("创建成功");
                        $scope.gridOptions.data.push(info);
                    });
                };

                // 编辑模式
                $scope.update = function (info) {

                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/business/bid/view/bid-maintain.html',
                        controller: 'business.controller.bid.maintain',
                        resolve: {
                            maintainParams: function () {
                                return {
                                    entityInfo: info,
                                }
                            }
                        }
                    }).result.then(function (id) {
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
                                bidService.backup(ids).then(function () {
                                    angular.forEach($scope.selectedRows, function (proj) {
                                        $scope.gridOptions.data.removeObj(proj);
                                    })
                                    bootbox.alert("恢复成功");
                                });
                            }
                        });
                    } else {
                        // 正常状态，删除选中对象

                        bootbox.confirm($scope.local.msg.confirmDelete, function (result) {
                            if (result === true) {
                                bidService.remove(ids).then(function () {
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
