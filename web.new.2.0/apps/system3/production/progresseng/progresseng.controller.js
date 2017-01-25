define(['apps/system3/production/production.controller',
        'apps/system3/production/volume/volume.service',
        'apps/system3/business/engineering/engineering.service'], function (app) {

            app.controller('production.controller.progresseng', function ($scope, volumeService, engineeringService, uiGridGroupingConstants) {
                // 查询条件
                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
                    orderby: 'ID'
                };


                // 默认模式
                $scope.isMaintain = false;
                $scope.isSearch = false;

                // 表格配置
                $scope.gridOptions = {
                    multiSelect: false,
                    enableGridMenu: true,
                    paginationPageSizes: $scope.pageSizes,
                    paginationPageSize: $scope.pageSize,
                    useExternalPagination: true,
                    useExternalSorting: true,
                    enableRowSelection: true,
                    enableRowHeaderSelection: false,
                    treeCustomAggregations: {
                        ag_number: {
                            label: 'ag_number',
                            aggregationFn: function (aggregation, fieldValue, numValue, row) {
                                aggregation.entity = row.entity;
                                this.index = 0;
                            },
                            finalizerFn: function (aggregation) {
                                switch (this.index) {
                                    case 0: {
                                        aggregation.rendered = aggregation.entity.Number;
                                        break;
                                    }
                                    case 1: {
                                        aggregation.rendered = aggregation.entity.Project.Number;
                                        break;
                                    }
                                    default:
                                }
                                this.index++;
                            }
                        },
                        ag_name: {
                            label: 'ag_name',
                            aggregationFn: function (aggregation, fieldValue, numValue, row) {
                                aggregation.entity = row.entity;
                                this.index = 0;
                            },
                            finalizerFn: function (aggregation) {
                                switch (this.index) {
                                    case 0: {
                                        aggregation.icon = 'fa fa-circle';
                                        aggregation.rendered = aggregation.entity.Name;
                                        break;
                                    }
                                    case 1: {
                                        aggregation.icon = 'fa fa-square';
                                        aggregation.rendered = aggregation.entity.Project.Name;
                                        break;
                                    }
                                   
                                    default:
                                }
                                this.index++;
                            }
                        },
                        ag_manager: {
                            label: 'ag_manager',
                            aggregationFn: function (aggregation, fieldValue, numValue, row) {
                                aggregation.entity = row.entity;
                                this.index = 0;
                            },
                            finalizerFn: function (aggregation) {
                                switch (this.index) {
                                    case 0: {
                                        aggregation.rendered = aggregation.entity.Manager;
                                        break;
                                    }
                                    case 1: {
                                        aggregation.rendered = aggregation.entity.Project.Manager;
                                        break;
                                    }
                                    default:
                                }
                                this.index++;
                            }
                        },
                        ag_startdate: {
                            label: 'ag_startdate',
                            aggregationFn: function (aggregation, fieldValue, numValue, row) {
                                aggregation.entity = row.entity;
                                this.index = 0;
                            },
                            finalizerFn: function (aggregation) {
                                switch (this.index) {
                                    case 0: {
                                        aggregation.rendered = aggregation.entity.CreateDate;
                                        break;
                                    }
                                    case 1: {
                                        aggregation.rendered = aggregation.entity.Project.CreateDate;
                                        break;
                                    }
                                    default:
                                }
                                this.index++;
                            }
                        },
                        ag_enddate: {
                            label: 'ag_enddate',
                            aggregationFn: function (aggregation, fieldValue, numValue, row) {
                                aggregation.entity = row.entity;
                                this.index = 0;
                            },
                            finalizerFn: function (aggregation) {
                                switch (this.index) {
                                    case 0: {
                                        aggregation.rendered = aggregation.entity.DeliveryDate;
                                        break;
                                    }
                                    case 1: {
                                        aggregation.rendered = aggregation.entity.Project.DeliveryDate;
                                        break;
                                    }
                                    default:
                                }
                                this.index++;
                            }
                        }
                    },
                    columnDefs: [
                         {
                             name: 'ProjectInfo.Number', displayName: $scope.local.proj.number, enableColumnMenu: true,
                             width: 130, maxWidth: 200, minWidth: 120, pinnedLeft: true, grouping: { groupPriority: 0 },
                             treeAggregationType: 'ag_number'
                         },
                        {
                            name: 'ProjectInfo.Name', displayName: $scope.local.proj.name, pinnedLeft: true,
                            enableColumnMenu: true, minWidth: 200, pinnedLeft: true,
                            treeAggregationType: 'ag_name'
                        },

                        {
                            name: 'Type', displayName: $scope.local.eng.type, width: '100',
                            cellFilter: 'enumMap:"EngineeringType"' // 
                        },
                        {
                            name: 'Phase', displayName: $scope.local.eng.phase, enableColumnMenu: false, width: '100',
                            cellFilter: 'enumMap:"EngineeringPhase"',
                            customTreeAggregationFinalizerFn: function (aggregation) {
                                aggregation.rendered = aggregation.value;
                            },
                            treeAggregationType: uiGridGroupingConstants.aggregation.MAX
                        },
                        {
                            name: 'Manager', displayName: $scope.local.eng.manager, enableColumnMenu: false, width: '100',
                            cellFilter: 'enumMap:"user"',
                            treeAggregationType: 'ag_manager'
                        },
                        {
                            name: 'Status', displayName: $scope.local.eng.status, enableColumnMenu: false, width: '100',
                            cellFilter: 'enumMap:"EngineeringStatus"'
                        },
                        {
                            name: 'CreateDate', displayName: $scope.local.eng.createdate, enableColumnMenu: false, width: '150',
                            cellFilter: 'TDate',
                            treeAggregationType: 'ag_startdate'
                        },
                        {
                            name: 'DeliveryDate', displayName: $scope.local.eng.deliverydate, enableColumnMenu: false, width: '150',
                            cellFilter: 'TDate',
                            treeAggregationType: 'ag_enddate'
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

                        gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
                            gridApi.selection.toggleRowSelection(newRowCol.row.entity);
                        })

                        gridApi.core.on.rowsRendered($scope, function (row, col) {
                            
                        });
                    }
                };

                // 加载页面数据
                $scope.loadSource = function () {

                    engineeringService.getEngineerings($scope.filter).then(function (result) {
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

                // 筛选模式
                $scope.moreSearch = function () {
                    $scope.isSearch = !$scope.isSearch;
                };

                // 关闭编辑模式
                $scope.closeMaintain = function () {
                    $scope.isMaintain = false;
                }

                // 监听筛选模式，关闭其他模式
                $scope.$watch("isSearch", function (newval, oldval) {
                    if (newval) {
                        $scope.isMaintain = false;
                    }
                });

                // 监听维护模式，关闭其他模式
                $scope.$watch("isMaintain", function (newval, oldval) {

                    if (newval == true) {
                        $scope.isSearch = false;
                    }
                });

                // 监听筛选条件查询数据
                $scope.$watch("filter", function (newval, oldval) {

                    if (newval.txtfilter != undefined && newval.txtfilter != "") {
                        return;
                    }

                    $scope.loadSource();
                }, true);

            });
        });