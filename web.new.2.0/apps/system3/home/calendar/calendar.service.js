define(['apps/system3/home/home.controller'], function (app) {

    app.factory("calendarService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getMyCalendar: function (filter) {
                filter = $.extend(filter, { self: 1 })
                return restSrv.one("calendar").get(filter);
            },
            add: function (info) {
                return restSrv.all("calendar").customPOST(info);
            },
            update: function (info) {
                return restSrv.one("calendar", info.ID).customPUT(info);
            },
            remove: function (id) {
                return restSrv.one("calendar", id).remove();
            }
        }
    });


});
