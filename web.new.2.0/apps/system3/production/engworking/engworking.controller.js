define(['apps/system3/production/production.controller',
    'apps/system3/production/production.directive',
    'apps/system3/production/engworking/engworking.controller.plan',
    'apps/system3/production/engworking/engworking.service'], function (app) {

        app.controller('production.controller.engworking', function ($scope, $uibModal, engworkingService, uiGridConstants) {

            // 查询条件
            $scope.filter = {
                pagesize: $scope.pageSize,
                pageindex: 1,
                orderby: 'ID',
                status: 2,
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
                showColumnFooter: false,
                rowTemplate: "<div " +
                   "ng-click=\"grid.appScope.gridApi.selection.toggleRowSelection(row.entity)\" ng-dblclick=\"grid.appScope.plan(row.entity)\" " +
                   "ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" " +
                   "ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
                columnDefs: [
                    {
                        name: 'Stop', displayName: '暂停', enableColumnMenu: false,cellClass : 't-center',
                        width: 60, pinnedLeft: true,
                        cellTemplate: '<div class="ui-grid-cell-contents " > <button type="button" ng-click="grid.appScope.stop(row.entity)" class="btn btn-sm btn-info btn-square">暂停</button> </div>',
                    },
                    {
                        name: 'Plan', displayName: '策划', enableColumnMenu: false,cellClass : 't-center',
                        width: 60, pinnedLeft: true,
                        cellTemplate: '<div class="ui-grid-cell-contents" > <button type="button" ng-click="grid.appScope.plan(row.entity)" class="btn btn-sm btn-primary btn-square">策划</button> </div>',
                    },
                    {
                        name: 'Process', displayName: '进度', enableColumnMenu: false, cellClass: 't-center',
                        width: 60, pinnedRight: true,
                        cellTemplate: '<div class="ui-grid-cell-contents" > <button type="button" ng-click="grid.appScope.process(row.entity)" class="btn btn-sm btn-primary btn-square">进度</button> </div>',
                    },
                    {
                        name: 'Follow', displayName: '关注', enableColumnMenu: false, cellClass: 't-center',
                        width: 60, pinnedRight: true,
                        cellTemplate: '<div class="ui-grid-cell-contents" > <button type="button" ng-click="grid.appScope.follow(row.entity)" class="btn btn-sm btn-primary btn-square"><i class="fa " ng-class="{\'fa-heart\' : row.entity.isFollow, \'fa-heart-o\' : !row.entity.isFollow }"></i></button> </div>',
                    },
                     {
                         name: 'Number', displayName: $scope.local.eng.number, enableColumnMenu: false,
                         maxWidth: 200, minWidth: 180, pinnedLeft: true,
                     },
                    {
                        name: 'Name', displayName: $scope.local.eng.name, enableColumnMenu: false,
                        minWidth: 250, pinnedLeft: true,
                    },
                    {
                        name: 'Type', displayName: $scope.local.eng.type, width: '100', enableColumnMenu: false, cellClass: 't-center',
                        cellFilter: 'enumMap:"EngineeringType"'
                    },
                    {
                        name: 'Manager', displayName: $scope.local.eng.manager, enableColumnMenu: false, width: '100', cellClass: 't-center',
                        cellFilter: 'enumMap:"user"',
                    },
                    {
                        name: 'Phase', displayName: $scope.local.eng.phase, enableColumnMenu: false, width: '100', cellClass: 't-center',
                        cellFilter: 'enumMap:"EngineeringPhase"'
                    },
                    {
                        name: 'DuringDay', displayName: '历时', enableColumnMenu: false, width: '100', cellClass: 't-center',
                        cellTemplate: '<div class="ui-grid-cell-contents" " >{{ row.entity.DuringDay}}天</div>',
                    },
                    {
                        name: 'IsTimeOut', displayName: '是否超期', enableColumnMenu: false, width: '80',
                        cellTemplate: '<div class="ui-grid-cell-contents t-center" ng-class="{\'bg-red\' : row.entity.IsTimeOut }" >{{ row.entity.IsTimeOut ? \"超期\" : \"正常\" }}</div>',
                    },
                    {
                        name: 'StartDate', displayName: '启动日期', enableColumnMenu: false, width: '150',
                        cellFilter: 'TDate', cellClass: 't-center',
                    },
                    {
                        name: 'DeliveryDate', displayName: $scope.local.eng.deliverydate, enableColumnMenu: false, width: '150',
                        cellFilter: 'TDate', cellClass: 't-center',
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

                    //gridApi.selection.on.rowSelectionChanged($scope, function (row, e) {
                    //    $scope.selectedRows = gridApi.selection.getSelectedRows();

                    //    if (row.isSelected) {
                    //        $scope.currentEngineering = row.entity;
                    //    }

                    //});

                }
            };

            // 加载页面数据
            $scope.loadSource = function () {
                $scope.gridPanel.block();
                engworkingService.getEngineerings($scope.filter).then(function (result) {

                    $scope.gridOptions.data = result.Source.map(function (item) {
                        
                        item.isFollow = $scope.userConfig.engFollows.contains(function (f) {
                            
                            return f.ConfigKey == item.ID.toString();
                        });
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
                    status: 2,
                };
            };

            // 关注
            $scope.follow = function (info) {

                if (info.isFollow) {
                    engworkingService.unfollow(info.ID).then(function () {
                        bootbox.alert("取消成功");

                        info.isFollow = false;

                        $scope.userConfig.engFollows.custRemove(function (item) {
                            return item.ConfigKey == info.ID.toString();
                        })
                    })
                } else {
                    engworkingService.follow(info.ID).then(function () {
                        bootbox.alert("关注成功");

                        info.isFollow = true;

                        $scope.userConfig.engFollows.push({
                            ConfigKey: info.ID.toString()
                        });
                    })
                }
            }

            // 策划
            $scope.plan = function (info) {
                $uibModal.open({
                    animation: false,
                    size: 'lg7',
                    templateUrl: 'apps/system3/production/engworking/view/engworking-plan.html',
                    controller: 'production.controller.engworking.plan',
                    resolve: {
                        maintainParams: function () {
                            return {
                                entityInfo: info
                            }
                        }
                    }
                }).result.then(function (info) {
                    bootbox.alert("策划成功");

                });
            };

            // 暂停
            $scope.stop = function (info) {
                $uibModal.open({
                    animation: false,
                    size: 'md',
                    templateUrl: 'apps/system3/production/engworking/view/eng-stop.html',
                    controller: 'production.controller.engworking.stop',
                    resolve: {
                        maintainParams: function () {
                            return {
                                entityInfo: info
                            }
                        }
                    }
                }).result.then(function (info) {
                    $scope.gridOptions.data.removeObj(info);
                    bootbox.alert("工程暂停");
                });
            };

            // 查看工程进度
            $scope.process = function (info) {
                $uibModal.open({
                    animation: false,
                    size: 'lg7',
                    templateUrl: 'apps/system3/production/engworking/view/eng-process.html',
                    controller: function ($scope,$uibModalInstance) {
                        $scope.EngineeringID = info.ID;

                        $scope.close = function () {
                            $uibModalInstance.dismiss('cancel');
                        }
                    }
                });
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