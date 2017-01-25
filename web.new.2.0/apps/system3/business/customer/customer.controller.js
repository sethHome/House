define(['apps/system3/business/business',
        'apps/system3/business/customer/customer.controller.maintain',
        'apps/system3/business/customer/customer.service'], function (app) {

            app.module.controller("business.controller.customer", function ($scope,$stateParams,$uibModal, $timeout, customerService, attachService) {

                // 查询条件
                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
                    orderby: 'ID',
                    trash: false,
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
                            name: 'Name', displayName: $scope.local.cust.name, enableColumnMenu: false
                        },
                        {
                            name: 'LevelRate', displayName: '星级', enableColumnMenu: false,width: '100',
                            cellTemplate: '<div class="ui-grid-cell-contents" > <ng-rate-it ng-model="row.entity.LevelRate" min="1" max="6" step="1" resetable="false" read-only="true"></ng-rate-it></div>',
                        },
                        {
                            name: 'Address', displayName: $scope.local.cust.address,  enableColumnMenu: false,
                        },
                        {
                            name: 'Tel', displayName: $scope.local.cust.tel, width: '200', enableColumnMenu: false,
                        },
                        {
                            name: 'Person', displayName: '联系人', width: '200', enableColumnMenu: false,
                            cellTemplate: '<div class="ui-grid-cell-contents" > {{row.entity.Persons ? row.entity.Persons[0].Name : "" }} </div>',
                        },
                        {
                            name: 'PersonTel', displayName: '联系电话', width: '200', enableColumnMenu: false,
                            cellTemplate: '<div class="ui-grid-cell-contents" > {{row.entity.Persons ? row.entity.Persons[0].Phone : "" }} </div>',
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
                                $scope.currentCustomer = row.entity;
                            }
                        });
                    }
                };

                // 加载页面数据
                $scope.loadSource = function () {

                    customerService.getCustomers($scope.filter).then(function (result) {
                        $scope.gridOptions.data = result.Source;
                        $scope.gridOptions.totalItems = result.TotalCount;
                    })
                };

                // 重置查询条件
                $scope.clearFilter = function () {
                    $scope.filter = {
                        pagesize: $scope.pageSize,
                        pageindex: 1,
                        orderby: 'ID',
                        type: $stateParams.type
                    };
                };

                // 新建模式
                $scope.create = function () {
                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/business/customer/view/customer-maintain.html',
                        controller: 'business.controller.customer.maintain',
                        resolve: {
                            maintainParams: function () {return {}}
                        }
                    }).result.then(function (info) {
                        bootbox.alert("创建成功");
                        $scope.gridOptions.data.push(info);
                    });
                };

                // 编辑模式
                $scope.update = function (info) {
                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/business/customer/view/customer-maintain.html',
                        controller: 'business.controller.customer.maintain',
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
                                customerService.backup(ids).then(function () {
                                    angular.forEach($scope.selectedRows, function (proj) {
                                        $scope.gridOptions.data.removeObj(proj);
                                    })
                                    bootbox.alert("恢复成功");
                                });
                            }
                        });
                    } else {
                        bootbox.confirm($scope.local.msg.confirmDelete, function (result) {
                            if (result === true) {
                                customerService.remove(ids.join(',')).then(function () {
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
