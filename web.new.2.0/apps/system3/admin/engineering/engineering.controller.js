define([
'apps/system3/admin/admin',
'apps/system3/admin/admin.service'], function (app) {

    app.module.controller("admin.controller.engineering", function ($scope, $stateParams, adminService) {

        $scope.currentEng = undefined;

        adminService.getEngineeringList({
            "projectid": $stateParams.proj ? $stateParams.proj : 0
        }).then(function (result) {
            $scope.engineeringList = result;


        });

        $scope.EngChanged = function (eng) {
            $scope.currentEng = eng;
        }
       
        $scope.setCurrent = function (eng) {
            if($stateParams.eng && eng.ID == $stateParams.eng){
                $scope.currentEng = eng;
            }
        }
    });
});
