define(['apps/system2/docquery/docquery', 'apps/system2/docquery/docquery.service'], function (app) {

    app.module.controller("docquery.controller.search", function ($scope,$rootScope, $uibModal, $filter, docqueryService, stdApiUrl, stdApiVersion) {

        $scope.downloadUrl = stdApiUrl + stdApiVersion;

        $scope.fonds = docqueryService.getArchiveList().$object;

        $scope.search = function () {

            $scope.listResult = true;

            var archives = $scope.fonds[0].Archives.where(function (a) { return a.choose; });


            var fields = [];
            var fieldsMap = {};

            angular.forEach(archives, function (a) {

                if (a.HasVolume) {
                    var vFields = a.VolumeFields.where(function (f) { return f.ForSearch; });

                    var fs = vFields.map(function (f) { var fid = "_f" + f.ID + "_v"; fieldsMap[fid] = f.Name; return fid; })
                    
                    fields = fields.concat(fs);
                } else {
                    var bField = a.BoxFields.where(function (f) { return f.ForSearch; }).map(function (f) { return "_f" + f.ID + "_b" });

                    var fs = bField.map(function (f) { var fid = "_f" + f.ID + "_b"; fieldsMap[fid] = f.Name; return fid; })

                    fields = fields.concat(fs);
                }

                if (a.HasProject) {
                    var pField = a.ProjectFields.where(function (f) { return f.ForSearch; });
                    var fs = pField.map(function (f) { var fid = "_f" + f.ID + "_p"; fieldsMap[fid] = f.Name; return fid; });
                    fields = fields.concat(fs);
                }

            });
            $scope.fields = fields;
            $scope.fieldsMap = fieldsMap;
            docqueryService.searchArchive($scope.searchKey, fields.join(',')).then(function (result) {
               
                $scope.searchResult = result;
            });

          
            
        }

        $scope.getMainInfo = function (datas) {

            for (var i = 0; i < $scope.fields.length; i++) {

                var f = $scope.fields[i];

                if (datas[f] && datas[f].indexOf('<font') >= 0) {
                    return datas[f];
                }
            }

            return datas[0];
        }

        $scope.getMainInfoField = function (datas) {

            for (var i = 0; i < $scope.fields.length; i++) {

                var f = $scope.fields[i];

                if (datas[f] && datas[f].indexOf('<font') >= 0) {
                    return $scope.fieldsMap[f];
                }
            }
        }

        $scope.setCurrentItem = function (item) {
            $scope.currentItem = item;
        }

        $scope.$watch("currentItem", function (newval,oldval) {
            if (newval) {
                loadArchiveInfo(newval.Datas.ArchiveType, newval.Datas.FondsNumber, newval.ID);
            }
        })

        $scope.getMainArchiveName = function (item) {

            var archives = $scope.fonds[0].Archives.where(a => a.Key == item.Datas.ArchiveType);

            var files = archives[0].VolumeFields.where(f => f.Main);

            if (files.length > 0) {
                return files.map(f => item.Datas["_f" + f.ID + "_v"]).join('-');
            } else {
                return item.Datas._f1_v;
            }
        }

       

       

        var loadArchiveInfo = function (type, fonds,id ) {
            
            var archives = $scope.fonds[0].Archives.where(a => a.Key == type);

            $scope.archiveFields = archives[0].VolumeFields;
            $scope.boxFields = archives[0].BoxFields;
            $scope.archiveFileFileds = archives[0].FileFields;
            $scope.projectFields = archives[0].ProjectFields;

            docqueryService.getArchiveVolumeData({
                ids: id,
                fonds: fonds,
                archive: type
            }).then(function (result) {
                if (result.Source && result.Source.length > 0) {
                    $scope.volumeInfo = result.Source[0];

                    if ($scope.volumeInfo.ProjectID > 0) {

                        docqueryService.getProjInfo($scope.volumeInfo.ProjectID).then(function (projInfo) {
                            $scope.projInfo = projInfo;
                        });
                    }
                    
                }
            });
           
            docqueryService.getArchiveFileData({
                volume: id,
                fonds: fonds,
                archive: type
            }).then(function (result) {

                $scope.archiveFiles = result.Source;
            });

            docqueryService.getArchiveLog(fonds, type, id).then(function (datas) {
                $scope.currentArchiveLogs = datas;
            });
        }

        $scope.getFiledValue = function (source, field) {
            if (source) {
                switch (field.DataType) {
                    case 3: return $filter('TDate')(source["_f" + field.ID]);
                    case 4: return $filter('enumMap')(source["_f" + field.ID], field.BaseData);
                    default: return source["_f" + field.ID];
                }
            }
        }

       
        $scope.getMainFileName = function (item) {

            var files = $scope.archiveFileFileds.where(f => f.Main);

            if (files.length > 0) {
                return files.map(f => item["_f" + f.ID]).join('-');
            } else {
                return item._f1;
            }
        }
        
        $scope.getMainArchiveName2 = function (archiveType, source) {

            var archives = $scope.fonds[0].Archives.where(a => a.Key == archiveType);

            var files = archives[0].VolumeFields.where(f => f.Main);

            if (files.length > 0) {
                return files.map(f => source["_f" + f.ID]).join('-');
            } else {
                return source._f1_v;
            }
        }

        $scope.addToBorrowList = function () {

            if ($rootScope.archiveBorrowList == undefined) {
                $rootScope.archiveBorrowList = [];
            }

            var id = $scope.currentItem.Datas.ArchiveType + "_" + $scope.volumeInfo.ID;

            if ($rootScope.archiveBorrowList.contains(a => a.id == id)) {
                bootbox.alert("该档案已在我的借阅列表中！");
            } else {
                
                var mainName = $scope.getMainArchiveName2($scope.currentItem.Datas.ArchiveType, $scope.volumeInfo);

                $rootScope.archiveBorrowList.push({
                    id: id,
                    archiveID : $scope.volumeInfo.ID,
                    fonds: $scope.volumeInfo.FondsNumber,
                    type : $scope.currentItem.Datas.ArchiveType,
                    typeName: $scope.currentItem.Datas.ArchiveTypeName,
                    name: mainName,
                    copies: $scope.volumeInfo.Copies
                });
            }
        }
    });
});
