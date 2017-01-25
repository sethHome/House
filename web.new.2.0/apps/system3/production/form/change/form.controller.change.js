define([
'apps/system3/production/production.controller',
'apps/system3/production/form/change/form.controller.change.maintain',
'apps/system3/production/form/form.service'], function (app) {

    app.controller('production.controller.form.change', function ($scope, formService, $uibModal) {
        // 查询条件
        $scope.filter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
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
            rowTemplate: "<div ng-dblclick=\"grid.appScope.openMaintain(row.entity)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
            columnDefs: [
                 {
                     name: 'ID', displayName: '进度', enableColumnMenu: false, width: 100, pinnedLeft: true,
                     cellTemplate: '<process-info class="m-5" key="FormChange" id="{{row.entity.ID}}"></process-info>',
                 },
                {
                    name: 'Engineering.Name', displayName: $scope.local.eng.name, enableColumnMenu: false,minWidth: 250,
                },
                {
                    name: 'SpecialtyID', displayName: $scope.local.specil.name, enableColumnMenu: false, width: 130, cellFilter: 'enumMap:"Specialty"'
                },
                {
                    name: 'Volume.Name', displayName: $scope.local.volume.name, enableColumnMenu: false, width: 130
                },
                {
                    name: 'Reason', displayName: '原因', enableColumnMenu: false, width: 200, 
                },
                {
                    name: 'CreateDate', displayName: '变更时间', enableColumnMenu: false, width: 100, cellFilter: 'TDate'
                },
                {
                    name: 'CreateUserID', displayName: '发起人', enableColumnMenu: false, width: 100, cellFilter: 'enumMap:"user"'
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
                        $scope.currentChange = row.entity;
                    }
                });

                gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
                    gridApi.selection.toggleRowSelection(newRowCol.row.entity);
                })
            }
        };

        // 加载页面数据
        $scope.loadSource = function () {

            formService.getChangeList($scope.filter).then(function (result) {
                $scope.gridOptions.data = result.Source;
                $scope.gridOptions.totalItems = result.TotalCount;
            })
        };

        $scope.$watch("isSearch", function (newval, oldval) {
            if (newval == true) {
                $scope.isMaintain = false;
            }
        });
        $scope.$watch("isMaintain", function (newval, oldval) {
            if (newval == true) {
                $scope.isSearch = false;
            }
        });

        // 打开编辑窗口
        $scope.openMaintain = function (entity) {

            if ($scope.currentChange == undefined && entity == undefined) {
                bootbox.alert('请选择行！');
                return;
            }

            if ($scope.maintainModel == 1) {

                $scope.isMaintain = true;

            } else {
                
                $scope.modalInstance = $uibModal.open({
                    animation: false,
                    size: 'lg',
                    templateUrl: 'apps/system3/production/form/change/view/change-maintain.html',
                    controller: 'production.controller.change.maintain',
                    resolve: {
                        modelParam: function () {
                            return {
                                objID: entity ? entity.ID : 0
                            }
                        },
                    }
                });

                $scope.modalInstance.result.then(function () {
                    $scope.loadSource();
                }, function () {
                    //dismissed
                });
            }
        };

        // 删除
        $scope.remove = function () {

            if ($scope.currentChange == undefined && entity == undefined) {
                bootbox.alert('请选择行！');
                return;
            }

            bootbox.confirm("确定删除？", function (result) {
                if (result === true) {
                    provideService.remove($scope.currentChange.ID).then(function () {
                        $scope.loadSource();
                    });
                }
            });
        }

        $scope.afterSave = function () {
            $scope.loadSource();
        }
        
        $scope.loadSource();

    });
});