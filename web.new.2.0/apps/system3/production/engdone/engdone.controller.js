define(['apps/system3/production/production.controller',
'apps/system3/production/engworking/engworking.service'], function (app) {

    app.controller('production.controller.engdone', function ($scope, $uibModal, engworkingService) {

        // 查询条件
        $scope.filter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
            status: 4, // 完成
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
                     name: 'Number', displayName: $scope.local.proj.number, enableColumnMenu: false,
                     width: 130, maxWidth: 200, minWidth: 120, pinnedLeft: true,
                 },
                {
                    name: 'Name', displayName: $scope.local.proj.name, enableColumnMenu: false,
                    minWidth: 200, pinnedLeft: true,
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
                    name: 'Phase', displayName: $scope.local.eng.phase, enableColumnMenu: false, width: '100', cellClass: 't-center',
                    cellFilter: 'enumMap:"EngineeringPhase"'
                },
                {
                    name: 'Status', displayName: $scope.local.eng.status, enableColumnMenu: false, width: '100',
                    cellFilter: 'enumMap:"EngineeringStatus"'
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
                    name: 'FinishDate', displayName: '完成日期', enableColumnMenu: false, width: '150',
                    cellFilter: 'TDate', cellClass: 't-center',
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