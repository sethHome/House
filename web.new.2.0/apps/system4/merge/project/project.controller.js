define([
'apps/system4/merge/merge',
'apps/system4/merge/merge.service'], function (app) {

    app.module.controller("merge.controller.project", function ($scope,mergeService, $uibModal) {

        // 查询条件
        $scope.filter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
        };

        $scope.projects = [];

        $scope.gridOptions = {
            data: "projects",
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
                    name: 'Number', displayName: '项目编号', enableColumnMenu: $scope.screenModel == 1,
                    width: 120, maxWidth: 200, minWidth: 70, pinnedLeft: $scope.screenModel == 1
                },
                {
                    name: 'Name', displayName: '项目名称', enableColumnMenu: $scope.screenModel == 1, minWidth: 200
                },
                {
                    name: 'Phase', displayName: '项目阶段', width: '150', enableColumnMenu: false,
                    cellFilter: 'enumMap:"ProjPhase"' 
                },
                {
                    name: 'Manager', displayName: '项目经理', enableColumnMenu: false, width: '100',
                    cellFilter: 'enumMap:"user"'
                },
                {
                    name: 'Area', displayName: '地区', width: '150', enableColumnMenu: false,
                    cellFilter: 'enumMap:"region"' // ,grouping: { groupPriority: 0 }, 
                },
                {
                    name: 'CreateDate', displayName: '创建日期', enableColumnMenu: false, width: '150',
                    cellFilter: 'TDate'
                }
            ],
            rowTemplate: "<div ng-click=\"grid.appScope.gridApi.selection.toggleRowSelection(row.entity)\" ng-dblclick=\"grid.appScope.updateProject(row.entity)\" " +
                   "ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" " +
                   "ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
            onRegisterApi: function (gridApi) {

                $scope.gridApi = gridApi;

                $scope.gridApi.core.on.sortChanged($scope, function (grid, sortColumns) {
                    //$scope.filter.orderby = sortColumns;

                    if (sortColumns.length == 0) {
                        $scope.filter.orderby = null;
                    } else {
                        $scope.filter.orderdirection = sortColumns[0].sort.direction;
                        $scope.filter.orderby = sortColumns[0].field;

                    }
                });

                gridApi.pagination.on.paginationChanged($scope, function (newPage, pageSize) {

                    $scope.filter.pagesize = pageSize,
                    $scope.filter.pageindex = newPage;

                });
            }
        }

        $scope.addProject = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system4/merge/project/view/project-maintain.html',
                size: 'lg',
                controller: "merge.controller.projectmaintain",
                resolve: {
                    maintainService: function () {
                        return {
                            update: false,

                            addFileInfo: function (info) {
                                $scope.fileSource.push(info);
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                $scope.projects.push(info);
            }, function () {
                //dismissed
            });
        }

        $scope.updateProject = function (info) {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system4/merge/project/view/project-maintain.html',
                size: 'lg',
                controller: "merge.controller.projectmaintain",
                resolve: {
                    maintainService: function () {
                        return {
                            update: true,
                            projInfo: info,
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                $scope.projects.push(info);
            }, function () {
                //dismissed
            });
        }

        $scope.deleteProject = function () {
            var rows = $scope.gridApi.selection.getSelectedRows();

            if (rows.length > 0) {

                bootbox.confirm("选中" + rows.length + "行,确认删除？", function (result) {
                    if (result === true) {
                        var ids = rows.map(function (item) { return item.ID }).join(',')

                        mergeService.deleteProject(ids).then(function (result) {

                            angular.forEach(rows,function (item) {
                                $scope.projects.removeObj(item);
                            });
                        })
                    }
                });

            } else {
                bootbox.alert("未选中行");
            }
        }

        // 监听筛选条件查询数据
        $scope.$watch("filter", function (newval, oldval) {
            if (newval ) {
                loadData();
            }
        }, true);

        var loadData = function () {
            mergeService.getProjects($scope.filter).then(function (result) {
                $scope.projects = result.Source;
                $scope.gridOptions.totalItems = result.TotalCount;
            });
        }
    });

    app.module.controller("merge.controller.projectmaintain", function ($scope, mergeService, $uibModal, $uibModalInstance, maintainService) {

        $scope.filters = [{ title: "OfficeWord", extensions: "doc,docx" }];

        $scope.maintainService = maintainService;
        $scope.projInfo = maintainService.projInfo;

        // 拷贝专业信息
        $scope.SpecialtysCopy = angular.copy($scope.getBaseData("Specialty"));

        angular.forEach($scope.SpecialtysCopy, function (s) {

            s.SpecilID = s.Value;

            if ($scope.projInfo && $scope.projInfo.ProjSpecils) {

                var findObj = $scope.projInfo.ProjSpecils.find(function (i) {
                    return (i.SpecilID + '') == s.Value;
                });

                if (findObj) {
                    findObj.isSelected = true;
                    angular.extend(s, findObj);
                }
            }
        })

        $scope.save = function () {

            $scope.projInfo.ProjSpecils = $scope.SpecialtysCopy.where(function (s) { return s.isSelected; });

            if (maintainService.update) {
                mergeService.updateProject($scope.projInfo.ID,$scope.projInfo).then(function (id) {
                    $uibModalInstance.dismiss('cancel');
                });
            } else {

                // 创建项目

                mergeService.createProject($scope.projInfo).then(function (id) {

                    $scope.projInfo.ID = id;
                    
                    $uibModalInstance.close($scope.projInfo);
                });
            }
        }

        $scope.currentStep = 1;

        $scope.next = function () {
            if ($scope.currentStep < 4) {
                $scope.currentStep++;;
            } else {
                $scope.save();
            }
        }

        $scope.sub = function () {
            if ($scope.currentStep > 1) {
                $scope.currentStep--;
            }
        }


        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }

    });
});
