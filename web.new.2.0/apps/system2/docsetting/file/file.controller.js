define([
'apps/system2/docsetting/docsetting',
'apps/system2/docsetting/docsetting.service'], function (app) {

    app.module.controller("docsetting.controller.file", function ($scope,$state, docsettingService, $uibModal) {

        var loadFileLibrarys = function () {
           
            var nodeTypes = {
                0: "cloud",
                1: "folder",
                2: "circle"
            }

            var convertTreeNode = function (parentNumber,fileNumber, nodes) {
                
                angular.forEach(nodes, function (item) {

                    item.text = item.Name;
                    item.type = nodeTypes[item.NodeType];
                    item.state = { 'opened': true };
                    item.src = item.Children == undefined || item.Children.length == 0 ? "docsetting.file.field" : "docsetting.file.op";
                    item.fileNumber = item.NodeType == 1 ? item.Number : fileNumber;

                    item.ParentFullKey = parentNumber ? parentNumber + ".Node." : "";
                    pkey = item.ParentFullKey + item.Number;

                    item.children = convertTreeNode(pkey,item.fileNumber, item.Children);
                })

                return nodes;
            }

			docsettingService.file.getList().then(function (result) {
			    
				angular.forEach(result, function (item) {

				    item.children = convertTreeNode("","", item.Children);
					item.text = item.Name;
					item.type = nodeTypes[item.NodeType]
					item.state = { 'opened': true };
					item.src = "docsetting.file.op";
				})

				$scope.documents = result;
			});
		}

        $scope.createFileLibrary = function (node,title) {
           
            if (node == undefined) {
                node = $scope.currentFile;
            }
		    var modalInstance = $uibModal.open({
		        animation: false,
		        templateUrl: 'apps/system2/docsetting/file/view/file-maintain.html',
		        size: 'sm',
		        controller: "docsetting.controller.file.maintain",
		        resolve: {
		            maintainInfo: function () {
		                return {
		                    update: false,
		                    title: title,
		                    fileInfo: {
		                        FondsNumber: node.FondsNumber,
		                        ParentFullKey: node.type == "cloud" ? "" : node.ParentFullKey + node.Number + ".Node."
		                    },
		                };
		            }
		        }
		    });

		    modalInstance.result.then(function (info) {
		        loadFileLibrarys();
		    }, function () {
		        //dismissed
		    });
		}

        var updateFileLibrary = function (fileInfo, title) {

		    var modalInstance = $uibModal.open({
		        animation: false,
		        templateUrl: 'apps/system2/docsetting/file/view/file-maintain.html',
		        size: 'sm',
		        controller: "docsetting.controller.file.maintain",
		        resolve: {
		            maintainInfo: function () {
		                return {
		                    update: true,
		                    title: title,
		                    fileInfo: fileInfo
		                };
		            }
		        }
		    });

		    modalInstance.result.then(function (info) {
		        loadFileLibrarys();
		    }, function () {
		        //dismissed
		    });
		}

		var delFileLibrary = function (fileInfo) {

		    bootbox.confirm("删除节点将同时删除该节点下的所有文件，确认删除？", function (result) {
		        if (result === true) {
		            docsettingService.file.del(fileInfo.FondsNumber, fileInfo.type == "cloud" ? "" : fileInfo.ParentFullKey + fileInfo.Number).then(function () {
		                loadFileLibrarys();
		            });
		        }
		    });
		}

		var generateFileLibrary = function (node) {
		    docsettingService.file.generate(node.FondsNumber, node.Number).then(function () {
		        bootbox.alert("文件库生成成功");
		    })
		}

		$scope.treeContextmenu = function (node) {
		    var add = {
		        "label": "添加文件库",
		        "icon": "fa fa-folder",
		        "action": function (obj) {
		           
		            $scope.createFileLibrary(node.original, "添加文件库");
		        },
		    };

		    var del = {
		        "label": "删除节点",
		        "icon": "fa fa-close",
		        "action": function (obj) {
		            delFileLibrary(node.original);
		        },
		    };

		    var update = {
		        "label": "修改节点",
		        "icon": "fa fa-edit",
		        "action": function (obj) {
		            updateFileLibrary(node.original, "修改节点");
		        },
		    };

		    var generate = {
		        "label": "生成文件库",
		        "icon": "fa fa-check-circle",
		        "action": function (obj) {
		            generateFileLibrary(node.original)
		        },
		    }

		    var addCategory = {
		        "label": "添加分类",
		        "icon": "fa fa-circle",
		        "action": function (obj) {
		            $scope.createFileLibrary(node.original, "添加分类");
		        },
		    }

		    if (node.type == "cloud") {
		        return { "add": add, };
		    } else if (node.type == "folder") {
		        addCategory.separator_after = true;
		        return {
		            "addCategory" : addCategory,
		            "del": del,
                    "update": update,
                    "generate": generate,
                };
		    } else if (node.type == "circle") {
		        addCategory.separator_after = true;
		        return {
		            "addCategory": addCategory,
		            "del": del,
		            "update": update,
		        };
		    }
		}

		$scope.changed = function (e, data) {
		    $scope.setCurrent(data.node.original);
		}

		$scope.setCurrent = function (info) {
		    $scope.$safeApply(function () {
		        $scope.currentFile = info;
		    })
		   
		    $state.go(info.src);
		}

		loadFileLibrarys();

	});


	app.module.controller("docsetting.controller.file.maintain", function ($scope, docsettingService, $uibModal, $uibModalInstance, maintainInfo) {
	    $scope.update = maintainInfo.update;
	    $scope.fileInfo = maintainInfo.fileInfo;
	    $scope.title = maintainInfo.title;

	    $scope.save = function () {
	        if (maintainInfo.update) {
	            docsettingService.file.update($scope.fileInfo).then(function () {
	                $uibModalInstance.close($scope.fileInfo);
	            });
	        } else {

	            docsettingService.file.check($scope.fileInfo).then(function (result) {
	                if (!result) {
	                    docsettingService.file.add($scope.fileInfo).then(function () {
	                        $uibModalInstance.close($scope.fileInfo);
	                    });
	                } else {
	                    bootbox.alert("文件库名称重复");
	                }
	            })
	        }
	    };

	    $scope.close = function () {
	        $uibModalInstance.dismiss('cancel');
	    }
	});



});
