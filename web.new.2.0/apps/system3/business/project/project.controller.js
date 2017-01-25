define(['apps/system3/business/business',
        'apps/system3/business/project/project.controller.maintain',
        'apps/system3/business/project/project.service'], function (app) {

        app.module.controller("business.controller.project", function ($scope,$timeout,$uibModal, projectService, attachService) {

            // 查询条件
            $scope.filter = {
                pagesize: $scope.pageSize,
                pageindex: 1,
                trash: false,
                orderby: 'ID'
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
                columnDefs: [
                    {
                        name: 'Number', displayName: $scope.local.proj.number, enableColumnMenu: $scope.screenModel == 1,
                        width: 120, maxWidth: 200, minWidth: 70, pinnedLeft: $scope.screenModel == 1
                    },
                    {
                        name: 'Name', displayName: $scope.local.proj.name, enableColumnMenu: $scope.screenModel == 1, minWidth: 200
                    },
                    {
                        name: 'Manager', displayName: $scope.local.proj.manager, enableColumnMenu: false, width: '100',
                        cellFilter: 'enumMap:"user"'
                    },
                    {
                        name: 'Customer.Name', displayName: $scope.local.proj.customer, width: '200', enableColumnMenu: false,
                    },
                    {
                        name: 'Type', displayName: $scope.local.proj.type, width: '150', enableColumnMenu: false,
                        cellFilter: 'enumMap:"ProjectType"'
                    },
                    {
                        name: 'VolLevel', displayName: $scope.local.proj.vollevel, enableColumnMenu: false, width: '100',
                        cellFilter: 'enumMap:"VolLev"'
                    },
                    {
                        name: 'CreateDate', displayName: $scope.local.proj.createdate, enableColumnMenu: false, width: '150',
                        cellFilter: 'TDate'
                    },
                    {
                        name: 'DeliveryDate', displayName: $scope.local.proj.deliverydate, enableColumnMenu: false, width: '150',
                        cellFilter: 'TDate'
                    }
                ],
                rowTemplate: "<div " +
                    "ng-click=\"grid.appScope.gridApi.selection.toggleRowSelection(row.entity)\" ng-dblclick=\"grid.appScope.update(row.entity)\" " +
                    "ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" " +
                    "ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
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
                            $scope.currentProject = row.entity;
                        }

                    });
                }
            };

            // 加载页面数据
            $scope.loadSource = function () {

                $scope.gridPanel.block();
                projectService.getProjects($scope.filter).then(function (result) {

                    $scope.gridOptions.data = result.Source;
                    $scope.gridOptions.totalItems = result.TotalCount;
                    $scope.gridApi.selection.selectRow($scope.gridOptions.data[0]);
                
                    $scope.gridPanel.unblock();
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

            // 新建
            $scope.create = function () {
                $uibModal.open({
                    animation: false,
                    size: 'lg',
                    templateUrl: 'apps/system3/business/project/view/project-maintain.html',
                    controller: 'business.controller.project.maintain',
                    resolve: {
                        maintainParams: function () {return {}}
                    }
                }).result.then(function (info) {
                    bootbox.alert("创建成功");
                    $scope.gridOptions.data.push(info);
                });
            };

            // 更新
            $scope.update = function (info) {
                $uibModal.open({
                    animation: false,
                    size: 'lg',
                    templateUrl: 'apps/system3/business/project/view/project-maintain.html',
                    controller: 'business.controller.project.maintain',
                    resolve: {
                        maintainParams: function () {
                            return {
                                entityInfo: info,
                            }
                        }
                    }
                }).result.then(function (info) {
                    bootbox.alert("更新成功");
                });
            };

            // 删除
            $scope.delete = function () {

                if ($scope.selectedRows == undefined) {
                    bootbox.alert($scope.local.msg.noRowSelected);
                }

                var ids = $scope.selectedRows.map(function (obj) {
                    return obj.ID;
                });

                if ($scope.filter.trash) {
                    // 回收站状态，恢复选中对象

                    bootbox.confirm($scope.local.msg.confirmBackup, function (result) {
                        if (result === true) {
                            projectService.backup(ids).then(function () {
                                angular.forEach($scope.selectedRows, function (proj) {
                                    $scope.gridOptions.data.removeObj(proj);
                                })
                                bootbox.alert("恢复成功");
                            });
                        }
                    });
                } else {
                    bootbox.confirm($scope.local.msg.confirmDelete, function (result) {
                        if (result === true) {
                            projectService.batchRemove(ids).then(function () {
                                angular.forEach($scope.selectedRows, function (proj) {
                                    $scope.gridOptions.data.removeObj(proj);

                                })
                                bootbox.alert("删除成功");
                            });
                        }
                    });
                }
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
