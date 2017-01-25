define(['apps/system3/production/production.controller',
'apps/system3/production/engworking/engworking.service'], function (app) {

    app.controller('production.controller.engstop', function ($scope, $uibModal, engworkingService) {

        // 查询条件
        $scope.filter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
            status: 3, // 暂停
        };

        // 表格配置
        $scope.gridOptions = {
            multiSelect: false,
            enableGridMenu: false,
            enableColumnResizing: true,
            paginationPageSizes: $scope.pageSizes,
            paginationPageSize: $scope.pageSize,
            useExternalPagination: true,
            useExternalSorting: false,
            enableRowSelection: true,
            enableRowHeaderSelection: true,
            columnDefs: [
                {
                    name: 'Start', displayName: '启动', enableColumnMenu: false, cellClass: 't-center',
                    width: 60, pinnedLeft: true,
                    cellTemplate: '<div class="ui-grid-cell-contents " > <button type="button" ng-click="grid.appScope.start(row.entity)" class="btn btn-sm btn-info btn-square">启动</button> </div>',
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
                    name: 'Phase', displayName: $scope.local.eng.phase, enableColumnMenu: false, width: '100', cellClass: 't-center',
                    cellFilter: 'enumMap:"EngineeringPhase"'
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
                    name: 'StopDate', displayName: '暂停日期', enableColumnMenu: false, width: '150',
                    cellFilter: 'TDate',
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

        // 启动工程
        $scope.start = function (info) {

            engworkingService.startEng(info.ID).then(function () {

                $scope.gridOptions.data.removeObj(info);

                bootbox.alert("工程启动");
            });
        };


        // 加载页面数据
        $scope.loadSource = function () {
            $scope.gridPanel.block();
            engworkingService.getEngineerings($scope.filter).then(function (result) {

                $scope.gridOptions.data = result.Source;
                $scope.gridOptions.totalItems = result.TotalCount;

                $scope.gridPanel.unblock();
            })
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