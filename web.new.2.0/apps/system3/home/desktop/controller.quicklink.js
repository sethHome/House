define(['apps/system3/home/home.controller'], function (app) {

    app.controller("quickLinkController", function ($rootScope, $scope, $uibModalInstance,myMenus, myQuickLinks, menuService) {

        $scope.myMenus = myMenus;

        $scope.select = function (menu) {
            menu.selected = !menu.selected;
            if (menu.selected) {

                menuService.favorites(menu.key);
                myQuickLinks.push(menu);
            } else {
                menuService.removeFavorites(menu.key);
                myQuickLinks.removeObj(menu);
            }
        };

        $scope.close = function () {
            $uibModalInstance.dismiss('cancel');
        }
    });
});
