define(['apps/system/system.controller',
    'apps/system/system.service.user',
    'apps/system/system.service.business'], function (app) {

    app.controller("system.controller.user", function ($scope, system_user_service, system_business_service) {
        
        $scope.userChanged = function (user) {
            $scope.currentUser = user;
        }

        //$scope.$watch("currentUser", function (newVal, oldVal) {
        //    if (newVal) {
                
        //        $scope.$broadcast("$UserSelected", newVal);
        //        $scope.$emit("$UserSelected", newVal);
        //    }
        //});

        $scope.setPage = function () {
            //$scope.userScroller.init();

            var tiles = $(".tile, .tile-small, .tile-sqaure, .tile-wide, .tile-large, .tile-big, .tile-super");

            $.each(tiles, function () {
                var tile = $(this);
                setTimeout(function () {
                    tile.css({
                        opacity: 1,
                        "-webkit-transform": "scale(1)",
                        "transform": "scale(1)",
                        "-webkit-transition": ".3s",
                        "transition": ".3s"
                    });
                }, Math.floor(Math.random() * 500));
            });

            $(".tile-group").animate({
                left: 0
            });
        }

        $scope.$watch('$viewContentLoaded', function () {

            
           
        });

        var loadUser = function () {
            
            $scope.userPanel.block();
            system_user_service.getUsersEx(true, true, true,true).then(function (result) {
                $scope.userPanel.unblock();
                $scope.users = result;
                $scope.currentUser = result[0];
            });
        }

        $scope.$watch('$viewContentLoaded', function () {
            loadUser();
            
        });
        
       
        
    });
   
});
