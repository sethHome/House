define(['apps/system3/demo/demo.controller'], function (app) {

    app.controller("demo.controller.icon", function ($scope) {

        $scope.$watch("$viewContentLoaded", function () {

            var icons = new Skycons({
                "color": "#5B5B5B"
            }),
              list = ["clear-day", "clear-night", "partly-cloudy-day", "partly-cloudy-night", "cloudy", "rain", "sleet", "snow", "wind", "fog"],
              i;
            for (i = list.length; i--;) icons.set(list[i], list[i]);
            icons.play();

        })

    });

});
