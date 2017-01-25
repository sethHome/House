define(['apps/system3/office/office'], function (app) {

    app.module.factory("newsService", function (Restangular, stdApiUrl, stdApiVersion) {
        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getNewsList: function (filter) {
                return restSrv.one("news").get(filter);
            },
            create: function (newsInfo) {
                return restSrv.all("news").customPOST(newsInfo);
            },
            update: function (newsInfo) {
                return restSrv.one("news", newsInfo.ID).customPUT(newsInfo);
            },
            remove: function (id) {
                return restSrv.one("news", id).remove();
            }
        }
    });

});
