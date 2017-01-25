define([
'apps/system2/docsetting/docsetting',
'apps/system2/docsetting/docsetting.service'], function (app) {

    app.module.controller("docsetting.controller.archive.category", function ($scope, docsettingService, $uibModal) {

        $scope.categoryChanged = function (e, data) {
            $scope.$safeApply(function () {
                $scope.currentNode = data.node;

                $scope.currentCategory = data.node.original;

                $scope.currentCategoryNumber = data.node.original.Number;
            })
        }

        //$scope.categorys = $scope.currentArchive.Categorys;

        $scope.$watch("currentArchive", function (newval, oldval) {

            if (newval) {
                loadTreeNode(newval.categorys, []);

                $scope.categorys = newval.categorys;
            }
        });

        var loadTreeNode = function (categorys,parents) {

            angular.forEach(categorys, function (a) {
                
                a.text = "<span class='c-blue m-r-5'>[" + a.Number + "]</span>" + a.Name;
                a.type = 'folder';
                a.state = { opened: true };
                a.ParentFullKey = parents.join('.');
                parents.push(a.Number);

                loadTreeNode(a.Children, parents);

                a.children = a.Children;
            });
        }

        $scope.add1 = function () {

            var parent = null;

            if ($scope.currentNode) {

                var parent = $scope.currentNode.parent;

                $scope.categoryInfo.Parent = $scope.currentCategory.ParentFullKey;
            }
            
            docsettingService.archive.addCategory($scope.currentArchive.fondsNumber, $scope.currentArchive.archiveKey, $scope.categoryInfo).then(function () {

                $scope.categoryTreeApi.create_node(parent, {
                    text: "<span class='c-blue m-r-5'>[" + $scope.categoryInfo.Number + "]</span>" + $scope.categoryInfo.Name,
                    type: 'folder',
                    state: { opened: true },

                    Number: $scope.categoryInfo.Number,
                    Name: $scope.categoryInfo.Name
                });
            });
        }

        $scope.add2 = function () {

            if ($scope.currentNode) {

                $scope.categoryInfo.Parent = $scope.currentCategory.ParentFullKey + "." + $scope.currentCategory.Number;

                docsettingService.archive.addCategory($scope.currentArchive.fondsNumber, $scope.currentArchive.archiveKey, $scope.categoryInfo).then(function () {

                    $scope.categoryTreeApi.create_node($scope.currentNode, {
                        text: "<span class='c-blue m-r-5'>[" + $scope.categoryInfo.Number + "]</span>" + $scope.categoryInfo.Name,
                        type: 'folder',
                        state: { opened: true },

                        Number: $scope.categoryInfo.Number,
                        Name: $scope.categoryInfo.Name
                    });
                });
            }
        }

        $scope.update = function () {
           
            if ($scope.currentNode) {

                $scope.currentCategory.Parent = $scope.currentCategory.ParentFullKey;

                docsettingService.archive.updateCategory($scope.currentArchive.fondsNumber, $scope.currentArchive.archiveKey, $scope.currentCategoryNumber, $scope.currentCategory).then(function () {

                    var text = "<span class='c-blue m-r-5'>[" + $scope.currentCategory.Number + "]</span>" + $scope.currentCategory.Name;
                    $scope.categoryTreeApi.rename_node($scope.currentNode, text);
                    
                });
            }
        }

        $scope.delete = function () {

            if ($scope.currentNode) {

                var key = $scope.currentCategory.ParentFullKey + "." + $scope.currentCategory.Number;

                docsettingService.archive.deleteCategory($scope.currentArchive.fondsNumber, $scope.currentArchive.archiveKey, key).then(function () {
                    $scope.categoryTreeApi.delete_node($scope.currentNode);
                });
            }
        }
    });

});
