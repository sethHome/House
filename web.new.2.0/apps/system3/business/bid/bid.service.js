define(['apps/system3/business/business'], function (app) {

    app.module.factory("bidService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getBids: function (filter) {
                return restSrv.one("bid").get(filter);
            },
            addBid: function (bid) {
                return restSrv.all("bid").customPOST(bid);
            },
            edit: function (bid) {
                return restSrv.one("bid", bid.ID).customPUT(bid);
            },
            remove: function (id) {
                return restSrv.one("bid", id).remove();
            },
            backup: function (id) {
                return restSrv.one("bid", id).customDELETE("backup");
            }
        }
    });

});
