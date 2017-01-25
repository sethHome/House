define([
'apps/system2/docquery/docquery'], function (app) {

    app.module.factory("docqueryService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getArchiveList: function () {
                return restSrv.all("archive").getList({ field: true });
            },
            searchArchive: function (key, fields) {
                return restSrv.all("archivedata").customGET("searcharchive", { "key": key, "fields": fields });
            },
            searchFile: function (key, fields) {
                return restSrv.all("archivedata").customGET("searchfile", { "key": key, "fields": fields });
            },
            getArchiveFields: function (f, a, k) {
                return restSrv.all("archive").customGET(f + "/" + a + "/field", { key: k });
            },
            getProjectFields: function () {
                return restSrv.all("field").getList({
                    key: "Project"
                });
            },
            getProjInfo: function (ID) {
                return restSrv.one("archivedata/project", ID).get();
            },
            getArchiveVolumeData: function (filter, condition) {
                return restSrv.all("archivedata").customPOST(condition, "queryvolume", filter);
            },
            getArchiveFileData: function (filter, condition) {
                return restSrv.all("archivedata").customPOST(condition, "queryfile", filter);
            },

            archiveBorrow: function (info) {
                return restSrv.all("archiveborrow").customPOST(info);
            },

            getMyArchiveBorrow: function () {
                return restSrv.all("archiveborrow").getList();
            },

            getMyArchiveItems: function (id) {
                return restSrv.one("archiveborrow", id).customGET("items");
            },

            getMyArchiveArrpve: function () {
                return restSrv.all("archiveborrow").customGET("approve");
            },

            giveBack: function (id) {
                return restSrv.one("archiveborrow", id).customPUT({}, "giveback");
            },
            getArchiveLog: function (f, t, id) {
                return restSrv.all("archivelog").getList({ fonds: f, type: t, id: id });
            }
        }
    });
});
