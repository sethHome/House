define(['apps/system3/business/business'], function (app) {

    app.module.factory("contractService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            loadSource: function (filter) {
                return restSrv.all("contract").customGET('source',filter);
            },
            getContracts: function (filter) {
                return restSrv.one("contract").get(filter);
            },
            create: function (proj, attachIDs) {
                proj.AttachIDs = attachIDs;
                return restSrv.all("contract").customPOST(proj);

            },
            edit: function (proj) {
                return restSrv.one("contract", proj.ID).customPUT(proj);
            },
            remove: function (ID) {
                return restSrv.one("contract", ID).remove();
            },
            backup: function (id) {
                return restSrv.one("contract", id).customDELETE("backup");
            },
            batchRemove: function (IDs) {
                return restSrv.one("contract", IDs.join(',')).remove();
            },
            getPayees: function (id) {
                return restSrv.one("contract", id).customGET('payee');
            },
            addPayee: function (id,payeeInfo) {
                return restSrv.one("contract", id).customPOST(payeeInfo, 'payee');
            },
            deletePayee: function (id) {
                return restSrv.one("contract/payee", id).remove();
            }

        }
    });


});
