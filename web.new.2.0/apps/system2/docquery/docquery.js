define(['angularAMD'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_docquery",
            ['ui.router'])

        // 本App的主路由
        var mainState = {
            name: 'docquery',
            url: '/system2/docquery',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'docquery.Sidebar@': {
                    controller: 'docquery.sidebar.controller',
                    templateUrl: 'apps/system2/docquery/view/silder.html'
                },
                'docquery.Content@': {
                    controller: 'docquery.content.controller',
                    templateUrl: 'apps/system2/docquery/view/main.html'
                },
                'header-menu@': {
                    controller: 'docquery.header.controller',
                    templateUrl: 'apps/system2/docquery/view/header.html'
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
                mainState.system = $rootScope.modulesSys["docquery"];
                _$stateProvider.state(mainState);


                angular.forEach($rootScope.modules["docquery"], function (state) {

                    var s = {
                        tab: true,
                        text: state.Text,
                        system: state.System,
                        name: 'docquery.' + state.Name,
                        url: '/' + state.Name,
                        deepStateRedirect: true,
                        sticky: true,
                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['docquery.content.' + state.Name] = {
                        templateUrl: "apps/system2/docquery/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "docquery.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system2/docquery/' + state.Name + '/' + state.Name + '.controller')
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
                                templateUrl: "apps/system2/docquery/" + state.Name + "/" + subState.Name + "/view/" + subState.Name + ".html",
                                controller: "docquery.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load('apps/system2/docquery/' + state.Name + '/' + subState.Name + '/' + subState.Name + '.controller')
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

        app.controller("docquery.sidebar.controller", function ($scope, $rootScope, $state) {


        });

        app.controller("docquery.content.controller", function ($scope, $rootScope) {

         

        });

        app.controller("docquery.header.controller", function ($scope, $rootScope) {


            $scope.removeArchiveItem = function (item) {
                $rootScope.archiveBorrowList.removeObj(item);
            }
           
        });

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

