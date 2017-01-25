define(['angularAMD'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_office",
            ['ui.router'])

        // 本App的主路由
        var mainState = {
            name: 'office',
            url: '/system3/office',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'office.Sidebar@': {
                    templateUrl: 'apps/system3/office/view/office-silder.html'
                },
                'office.Content@': {
                    templateUrl: 'apps/system3/office/view/office.html'
                },
            }
        }

        var _$stateProvider = undefined;

        // app的配置阶段
        app.config(['$stateProvider',
            function ($stateProvider) {

                // 将路由添加到路由表中
                _$stateProvider = $stateProvider;

            }]);

        //app运行时
        app.run(["$rootScope",
            function ($rootScope) {
                // 将路由添加到路由表中
                mainState.system = $rootScope.modulesSys["office"];
                _$stateProvider.state(mainState);

                angular.forEach($rootScope.modules["office"], function (state) {

                    var s = {
                        system: state.System,
                        name: 'office.' + state.Name,
                        url: '/' + state.Name,

                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['office.content.' + state.Name] = {
                        templateUrl: "apps/system3/office/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "office.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system3/office/' + state.Name + '/' + state.Name + '.controller')
                        }
                    };

                    _$stateProvider.state(s);

                    // 子路由
                    if (state.SubModules) {
                        angular.forEach(state.SubModules, function (subState) {
                            var ss = {
                                name: s.name + '.' + subState.Name,
                                url: '/' + subState.Name,

                                templateUrl: "apps/system3/office/" + state.Name +  "/view/" + subState.Name + ".html",
                                controller: "office.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load("apps/system3/office/" + state.Name + "/" + state.Name + ".controller." + subState.Name)
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

