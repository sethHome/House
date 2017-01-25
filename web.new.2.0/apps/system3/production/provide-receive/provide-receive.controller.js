define(['apps/system3/production/production.controller',
        'apps/system3/production/provide/provide.controller.maintain',
        'apps/system3/production/provide/provide.service',
        'apps/system3/production/specialty/specialty.service'], function (app) {

            app.controller('production.controller.provide-receive', function ($scope, provideService, $uibModal, uiGridGroupingConstants) {

                // 查询条件
                $scope.filter = {
                    pagesize: $scope.pageSize,
                    pageindex: 1,
                    myreceive: 1,
                    orderby: 'LimitDate',
                    orderdirection:'asc'
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
                    columnDefs: [
                        {
                            name: 'Engineering.Name', displayName: $scope.local.eng.name, enableColumnMenu: false,width: 200,
                        },
                        {
                            name: 'Specialty.SpecialtyID', displayName: $scope.local.specil.name, enableColumnMenu: false, width: 100, cellFilter: 'enumMap:"Specialty"'
                        },
                        {
                            name: 'LimitDate', displayName: $scope.local.provide.limitDate, enableColumnMenu: false, width: 130,
                            cellTemplate: '<div class="ngCellText p-5" ng-class="col.colIndex()" ng-show="row.entity.LimitDate"><span class="glyphicon glyphicon-time "></span> <span class="badge badge-info">剩余{{row.entity.LimitDate | leaveDate}}</span> </div>',
                        },
                         {
                             name: 'DocName', displayName: $scope.local.provide.docName, enableColumnMenu: false, width: 200, cellFilter: 'subStr:"20"'
                         },
                       {
                           name: 'DocContent', displayName: $scope.local.provide.docContent, enableColumnMenu: false, minWidth: 250, cellFilter: 'subStr:"200"'
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
                                $scope.currentProvide = row.entity;
                            }
                        });

                        gridApi.cellNav.on.navigate($scope, function (newRowCol, oldRowCol) {
                            gridApi.selection.toggleRowSelection(newRowCol.row.entity);
                        })
                    },
                    rowTemplate: "<div ng-dblclick=\"grid.appScope.openProvideMaintain(row.entity)\" ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>"
                };

                $scope.$watch("isSearch", function (newval, oldval) {
                    $scope.isMaintain = !$scope.isSearch;
                });
                $scope.$watch("isMaintain", function (newval, oldval) {
                    $scope.isSearch = !$scope.isMaintain;
                });

                $scope.setCol = function (row, label) {
                    if (row.entity.Engineering == undefined) {
                        for (var i in row.entity) {
                            if (row.entity[i] && row.entity[i].label == label) {
                                return row.entity[i].rendered;
                            }
                        }
                    } else {

                        if (label == 'ag_name') {
                            return row.entity.DocName;
                        }
                        if (label == 'ag_manager' && row.entity.ID) {
                            return row.entity.SendUserID;
                        }
                        return "";
                    }

                }
                $scope.haveChat = function (row) {

                    $scope.openChat(row.entity.UserID, row.entity.Content);
                }
                // 加载页面数据
                $scope.loadSource = function () {
                    
                    provideService.getSource($scope.filter).then(function (result) {
                        $scope.gridOptions.data = result.Source;
                        $scope.gridOptions.totalItems = result.TotalCount;
                    })
                };

                // 打开编辑窗口
                $scope.openProvideMaintain = function (entity) {

                    if ($scope.currentProvide == undefined && entity == undefined) {
                        bootbox.alert('请选择行！');
                        return;
                    }

                    if ($scope.maintainModel == 1) {

                        // 查看或者编辑提资
                        $scope.maintainProvide(entity);

                        $scope.isMaintain = true;
                    } else {

                        $scope.modalInstance = $uibModal.open({
                            animation: false,
                            size: 'lg',
                            templateUrl: 'apps/system3/production/provide/view/provide-maintain.html',
                            controller: 'production.controller.provide.maintain',
                            resolve: {
                                provideParams: function () {
                                    return {
                                        view : true,
                                        currentProvide: entity ? entity : $scope.currentProvide,
                                        edit: entity != undefined && entity.ID > 0,
                                        modelType: 'window',
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

                $scope.loadSource();
            });
        });