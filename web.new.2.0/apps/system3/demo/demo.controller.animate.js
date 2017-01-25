define(['apps/system3/demo/demo.controller'], function (app) {

    app.controller("demo.controller.animate", function ($scope) {

        $scope.$watch("$viewContentLoaded", function () {

            $('.js_triggerAnimation').click(function () {
                var anim = $('.js_animations').select2('data').text;
                testAnim(anim);
            });

            $('.js_animations').change(function () {
                var anim = $(this).val();
                testAnim(anim);
            });

            function testAnim(x) {
                $('#animationSandbox').removeClass().addClass(x + ' animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
                    $(this).removeClass();
                });
            };

        })
    });

});
