define(['apps/system3/production/production.controller',
        'apps/system3/system/system.service.user',
        'apps/system3/home/desktop/desktop.service'], function (app) {

            app.controller('production.controller.userwork', function ($scope, $stateParams, system_user_service, desktopService, permissionCheckService) {

                permissionCheckService.check("DATA_AllUserWork").then(function (result) {
                    $scope.DATA_AllUserWork = result
                });
                permissionCheckService.check("DATA_DeptUserWork").then(function (result) {
                    $scope.DATA_DeptUserWork = result
                });

                // 查询条件
                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
                    status: $stateParams.type
                };

                $scope.$watch('$viewContentLoaded', function () {
                    $scope.loadSource();
                });

                $scope.userChanged = function (user) {
                    $scope.currentEmp = user;

                    $scope.loadSource();
                }

                // 加载页面数据
                $scope.loadSource = function () {

                    $scope.thisTaskPanel.block();

                    $scope.filter.user = $scope.currentEmp ? $scope.currentEmp.ID : 0;

                    desktopService.getProductionTasks($scope.filter).then(function (result) {

                        $scope.gridOptions.data = result.Source;
                        $scope.gridOptions.totalItems = result.TotalCount;

                        $scope.thisTaskPanel.unblock();
                    })
                };

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
                    enableGroupHeaderSelection: true,
                    rowTemplate: rowTemplate(),
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
                                        aggregation.rendered = "";
                                        break;
                                    }
                                    case 1: {
                                        aggregation.rendered = aggregation.entity.Engineering.Number;
                                        break;
                                    }
                                    case 2: {
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
                                        aggregation.icon = 'fa fa-star';
                                        aggregation.rendered = $scope.Specialty_enum[aggregation.entity.Specialty.SpecialtyID];
                                        break;
                                    }
                                    case 1: {
                                        aggregation.icon = 'fa fa-circle';
                                        aggregation.rendered = aggregation.entity.Engineering.Name;
                                        break;
                                    }
                                    case 2: {
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
                                        aggregation.rendered = aggregation.entity.Specialty.Manager;
                                        break;
                                    }
                                    case 1: {
                                        aggregation.rendered = aggregation.entity.Engineering.Manager;
                                        break;
                                    }
                                    case 2: {
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
                                        aggregation.rendered = aggregation.entity.Specialty.StartDate;
                                        break;
                                    }
                                    case 1: {
                                        aggregation.rendered = aggregation.entity.Engineering.CreateDate;
                                        break;
                                    }
                                    case 2: {
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
                                        aggregation.rendered = aggregation.entity.Specialty.EndDate;
                                        break;
                                    }
                                    case 1: {
                                        aggregation.rendered = aggregation.entity.Engineering.DeliveryDate;
                                        break;
                                    }
                                    case 2: {
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
                             name: 'Engineering.Number', displayName: $scope.local.eng.number, enableColumnMenu: true,
                             width: 170,
                         },
                         {
                             name: 'Engineering.Name', displayName: $scope.local.eng.name, enableColumnMenu: true,
                             minWidth: 250,
                         },
                         {
                             name: 'Volume.SpecialtyID', enableColumnMenu: false, displayName: "专业", width: '100',
                             cellFilter: 'enumMap:"Specialty"'
                         },
                         {
                             name: 'Volume.Number', enableColumnMenu: false, displayName: "卷册编号", width: '80',
                             cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                                 if (row.entity.IsOverdue) {
                                     return 'task-overdue';
                                 }
                             }
                         },
                         {
                             name: 'Volume.Name', enableColumnMenu: false, displayName: "卷册名称", width: '100',
                             cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                                 if (row.entity.IsOverdue) {
                                     return 'task-overdue';
                                 }
                             }
                         },
                        {
                            displayName: "负责人", enableColumnMenu: false, width: '100',
                            name: 'UserID', cellFilter: 'enumMap:"user"'
                        },
                        {
                            displayName: "收到日期", enableColumnMenu: false, width: '100',
                            name: 'ReceiveDate', cellFilter: 'TDate'
                        },
                        {
                            displayName: "计划完成日期", enableColumnMenu: false, width: '100',
                            name: 'LctDate', cellFilter: 'TDate'
                        },
                         {
                             displayName: "实际完成日期", enableColumnMenu: false, width: '100',
                             name: 'FinishDate', cellFilter: 'TDate', visible: $stateParams.type != 1,

                         },
                        {
                            name: 'Name', displayName: "任务", enableColumnMenu: false, width: '100',
                            cellTemplate: '<div class="ui-grid-cell-contents" >' +
                                   '<span>{{row.entity.Name}}[{{row.entity.Time}}]</span>' +
                               '</div>',
                            cellClass: function (grid, row, col, rowRenderIndex, colRenderIndex) {
                                if (row.entity.Status == 1) {
                                    return 'bg-aero';
                                } else if (row.entity.Status == 2) {
                                    return 'bg-primary';
                                }

                            }
                        }
                    ],
                    onRegisterApi: function (gridApi) {

                        $scope.gridApi = gridApi;

                        gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                            $scope.filter.pagesize = pageSize,
                            $scope.filter.pageindex = newPage;
                        });
                    }
                };



                var taskCellClass = function (grid, row, cell) {
                    return "bg-primary"

                }

                function rowTemplate() {
                    return '<div ng-dblclick="grid.appScope.rowDblClick(row)" >' +
                                 '  <div ng-repeat="(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name" class="ui-grid-cell" ng-class="{ \'ui-grid-row-header-cell\': col.isRowHeader }"  ui-grid-cell></div>' +
                                 '</div>';
                }

                $scope.rowDblClick = function () {
                    alert(2)
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