define(['apps/system2/book/book',
    'apps/system2/book/book.service'], function (app) {

        app.module.controller("book.controller.maintain", function ($scope, $uibModal, $filter, bookService) {

            $scope.bookInfo = {Style : 1};

            $scope.bookStyles = [{ Value: "1", Text: '样式一' }, { Value: "2", Text: '样式二' }, { Value: "3", Text: '样式三' }]

            $scope.bookCategorys = [{
                text : '全分类',
                type : 'folder',
                state: { opened: true }
            }];

            $scope.save = function () {

                bookService.createBook($scope.bookInfo).then(function (id) {
                    bootbox.alert("新建图书成功");
                });
            }

            $scope.treeContextmenu = function (node) {

                return {
                    "m1": {
                        "label": "添加分类",
                        "icon": "fa fa-plus",
                        "_disabled": node.original.Number == undefined,
                        "action": function (obj) {

                            addCategoryModal(node.original.ParentFullKey);
                        },
                    },
                    "m2": {
                        "label": "添加子分类",
                        "icon": "fa fa-plus",
                        "action": function (obj) {
                            var parent = "";

                            if (node.original.Number) {
                                if (node.original.ParentFullKey) {
                                    parent = node.original.ParentFullKey + "." + node.original.Number;
                                } else {
                                    parent = node.original.Number;
                                }
                            }

                            addCategoryModal(parent);
                        },
                    },
                    "m3": {
                        "label": "重命名",
                        "separator_before": true,
                        "_disabled": node.original.Number == undefined,
                        "icon": "fa fa-edit",
                        "action": function (obj) {
                            editCategoryModal(node.original);
                        },
                    },
                    "m4": {
                        "label": "删除节点",
                        "icon": "fa fa-trash",
                        "_disabled": node.original.Number == undefined,
                        "action": function (obj) {

                            var parent = "";

                            if (node.original.Number) {
                                if (node.original.ParentFullKey) {
                                    parent = node.original.ParentFullKey + "." + node.original.Number;
                                } else {
                                    parent = node.original.Number;
                                }
                            }

                            deleteCategory(parent);
                        },
                    },
                }
            }

            $scope.bookCategoryChanges = function (e, data) {
                $scope.$safeApply(function () {
                    var parent = "";

                    if (data.node.original.Number) {
                        if (data.node.original.ParentFullKey) {
                            parent = data.node.original.ParentFullKey + "." + data.node.original.Number;
                        } else {
                            parent = data.node.original.Number;
                        }
                    }
                    $scope.bookInfo.Category = parent;
                })
            }

            var loadBookCategorys = function () {

                var loadTreeNode = function (categorys, parents) {

                    angular.forEach(categorys, function (a) {

                        a.text = "<span class='c-blue m-r-5'>[" + a.Number + "]</span>" + a.Name;
                        a.type = 'folder';
                        a.state = { opened: true };

                        a.ParentFullKey = parents.join('.');

                        if (a.Children && a.Children.length > 0) {
                            
                            var ps = parents.concat();
                            ps.push(a.Number);

                            loadTreeNode(a.Children, ps);
                        } 

                        a.children = a.Children;
                    });
                }

                bookService.getBookCategorys().then(function (datas) {

                    loadTreeNode(datas, []);

                    $scope.bookCategorys[0].children = datas;
                });
            }

            var addCategoryModal = function (parent) {

                $uibModal.open({
                    animation: false,
                    templateUrl: 'apps/system2/book/maintain/view/category.html',
                    size: 'sm',
                    controller: "book.controller.category",
                    resolve: {
                        maintainInfo: function () {
                            return {
                                update: false,
                                info: {
                                    Parent: parent
                                }
                            };
                        }
                    }
                }).result.then(function (info) {
                    loadBookCategorys();
                }, function () {
                    //dismissed
                });

            }

            var editCategoryModal = function (category) {

                $uibModal.open({
                    animation: false,
                    templateUrl: 'apps/system2/book/maintain/view/category.html',
                    size: 'sm',
                    controller: "book.controller.category",
                    resolve: {
                        maintainInfo: function () {
                            return {
                                update: true,
                                info: {
                                    Parent: category.ParentFullKey,
                                    Number: category.Number,
                                    Name: category.Name
                                },
                                oldNumber: category.Number
                            };
                        }
                    }
                }).result.then(function (info) {
                    loadBookCategorys();
                }, function () {
                    //dismissed
                });

            }

            var deleteCategory = function (key) {

                bootbox.confirm("确认删除？", function (result) {
                    if (result === true) {
                        bookService.deleteBookCategory(key).then(function () {
                            loadBookCategorys();
                        })
                    }
                });
            }
            loadBookCategorys();
        });

        app.module.controller("book.controller.category", function ($scope, $uibModalInstance, $filter, maintainInfo, bookService) {

            $scope.categoryInfo = maintainInfo.info;

            $scope.save = function () {
                if (maintainInfo.update) {

                    bookService.editBookCategory(maintainInfo.oldNumber, $scope.categoryInfo).then(function () {
                        $uibModalInstance.close($scope.categoryInfo);
                    })
                } else {
                  
                    bookService.addBookCategory($scope.categoryInfo).then(function () {
                        $uibModalInstance.close($scope.categoryInfo);
                    })
                }
            }

            // 关闭
            $scope.close = function () {
                $uibModalInstance.dismiss('cancel');
            }

        });
    });
