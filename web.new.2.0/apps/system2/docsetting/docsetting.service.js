define([
'apps/system2/docsetting/docsetting'], function (app) {

    app.module.factory("docsettingService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            fonds: {
                // 获取全宗列表
                getList: function () {
                    return restSrv.all("fonds").getList();
                },
                // 添加全宗
                add: function (fonds) {
                    return restSrv.all("fonds").customPOST(fonds);
                },
                // 删除
                remove: function (number) {
                    return restSrv.one("fonds", number).customDELETE();
                },
                // 更新
                update: function (fonds) {
                    return restSrv.all("fonds").customPUT(fonds);
                },
                // 全宗号重复校验
                check: function (number) {
                    return restSrv.one("fonds", number).customGET("check");
                }
            },
            archive: {
                getList: function () {
                    return restSrv.all("archive").getList({ category : true});
                },
                // 添加档案库
                add: function (archiveInfo) {
                    return restSrv.all("archive").customPOST(archiveInfo);
                },
                // 更新
                update: function (archiveInfo) {
                    return restSrv.all("archive").customPUT(archiveInfo);;
                },
                // 档案库名称重复校验
                check: function (name, fondsNumber) {
                    return restSrv.one("archive", fondsNumber).customGET("check/" + name);
                },
                // 禁用档案库
                remove: function (fondsNumber, key) {
                    return restSrv.one("archive/disable", fondsNumber).customDELETE(key);
                },
                // 启用档案库
                visiable: function (fondsNumber, key) {
                    return restSrv.one("archive/visiable", fondsNumber).customDELETE(key);
                },
                // 删除档案库
                del: function (fondsNumber, key) {
                    return restSrv.one("archive", fondsNumber).customDELETE(key);
                },
                getFields: function (f, a, k, m) {
                    return restSrv.all("archive").customGET(f + "/" + a + "/field", { key: k, mapping: m ? 1 : 0 });
                },
                checkField: function (field) {
                    return restSrv.all("archive/field/check").customPOST(field);
                },
                // 添加字段
                addField: function (field) {
                    return restSrv.all("archive/field").customPOST(field);
                },
                // 更新
                updateField: function (field) {
                    return restSrv.all("archive/field").customPUT(field);;
                },
                // 删除
                removeField: function (f, a, k, id) {
                    return restSrv.one("archive", f + "/" + a + "/" + k).customDELETE("field/" + id);
                },

                // 添加分类
                addCategory: function (f, a, info) {
                    return restSrv.all("archive/" + f + "/" + a).customPOST(info, "category");
                },
                // 更新分类
                updateCategory: function (f, a, n, info) {
                    return restSrv.one("archive/" + f + "/" + a, n).customPUT(info, "category");
                },
                // 删除分类
                deleteCategory: function (f, a, n) {
                    return restSrv.one("archive/" + f + "/" + a, n).customDELETE("category");
                },
                // 生成档案库库
                generate: function (f, n) {
                    return restSrv.all("archive").customPOST({
                        FondsNumber: f,
                        Key: n
                    }, "generate");
                },
                // 生成项目库
                generateProject: function () {
                    return restSrv.all("archive").customPOST({}, "project/generate");
                }
            },
            file: {
                getList: function () {
                    return restSrv.all("filelibrary").getList();
                },
                // 添加
                add: function (filelibraryInfo) {
                    return restSrv.all("filelibrary").customPOST(filelibraryInfo);
                },
                // 更新
                update: function (filelibraryInfo) {
                    return restSrv.all("filelibrary").customPUT(filelibraryInfo);;
                },
                // 名称重复校验
                check: function (filelibraryInfo) {
                    return restSrv.all("filelibrary").customPOST(filelibraryInfo, "check");
                },
                // 删除
                del: function (fondsNumber, key) {
                    return restSrv.one("filelibrary", fondsNumber).customDELETE("", { key: key });
                },
                getFields: function (f, a, k) {
                    return restSrv.all("filelibrary").customGET(f + "/" + a + "/field");
                },
                checkField: function (field) {
                    return restSrv.all("filelibrary/field/check").customPOST(field);
                },
                // 添加字段
                addField: function (field) {
                    return restSrv.all("filelibrary/field").customPOST(field);
                },
                // 更新
                updateField: function (field) {
                    return restSrv.all("filelibrary/field").customPUT(field);;
                },
                // 删除
                removeField: function (f, a, id) {
                    return restSrv.one("filelibrary", f + "/" + a).customDELETE("field/" + id);
                },
                // 生成文件库
                generate: function (f, n) {
                    return restSrv.all("filelibrary").customPOST({
                        FondsNumber: f,
                        Number: n
                    }, "generate");
                }
            },
            field: {
                getList: function (key) {
                    return restSrv.all("field").getList({
                        key: key
                    });
                },
                check: function (field) {
                    return restSrv.all("field/check").customPOST(field);
                },
                // 添加字段
                add: function (field) {
                    return restSrv.all("field").customPOST(field);
                },
                // 更新
                update: function (field) {
                    return restSrv.all("field").customPUT(field);;
                },
                // 删除
                remove: function (key, id) {
                    return restSrv.one("field", key).customDELETE(id);
                },
            },
            argument: {
                saveMapping: function (mappings) {
                    return restSrv.all("argumentsetting").customPOST(mappings, "mapping");
                }
            }
        }
    });
});
