define(['angularAMD', 'jsBarcode'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_book",
            ['ui.router'])

        // 本App的主路由
        var mainState = {
            name: 'book',
            url: '/system2/book',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'book.Sidebar@': {
                    controller: 'book.sidebar.controller',
                    templateUrl: 'apps/system2/book/view/silder.html'
                },
                'book.Content@': {
                    controller: 'book.content.controller',
                    templateUrl: 'apps/system2/book/view/main.html'
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
                mainState.system = $rootScope.modulesSys["book"];
                _$stateProvider.state(mainState);


                angular.forEach($rootScope.modules["book"], function (state) {

                    var s = {
                        tab: true,
                        text: state.Text,
                        system: state.System,
                        name: 'book.' + state.Name,
                        url: '/' + state.Name,
                        deepStateRedirect: true,
                        sticky: true,
                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['book.content.' + state.Name] = {
                        templateUrl: "apps/system2/book/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "book.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system2/book/' + state.Name + '/' + state.Name + '.controller')
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
                                templateUrl: "apps/system2/book/" + state.Name + "/" + subState.Name + "/view/" + subState.Name + ".html",
                                controller: "book.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load('apps/system2/book/' + state.Name + '/' + subState.Name + '/' + subState.Name + '.controller')
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

        app.controller("book.sidebar.controller", function ($scope, $rootScope, $state) {


        });

        app.controller("book.content.controller", function ($scope, $rootScope) {



        });

        app.controller("book.header.controller", function ($scope, $rootScope) {


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

