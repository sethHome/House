define([
'apps/system2/book/book'], function (app) {

    app.module.factory("bookService", function ($rootScope, Restangular, stdApiUrl, stdApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(stdApiUrl + stdApiVersion);
        })

        return {
            getBooks: function (filter) {
                return restSrv.one("book").get(filter);
            },
            getBookCategorys: function () {
                return restSrv.all("book").customGET("category");
            },
            addBookCategory: function (info) {
                return restSrv.all("book").customPOST(info, 'category');
            },
            editBookCategory: function (number,info) {
                return restSrv.all("book").customPUT(info, 'category/' + number);
            },
            deleteBookCategory: function (number) {
                return restSrv.all("book").customDELETE('category/' + number);
            },
            createBook: function (info) {
                return restSrv.all("book").customPOST(info);
            }
        }
    });
});
