define(['apps/system3/production/production.controller',
        'apps/system3/production/specialty/specialty.controller.plan',
        'apps/system3/production/specialty/specialty.service'], function (app) {

            app.controller('production.controller.specialty', function ($scope, $uibModal, specialtyService, uiGridGroupingConstants) {
                // 查询条件
                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
                    orderby: 'ID'
                };

                // 默认模式
                $scope.isMaintain = $scope.maintainModel == 1;
                $scope.isSearch = false;

                // 表格配置
                $scope.gridOptions = {
                    multiSelect: false,
                    enableSorting: false,
                    enableGridMenu: true,
                    paginationPageSizes: $scope.pageSizes,
                    paginationPageSize: $scope.pageSize,
                    useExternalPagination: true,
                    useExternalSorting: false,
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
                                aggregation.rendered = this.index == 1 ? aggregation.entity.Project.Number : aggregation.entity.Engineering.Number;
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
                                aggregation.rendered = this.index == 1 ? aggregation.entity.Project.Name : aggregation.entity.Engineering.Name;
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
                                aggregation.rendered = this.index == 1 ? aggregation.entity.Project.Manager : aggregation.entity.Engineering.Manager;
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
                                aggregation.rendered = this.index == 1 ? aggregation.entity.Project.CreateDate : aggregation.entity.Engineering.CreateDate;
                                this.index++;
                            }
                        },
                        ag_enddate: {
                            label: 'ag_enddate',
                            aggregationFn: function (aggregation, fieldValue, numValue, row) {
                                aggregation.entity = row.entity;
                                this.index++;
                            },
                            finalizerFn: function (aggregation) {
                                aggregation.rendered = this.index == 1 ? aggregation.entity.Project.DeliveryDate : aggregation.entity.Engineering.DeliveryDate;
                                this.index++;
                            }
                        }
                    },
                    columnDefs: [
                        { name: 'Project.ID', grouping: { groupPriority: 0 }, visible: false }, { name: 'Engineering.ID', grouping: { groupPriority: 1 }, visible: false },
                         {
                             name: 'Engineering.Number', displayName: $scope.local.proj.number, enableColumnMenu: true,
                             width: 170,  pinnedLeft: true,
                             cellTemplate: '<div class="ui-grid-cell-contents" >{{grid.appScope.setCol1(row)}}</div>',
                             treeAggregationType: 'ag_number'
                         },
                         {
                             name: 'SpecialtyID', displayName: $scope.local.proj.name, enableColumnMenu: true,
                             minWidth: 250, maxWidth: 350, pinnedLeft: true, 
                             cellTemplate: '<div class="ui-grid-cell-contents " ng-dblclick="grid.appScope.plan()" ng-class="{ \'bg-orange\' : row.entity.Engineering && !row.entity.SpecialtyID}" ><span ng-class="{ \'pull-right\' : row.entity.Engineering}" >{{grid.appScope.setCol2(row) | enumMap:"Specialty"}}</span></div>',
                             treeAggregationType: 'ag_name'
                         },
                        {
                            displayName: $scope.local.specil.manager, enableColumnMenu: false, width: '100',
                            name: 'Manager',
                            cellTemplate: '<div class="ui-grid-cell-contents" >{{grid.appScope.setCol3(row) | enumMap:"user"}} ' +
                                '<a ng-show="!row.entity.Engineering || row.entity.SpecialtyID" class="btn btn-transparent msgChatBtn" ng-click="grid.appScope.haveChat(row)"><i class="fa fa-weixin"></i></a></div>',
                            cellFilter: 'enumMap:"user"',
                            treeAggregationType: 'ag_manager'
                        },
                        {
                            name: 'StartDate', displayName: $scope.local.specil.startDate, enableColumnMenu: false, width: '150',
                            cellFilter: 'TDate',
                            treeAggregationType: 'ag_startdate'
                            
                        },
                        {
                            name: 'EndDate', displayName: $scope.local.specil.endDate, enableColumnMenu: false, width: '150',
                            cellFilter: 'TDate',
                            treeAggregationType: 'ag_enddate'
                        },
                        {
                            name: 'Note', displayName: $scope.local.specil.note, enableColumnMenu: false, 
                        }
                    ],
                    onRegisterApi: function (gridApi) {

                        $scope.gridApi = gridApi;

                        //$scope.gridApi.grid.registerColumnsProcessor(setGroupValues, 410);

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
                                $scope.currentSpec = row.entity;

                                angular.forEach($scope.SpecialtysCopy, function (item) {
                                    item.Manager = undefined;
                                    item.ProcessModel = undefined;
                                    item.StartDate = undefined;
                                    item.EndDate = undefined;
                                    item.Note = undefined;

                                    if ($scope.currentSpec.Specialtys == undefined) {
                                        item.isSelected = false;
                                    } else {
                                        var obj = $scope.currentSpec.Specialtys.get(function (s) { return s.SpecialtyID == item.Value });

                                        if (obj) {
                                            item.Manager = obj.Manager;
                                            item.ProcessModel = obj.ProcessModel;
                                            item.StartDate = obj.StartDate;
                                            item.EndDate = obj.EndDate;
                                            item.Note = obj.Note;
                                        }

                                        item.isSelected = obj != undefined;
                                    }
                                });
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

                $scope.setCol1 = function (row) {
                    if (row.entity.Engineering == undefined) {

                        for (var i in row.entity) {
                            if (row.entity[i].label == "ag_number") {
                                return row.entity[i].rendered;
                            }
                        }

                    } else {
                        return "";
                    }
                }

                $scope.setCol2 = function (row) {
                    if (row.entity.Engineering == undefined) {

                        for (var i in row.entity) {
                            if (row.entity[i].label == "ag_name") {
                                return row.entity[i].rendered;
                            }
                        }

                    } else {
                        if (row.entity.SpecialtyID) {
                            return row.entity.SpecialtyID;
                        }
                        return "未策划";
                    }
                }

                $scope.setCol3 = function (row) {
                    if (row.entity.Engineering == undefined) {

                        for (var i in row.entity) {
                            if (row.entity[i].label == "ag_manager") {
                                return row.entity[i].rendered;
                            }
                        }
                    } else {
                        return row.entity.Manager;
                    }
                }

                $scope.haveChat = function (row) {
                   
                    if (row.entity.Engineering == undefined) {

                        for (var i in row.entity) {
                            if (row.entity[i].label == "ag_manager") {
                                $scope.openChat(row.entity[i].rendered, "[专业策划] - " + row.entity[i].entity.Engineering.Name);
                                break;
                            }
                        }
                    } else {
                        $scope.openChat(row.entity.Manager,"[专业策划] - " + row.entity.Engineering.Name + " 专业 :" + $scope.Specialty_enum[row.entity.SpecialtyID]);
                    }
                }

                // 加载页面数据
                $scope.loadSource = function () {

                    $scope.thisPanel.block();
                    var Specialtys = $scope.getBaseData("Specialty");

                    $scope.SpecialtysCopy = angular.copy(Specialtys);

                    specialtyService.getSpecialtys($scope.filter).then(function (result) {
                        
                        $scope.gridOptions.data = result.Source;
                        $scope.gridOptions.totalItems = result.TotalCount;

                        $scope.thisPanel.unblock();
                    })
                };

                // 保存专业信息
                $scope.updateSpecialtys = function () {
                    var result = [];

                    angular.forEach($scope.SpecialtysCopy, function (item) {
                        if (item.isSelected) {
                            item.SpecialtyID = item.Value;
                            result.push(item);
                        }
                    });

                    specialtyService.updateSpecialtys($scope.currentSpec.Engineering.ID, result).then(function () {
                        $scope.loadSource();
                    })
                };

                // 打开策划
                $scope.plan = function () {

                    if ($scope.currentSpec == undefined) {
                        bootbox.alert('请选择工程！');
                        return;
                    }
                    if ($scope.maintainModel == 1) {
                        $scope.isMaintain = true;
                    } else {

                        $scope.modalInstance = $uibModal.open({
                            animation: false,
                            size: 'lg',
                            templateUrl: 'apps/system3/production/specialty/view/specialty-maintain.html',
                            controller: 'production.controller.specialty.plan',
                            resolve: {
                                planParams: function () {
                                    return {
                                        specInfo: $scope.currentSpec
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