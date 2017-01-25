define(['apps/base/base.directive', 'plupload'],
    function (app) {

        app.provider('plUploadService', function (attachUpload) {

            var config = {
                flashPath: 'bower_components/plupload-angular-directive/dist/plupload.flash.swf',
                silverLightPath: 'bower_components/plupload-angular-directive/dist/plupload.silverlight.xap',
                uploadPath: attachUpload
            };

            this.setConfig = function (key, val) {
                config[key] = val;
            };

            this.getConfig = function (key) {
                return config[key];
            };

            var that = this;

            this.$get = [function () {

                return {
                    getConfig: that.getConfig,
                    setConfig: that.setConfig
                };

            }];

        });

        app.directive('plUpload', ['$rootScope', '$parse', 'plUploadService', function ($rootScope, $parse, plUploadService) {
            return {
                restrict: 'A',
                scope: {
                    'plProgressModel': '=',
                    'plFilesModel': '=',
                    'plFiltersModel': '=',
                    'plMultiParamsModel': '=',
                    'plInstance': '='
                },
                link: function (scope, iElement, iAttrs) {

                    scope.randomString = function (len, charSet) {
                        charSet = charSet || 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
                        var randomString = '';
                        for (var i = 0; i < len; i++) {
                            var randomPoz = Math.floor(Math.random() * charSet.length);
                            randomString += charSet.substring(randomPoz, randomPoz + 1);
                        }
                        return randomString;
                    }

                    if (!iAttrs.id) {
                        var randomValue = scope.randomString(5);
                        iAttrs.$set('id', randomValue);
                    }
                    if (!iAttrs.plAutoUpload) {
                        iAttrs.$set('plAutoUpload', 'true');
                    }
                    if (!iAttrs.plMaxFileSize) {
                        iAttrs.$set('plMaxFileSize', '1000mb');
                    }
                    if (!iAttrs.plUrl) {
                        iAttrs.$set('plUrl', plUploadService.getConfig('uploadPath'));
                    }
                    if (!iAttrs.plFlashSwfUrl) {
                        iAttrs.$set('plFlashSwfUrl', plUploadService.getConfig('flashPath'));
                    }
                    if (!iAttrs.plSilverlightXapUrl) {
                        iAttrs.$set('plSilverlightXapUrl', plUploadService.getConfig('silverLightPath'));
                    }
                    if (typeof scope.plFiltersModel == "undefined") {
                        scope.filters = [
                            { title: "图片", extensions: "jpg,jpeg,gif,png,tiff,pdf" },
                            { title: "Office文档", extensions: "doc,docx,xls,xlsx,ppt,pptx" },
                            { title: "CAD", extensions: "dwg" },
                            { title: "其他", extensions: "txt,zip,rar" }];
                    } else {
                        scope.filters = scope.plFiltersModel;
                    }


                    var options = {
                        runtimes: 'html5,flash,silverlight',
                        browse_button: iAttrs.id,
                        drop_element: iAttrs.dropElement,
                        multi_selection: true,
                        //		container : 'abc',
                        max_file_size: iAttrs.plMaxFileSize,
                        url: iAttrs.plUrl,
                        flash_swf_url: iAttrs.plFlashSwfUrl,
                        silverlight_xap_url: iAttrs.plSilverlightXapUrl,
                        filters: scope.filters
                    }


                    if (scope.plMultiParamsModel) {
                        options.multipart_params = scope.plMultiParamsModel;
                    }


                    var uploader = new plupload.Uploader(options);

                    uploader.settings.headers = plUploadService.getConfig('headers');

                    uploader.init();

                    uploader.bind('Error', function (up, err) {
                        if (iAttrs.onFileError) {
                            scope.$parent.$apply(onFileError);
                        }

                        alert("Cannot upload, error: " + err.message + (err.file ? ", File: " + err.file.name : "") + "");
                        up.refresh(); // Reposition Flash/Silverlight
                    });

                    uploader.bind('FilesAdded', function (up, files) {

                        //uploader.start();
                        scope.$apply(function () {
                            if (iAttrs.plFilesModel) {
                                angular.forEach(files, function (file, key) {
                                    scope.plFilesModel.push(file);
                                });
                            }

                            if (iAttrs.onFileAdded) {
                                scope.$parent.$eval(iAttrs.onFileAdded);
                            }
                        });

                        if (iAttrs.plAutoUpload == "true") {
                            uploader.start();
                        }
                    });

                    uploader.bind('BeforeUpload', function (up, file) {

                        up.settings.headers = up.settings.headers || {};

                        up.settings.headers.Authorization = $rootScope.currentUser.Token;

                        //up.setOption("multipart_params", { "modification-date": file.lastModifiedDate });

                        if (iAttrs.onBeforeUpload) {

                            var fn = $parse(iAttrs.onBeforeUpload);
                            scope.$apply(function () {
                                fn(scope.$parent, { $file: file });
                            });
                        }
                    });

                    uploader.bind('FileUploaded', function (up, file, res) {
                        var result = JSON.parse(res.response);

                        if (iAttrs.onFileUploaded) {

                            //if (iAttrs.plFilesModel) {
                            //    scope.$apply(function () {
                            //        angular.forEach(scope.plFilesModel, function (file, key) {
                            //            scope.allUploaded = false;
                            //            if (file.percent == 100)
                            //                scope.allUploaded = true;
                            //        });

                            //        if (scope.allUploaded) {

                            //            var fn = $parse(iAttrs.onFileUploaded);
                            //            fn(scope.$parent, { $response: res });
                            //        }

                            //    });
                            //} else {
                            var fn = $parse(iAttrs.onFileUploaded);
                            scope.$apply(function () {
                                fn(scope.$parent, { $response: result, $file: file });
                            });
                            //}
                            //scope.$parent.$apply(iAttrs.onFileUploaded);
                        }
                    });

                    uploader.bind('UploadProgress', function (up, file) {
                        if (!iAttrs.plProgressModel) {
                            return;
                        }

                        if (iAttrs.plFilesModel) {
                            scope.$apply(function () {
                                scope.sum = 0;

                                angular.forEach(scope.plFilesModel, function (file, key) {
                                    scope.sum = scope.sum + file.percent;
                                });

                                scope.plProgressModel = scope.sum / scope.plFilesModel.length;
                            });
                        } else {
                            scope.$apply(function () {
                                scope.plProgressModel = file.percent;
                            });
                        }


                        if (iAttrs.onFileProgress) {
                            scope.$parent.$eval(iAttrs.onFileProgress);
                        }
                    });

                    if (iAttrs.plInstance) {
                        scope.plInstance = uploader;
                    }

                    // scope.upload = function(){
                    // 	uploader.start();
                    // };
                }
            };
        }]);

        app.directive('imagePreview', function () {

            return {
                restrict: 'E',
                link: function (scope, iElement, iAttrs) {

                    var image = $(new Image()).css({ 'width': '100%', 'height': iAttrs.height }).appendTo(iElement);


                    var preloader = new mOxie.Image();

                    preloader.onload = function () {

                        //preloader.downsize(200, 700);

                        image.prop("src", preloader.getAsDataURL());

                    };

                    preloader.load(scope[iAttrs.model].getSource());
                }
            };
        });
       
        app.directive('attachTypeSetter', function (attachTypeService) {
            return {
                restrict: 'A',
                link: function ($scope, element, $attrs) {
                    var attachType = $scope[$attrs.attachTypeSetter].type;
                    var info = attachTypeService.getType(attachType);

                    $scope[$attrs.attachTypeSetter].typeID = info.typeID;
                    $scope[$attrs.attachTypeSetter].typeName = info.typeName;
                }
            };
        });

        app.factory('attachTypeService', function () {
            return {
                getType: function (attachType) {
                    var info = {};

                    if (attachType.indexOf('image') > -1) {
                        info.typeID = 1;
                        info.typeName = '图片';
                    } else if (attachType.indexOf('wordprocessingml') > -1) {
                        info.typeID = 2;
                        info.typeName = 'Office World';
                    } else if (attachType.indexOf('sheet') > -1) {
                        info.typeID = 6;
                        info.typeName = 'Office Excel';
                    } else if (attachType.indexOf('presentationml') > -1) {
                        info.typeID = 7;
                        info.typeName = 'Office PPT';
                    } else if (attachType.indexOf('dwg') > -1) {
                        info.typeID = 3;
                        info.typeName = 'CAD设计文件';
                    } else if (attachType.indexOf('application/pdf') > -1) {
                        info.typeID = 10;
                        info.typeName = 'PDF';
                    } else if (attachType.indexOf('application/rar') > -1) {
                        info.typeID = 4;
                        info.typeName = 'Rar压缩文件';
                    } else if (attachType.indexOf('application/zip') > -1) {
                        info.typeID = 9;
                        info.typeName = 'Zip压缩文件';
                    } else if (attachType.indexOf('text/plain') > -1) {
                        info.typeID = 8;
                        info.typeName = '记事本';
                    } else {
                        info.typeID = 5;
                        info.typeName = '未知';
                    }

                    return info;
                }
            }
        });

        // 附件上传控件
        app.directive('attachUpload', function () {

            return {
                restrict: 'E',
                templateUrl: 'apps/base/view/attach-upload.html',
                scope: {
                    'attachIds': '=?',
                    'objId': '=?',
                    'attachFilter': '=?',
                    'objName': '@',
                    'name': '@?',
                    'plFiltersModel' : '=?',
                    'attachUploaded': '&uploaded',
                    'beforeUpload': '&beforeUpload'
                },
                controller: function ($scope, $rootScope, $parse, localService,
                    attachService, tagService, $uibTooltip,$timeout,
                    imagePreviewUrl, attachDownloadUrl, officePreviewUrl) {

                    $scope.local = localService.local;
                    $scope.imagePreviewUrl = imagePreviewUrl;
                    $scope.officePreviewUrl = officePreviewUrl;
                    $scope.attachDownloadUrl = attachDownloadUrl;
                    $scope.upAttachs = [];
                    $scope.selectedAttachs = [];
                    $scope.tagPopover = {
                        templateUrl: 'myPopoverTemplate.html',
                        tags: []
                    };

                    $scope.$parent[$scope.name] = {
                        clear: function () {
                            $scope.attachIds = [];
                        }
                    };

                    // 监听绑定的附件IDs，有变化时加载附件信息
                    $scope.$watch("attachIds", function (newval, oldval) {
                        if (newval && newval.length > 0) {
                            $scope.attachFiles = attachService.getAttachs(newval.join(','), true).$object;
                        } else {
                            $scope.attachFiles = [];
                        }
                        $scope.selectedAttachs = [];
                        $scope.upAttachs = [];
                    });

                    // 监听绑定的业务对象ID，有变化时加载附件信息
                    $scope.$watch("objId", function (newval, oldval) {
                        if (newval) {
                            attachService.object.getAttachIDs($scope.objName, newval).then(function (result) {
                                $scope.attachIds = result;
                            });
                        }

                        $scope.attachIds = [];
                    });

                    // 监听选中的附件，当只选中一个附件的时候，设置标签编辑的值
                    $scope.$watch("selectedAttachs", function (newval, oldval) {
                        if (newval && newval.length == 1) {
                            $scope.tagPopover.tags = newval[0].Tags;
                        }
                    }, true);

                    //  选择附件
                    $scope.attachSelected = function (attach) {
                        attach.selected = !attach.selected;

                        if (attach.selected) {
                            $scope.canHidePop = true;

                            $scope.selectedAttachs.push(attach);
                        } else {
                            $scope.selectedAttachs.removeObj(attach);
                        }
                    };

                    // 清除选择项
                    $scope.clearSelected = function () {
                        $scope.selectedAttachs = [];
                    }

                    // 获取选中附件的IDs
                    $scope.getSelecedIDs = function () {
                        var ids = [];
                        angular.forEach($scope.selectedAttachs, function (a) {
                            ids.push(a.ID);
                        });

                        return ids.join(',');
                    };

                    // 全选
                    $scope.selectAll = function () {
                        angular.forEach($scope.upAttachs, function (a) {
                            if (!a.selected) {
                                a.selected = true;
                                $scope.selectedAttachs.push(a);
                            }
                        });
                        angular.forEach($scope.attachFiles, function (a) {
                            if (!a.selected) {
                                a.selected = true;
                                $scope.selectedAttachs.push(a);
                            }
                        });
                    }

                    // 反选
                    $scope.unSelectAll = function () {

                        angular.forEach($scope.upAttachs, function (a) {
                            $scope.attachSelected(a);
                        });
                        angular.forEach($scope.attachFiles, function (a) {
                            $scope.attachSelected(a);
                        });
                    }

                    // 删除附件
                    $scope.removeAttach = function () {
                        var ids = [];

                        angular.forEach($scope.selectedAttachs, function (a) {
                            ids.push(a.ID);
                        });

                        attachService.object.removeAttachs(ids.join(','), $scope.objName).then(function () {

                            angular.forEach($scope.selectedAttachs, function (a) {
                                $scope.upAttachs.removeObj(a);
                                $scope.attachFiles.removeObj(a);
                                $scope.attachIds.removeObj(a.ID);

                                // 通知预览区域更新
                                $rootScope.$broadcast("$attachChanged", $scope.attachIds);
                            });
                            $scope.selectedAttachs = [];
                        });
                    };

                    // 保存附件的标签
                    $scope.saveTags = function () {
                        tagService.object.saveTags("Attach", $scope.selectedAttachs[0].ID, $scope.tagPopover.tags).then(function () {

                            $scope.attachFiles = attachService.getAttachs($scope.attachIds.join(','), true).$object;
                            $scope.selectedAttachs = [];
                            $scope.upAttachs = [];
                        });
                    }

                    $scope.filterTags = function (filter) {
                        return tagService.getTags(filter, 'Attach');
                    }

                   

                    // 附件上传完成后的回调
                    $scope.uploaded = function (result, file) {

                        if (result.files && result.files.length > 0) {

                            file.ID = result.files[0].id;
                            $scope.attachIds.push(file.ID);

                            // 指定了业务对象的主键，则自动添加附件
                            if ($scope.objId > 0) {
                                attachService.object.addAttach($scope.objName, $scope.objId, file.ID).then(function () {
                                    // 通知预览区域更新
                                    $rootScope.$broadcast("$attachChanged", $scope.attachIds);
                                });
                            } else {
                               
                                // 没有指定业务对象主键ID，标记等待业务主键，当设置了业务主键自动把附件绑定到业务主键上
                                $scope.$watch("objId", function (newval, oldval) {
                                    
                                    if (newval > 0) {
                                       
                                        attachService.object.addAttach($scope.objName, newval, file.ID).then(function () {
                                            // 通知预览区域更新
                                            $rootScope.$broadcast("$attachChanged", $scope.attachIds);
                                        });
                                    }

                                });
                            }

                            // 附件上传完成通知
                            $scope.attachUploaded({
                                $attachID: file.ID,
                                $changeCB: function () {
                                    // 通知预览区域更新
                                    $rootScope.$broadcast("$attachChanged", $scope.attachIds);
                                }
                            });
                        }
                    };

                    $scope.openChat = function (userID) {
                        $rootScope.openChat(userID)
                    };

                    $scope.canHidePop = true;

                    $scope.showPop = function (attach) {
                        attach.isOpen = true
                    }

                    $scope.hidePop = function (attach) {
                        $timeout(function () {
                            attach.isOpen = !$scope.canHidePop
                        }, 100)
                       
                    }
                    $scope.stopHide = function () {
                        $scope.canHidePop = false;
                    }

                    $scope.beginHide = function (attach) {
                        $scope.canHidePop = true;
                        attach.isOpen = false;
                    }
                },
                link: function (scope, iElement, iAttrs) {

                }
            };
        });

        // 附件预览控件
        app.directive('attachPreview', function ($timeout,attachService, imagePreviewUrl, attachDownloadUrl) {

            return {
                restrict: 'E',
                templateUrl: 'apps/base/view/attach-preview.html',
                scope: {
                    'attachIds': '=?',
                    'objId': '=?',
                    'attachFilter': '=?',
                    'objName': '@'
                },
                controller: function ($scope, $rootScope, localService, officePreviewUrl) {
                    $scope.local = localService.local;
                    $scope.imagePreviewUrl = imagePreviewUrl;
                    $scope.officePreviewUrl = officePreviewUrl;
                    $scope.attachDownloadUrl = attachDownloadUrl;
                    $scope.selectedAttachs = [];
                    

                    // 监听绑定的附件IDs，有变化时加载附件信息
                    $scope.$watch("attachIds", function (newval, oldval) {

                        if (newval && newval.length > 0) {
                            $scope.attachFiles = attachService.getAttachs(newval.join(','), true).$object;
                        } else {
                            $scope.attachFiles = [];
                        }
                        $scope.selectedAttachs = [];
                    }, true);

                    // 对象的附件增加或者删除的通知
                    $scope.$on("$attachChanged", function (e, ids) {
                        $scope.attachIds = ids;
                    });

                    // 监听绑定的业务对象ID，有变化时加载附件信息
                    $scope.$watch("objId", function (newval, oldval) {
                        if (newval) {
                            attachService.object.getAttachIDs($scope.objName, newval).then(function (result) {
                                $scope.attachIds = result;
                            });
                        }
                    });

                    //  选择附件
                    $scope.attachSelected = function (attach) {
                        $scope.myPopover.open();
                        attach.selected = !attach.selected;

                        if (attach.selected) {
                            $scope.selectedAttachs.push(attach);
                        } else {
                            $scope.selectedAttachs.removeObj(attach);
                        }
                    };

                    $scope.getSelecedIDs = function () {
                        var ids = [];
                        angular.forEach($scope.selectedAttachs, function (a) {
                            ids.push(a.ID);
                        });

                        return ids.join(',');
                    };

                    // 全选
                    $scope.selectAll = function () {
                        angular.forEach($scope.attachFiles, function (a) {



                            if (!a.selected) {
                                a.selected = true;
                                $scope.selectedAttachs.push(a);
                            }
                        });
                    };

                    // 反选
                    $scope.unSelectAll = function () {
                        angular.forEach($scope.attachFiles, function (a) {
                            $scope.attachSelected(a);
                        });
                    };

                    $scope.openChat = function (userID) {
                        $rootScope.openChat(userID)
                    };

                    $scope.canHidePop = true;

                    $scope.showPop = function (attach) {
                        attach.isOpen = true
                    }

                    $scope.hidePop = function (attach) {
                        $timeout(function () {
                            attach.isOpen = !$scope.canHidePop
                        }, 100)

                    }
                    $scope.stopHide = function () {
                        $scope.canHidePop = false;
                    }

                    $scope.beginHide = function (attach) {
                        $scope.canHidePop = true;
                        attach.isOpen = false;
                    }
                },
                link: function (scope, iElement, iAttrs) {

                }
            };
        });

        app.directive('attachInfo', function (attachService, imagePreviewUrl, attachDownloadUrl) {

            return {
                restrict: 'E',
                templateUrl: 'apps/base/view/attach-info.html',
                scope: {
                    attachId: '=',
                    visiable: '='
                },
                link: function (scope, iElement, iAttrs) {
                    if (scope.visiable) {
                        scope.attach = attachService.getAttach(scope.attachId).$object;
                        scope.imagePreviewUrl = imagePreviewUrl;
                        scope.attachDownloadUrl = attachDownloadUrl;
                    }
                }
            };
        });
    });
