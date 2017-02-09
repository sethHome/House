define([
'apps/system4/merge/merge',
'apps/system4/merge/merge.service'], function (app) {

    app.module.controller("merge.controller.merge", function ($scope, mergeService, $uibModal, attachDownloadUrl) {

        $scope.currentTaskTab = 1;

		// 查询条件
		$scope.filter = {
			pagesize: $scope.pageSize,
			pageindex: 1,
			orderby: 'ID',
		};

		$scope.projects = [];

		$scope.attachDownloadUrl = attachDownloadUrl;

        // 合并的历史记录
		$scope.mergeHistorys = [];

		$scope.HeadNumbers = [{ "Value": 1, "Text": "中文" }, { "Value": 2, "Text": "数字" }];
		$scope.newPageChapters = [];
		for (var i = 1; i < 18; i++) {
		    $scope.newPageChapters.push({ "Value": i, "Text": "第" + i + "章" });
		}


        $scope.MergeOption = {
            'HeadNumber':2,
            'Head1Style': {
                'Align':0,
                'FontFamily': 1,
                'Size':"12",
            },
            'Head2Style': {
                'Align': 0,
                'FontFamily': 1,
                'Size': "12",
            },
            'ContentStyle': {
                'FontFamily': 1,
                'Size': "12",
            },
            'ListStyle': {
                'FontFamily': 1,
                'Size': "12",
            },
            'PageNumberStyle': {
                'FontFamily': 1,
                'Size': "12",
                'Align': 1,
            }
        };

        $scope.gridOptions = {
		    data: "projects",
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
                    name: 'Number', displayName: '项目编号', enableColumnMenu: $scope.screenModel == 1,
                    width: 120, maxWidth: 200, minWidth: 70, pinnedLeft: $scope.screenModel == 1
                },
                {
                    name: 'Name', displayName: '项目名称', enableColumnMenu: $scope.screenModel == 1, minWidth: 200
                },
		    ],
		    rowTemplate: "<div ng-click=\"grid.appScope.gridApi.selection.toggleRowSelection(row.entity)\" " +
                   "ng-repeat=\"(colRenderIndex, col) in colContainer.renderedColumns track by col.colDef.name\" class=\"ui-grid-cell\" " +
                   "ng-class=\"{ 'ui-grid-row-header-cell': col.isRowHeader }\" ui-grid-cell ></div>",
		    onRegisterApi: function (gridApi) {

		        $scope.gridApi = gridApi;

		        $scope.gridApi.selection.on.rowSelectionChanged($scope, function (selected) {
		            if (selected.isSelected) {
		                $scope.currentProj = selected.entity;
		            } else {
		                $scope.currentProj = undefined;
		            }

		        });
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
		};

        // 监听筛选条件查询数据
		$scope.$watch("options.all", function (newval, oldval) {

		    for (var item in $scope.options) {
		        if (item != "all") {
		            $scope.options[item] = newval;
		        }
		    }

		}, true);

		// 监听筛选条件查询数据
		$scope.$watch("filter", function (newval, oldval) {
			if (newval) {
				loadData();
			}
		}, true);

	    // 监听筛选条件查询数据
		$scope.$watch("currentProj", function (newval, oldval) {
		    if (newval) {
		        loadHistory();
		        $scope.mergeResult = undefined;
		    }
		}, true);

		$scope.merge = function () {

		    if (!$scope.currentProj) {
		        bootbox.alert("未选中项目");
		        return;
		    }

		    if (!$scope.currentProj.ProjSpecils.contains(function (item) { return item.IsFinish; })) {
		        bootbox.alert("项目没有完成上传的专业");
		        return;
		    }
		    $scope.mergePanel.block();

            $scope.MergeOption.NewPageChapters = $scope.newPageChapters.where(p => p.isSelected).map(p => p.Value);

            mergeService.merge($scope.currentProj.ID, $scope.MergeOption).then(function (result) {

		        $scope.mergeResult = result;

		        $scope.currentTaskTab = 3;

		        loadHistory();

		        $scope.mergePanel.unblock();

		        //$scope.mergeHistorys.push(result.Attach);
		    })
	    }

		var loadHistory = function () {
		    mergeService.getMergeHistory($scope.currentProj.ID).then(function (result) {
		        $scope.mergeHistorys = result;
		    });
		}

		var loadData = function () {
			mergeService.getProjects($scope.filter).then(function (result) {
				$scope.projects = result.Source;
				$scope.gridOptions.totalItems = result.TotalCount;
			});
		}
	});

});
