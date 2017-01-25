define(['apps/system2/docquery/docquery', 'apps/system2/docquery/docquery.service'], function (app) {

    app.module.controller("docquery.controller.myarchive", function ($scope, stdApiUrl,stdApiVersion, $uibModal, $filter, docqueryService) {

        $scope.downloadUrl = stdApiUrl + stdApiVersion;

        docqueryService.getMyArchiveBorrow().then(function (result) {
            $scope.myBorrowList = result;
        });

        $scope.setCurrentItem = function (item) {
            $scope.currentItem = item;
            $scope.archiveFields = item.ArchiveInfo.HasVolume ? item.ArchiveInfo.VolumeFields : item.ArchiveInfo.BoxFields;
        }

        $scope.$watch("currentItem", function (newval, oldval) {
            if (newval) {
                loadArchiveInfo(newval.ArchiveType, newval.Fonds, newval.ArchiveID);
            }
        })

        $scope.giveBack = function (item) {

            docqueryService.giveBack(item.BorrowID).then(function () {
                item.Status = 4;
            })
        }

        var loadArchiveInfo = function (type, fonds, id) {

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

            var files = $scope.currentItem.ArchiveInfo.FileFields.where(f => f.Main);

            if (files.length > 0) {
                return files.map(f => item["_f" + f.ID]).join('-');
            } else {
                return item._f1;
            }
        }
    });

});
