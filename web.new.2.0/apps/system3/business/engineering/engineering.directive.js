define(['apps/system3/business/business'], function (app) {

    app.module.directive('engineeringFilterView', function () {
        return {
            restrict: 'E',
            
            templateUrl: 'apps/system3/business/engineering/view/engineering-filter.html',
            link: function ($scope, element, $attrs) {
                
                $scope.filterArg = {
                    Specialty : $attrs["specialty"] == "true"
                }
            }
        };
    });
});

