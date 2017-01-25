define(['angularAMD'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_statistics",
            ['ui.router', 'module_statistics_controller'])

        // 本App的主路由
        var mainState = {
            name: 'statistics',
            url: '/system3/statistics',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'statistics.Sidebar@': {
                    controller: 'statistics.sidebar.controller',
                    templateUrl: 'apps/system3/statistics/view/statistics-silder.html'
                },
                'statistics.Content@': {
                    controller: 'statistics.content.controller',
                    templateUrl: 'apps/system3/statistics/view/statistics.html'
                }
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
                mainState.system = $rootScope.modulesSys["statistics"];
                _$stateProvider.state(mainState);


                angular.forEach($rootScope.modules["statistics"], function (state) {

                    var s = {
                        system: state.System,
                        name: 'statistics.' + state.Name,
                        url: '/' + state.Name,

                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['statistics.content.' + state.Name] = {
                        templateUrl: "apps/system3/statistics/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "statistics.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system3/statistics/' + state.Name + '/' + state.Name + '.controller')
                        }
                    };

                    _$stateProvider.state(s);

                    // 子路由
                    if (state.SubModules) {
                        angular.forEach(state.SubModules, function (subState) {
                            var ss = {
                                system: subState.System,
                                name: s.name + '.' + subState.Name,
                                url: '/' + subState.Name,
                                templateUrl: "apps/system3/statistics/" + state.Name + "/view/" + state.Name + '.' + subState.Name + ".html",
                                controller: "statistics.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load('apps/system3/statistics/' + state.Name + '/' + state.Name + '.' + subState.Name + '.controller')
                                }
                            };

                            if (subState.Param) {
                                ss.url += subState.Param;
                            };

                            _$stateProvider.state(ss);
                        });
                    }

                });
            }]);

        app.controller("statistics.sidebar.controller", function ($scope, $rootScope, $state) {

           
        });

        app.controller("statistics.content.controller", function ($scope, $rootScope) {
        });

        app.controller("statistics.header.controller", function ($scope, $rootScope) {
        });

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

