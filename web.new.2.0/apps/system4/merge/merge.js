define(['angularAMD'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_docfile",
            ['ui.router'])

        // 本App的主路由
        var mainState = {
            name: 'merge',
            url: '/system4/merge',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'merge.Sidebar@': {
                    controller: 'merge.sidebar.controller',
                    templateUrl: 'apps/system4/merge/view/silder.html'
                },
                'merge.Content@': {
                    controller: 'merge.content.controller',
                    templateUrl: 'apps/system4/merge/view/main.html'
                },
                'header-menu@': {
                    template: ''
                },
            }
        }

        var _$stateProvider = undefined;

        // app的配置阶段
        app.config(['$stateProvider', '$compileProvider',
            function ($stateProvider, $compileProvider) {

                //$compileProvider.debugInfoEnabled(true); // Remove debug info (angularJS >= 1.3)

                _$stateProvider = $stateProvider;


            }]);

        //app运行时
        app.run(["$rootScope", "$state",
            function ($rootScope, $state) {

                // 将路由添加到路由表中
                mainState.system = $rootScope.modulesSys["merge"];
                _$stateProvider.state(mainState);


                angular.forEach($rootScope.modules["merge"], function (state) {

                    var s = {
                        tab: true,
                        text: state.Text,
                        system: state.System,
                        name: 'merge.' + state.Name,
                        url: '/' + state.Name,

                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['merge.content.' + state.Name] = {
                        templateUrl: "apps/system4/merge/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "merge.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system4/merge/' + state.Name + '/' + state.Name + '.controller')
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
                                templateUrl: "apps/system4/merge/" + state.Name + "/" + subState.Name + "/view/" + subState.Name + ".html",
                                controller: "merge.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load('apps/system4/merge/' + state.Name + '/' + subState.Name + '/' + subState.Name + '.controller')
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

        app.controller("merge.sidebar.controller", function ($scope, $rootScope, $state) {
        });

        app.controller("merge.content.controller", function ($scope, $rootScope) {
        });

        app.controller("merge.header.controller", function ($scope, $rootScope) {
        });

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

