define([
    'apps/system3/office/office',
    'apps/system3/office/car/car.service'], function (app) {

        app.module.controller("office.controller.car.back", function ($scope, $stateParams, $uibModal, $timeout, carService) {
            
            $scope.showMyApplyCar();
        });

        app.module.controller("carBackModalCtl", function ($scope, carService, modelParam, $uibModalInstance) {

            $scope.modelType = 'window';

        });

        app.module.controller("carBackCtl", function ($scope, $stateParams, $uibModal, $timeout, carService) {

            // 查询条件
            $scope.filter = {
                pagesize: $scope.pageSize,
                pageindex: 1,
            };

            // 表格配置
            $scope.gridOptions = {
                multiSelect: false,
                enableGridMenu: false,
                paginationPageSizes: $scope.pageSizes,
                paginationPageSize: $scope.pageSize,
                useExternalPagination: true,
                useExternalSorting: true,
                enableRowSelection: true,
                enableRowHeaderSelection: false,
                columnDefs: [
                    {
                        name: 'TargetPlace', displayName: "目的地", minWidth: 200,
                    },
                    {
                        name: 'StartDate', displayName: "开始日期", width: 120,  cellFilter: 'TDate'
                    },
                    {
                        name: 'BackDate', displayName: "结束日期", width: 120, cellFilter: 'TDate'
                    },
                    {
                        name: 'Mileage', displayName: "里程", width: 100, 
                    },
                    {
                        name: 'PeerStaff', displayName: "同行人员",  cellFilter: 'users'
                    },
                ],
                onRegisterApi: function (gridApi) {

                    $scope.gridApi = gridApi;

                    gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                        $scope.filter.pagesize = pageSize,
                        $scope.filter.pageindex = newPage;
                    });

                    gridApi.selection.on.rowSelectionChanged($scope, function (row, e) {
                        $scope.selectedRows = gridApi.selection.getSelectedRows();
                    });

                    gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
                        gridApi.selection.toggleRowSelection(newRowCol.row.entity);
                    });
                }
            };

            $scope.loadSource = function () {
                carService.getMyUse($scope.filter).then(function (result) {
                    
                    $scope.gridOptions.data = result.Source;
                    $scope.gridOptions.totalItems = result.TotalCount;
                });
            }

            $scope.$watch("currentCar", function (newval, oldval) {
                if (newval) {
                    $scope.carInfo = newval;
                    $scope.filter.car = newval.ID;
                }
            });

            $scope.$watch("filter", function (newval, oldval) {
                if (newval) {
                    $scope.loadSource();
                }
            },true);

        });
    });
