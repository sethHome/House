define(['angularAMD','BigInt',
    'css!apps/system/css/system',
    'ngload!apps/system/system.controller',
    'ngload!apps/system/system.service',
    'ngload!apps/system/system.directive',

    'apps/system/system.controller.department'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_system",
            ['ui.router', 'module_system_controller'])

        // 本App的主路由
        var mainState = {
            name: 'system',
            url: '/system',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'system.Sidebar@': {
                    controller: 'system.controller.sidebar',
                    templateUrl: 'apps/system/view/system-silder.html'
                },
                'system.Content@': {
                    controller: 'system.controller.content',
                    templateUrl: 'apps/system/view/system.html'
                }
            }
        }
        var _$stateProvider = null;

        // app的配置阶段
        app.config(['$stateProvider',
            function ($stateProvider) {

                _$stateProvider = $stateProvider;

            }]);

        //app运行时
        app.run(["$rootScope", "$state", 
            function ($rootScope, $state) {

                // 将路由添加到路由表中
                mainState.system = $rootScope.modulesSys["system"];
                _$stateProvider.state(mainState);

                angular.forEach($rootScope.modules["system"], function (state) {
                    
                    var s = {
                        tab: state.Tab,
                        text: state.Text,
                        system: state.System,
                        name: 'system.' + state.Name,
                        url: '/' + state.Name ,
                        deepStateRedirect: true,
                        sticky: true,
                        views: {}
                    };

                    if (state.Param) {
                        s.url +=  state.Param;
                    };
                    
                    s.views['system.content.' + state.Name] = {
                        templateUrl: "apps/system/view/" + state.Name + ".html",
                        controller: "system.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system/system.controller.' + state.Name)
                        }
                    };

                    _$stateProvider.state(s);

                    // 子路由
                    if (state.SubModules) {
                        angular.forEach(state.SubModules, function (subState) {
                            var ss = {
                                tab: subState.Tab,
                                text: subState.Text,
                                system: subState.System,
                                name: s.name + '.' + subState.Name,
                                url: '/' + subState.Name,
                                templateUrl: "apps/system/view/" + state.Name + "." + subState.Name + ".html",
                                controller: "system.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load("apps/system/system.controller." + state.Name + "." + subState.Name)
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

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

