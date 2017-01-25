define(['apps/system3/production/production.controller',
        'apps/system3/production/provide/provide.controller.maintain',
        'apps/system3/production/provide/provide.service',
        'apps/system3/production/specialty/specialty.service'], function (app) {

            app.controller('production.controller.provide', function ($scope, provideService, $uibModal) {

                // 查询条件
                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
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
                            name: 'ID', displayName: '进度', enableColumnMenu: false, width: 100, pinnedLeft: true,
                            cellTemplate: '<process-info class="m-5" key="EngineeringSpecialtyProvide" id="{{row.entity.ID}}"></process-info>',
                        },
                        {
                            name: 'Engineering.Number', displayName: $scope.local.eng.number, enableColumnMenu: false, width: 120
                        },
                        {
                            name: 'Engineering.Name', displayName: $scope.local.eng.name, enableColumnMenu: false, width: 200
                        },
                        {
                            name: 'SendSpecialtyID', displayName: $scope.local.provide.sendSpecialty, enableColumnMenu: false, width: 120, cellFilter: 'enumMap:"Specialty"'
                        },
                        {
                            name: 'ReceiveSpecialtyID', displayName: $scope.local.provide.receiveSpecialty, enableColumnMenu: false, width: 120, cellFilter: 'enumMap:"Specialty"'
                        },
                        {
                            name: 'ReceiveUserIDs', displayName: $scope.local.provide.receiveUsers, enableColumnMenu: false, width: 150, cellFilter: "users"
                        },
                        {
                            name: 'CreateDate', displayName: $scope.local.provide.createDate, enableColumnMenu: false, width: 130, cellFilter: "TDate"
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

                    provideService.getSource($scope.filter).then(function (result) {
                        $scope.gridOptions.data = result.Source;
                        $scope.gridOptions.totalItems = result.TotalCount;
                    })
                };

                // 打开编辑窗口
                $scope.create = function () {

                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/production/provide/view/provide-maintain.html',
                        controller: 'production.controller.provide.maintain',
                        resolve: {
                            maintainParams: function () {
                                return {
                                }
                            },
                        }
                    }).result.then(function () {
                        $scope.loadSource();
                    }, function () {
                        //dismissed
                    });
                };

                $scope.update = function (entity) {

                    $uibModal.open({
                        animation: false,
                        size: 'lg',
                        templateUrl: 'apps/system3/production/provide/view/provide-maintain.html',
                        controller: 'production.controller.provide.maintain',
                        resolve: {
                            provideParams: function () {
                                return {
                                    entityInfo: entity 
                                }
                            },
                        }
                    }).result.then(function () {
                        $scope.loadSource();
                    }, function () {
                        //dismissed
                    });
                };

                // 删除
                $scope.remove = function () {

                    if ($scope.currentProvide == undefined && entity == undefined) {
                        bootbox.alert('请选择行！');
                        return;
                    }

                    bootbox.confirm("确定删除？", function (result) {
                        if (result === true) {
                            provideService.remove($scope.currentProvide.ID).then(function () {
                                $scope.loadSource();
                            });
                        }
                    });
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