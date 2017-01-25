
define([
'apps/system2/docmgr/docmgr'], function (app) {

    app.module.factory("docmgrService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getArchiveTypes: function () {
                return restSrv.all("archive").getList({ field: true, category: true });
            },
            getArchiveList: function () {
                return restSrv.all("archivenode").getList();
            },
            getFields: function (fonds, archiveType) {
                return restSrv.all("archive").customGET(fonds + "/" + archiveType + "/field");
            },
            addNode: function (node) {
                return restSrv.all("archivenode").customPOST(node);;
            },
            updateNode: function (node) {
                return restSrv.all("archivenode").customPUT(node);
            },
            deleteNode: function (fonds, key) {
                return restSrv.all("archivenode").customDELETE("", {
                    fonds: fonds,
                    key: key
                });
            },
            disableNode: function (fonds, key) {
                return restSrv.all("archivenode").customDELETE("disable", {
                    fonds: fonds,
                    key: key
                });
            },
            visiableNode: function (fonds, key) {
                return restSrv.all("archivenode").customDELETE("visiable", {
                    fonds: fonds,
                    key: key
                });
            },

            getArchiveFields: function (f, a, k) {
                return restSrv.all("archive").customGET(f + "/" + a + "/field", { key: k });
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
            getProjectFields: function () {
                return restSrv.all("field").getList({
                    key: "Project"
                });
            },
            getProjInfo: function (ID) {
                return restSrv.one("archivedata/project", ID).get();
            },
            setArchiveStatus: function (ID, args) {
                return restSrv.one("archivedata", ID).customPUT(args, "status");
            },

            getArchiveLog: function (f, t, id) {
                return restSrv.all("archivelog").getList({ fonds: f, type: t, id: id });
            }
        }
    });
});
