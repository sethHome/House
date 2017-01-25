define(['apps/system3/business/business'], function (app) {

    app.module.factory("customerService", function (Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getCustomers: function (filter) {
                return restSrv.one("customer").get(filter);
            },
            addCustomer: function (customer) {
                return restSrv.all("customer").customPOST(customer);
            },
            edit: function (customer) {
                return restSrv.one("customer", customer.ID).customPUT(customer);
            },
            remove: function (ID) {
                return restSrv.one("customer", ID).remove();
            },
            backup: function (id) {
                return restSrv.one("customer", id).customDELETE("backup");
            },
            addPerson: function (id,person) {
                return restSrv.one("customer", id).customPOST(person, 'person');
            },
            updatePerson: function (person) {
                return restSrv.all("customer").customPUT(person, 'person/' + person.ID);
            },
            deletePerson: function (id) {
                return restSrv.all("customer").customDELETE('person/' +id);
            },
            loadSource: function (filter) {
                return restSrv.all("customer").customGET('source', filter);
            },
        }
    });
    
});
