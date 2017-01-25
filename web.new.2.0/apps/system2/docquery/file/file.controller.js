define(['apps/system2/docquery/docquery', 'apps/system2/docquery/docquery.service'], function (app) {

    app.module.controller("docquery.controller.file", function ($scope,$sce, $uibModal, $filter, docqueryService) {

        $scope.fonds = docqueryService.getArchiveList().$object;
        $scope.$sce = $sce;
        
        $scope.search = function () {

            $scope.listResult = true;

            var archives = $scope.fonds[0].Archives.where(function (a) { return a.choose; });


            var fields = [];
            var fieldsMap = {};

            angular.forEach(archives, function (a) {

                var vFields = a.FileFields.where(function (f) { return f.ForSearch; });

                var fs = vFields.map(function (f) { var fid = "_f" + f.ID ; fieldsMap[fid] = f.Name; return fid; })

                fields = fields.concat(fs);
            });
            $scope.fields = fields;
            $scope.fieldsMap = fieldsMap;
            docqueryService.searchFile($scope.searchKey, fields.join(',')).then(function (result) {
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

        $scope.$watch("currentItem", function (newval, oldval) {
            if (newval) {
                loadArchiveInfo(newval.Datas.ArchiveType, newval.Datas.FondsNumber, newval.Datas.ArchiveID);
            }
        })

        var loadArchiveInfo = function (type, fonds, id) {

            var archives = $scope.fonds[0].Archives.where(function (a) { return a.Key == type; });
            
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

                    if (archives[0].HasProject) {
                        docqueryService.getProjInfo($scope.volumeInfo.ProjectID).then(function (projInfo) {
                            $scope.projInfo = projInfo;
                        });
                    }
                }
            });

            //docqueryService.getArchiveFileData({
            //    volume: id,
            //    fonds: fonds,
            //    archive: type
            //}).then(function (result) {

            //    $scope.archiveFiles = result.Source;
            //});
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
    });

});
