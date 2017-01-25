define(['apps/system3/document/document', 'apps/system3/document/document.service'], function (app) {

    app.module.controller("document.controller.query", function ($scope, documentService, attachService, tagService, imagePreviewUrl, attachDownloadUrl) {

        $scope.filter = {};
        $scope.currentNode = undefined;
        $scope.imagePreviewUrl = imagePreviewUrl;
        $scope.attachDownloadUrl = attachDownloadUrl;
        $scope.upFiles = [];

        //$scope.$watch("$viewContentLoaded", function () {
        //    console.log("2")
        //    $scope.load();
        //});

        $scope.$on("$document_query", function (e, filter) {
            $scope.filter = filter;
        });

        $scope.$watch("filter", function (newval, oldval) {
            $scope.load();
        },true);

        //加载对象树
        $scope.load = function () {

            $scope.treePanel.block();
            $scope.upFiles = [];
            $scope.files = [];
            $scope.currentNode = undefined;
            //$scope.filter.deep = 0;
            documentService.getTree($scope.filter).then(function (source) {
                $scope.documents = paraseTreeData(source);
                $scope.treePanel.unblock();
            });

            // 将服务端数据转换为界面tree识别的数据
            var paraseTreeData = function (nodes) {
                var newNodes = [];
                angular.forEach(nodes, function (node) {
                    var newNode = {};
                    newNode.text = node.ObjectText;

                    switch (node.ObjectKey) {
                        case "Volume": { newNode.type = "file"; newNode.text += node.IsDone ? "" : "<span style='color:red;'>【未完成】</span>"; break; }
                        case "VolumeCheck": { newNode.type = "file"; break; }
                        case "Specialty": { newNode.type = "star"; break; }
                        case "Change": { newNode.type = "edit"; break; }

                        default: newNode.type = "folder";
                    }
                    
                    newNode.state = { 'opened': true };
                    newNode.children = paraseTreeData(node.Children);

                    newNodes.push($.extend(newNode, node));
                });
                return newNodes;
            }
        }

        // 加载文件
        $scope.loadFiles = function () {
            $scope.filePanel.block();
            $scope.upFiles = [];
            attachService.getObjAttachs($scope.currentNode.ObjectKey, $scope.currentNode.ObjectID.split('_')[1], true).then(function (result) {
                $scope.files = result;
                $scope.filePanel.unblock();
            });
        }

        $scope.nodeChange = function (e, data) {
            $scope.$safeApply(function () {
                $scope.currentNode = data.node.original;
            });
        }

        $scope.treeReady = function () {
            $scope.treeScroll.init()
        }

        $scope.$watch("currentNode", function (newval, oldval) {

            if (newval) {
                $scope.loadFiles();
            }
        });

        // 获取选中文件的ID
        $scope.getSelecedIDs = function () {
            var fs = [];

            angular.forEach($scope.files, function (f) {
                if (f.selected) {
                    fs.push(f.ID);
                }
            });

            return fs.join(',');
        }

        // 附件上传完成后的回调
        $scope.uploaded = function (result, file) {

            if (result.files && result.files.length > 0) {

                file.id = result.files[0].id;

                attachService.object.addAttach($scope.currentNode.ObjectKey, $scope.currentNode.ObjectID.split('_')[1], file.id);
            }
        };

        // 删除文件
        $scope.removeFiles = function () {
            
            var ids = $scope.getSelecedIDs();
            
            attachService.object.removeAttachs(ids, $scope.currentNode.ObjectKey).then(function () {
                $scope.loadFiles();
            });
        }
    });
});
