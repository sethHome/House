define(['angularAMD'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_docfile",
            ['ui.router'])

        // 本App的主路由
        var mainState = {
            name: 'docfile',
            url: '/system2/docfile',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'docfile.Sidebar@': {
                    controller: 'docfile.sidebar.controller',
                    templateUrl: 'apps/system2/docfile/view/silder.html'
                },
                'docfile.Content@': {
                    controller: 'docfile.content.controller',
                    templateUrl: 'apps/system2/docfile/view/main.html'
                },

            }
        }

        var _$stateProvider = undefined;

        // app的配置阶段
        app.config(['$stateProvider','$compileProvider',
            function ($stateProvider,$compileProvider) {

                //$compileProvider.debugInfoEnabled(true); // Remove debug info (angularJS >= 1.3)

                _$stateProvider = $stateProvider;
               

            }]);

        //app运行时
        app.run(["$rootScope", "$state",
            function ($rootScope, $state) {

                // 将路由添加到路由表中
                mainState.system = $rootScope.modulesSys["docfile"];
                _$stateProvider.state(mainState);


                angular.forEach($rootScope.modules["docfile"], function (state) {

                    var s = {
                        tab: true,
                        text: state.Text,
                        deepStateRedirect: true,
                        sticky: true,
                        system: state.System,
                        name: 'docfile.' + state.Name,
                        url: '/' + state.Name,
                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['docfile.content.' + state.Name] = {
                        templateUrl: "apps/system2/docfile/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "docfile.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system2/docfile/' + state.Name + '/' + state.Name + '.controller')
                        }
                    };

                    _$stateProvider.state(s);

                    // 子路由
                    if (state.SubModules) {
                        angular.forEach(state.SubModules, function (subState) {
                            var ss = {
                                tab: true,
                                text: state.Text,
                                deepStateRedirect: true,
                                sticky: true,
                                system: subState.System,
                                name: s.name + '.' + subState.Name,
                                url: '/' + subState.Name,
                                templateUrl: "apps/system2/docfile/" + state.Name + "/" + subState.Name + "/view/" + subState.Name + ".html",
                                controller: "docfile.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load('apps/system2/docfile/' + state.Name + '/' + subState.Name + '/' + subState.Name + '.controller')
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

        app.controller("docfile.sidebar.controller", function ($scope, $rootScope, $state) {


        });

        app.controller("docfile.content.controller", function ($scope, $rootScope) {
        });

        app.controller("docfile.header.controller", function ($scope, $rootScope) {
        });

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

