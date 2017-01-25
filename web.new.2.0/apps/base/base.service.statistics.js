define(['apps/base/base.service'],
    function (module) {

        module.factory("statisticsService", function (Restangular, userApiUrl, userApiVersion) {

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            });

            return {
                getEngineeringCount: function (category, year) {
                    return restSrv.all("statistics").customGET("engineering/by/" + category, { "year": year ? year : 0 });
                },
                getYearEngineeringCount: function () {
                    return restSrv.all("statistics").customGET("engineering" );
                },
                getMonthEngineeringCount: function (year) {
                    return restSrv.all("statistics").customGET("engineering/" + year);
                },

                getYearContractMoney: function (type) {
                    return restSrv.all("statistics").customGET("contract/money", { "type": type });
                },
                getMonthContractMoney: function (type,year) {
                    return restSrv.all("statistics").customGET("contract/money/" + year, { "type": type });
                },
                getYearContractCount: function () {
                    return restSrv.all("statistics").customGET("contract/count");
                },
                getMonthContractCount: function (year) {
                    return restSrv.all("statistics").customGET("contract/count/" + year);
                },

            }
        });
    });
