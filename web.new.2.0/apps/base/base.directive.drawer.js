define(['apps/base/base.directive'],
    function (app) {

        app.directive('drawer', function ($timeout, $parse) {
            return {
                restrict: 'C',
                link: function ($scope, element, $attrs, ngModel) {

                    if (!$scope.drawer) {
                        $scope.drawer = 1;
                    }

                    $(element).addClass("top-b-" + $scope.drawer);
                    $(element).find('.drawer-toggle').addClass("top-" + $scope.drawer).click(function () {

                        if ($(element).hasClass('open')) {
                            $(element).find("#quickview-sidebar").removeClass('open');
                            $(element).removeClass('open');
                        } else {
                            $(element).find("#quickview-sidebar").addClass('open');
                            $(element).addClass('open');
                        }
                    });

                    $scope.drawer++;
                }
            };
        });

    });
