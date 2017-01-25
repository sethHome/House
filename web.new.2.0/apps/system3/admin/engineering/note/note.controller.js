define([
'apps/system3/admin/admin',
'apps/system3/admin/admin.service'], function (app) {

    app.module.controller("admin.controller.engineering.note", function ($scope, adminService) {

        var loadEngNote = function (id) {
            adminService.getEngineeringNotes(id).then(function(result){
                $scope.notes = result
            });
            //$scope.notes = adminService.getEngineeringNotes(id).$object;
        }

        $scope.$watch("currentEng", function (newval, oldval) {
            if (newval) {
                loadEngNote(newval.ID);
            }
        });
    });
});
