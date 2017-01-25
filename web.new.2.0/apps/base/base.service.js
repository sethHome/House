define(["restangular", "angular-local-storage"],
    function () {

        var app = angular.module('base.service', ["restangular", "LocalStorageModule"]);

        app.constant("userApiUrl", "http://localhost:8002/api/");
        app.constant("userApiUrlRemote", "http://218.93.7.150:8010/api/");
        app.constant("userApiVersion", "v1");

        app.constant("attachUpload", "http://localhost:8002/api/v1/attach");
        app.constant("attachDownloadUrl", "http://localhost:8002/api/v1/download/");
        app.constant("imagePreviewUrl", "http://localhost:8002/api/v1/image/");
        app.constant("officePreviewUrl", "http://localhost:8002/api/v1/preview/");

        //app.constant("wsAddress", "ws://218.93.7.150:8012/");
        app.constant("wsAddress", "ws://localhost:8012/");
        //app.constant("wsAddress", "ws://localhost:5000/");

        app.constant("stdApiUrl", "http://localhost:8002/api/");
        app.constant("stdApiVersion", "v1");

        // 为每次的Http请求添加Authorization头信息
        app.factory('authInterceptor', function (localStorageService, $rootScope) {

            return {
                request: function (config) {
                   
                     if ($rootScope.currentUser) {
                        
                        config.headers = config.headers || {};
                        config.headers.Authorization = $rootScope.currentUser.Token;
                    }

                    return config;
                },
                //response: function (res) {

                //    if (res.status == 500) {
                //        alert("1111");
                //    }
                //    return res;
                //    // todo: 如果令牌过了有效期，或者没有令牌返回401 Unauthorized，
                //    // 这里需要重新回到登录页面，用户登录成功后应该重新回到原来的路由状态
                //}
            };
        });
        
        return app;
    });
