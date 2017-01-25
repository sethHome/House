define(['angularAMD',
     'css!apps/system/css/system',
     'ngload!apps/system3/production/production.controller'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_production",
            ['ui.router', 'module_production_controller'])

        // 本App的主路由
        var mainState = {
            name: 'production',
            url: '/system3/production',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'production.Sidebar@': {
                    controller: 'production.sidebar.controller',
                    templateUrl: 'apps/system3/production/view/production-silder.html'
                },
                'production.Content@': {
                    controller: 'production.content.controller',
                    templateUrl: 'apps/system3/production/view/production.html'
                },
                'header-menu@': {
                    controller: 'production.header.controller',
                    templateUrl: 'apps/system3/production/view/production-header-menu.html'
                },
            }
        }

        var _$stateProvider = undefined;

        // app的配置阶段
        app.config(['$stateProvider',
            function ($stateProvider) {

                _$stateProvider = $stateProvider;

            }]);

        //app运行时
        app.run(["$rootScope", "$state",
            function ($rootScope, $state) {

                // 将路由添加到路由表中
                mainState.system = $rootScope.modulesSys["production"];
                _$stateProvider.state(mainState);

                angular.forEach($rootScope.modules["production"], function (state) {

                    var s = {
                        tab: true,
                        text: state.Text,
                        system: state.System,
                        name: 'production.' + state.Name,
                        url: '/' + state.Name,
                        deepStateRedirect: true,
                        sticky: true,
                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['production.content.' + state.Name] = {
                        templateUrl: "apps/system3/production/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "production.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system3/production/' + state.Name + '/' + state.Name + '.controller')
                        }
                    };

                    _$stateProvider.state(s);

                    // 子路由
                    if (state.SubModules) {
                        angular.forEach(state.SubModules, function (subState) {
                            var ss = {
                                tab: true,
                                text: subState.Text,
                                system: subState.System,
                                name: s.name + '.' + subState.Name,
                                url: '/' + subState.Name,
                                deepStateRedirect: true,
                                sticky: true,

                                templateUrl: "apps/system3/production/" + state.Name + "/" + subState.Name + "/view/" + subState.Name + ".html",
                                controller: "production.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load("apps/system3/production/" + state.Name + "/" + subState.Name + "/" + state.Name + ".controller." + subState.Name)
                                }
                            };

                            if (subState.Param) {
                                ss.url += subState.Param;
                            };

                            //ss.views = subStateView[state.Name][subState.Name];

                            _$stateProvider.state(ss);
                        });
                    }
                });
            }]);

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

