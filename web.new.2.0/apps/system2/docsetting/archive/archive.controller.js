define([
'apps/system2/docsetting/docsetting',
'apps/system2/docsetting/docsetting.service'], function (app) {

    app.module.controller("docsetting.controller.archive", function ($state, $scope, docsettingService, $uibModal) {

        var loadArchive = function () {
            docsettingService.archive.getList().then(function (result) {

                angular.forEach(result, function (item) {

                    angular.forEach(item.Archives, function (a) {

                        a.text = a.Name;
                        a.type = "database";
                        a.state = { 'opened': false };
                        a.FondsNumber = item.Number;
                        a.src = "docsetting.archive.op";

                        if (a.Disabled) {
                            a.type = "disabled";
                            a.text = "<span class='bg-orange'>" + a.Name + "&nbsp;[禁用]</span>"
                        }else {
                            a.children = [];
                            //if (a.HasProject) {
                            //    a.children.push({
                            //        text: "项目",
                            //        type: 'file',
                            //        fondsNumber: item.Number,
                            //        archiveKey: a.Key,
                            //        key: 'Project',
                            //        src : "docsetting.archive.field"
                            //    });
                            //}

                            if (a.HasVolume) {
                                a.children.push({
                                    text: "案卷",
                                    type: 'cube',
                                    fondsNumber: item.Number,
                                    archiveKey: a.Key,
                                    key: 'Volume',
                                    src: "docsetting.archive.field"
                                });
                            }

                            a.children.push({
                                text: "文件",
                                type: 'file',
                                fondsNumber: item.Number,
                                archiveKey: a.Key,
                                key: 'File',
                                src: "docsetting.archive.field"
                            });

                            if (a.HasCategory) {
                                a.children.push({
                                    text: "分类表",
                                    type: 'list',
                                    fondsNumber: item.Number,
                                    archiveKey: a.Key,
                                    categorys : a.Categorys,
                                    key: 'Category',
                                    src: "docsetting.archive.category"
                                });
                            }
                        }
                    })

                    item.children = item.Archives;
                    item.text = item.Name;
                    item.type = "cloud"
                    item.state = { 'opened': true };
                    item.src = "docsetting.archive.op";
                    //$scope.archives.push(item);
                })

                $scope.documents = result;
            });
        }

        var createArchiveLibrary = function (fondsNumber) {

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/archive/view/archive-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.archive.maintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: false,
                            archiveInfo: undefined,
                            fondsNumber: fondsNumber
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                loadArchive();
            }, function () {
                //dismissed
            });

        }

        var updateArchiveLibrary = function (archiveInfo) {

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/archive/view/archive-maintain.html',
                size: 'sm',
                controller: "docsetting.controller.archive.maintain",
                resolve: {
                    maintainInfo: function () {
                        return {
                            update: true,
                            archiveInfo: archiveInfo
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                loadArchive();
            }, function () {
                //dismissed
            });
        }

        var removeArchiveLibrary = function (archiveInfo) {

            bootbox.confirm("确定禁用此档案库？", function (result) {
                if (result === true) {
                    docsettingService.archive.remove(archiveInfo.FondsNumber, archiveInfo.Key).then(function () {
                        loadArchive();
                    });
                }
            });
        }

        var visiableArchiveLibrary = function (archiveInfo) {

            docsettingService.archive.visiable(archiveInfo.FondsNumber, archiveInfo.Key).then(function () {
                loadArchive();
            });
        }

        var deleteArchiveLibrary = function (archiveInfo) {
            bootbox.confirm("确定删除此档案库？", function (result) {
                if (result === true) {
                    docsettingService.archive.del(archiveInfo.FondsNumber, archiveInfo.Key).then(function () {
                        loadArchive();
                    });
                }
            });

          
        }

        var categoryMaintain = function (archiveInfo) {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docsetting/archive/view/category.html',
                size: 'md',
                controller: "docsetting.controller.archive.category",
                resolve: {
                    archiveInfo: function () {
                        return archiveInfo;
                    }
                }
            });

            modalInstance.result.then(function (info) {
                loadArchive();
            }, function () {
                //dismissed
            });
        }

        var generateArchiveLibrary = function (node) {
            docsettingService.archive.generate(node.FondsNumber, node.Key).then(function () {
                bootbox.alert("档案库生成成功");
            })
        }

        $scope.treeContextmenu = function (node) {
            var add = {
                "label": "添加档案库",
                "icon": "fa fa-database",
                "action": function (obj) {
                    createArchiveLibrary(node.original.Number);
                },
            };

            var del = {
                "label": "删除档案库",
                "icon": "fa fa-close",
                "action": function (obj) {
                    deleteArchiveLibrary(node.original);
                },
            };

            var remove = {
                "label": "禁用档案库",
                "icon": "fa fa-ban",
                "action": function (obj) {
                    removeArchiveLibrary(node.original);
                },
            };

            var update = {
                "label": "修改档案库",
                "icon": "fa fa-edit",
                "action": function (obj) {
                    updateArchiveLibrary(node.original);
                },
            };

            var visable = {
                "label": "启用档案库",
                "icon": "fa fa-check-circle",
                "action": function (obj) {
                    visiableArchiveLibrary(node.original);
                },
            }

            var generate = {
                "label": "生成档案库",
                "icon": "fa fa-check-circle",
                "action": function (obj) {
                    generateArchiveLibrary(node.original);
                },
            }

            var designCategory = {
                "label": "定义分类表",
                "icon": "fa fa-list",
                "action": function (obj) {
                    $state.go("docsetting.archive.category");
                    //categoryMaintain(node.original)
                },
            }

            //if (a.HasCategory) {
            //    a.children.push({
            //        text: "分类表",
            //        type: 'file',
            //        fondsNumber: item.Number,
            //        archiveKey: a.Key,
            //        key: 'Category'
            //    });
            //}
            if (node.type == "cloud") {
                return { "add": add, };
            } else if (node.type == "database") {
                update.separator_after = true;
                var menu = {
                    "remove": remove,
                    "del" :del,
                    "update": update,
                    "generate": generate,
                };
                if (node.original.HasCategory) {
                    menu.designCategory = designCategory;
                }

               
                return menu;
            } else if (node.type == "disabled") {
                return { "visable": visable, "del": del, };
            }
        }
      
        $scope.changed = function (e, data) {
            
            $scope.setCurrent(data.node.original);
        }

        $scope.setCurrent = function (info) {
            $scope.$safeApply(function () {
                $scope.currentArchive = info;
            })

            $state.go(info.src);
        }

        loadArchive();
       
    });
   
    app.module.controller("docsetting.controller.archive.maintain", function ($scope, docsettingService, $uibModal, $uibModalInstance, maintainInfo) {
        $scope.update = maintainInfo.update;
        $scope.archive = maintainInfo.archiveInfo;

        $scope.$watch("archive.HasProject", function (newval, oldval) {
            if (newval != undefined && newval == true) {
                $scope.archive.HasVolume = true;
            }
        });

        $scope.save = function () {
            if (maintainInfo.update) {
                docsettingService.archive.update($scope.archive).then(function () {
                    $uibModalInstance.close($scope.archive);
                });
            } else {

                docsettingService.archive.check($scope.archive.Name, maintainInfo.fondsNumber).then(function (result) {
                    if (!result) {
                        $scope.archive.FondsNumber = maintainInfo.fondsNumber;
                        docsettingService.archive.add($scope.archive).then(function () {
                            $uibModalInstance.close($scope.archive);
                        });
                    } else {
                        bootbox.alert("档案名称重复");
                    }
                })
            }
        };

        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    });

   

});
