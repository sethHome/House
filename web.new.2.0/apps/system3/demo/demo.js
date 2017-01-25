define(['angularAMD',
    
    'ngload!apps/system3/demo/demo.controller'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_demo",
            ['ui.router', 'module_demo_controller'])

        // 本App的主路由
        var mainState = {
            name: 'demo',
            url: '/system3/demo',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'demo.Sidebar@': {
                    controller: 'demo.sidebar.controller',
                    templateUrl: 'apps/system3/demo/view/demo-silder.html'
                },
                'demo.Content@': {
                    controller: 'demo.content.controller',
                    templateUrl: 'apps/system3/demo/view/demo.html'
                }
            }
        }

        var _$stateProvider;

        // app的配置阶段
        app.config(['$stateProvider',
            function ($stateProvider) {

               

                _$stateProvider = $stateProvider;

            }]);

        //app运行时
        app.run(["$rootScope",
            function ($rootScope) {

                // 将路由添加到路由表中
                mainState.system = $rootScope.modulesSys["demo"];
                _$stateProvider.state(mainState);

                angular.forEach($rootScope.modules["demo"], function (state) {

                    var s = {
                        system: state.System,
                        name: 'demo.' + state.Name,
                        url: '/' + state.Name,
                        deepStateRedirect: true,
                        sticky: true,
                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['demo.content.' + state.Name] = {
                        templateUrl: "apps/system3/demo/view/" + state.Name + ".html",
                        controller: "demo.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system3/demo/demo.controller.' + state.Name)
                        }
                    };

                    _$stateProvider.state(s);
                });

            }]);

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

