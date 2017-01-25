define(['apps/base/base.directive'],
    function (app) {

      

        app.directive('permissionCheck', function ($timeout, permissionCheckService) {

            var check = function ($attrs, element) {
                permissionCheckService.check($attrs.permissionCheck).then(function (result) {
                    // 拒绝
                    if (result == 1) {
                        $(element).hide();
                    } else {
                        $(element).show();
                    }
                })
            };

            return {
                link: function ($scope, element, $attrs) {

                    check($attrs, element);

                    $scope.$on("permissionChanged", function () {
                        check($attrs, element);
                    })
                }
            };
        });
    });
