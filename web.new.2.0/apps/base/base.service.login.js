define(['apps/base/base.service'],
    function (module) {

        module.factory("loginService", function ($rootScope, Restangular, userApiUrl, userApiVersion) {

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            })

            return {
                login: function (account, password) {
                    return restSrv.one("login").customGET(account + "/" + password);
                }
            }
        });

        module.factory("userService", function ($rootScope, Restangular, userApiUrl, userApiVersion) {

            var restSrv = Restangular.withConfig(function (configSetter) {
                configSetter.setBaseUrl(userApiUrl + userApiVersion);
            })

            return {
                getDept: function () {
                    return restSrv.all("department").getList();
                },
                getUsers: function () {
                    return restSrv.all("user").getList();
                },
                getUsersEx: function () {
                    return restSrv.all("user").customGET("ex", {
                        "withdept": true,
                        "withrole": true,
                        "withpermission": false,
                        "withsys": false
                    });
                },
                changepassword: function (oldPsw, newPsw) {
                    return restSrv.all("user").customPUT({
                        OldPassword: oldPsw,
                        NewPassword: newPsw
                    }, 'changepassword');
                },
                setUserImage: function (img, imgLarge) {
                    return restSrv.all("user").customPUT({
                        PhotoImg: img,
                        PhotoImgLarge: imgLarge
                    }, 'photo');
                },
                resetPassword: function () {
                    return restSrv.all("user").customPUT({}, 'resetpsw');
                },
                addUserConfig: function (config) {
                    return restSrv.all("user").customPOST(config, 'config');
                },
                removeUserConfig: function (name, key) {
                    return restSrv.all("user").customDELETE('config/' + name + '/' + key);
                },
                getUserConfig: function (config) {
                    return restSrv.all("user/config").getList();
                }
            }
        });
    });
