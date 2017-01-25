define(['angularAMD',
    'angular-ui-router',
    'ui-router-extras',

    'apps/base/base.directive',

    'apps/base/base.service',
    'apps/base/base.service.menu',
    'apps/base/base.service.local',

    'apps/base/base.controller',
    'apps/base/base.controller.chat',
    'apps/base/base.controller.changepassword',
    'apps/base/base.controller.createchatgroup',
    'apps/base/base.controller.login',
    'apps/base/base.controller.chooseuser'

], function (angularAMD) {

    // Controller加载器
    angularAMD.$load = function (name) {
        return ['$q', '$stateParams',
            function ($q, $stateParams) {

                var deferred = $q.defer();
                require([name],
                    function () {
                        deferred.resolve();
                    });
                return deferred.promise;
            }];
    }

    var app = angular.module('base', [
            'ui.router',
            'ct.ui.router.extras',
            'base.controller',
            'base.service',
            'base.directive']);

    app.config(function ($stateProvider, $urlRouterProvider, $stickyStateProvider, $httpProvider, $futureStateProvider) {

        //$stickyStateProvider.enableDebug(true);

        // http 请求拦截器，添加权限验证的Token,以及返回的权限验证结果
        $httpProvider.interceptors.push('authInterceptor');

        // futurestate ngload模块加载
        $futureStateProvider.stateFactory('ngload', function ($q, futureState) {
            var ngloadDeferred = $q.defer();

            require(["ngload!" + futureState.src, 'ngload', 'angularAMD'],
                function ngloadCallback(result, ngload, angularAMD) {

                    angularAMD.processQueue();
                    ngloadDeferred.resolve(undefined);
                });

            return ngloadDeferred.promise;
        });

        // 系统初始化
        $stateProvider.state('login', {
            
            url: '/login/:account/:password',
            views: {
                "login@": {
                    templateUrl: 'apps/base/view/login.html',
                    controller: "base.controller.login"
                }
            }
        });

        $urlRouterProvider.otherwise(function ($injector, $location) {

            return "/login//";
        });

    });

    app.run(function ($rootScope, $state, $futureState, localStorageService, chatClient, localService,imagePreviewUrl, settingsService) {

        $rootScope.$state = $state;

        $rootScope.$safeApply = function (fn) {
            var phase = this.$root.$$phase;
            if (phase == '$apply' || phase == '$digest') {
                if (fn && (typeof (fn) === 'function')) {
                    fn();
                }
            } else {
                this.$apply(fn);
            }
        };


        $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
            if (toState.tab) {
                
                if (!$rootScope.currentBusiness.tabs.contains(function (s) {
                    return s.state.name.indexOf(toState.name) >= 0;
                })) {

                    $rootScope.currentBusiness.tabs.push({
                        state: toState,
                        params: toParams,
                        first: $rootScope.currentBusiness.tabs.length == 0
                    });
                }
            }
        });


        $rootScope.closeAll = function () {

            var firstTab = $rootScope.currentBusiness.tabs[0];

            $rootScope.currentBusiness.tabs = [firstTab];

            $state.go(firstTab.state, firstTab.params);
        }

        $rootScope.closeOthers = function () {

            var firstTab = $rootScope.currentBusiness.tabs[0];

            if ($state.current == firstTab.state) {
                
                $rootScope.currentBusiness.tabs = [firstTab];

            } else {

                $rootScope.currentBusiness.tabs = [firstTab, {
                    state: $state.current,
                    params: $state.params
                }];
            }
        }

        $rootScope.closeTab = function (tab) {

            for (var i = 0; i < $rootScope.currentBusiness.tabs.length; i++) {
                if ($rootScope.currentBusiness.tabs[i] == tab) {
                    
                    var subState = $rootScope.currentBusiness.tabs[i - 1];

                    $state.go(subState.state, subState.params);

                    $rootScope.currentBusiness.tabs.splice(i, 1);
                    break;
                }
            }
        }
        $rootScope.closeCurrentTab = function () {
            
            for (var i = 0; i < $rootScope.currentBusiness.tabs.length; i++) {
                
                if ($rootScope.currentBusiness.tabs[i].state.name == $state.current.name) {

                    var subState = $rootScope.currentBusiness.tabs[i - 1];

                    $state.go(subState.state, subState.params);

                    $rootScope.currentBusiness.tabs.splice(i, 1);
                    break;
                }
            }
        }

        $rootScope.addToQuickLink = function () {

        }
        

    });

    // 启动 angular 系统
    angularAMD.bootstrap(app);

    return app;

});