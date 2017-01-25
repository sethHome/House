define(['apps/system3/business/business',
        'apps/system3/business/contract/contract.controller.maintain',
        'apps/system3/business/contract/contract.controller.payee',
        'apps/system3/business/contract/contract.service'], function (app) {

            app.module.controller("business.controller.contract", function ($scope, $timeout, $uibModal, contractService) {
                
                // 查询条件
                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
                    trash: false,
                    orderby: 'ID'
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
                            name: 'Number', displayName: $scope.local.con.number, enableColumnMenu: false,
                            width: 120, maxWidth: 200, minWidth: 70
                        },
                        {
                            name: 'Name', displayName: $scope.local.con.name, enableColumnMenu: false, minWidth: 200
                        },
                        {
                            name: 'Customer.Name', displayName: $scope.local.con.customer, width: '200', enableColumnMenu: false,
                        },
                        {
                            name: 'Fee', displayName: $scope.local.con.fee, width: '100', enableColumnMenu: false,
                        },
                        {
                            name: 'PayeeFee', displayName: $scope.local.conPay.ac, width: '100', enableColumnMenu: false,
                        },
                        {
                            name: 'PayeeInvoice', displayName: $scope.local.conPay.bl, width: '100', enableColumnMenu: false,
                        },
                        {
                            name: 'Type', displayName: $scope.local.con.type, width: '150', enableColumnMenu: false,
                            cellFilter: 'enumMap:"ContractType"' 
                        },
                        {
                            name: 'Status', displayName: $scope.local.con.status, enableColumnMenu: false, width: '100',
                            cellFilter: 'enumMap:"ContractStatus"'
                        },
                        {
                            name: 'SignDate', displayName: $scope.local.con.signdate, enableColumnMenu: false, width: '150',
                            cellFilter: 'TDate'
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
                                $scope.currentContract = row.entity;
                            }

                        });
                    }
                };

                // 加载页面数据
                $scope.loadSource = function () {
                    $scope.gridPanel.block();
                    contractService.getContracts($scope.filter).then(function (result) {
                        $scope.gridOptions.data = result.Source;
                        $scope.gridOptions.totalItems = result.TotalCount;
                        $scope.gridPanel.unblock();
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

                $scope.chooseCustomer = {};
               
                $scope.$watch("chooseCustomer.info", function (newval, oldval) {
                    if (newval) {
                        $scope.filter.customer = newval.ID;
                    }
                });

                // 新建
                $scope.create = function () {
                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/business/contract/view/contract-maintain.html',
                        controller: 'business.controller.contract.maintain',
                        resolve: {
                            maintainParams: function () {
                                return {
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
                        templateUrl: 'apps/system3/business/contract/view/contract-maintain.html',
                        controller: 'business.controller.contract.maintain',
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

                // 收费
                $scope.payee = function () {
                    if ($scope.currentContract == undefined) {
                        bootbox.alert('请选择合同！');
                        return;
                    }
                    $scope.payeeModalInstance = $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/business/contract/view/contract-payee.html',
                        controller: 'business.controller.contract.payee',
                        resolve: {
                            maintainParams: function () {
                                return {
                                    entityInfo: $scope.currentContract,
                                }
                            }
                        }
                    });

                    $scope.payeeModalInstance.result.then(function (id) {
                        //success
                        if (id) {
                            $scope.createContractNotify.show();
                            $scope.loadSource();
                        } else {
                            $scope.editContractNotify.show();
                        }

                    }, function () {

                        //dismissed
                    });
                }

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
                                contractService.backup(ids).then(function () {
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
                                contractService.batchRemove(ids).then(function () {

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
