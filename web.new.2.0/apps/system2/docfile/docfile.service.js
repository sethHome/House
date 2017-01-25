define([
'apps/system2/docfile/docfile'], function (app) {

    app.module.factory("docfileService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getFileLibraryList: function () {
                return restSrv.all("filelibrary").getList();
            },
            getFileFields: function (f, a) {
                return restSrv.all("filelibrary").customGET(f + "/" + a + "/field");
            },
            getFileData: function (fondsNumber, fileNumber, params,condition) {
                return restSrv.one("filedata", fondsNumber + "/" + fileNumber).customPOST(condition, "query", params);
            },
            addFileData: function (fondsNumber, fileNumber, fields, nodeID, dept) {
                return restSrv.all("filedata").customPOST(fields, fondsNumber + "/" +fileNumber, { nodeid: nodeID,dept : dept });
            },
            updateFileData: function (fondsNumber, fileNumber,fileID, fields) {
                return restSrv.one("filedata", fileID).customPUT(fields, fondsNumber + "/" + fileNumber);
            },
            deleteFileData: function (fondsNumber, fileNumber, fileID) {
                return restSrv.one("filedata", fileID).customDELETE(fondsNumber + "/" + fileNumber);
            },
            batchUpdate: function (fondsNumber, fileNumber,batchInfo,params) {
                return restSrv.one("filedata", fondsNumber + "/" + fileNumber).customPUT(batchInfo, "batchupdate", params);
            },
            exportExcel: function (fondsNumber, fileNumber, params, condition) {
                var url = stdApiUrl + stdApiVersion + "/filedata/" + fondsNumber + "/" + fileNumber + "/export?";
                
                for (var key in params) {
                    url += key + "=" + params[key] + "&";
                }
                
                if (PmEx) {
                    PmEx.Download(url, JSON.stringify(condition));
                }
                else {
                    fetch(url, {
                        method: "POST",
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify(condition),
                    }).then(res => {

                        if (res.ok) {
                            $rootScope.saveAs(res, encodeURI("数据导出.xls"));
                        }
                    })
                }
            },

            getArchiveList: function () {
                return restSrv.all("archivenode").getList({});
            },
            getArchiveTypes: function () {
                return restSrv.all("archive").getList({ field: true, category: true});
            },
            getArchiveFields: function (f, a, k) {
                return restSrv.all("archive").customGET(f + "/" + a + "/field", { key: k});
            },
            // 自动组卷
            autoCreateArchive: function (info,filter) {
                return restSrv.all("archivedata").customPOST(info, "auto", filter);
            },
            getArchiveVolumeData: function (filter, condition, sqlStr) {
                return restSrv.all("archivedata").customPOST({
                    ConditionsSqlStr: sqlStr,
                    Conditions: condition
                }, "queryvolume", filter);
            },
            getArchiveFileData: function (filter, condition) {
                return restSrv.all("archivedata").customPOST(condition, "queryfile", filter);
            },
            createArchive: function (info) {
                return restSrv.all("archivedata").post(info);
            },
            updateArchive: function (id, info) {
                return restSrv.one("archivedata", id).customPUT(info,"");
            },
            deleteArchive: function (ids, args) {
                return restSrv.one("archivedata", ids).remove(args);
            },
            deleteArchiveFile: function (ID, args) {
                return restSrv.one("archivedata", ID).remove(args);
            },
            removeArchiveFile: function (ids,fonds,archveType) {
                return restSrv.one("archivedata", ids).customDELETE("file", { fonds: fonds, archive: archveType });
            },
            addArchiveFile: function (info) {
                return restSrv.all("archivedata").customPOST(info, "file");
            },
            setArchiveStatus: function (ID, args) {
                return restSrv.one("archivedata", ID).customPUT(args, "status");
            },
            getProjectFields: function () {
                return restSrv.all("field").getList({
                    key: "Project"
                });
            },
            getProjInfo: function (ID) {
                return restSrv.one("archivedata/project",ID).get();
            },
            loadProjSource: function (filter) {
                return restSrv.all("archivedata/project/source").getList(filter);
            }
        }
    });
});
