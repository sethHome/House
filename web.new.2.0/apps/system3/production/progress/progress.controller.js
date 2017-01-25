define(['apps/system3/production/production.controller',
        'apps/system3/production/progress/progress.controller.detail',
        'apps/system3/production/volume/volume.service'], function (app) {

            app.controller('production.controller.progress', function ($scope, $uibModal, volumeService, processService, uiGridGroupingConstants) {
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
                    enableGroupHeaderSelection: true,
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
                             name: 'Project.Number', displayName: $scope.local.proj.number, enableColumnMenu: true,
                             width: 170, pinnedLeft: true, grouping: { groupPriority: 0 },
                             cellTemplate: '<div class="ui-grid-cell-contents" ><span ng-class="{ \'pull-right\' : row.entity.Engineering}">{{grid.appScope.setCol(row,"ag_number")}}</span></div>',
                             treeAggregationType: 'ag_number'
                         },
                         {
                             name: 'Engineering.Name', displayName: $scope.local.proj.name, enableColumnMenu: true,
                             minWidth: 250, maxWidth: 350, pinnedLeft: true, grouping: { groupPriority: 1 },
                             cellTemplate: '<div class="ui-grid-cell-contents" ng-dblclick="grid.appScope.openDetail()" ng-class="{ \'bg-orange\' : row.entity.Engineering && !row.entity.ID}" > ' +
                                    '<i class="{{grid.appScope.getIcon(row,\'ag_name\')}}" ></i>' +
                                    '<span ng-class="{ \'pull-right\' : row.entity.Engineering}">{{grid.appScope.setCol(row,"ag_name")}}</span>' +
                                 '</div>',
                             treeAggregationType: 'ag_name'
                         },
                         {
                             name: 'Specialty.SpecialtyID', enableColumnMenu: false,
                             grouping: { groupPriority: 2 }, visible: false
                         },
                        {
                            displayName: $scope.local.user.manage, enableColumnMenu: false, width: '100',
                            name: 'Manager',
                            cellTemplate: '<div class="ui-grid-cell-contents" >{{grid.appScope.setCol(row,"ag_manager") | enumMap:"user"}} ' +
                                '<a ng-show="!row.entity.Engineering || row.entity.ID" class="btn btn-transparent msgChatBtn" ng-click="grid.appScope.haveChat(row,\'Designer\')"><i class="fa fa-weixin"></i></a></div>',
                            treeAggregationType: 'ag_manager'
                        },
                        //{
                        //    name: 'Checker', displayName: '进度', enableColumnMenu: false,
                        //    cellTemplate: '<div class="ui-grid-cell-contents" >' +
                        //            '<process-info ng-show="row.entity.ID > 0" class="m-5" key="Volume" id="{{row.entity.ID}}"></process-info>' +
                        //        '</div>',
                        //},

                    ],
                    onRegisterApi: function (gridApi) {

                        $scope.gridApi = gridApi;



                        //$scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                        //    $scope.filter.orderby = sortColumns;

                        //    if (sortColumns.length == 0) {
                        //        $scope.filter.orderby = null;
                        //    } else {
                        //        $scope.filter.orderby = sortColumns[0].field;
                        //        $scope.filter.orderdirection = sortColumns[0].sort.direction;
                        //    }
                        //});

                        gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                            $scope.filter.pagesize = pageSize,
                            $scope.filter.pageindex = newPage;
                        });

                        gridApi.selection.on.rowSelectionChanged($scope, function (row, e) {
                            $scope.selectedRows = gridApi.selection.getSelectedRows();

                            if (row.isSelected) {
                                $scope.currentVolume = row.entity;


                            }
                        });

                        gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
                            gridApi.selection.toggleRowSelection(newRowCol.row.entity);
                        })

                        gridApi.core.on.rowsRendered($scope, function (row, col) {
                            //$scope.gridApi.cellNav.scrollToFocus($scope.gridOptions.data[0], $scope.gridOptions.columnDefs[1]);
                            //gridApi.selection.selectRow($scope.gridOptions.data[0]);
                            //gridApi.selection.toggleRowSelection(newRowCol.row.entity);
                            //gridApi.treeBase.expandAllRows();
                            //alert(1)
                        });

                        $scope.gridApi.grid.registerDataChangeCallback(function () {
                            if ($scope.gridApi.grid.treeBase.tree instanceof Array) {
                                $scope.gridApi.treeBase.expandAllRows();
                            }
                        });
                    }
                };

                $scope.getIcon = function (row, label) {
                    if (row.entity.Engineering == undefined) {
                        for (var i in row.entity) {
                            if (row.entity[i].label == label) {
                                return row.entity[i].icon;
                            }
                        }
                    }
                }

                $scope.setCol = function (row, label) {
                    if (row.entity.Engineering == undefined) {
                        for (var i in row.entity) {
                            if (row.entity[i].label == label) {

                                return row.entity[i].rendered;
                            }
                        }
                    } else {
                        if (label == 'ag_number' && row.entity.ID) {
                            return row.entity.Number;
                        }
                        if (label == 'ag_name') {
                            return row.entity.ID ? row.entity.Name : "无卷册";
                        }
                        if (label == 'ag_manager' && row.entity.ID) {
                            return row.entity.Designer;
                        }
                        return "";
                    }
                }

                $scope.haveChat = function (row, user) {

                    if (row.entity.Engineering == undefined) {

                        for (var i in row.entity) {
                            if (row.entity[i].label == "ag_manager") {
                                $scope.openChat(row.entity[i].rendered, "[卷册策划] - " + row.entity[i].entity.Engineering.Name);
                                break;
                            }
                        }
                    } else {
                        var title = "[卷册策划] - " + row.entity.Engineering.Name + " 专业 :" + $scope.Specialty_enum[row.entity.Specialty.SpecialtyID];
                        title += " 卷册 : " + row.entity.Name
                        $scope.openChat(row.entity[user], title);
                    }
                }

                // 加载页面数据
                $scope.loadSource = function () {

                    $scope.thisPanel.block();

                    volumeService.getVolumeProcess($scope.filter).then(function (result) {

                        $scope.gridOptions.data = result.Source;
                        $scope.gridOptions.totalItems = result.TotalCount;

                        $scope.thisPanel.unblock();
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

                $scope.$watch('currentVolume', function (newval, oldval) {
                    if (newval && newval.ID > 0 && $scope.isMaintain) {
                        $scope.flowInfo = processService.getFlowDetailByObj('Volume', newval.ID).$object;
                        $scope.volumeStatistics = volumeService.getVolumeStatisticsInfo(newval.ID).$object;
                    } else {
                        $scope.flowInfo = {};
                        $scope.volumeStatistics = {}
                    }
                })

                // 打开卷册详细界面
                $scope.openDetail = function () {
                    if ($scope.currentVolume == undefined) {
                        bootbox.alert('请选择卷册！');
                        return;
                    }

                    if ($scope.currentVolume.ID == 0) {
                        bootbox.alert('无卷册！');
                        return;
                    }

                    if ($scope.maintainModel == 1) {
                        $scope.isMaintain = true;
                        $scope.flowInfo = processService.getFlowDetailByObj('Volume', $scope.currentVolume.ID).$object;
                        $scope.volumeStatistics = volumeService.getVolumeStatisticsInfo($scope.currentVolume.ID).$object;
                    } else {

                        $scope.modalInstance = $uibModal.open({
                            animation: false,
                            size: 'lg',
                            templateUrl: 'apps/system3/production/progress/view/progress-detail.html',
                            controller: 'production.controller.progress.detail',
                            resolve: {
                                volParams: function () {
                                    return {
                                        volumeInfo: $scope.currentVolume
                                    }
                                }
                            }
                        });

                        $scope.modalInstance.result.then(function () {
                            $scope.loadSource();
                        }, function () {
                            //dismissed
                        });
                    }
                }

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