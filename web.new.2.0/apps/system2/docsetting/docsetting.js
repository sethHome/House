define(['angularAMD'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_docsetting",
            ['ui.router'])

        // 本App的主路由
        var mainState = {
            name: 'docsetting',
            url: '/system2/docsetting',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'docsetting.Sidebar@': {
                    controller: 'docsetting.sidebar.controller',
                    templateUrl: 'apps/system2/docsetting/view/silder.html',
                    
                },
                'docsetting.Content@': {
                    controller: 'docsetting.content.controller',
                    templateUrl: 'apps/system2/docsetting/view/main.html',
                   
                }
            }
        }

        var _$stateProvider = undefined;

        // app的配置阶段
        app.config(['$stateProvider',
            function ($stateProvider, $compileProvider) {

                //$compileProvider.debugInfoEnabled(true); // Remove debug info (angularJS >= 1.3)

                _$stateProvider = $stateProvider;

            }]);

        //app运行时
        app.run(["$rootScope", "$state",
            function ($rootScope, $state) {

                // 将路由添加到路由表中
                mainState.system = $rootScope.modulesSys["docsetting"];
                _$stateProvider.state(mainState);


                angular.forEach($rootScope.modules["docsetting"], function (state) {
                    
                    var s = {
                        tab: state.Tab,
                        text: state.Text,
                        system: state.System,
                        deepStateRedirect: true,
                        sticky: true,
                        name: 'docsetting.' + state.Name,
                        url: '/' + state.Name,
                        views: {},
                        
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['docsetting.content.' + state.Name] = {
                        templateUrl: "apps/system2/docsetting/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "docsetting.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system2/docsetting/' + state.Name + '/' + state.Name + '.controller')
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
                                deepStateRedirect: true,
                                sticky: true,
                                url: '/' + subState.Name,
                                templateUrl: "apps/system2/docsetting/" + state.Name + "/" + subState.Name + "/view/" + subState.Name + ".html",
                                controller: "docsetting.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load('apps/system2/docsetting/' + state.Name + '/' + subState.Name + '/' + subState.Name + '.controller')
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

        app.controller("docsetting.sidebar.controller", function ($scope, $rootScope, $state) {


        });

        app.controller("docsetting.content.controller", function ($scope, $rootScope) {
        });

        app.controller("docsetting.header.controller", function ($scope, $rootScope) {
        });

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

