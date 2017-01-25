define([
'apps/system2/docfile/docfile',
'apps/system2/docfile/docfile.controller',
'apps/system2/docfile/docfile.service'], function (app) {

    app.module.controller("docfile.controller.collect", function ($scope,$rootScope, docfileService, $uibModal, uiGridConstants) {

        // 查询条件
        $scope.filter = {
            pagesize: $scope.pageSize,
            pageindex: 1,
            orderby: 'ID',
            status: 0,
            dept: '0'
        };
        
        $scope.depts = [{ key: '0', name: '全部部门' }];

        $scope.$on("UsersExLoaded", function () {
            angular.forEach($rootScope.department, function (d) {
                $scope.depts.push({ key: d.key, name: d.name });
            })
        });

        // 文件数据
        $scope.fileSource = [];

        // 筛选条件
        $scope.conditions = [];

        // 最后一次筛选条件
        $scope.lastConditions = [];

        var loadFileLibrarys = function () {

            var nodeTypes = {
                0: "cloud",
                1: "folder",
                2: "circle"
            }

            var convertTreeNode = function (fileNumber,nodes) {

                angular.forEach(nodes, function (item) {

                    item.text = item.Name;
                    item.type = nodeTypes[item.NodeType];
                    item.state = { 'opened': true };
                    item.fileNumber = item.NodeType == 1 ? item.Number : fileNumber;

                    item.children = convertTreeNode(item.fileNumber,item.Children);
                })

                return nodes;
            }

            docfileService.getFileLibraryList().then(function (result) {

                $scope.documents = convertTreeNode("",result);
            });
        }

        var loadFileFields = function () {
            $scope.fileFileds = [];
            var columns = [];

            docfileService.getFileFields($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber).then(function (result) {
                $scope.fileFileds = result;
                
                columns.push({
                    name: "ROWNUMBER" , displayName: "序号", width: 50, enableColumnMenu: false
                });
                columns.push({
                    name: "FondsNumber", displayName: "全宗号", width: 80, enableColumnMenu: false
                });

                angular.forEach(result, function (item) {
                    var col = {
                        name: "_f" + item.ID, displayName: item.Name, minWidth: 80, enableColumnMenu: false,
                        filter: item.DataType == 3 ? "cellFilter: 'TDate'" : ""
                    };

                    if (item.DataType == 3) {
                        col.cellFilter = 'TDate';
                    } else if (item.DataType == 4) {
                        col.cellFilter = 'enumMap:"'+item.BaseData+'"';
                    }

                    columns.push(col);
                });

                columns.push({
                    name: "CreateUser", displayName: "登记人", width: 100, enableColumnMenu: false,cellFilter : 'enumMap:"user"'
                });

                $scope.fileGridOptions.columnDefs = columns;

                loadFileData();
            });
        }

        // 加载文件数据
        var loadFileData = function (condition) {

            $scope.lastConditions = condition;

            docfileService.getFileData($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber, $scope.filter, condition).then(function (result) {
                $scope.fileSource = result.Source;
                $scope.fileGridOptions.totalItems = result.TotalCount;
            });
        }

        $scope.file_changed = function (e, data) {

            $scope.$safeApply(function () {
                $scope.currentFile = data.node.original;
                if ($scope.currentFile.NodeType > 0) {
                    $scope.filter.nodeid = $scope.currentFile.ID;
                }
                
            })
        }

        $scope.$watch("currentFile", function (newval, oldval) {
            if (newval) {

                $scope.fileSource = [];

                if (newval.NodeType > 0 && (oldval == undefined || newval.FondsNumber != oldval.FondsNumber || newval.fileNumber != oldval.fileNumber)) {
                    loadFileFields();
                } 
            }
        });

        // 表格配置
        $scope.fileGridOptions = {
            data : "fileSource",
            multiSelect: true,
            enableGridMenu: false,
            enableColumnResizing: true,
            paginationPageSizes: $scope.pageSizes,
            paginationPageSize: $scope.pageSize,
            useExternalPagination: true,
            useExternalSorting: false,
            enableRowSelection: true,
            enableRowHeaderSelection: true,
            columnDefs:[],
            rowTemplate: "<div ng-click=\"grid.appScope.gridApi.selection.toggleRowSelection(row.entity)\" ng-dblclick=\"grid.appScope.updateFile(row.entity)\" " +
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
        };

        // 设定数据范围
        $scope.setStatus = function (status) {
            $scope.filter.status = status;
        }

        // 添加文件信息
        $scope.addFile = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/collect/view/file-maintain.html',
                size: 'lg',
                controller: "docfile.controller.collect.fileMaintain",
                resolve: {
                    maintainService: function () {
                        return {
                            update: false,
                            fieldList: $scope.fileFileds,
                            fondsNumber: $scope.currentFile.FondsNumber,
                            fileNumber: $scope.currentFile.fileNumber,
                            NodeID: $scope.currentFile.ID,
                            allCount: $scope.fileSource.length,
                            dept : $scope.filter.dept,
                            addFileInfo: function (info) {
                                $scope.fileSource.push(info);
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (info) {
                $scope.fileSource.push(info);
            }, function () {
                //dismissed
            });
        };

        // 更新文件信息
        $scope.updateFile = function (info) {
           
            $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/collect/view/file-maintain.html',
                size: 'lg',
                controller: "docfile.controller.collect.fileMaintain",
                resolve: {
                    maintainService: function () {
                        return {
                            update: true,
                            fieldList: $scope.fileFileds,
                            fondsNumber: $scope.currentFile.FondsNumber,
                            fileNumber: $scope.currentFile.fileNumber,
                            NodeID: $scope.currentFile.ID,
                            fileInfo: info,
                            allCount : $scope.fileSource.length,
                            get: function (index) {
                                if (index >= 0 && index < $scope.fileSource.length) {
                                    return $scope.fileSource[index];
                                }
                            }
                        };
                    }
                }
            });
        };

        // 删除文件信息
        $scope.deleteFile = function () {

            var rows = $scope.gridApi.selection.getSelectedRows();

            if (rows.length > 0) {

                bootbox.confirm("选中" + rows.length + "行,确认删除？", function (result) {
                    if (result === true) {
                        var ids = rows.map(function (item) { return item.ID }).join(',')

                        docfileService.deleteFileData($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber, ids).then(function (result) {

                            //angular.forEach(rows,function (item) {
                            //    $scope.fileSource.removeObj(item);
                            //});

                            loadFileData();

                        })
                    }
                });

            } else {
                bootbox.alert("未选中行");
            }
        };

        // 文件查询条件
        $scope.addCondition = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/view/field-condition.html',
                size: 'md',
                controller: "docfile.controller.fieldcondition",
                resolve: {
                    conditionService: function () {
                        
                        return {
                           
                            fieldList: $scope.fileFileds,
                            conditions: $scope.conditions,
                            clearConditions: function () {
                                $scope.conditions = [];
                            }
                        };
                    }
                }
            });

            modalInstance.result.then(function (condition) {
                
                loadFileData(condition);

            }, function () {
                //dismissed
            });
        };

        // 表格列的显示
        $scope.displayColumns = function () {
            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/view/column-display.html',
                size: 'md',
                controller: "docfile.controller.collect.columns",
                resolve: {
                    columnService: function () {
                        return {
                            columns: $scope.fileGridOptions.columnDefs,
                        };
                    }
                }
            });

            modalInstance.result.then(function (columns) {
                $scope.fileGridOptions.columnDefs = columns;
                $scope.gridApi.core.notifyDataChange(uiGridConstants.dataChange.COLUMN);
            }, function () {
                //dismissed
            });


        };

        // 批量修改
        $scope.batchUpdate = function () {

            // 更新选择行
            var rows = $scope.gridApi.selection.getSelectedRows();

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/view/batch-update.html',
                size: 'md',
                controller: "docfile.controller.collect.batchupdate",
                resolve: {
                    batchService: function () {
                        return {
                            fieldList: $scope.fileFileds,
                            updateRegion: rows.length > 0 ? 2 : 1 
                        };
                    }
                }
            });

            modalInstance.result.then(function (batchUpdateInfo) {

                if (batchUpdateInfo.UpdateRegion == 1) {
                    // 筛选数据集
                    batchUpdateInfo.Conditions = $scope.lastConditions;

                } else {
                   
                    if(rows.length == 0){
                        bootbox.alert("未选中行，无法更新");
                        return;
                    }else{
                        batchUpdateInfo.UpdateIDs = rows.map(function (item) { return item.ID }).join(',');
                    }
                }
                
                docfileService.batchUpdate($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber, batchUpdateInfo, {nodeid :$scope.currentFile.ID }).then(function () {
                    loadFileData();
                });

            }, function () {
                //dismissed
            });
        };

        // 批量挂接
        $scope.batchUpload = function () {

            // 更新选择行
            var rows = $scope.gridApi.selection.getSelectedRows();

            if (rows.length == 0) {
                bootbox.alert("未选中行，无法批量挂接");
                return;
            }

            var modalInstance = $uibModal.open({
                animation: false,
                templateUrl: 'apps/system2/docfile/view/batch-upload.html',
                size: 'md',
                controller: "docfile.controller.collect.batchupload",
                resolve: {
                    uploadService: function () {
                        return {
                            fondsNumber: $scope.currentFile.FondsNumber,
                            fileNumber: $scope.currentFile.fileNumber,
                            fieldList: $scope.fileFileds,
                            selectedFiles: rows
                        };
                    }
                }
            });
        };

        // 文件数据导出
        $scope.exprotExcel = function () {
            docfileService.exportExcel($scope.currentFile.FondsNumber, $scope.currentFile.fileNumber, $scope.filter, $scope.lastConditions);
        }

        // 监听筛选条件查询数据
        $scope.$watch("filter", function (newval, oldval) {
            if (newval && $scope.fileFileds && $scope.fileFileds.length > 0) {
                loadFileData();
            }
        }, true);

       

        loadFileLibrarys();
    });

    app.module.controller("docfile.controller.collect.fileMaintain", function ($scope,$validation, docfileService, $uibModal, $uibModalInstance, maintainService) {

        $scope.maintainService = maintainService;
        $scope.fileInfo = maintainService.fileInfo;
        $scope.attachIDs = [];

        // 字段重组
        var resetFiels = function () {
            $scope.fields = [];

            var temp = { fields: [] };

            angular.forEach(maintainService.fieldList, function (item) {

                if (maintainService.update) {
                    item.Value = maintainService.fileInfo["_f" + item.ID];
                } else {
                    item.Value = undefined;
                }

                if (temp.fields.length == 2) {
                    $scope.fields.push(temp);

                    temp = { fields: [] };
                }

                temp.fields.push(item);
            });

            if (temp.fields.length > 0) {
                $scope.fields.push(temp);
            }
        }

        // 附件上传完成后回调
        $scope.attachUploaded = function (id, attachChangedCB) {

            if (!maintainService.update) {
                $scope.attachIDs.push(id);
                //$scope.attachChangedCB = attachChangedCB;
            }
        };

        // 保存
        $scope.save = function (close, form) {

            $scope.blockHander.block();

            var newFiles = [];
            var info = {
                FondsNumber: maintainService.fondsNumber,
                CreateUser: $scope.currentUser.Account.ID
            };

            angular.forEach($scope.fields, function (item) {
                angular.forEach(item.fields, function (subitem) {

                    newFiles.push(subitem);

                    info["_f" + subitem.ID] = subitem.Value;

                    if (maintainService.update) {
                        maintainService.fileInfo["_f" + subitem.ID] = subitem.Value;
                    }
                });
            });

            if (maintainService.update) {
                docfileService.updateFileData(maintainService.fondsNumber, maintainService.fileNumber, maintainService.fileInfo.ID, newFiles).then(function (result) {
                    $scope.blockHander.unblock();
                    if (close) {
                        $uibModalInstance.dismiss('cancel');
                    }
                    
                })
            } else {

                var addInfo = {
                    Fields: newFiles,
                    AttachIDs: $scope.attachIDs
                };

                docfileService.addFileData(maintainService.fondsNumber, maintainService.fileNumber, addInfo, maintainService.NodeID, maintainService.dept).then(function (result) {
                    $scope.blockHander.unblock();

                    info.ID = result;
                    info.ROWNUMBER = maintainService.allCount + 1;

                    if (close) {
                        $uibModalInstance.close(info);
                    } else {
                        maintainService.addFileInfo(info);
                        maintainService.allCount++;

                        $validation.reset(form);
                    }
                })
            }
        }

     
        // 第一条
        $scope.first = function () {
            
            maintainService.fileInfo = maintainService.get(0);
            $scope.fileInfo = maintainService.fileInfo;
            resetFiels();
        }

        // 上一条
        $scope.sub = function () {
           
            var index = maintainService.fileInfo.ROWNUMBER - 2;

            maintainService.fileInfo = maintainService.get(index);
            $scope.fileInfo = maintainService.fileInfo;
            resetFiels();
        }

        // 下一条
        $scope.next = function () {
            var index = maintainService.fileInfo.ROWNUMBER ;

            maintainService.fileInfo = maintainService.get(index);
            $scope.fileInfo = maintainService.fileInfo;
            resetFiels();
        }

        // 最后一条
        $scope.last = function () {

            maintainService.fileInfo = maintainService.get(maintainService.allCount - 1);
            $scope.fileInfo = maintainService.fileInfo;
            resetFiels();
        }

        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }

        resetFiels();
    });

    app.module.controller("docfile.controller.collect.columns", function ($scope, columnService, $uibModalInstance) {

        $scope.columns = columnService.columns.map(function (c) {
            if (c.visible == undefined) {
                c.visible = true;
            }
            return c;
        });
        
        $scope.ok = function () {
            $uibModalInstance.close($scope.columns);
        }

        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    })

    app.module.controller("docfile.controller.collect.batchupdate", function ($scope,docfileService, batchService, $uibModalInstance) {

        $scope.fields = batchService.fieldList;
        $scope.info = {};
        $scope.expressionStr = "";
        $scope.result = {
            UpdateRegion: batchService.updateRegion,
            Expressions: []
        };

        // 添加字符串
        $scope.addValue = function () {
            $scope.expressionStr += "[" + $scope.info.Value + "]  ";
            $scope.result.Expressions.push({
                Value: $scope.info.Value
            });
        }

        // 添加数据字段
        $scope.addDataField = function () {
            $scope.expressionStr += "[" + $scope.info.DataField.Name + "]  ";

            $scope.result.Expressions.push({
                Field: $scope.info.DataField
            });
        }

        // 添加自动编号
        $scope.addAutoNumber = function () {

            var a = $scope.info.autoNumber.start.PadLeft($scope.info.autoNumber.length, $scope.info.autoNumber.fill);

            $scope.expressionStr += "[" + a + "]  ";

            $scope.result.Expressions.push({
                IdentityStart: $scope.info.autoNumber.start,
                IdentityLength: $scope.info.autoNumber.length,
                IdentityFill: $scope.info.autoNumber.fill,
            });
        }

        // 清除
        $scope.clear = function () {
            $scope.expressionStr = "";
            $scope.result.Expressions = [];
        }

        // 返回
        $scope.ok = function () {
            $uibModalInstance.close($scope.result);
        }

        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    })

    app.module.controller("docfile.controller.collect.batchupload", function ($scope, docfileService,attachService, uploadService, $uibModalInstance) {

        $scope.info = {};
        $scope.currentStep = 1;
        $scope.expressionStr = "";
        $scope.fields = uploadService.fieldList;
        $scope.result = {
            FieldRegion: 1,
            FileMatch : 1,
            Expressions : []
        }
        $scope.upAttachs = []; // 上传文件列表
        $scope.myPLInstance = {}; // 上传控件 Api
        $scope.percent = 0; // 上传总进度
        $scope.allFileCount = 0;    // 选择的文件总数

        // 添加字符串
        $scope.addValue = function () {
            $scope.expressionStr += "[" + $scope.info.Value + "]  ";
            $scope.result.Expressions.push({
                Value: $scope.info.Value
            });

        }

        // 添加数据字段
        $scope.addDataField = function () {

            if ($scope.result.FieldRegion == 1) {
                $scope.expressionStr += "[" + $scope.info.DataField.Name + "]  ";

                $scope.result.Expressions.push({
                    Field: $scope.info.DataField
                });
            } else {
                $scope.expressionStr += "[" + $scope.info.DataField.Name + "(" + $scope.info.field.start + "," + $scope.info.field.length + ")]  ";

                $scope.result.Expressions.push({
                    Field: $scope.info.DataField,
                    FieldLength: $scope.info.field.length,
                    FieldStart: $scope.info.field.start
                });
            }
        }

        // 添加自动编号
        $scope.addAutoNumber = function () {

            var a = $scope.info.autoNumber.start.PadLeft($scope.info.autoNumber.length, $scope.info.autoNumber.fill);

            $scope.expressionStr += "[" + a + "]  ";

            $scope.result.Expressions.push({
                IdentityStart: $scope.info.autoNumber.start,
                IdentityLength: $scope.info.autoNumber.length,
                IdentityFill: $scope.info.autoNumber.fill,
            });
        }

        // 清除
        $scope.clear = function () {
            $scope.expressionStr = "";
            $scope.result.Expressions = [];
        }

        // 清空上传文件
        $scope.clearUpFiles = function () {
            $scope.percent = 0;
            $scope.allFileCount = 0;
            $scope.myPLInstance.files = [];
            $scope.upAttachs = [];
        }

        // 选择文件后过滤不匹配的文件
        $scope.fileAdded = function () {

            $scope.allFileCount = $scope.upAttachs.length;

            var exFiles = uploadService.selectedFiles.map(function (item) {
                var exNames = getExpression(item);
                return {
                    entity: item,
                    names: exNames
                }
            });

            $scope.upAttachs = $scope.upAttachs.where(function (a) {
                var noExtensionName = a.name.removeFileExtension();

                var rs = exFiles.where(function (e) {

                    return e.names.contains(function (n) {

                        if ($scope.result.FileMatch == 1) {
                            // 完全匹配
                            return noExtensionName === n;

                        } else if ($scope.result.FileMatch == 2) {
                            // 前缀匹配
                            return noExtensionName.startWith(n);
                        } else {
                            // 包含匹配
                            return noExtensionName.indexOf(n, 0) >= 0;
                        }
                    });
                });

                if (rs && rs.length > 0) {
                    a.FileEntitys = rs.map(function (i) { return i.entity; });
                    return true;
                } else {
                    $scope.myPLInstance.removeFile(a);
                    return false;
                }
            });
        }

        // 上传完毕后回调
        $scope.uploaded = function (result, file) {

            var fileID = result.files[0].id;
            file.Done = 0;
            angular.forEach(file.FileEntitys, function (entity) {

                attachService.object.addAttach('FileDoc_' + uploadService.fondsNumber + '_' + uploadService.fileNumber, entity.ID, fileID).then(function () {
                    file.Done++;
                });
            });
           
        }

        // 根据表达式计算文件名
        var getExpression = function(fileInfo){
            var result = "";
            var index = 0;
            var identitys = [];
            var names = [];

            angular.forEach($scope.result.Expressions,function(item){
                if(item.Value){
                    result += item.Value;
                }else if(item.Field){
                    
                    var fileValue = fileInfo["_f"+item.Field.ID].toString();

                    if(item.Field.DataType == 3){
                        fileValue = fileValue.toTDate('yyyy/MM/dd');
                    }else if(item.Field.DataType == 4){
                        fileValue = $scope.getBaseEnum(item.Field.BaseData)[fileValue];
                    }

                    if(item.FieldLength){
                        result += fileValue.substr(item.FieldStart, item.FieldLength);
                    }else{
                        result += fileValue;
                    }

                } else if (item.IdentityLength) {
                    result += "{" + index + "}";
                    identitys.push(item);
                    index++;
                }
            });

            if (index > 0) {

                for (var j = 0; j < 10; j++) {

                    var args = [];

                    for (var i = 0; i < index; i++) {
                        var iden = identitys[i];
                        args.push((iden.IdentityStart.toInt() + j).toString().PadLeft(iden.IdentityLength, iden.IdentityFill));
                    }
                    
                    names.push(result.format(args));
                }

                return names;
            }

            return [result];
        }

        // 上一步
        $scope.subStep = function () {
            $scope.currentStep--
        }

        // 下一步
        $scope.nextStep = function () {
            $scope.currentStep++;
        }

        // 开始上传，挂接
        $scope.startUpload = function () {
            $scope.myPLInstance.start();
        }

        // 关闭
        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    })
});
