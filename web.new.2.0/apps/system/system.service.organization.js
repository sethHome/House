define(['apps/system/system.service'], function (app) {

    app.factory("system_organization_service", function ($rootScope,Restangular, userApiUrl, userApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(userApiUrl + userApiVersion);
        })

        return {
            createDept: function (deptName, parentKey) {
                return restSrv.all("department").customPOST({ Key: parentKey, Name: deptName });
            },
            renameDept: function (deptName, deptKey) {
                return restSrv.all("department").customPUT({ Key: deptKey, Name: deptName });
            },
            removeDept: function (key) {
                return restSrv.all("department").customDELETE("", { Key: key });
            },
            createRole: function (name) {
                return restSrv.all("role").customPOST({  Name: name });
            },
            renameRole: function (name, key) {
                return restSrv.all("role").customPUT({ Key: key, Name: name });
            },
            removeRole: function (key) {
                return restSrv.all("role").customDELETE("", { Key: key });
            },

            getDepartment: function () {
                return restSrv.all("department").getList();
            },
            getRole: function () {
                return restSrv.all("role").getList();
            },
            changeDeptUsers: function (deptKey,users) {
                return restSrv.one("department", deptKey).customPUT(users,"user");
            },
            changeDeptPermission: function (deptKey, permissions) {
                return restSrv.one("department", deptKey).customPUT(permissions, $rootScope.currentBusiness.Key + "/permission");
            },
            changeRoleUsers: function (roleKey, users) {
                return restSrv.one("role", roleKey).customPUT(users, "user");
            },
            changeRolePermission: function (roleKey, permissions) {
                return restSrv.one("role", roleKey).customPUT(permissions,$rootScope.currentBusiness.Key + "/permission");
            }
        }
    });
});
