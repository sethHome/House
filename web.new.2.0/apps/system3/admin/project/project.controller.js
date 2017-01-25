define([
'apps/system3/admin/admin',
'apps/system3/admin/admin.service'], function (app) {

    app.module.controller("admin.controller.project", function ($scope, adminService) {

        $scope.currentProj = undefined;

        adminService.getProjectList({
            "Project.Name": $scope.filter
        }).then(function (result) {
            $scope.projectList = result;
        });


        $scope.ProjChanged = function (proj) {
            $scope.currentProj = proj;
        }
    });
});
