define(['angularAMD', 'angular-gantt', 'angular-gantt-plugins'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_admin",
            ['ui.router', 
            'gantt', // angular-gantt.
            'gantt.sortable',
            'gantt.movable',
            'gantt.drawtask',
            'gantt.tooltips',
            'gantt.bounds',
            'gantt.progress',
            'gantt.table',
            'gantt.tree',
            'gantt.groups',
            'gantt.overlap',
            'gantt.resizeSensor',
            'mgcrea.ngStrap'])

        // 本App的主路由
        var mainState = {
            name: 'admin',
            url: '/system3/admin',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'admin.Sidebar@': {
                    controller: 'admin.sidebar.controller',
                    templateUrl: 'apps/system3/admin/view/admin-silder.html'
                },
                'admin.Content@': {
                    controller: 'admin.content.controller',
                    templateUrl: 'apps/system3/admin/view/admin.html'
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
                mainState.system = $rootScope.modulesSys["admin"];
                _$stateProvider.state(mainState);


                angular.forEach($rootScope.modules["admin"], function (state) {

                    var s = {
                        system: state.System,
                        name: 'admin.' + state.Name,
                        url: '/' + state.Name,

                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['admin.content.' + state.Name] = {
                        templateUrl: "apps/system3/admin/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "admin.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system3/admin/' + state.Name + '/' + state.Name + '.controller')
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
                                templateUrl: "apps/system3/admin/" + state.Name + "/" + subState.Name + "/view/" + subState.Name + ".html",
                                controller: "admin.controller." + state.Name + "." + subState.Name,
                                resolve: {
                                    loadController: angularAMD.$load('apps/system3/admin/' + state.Name + '/' + subState.Name + '/' + subState.Name + '.controller')
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

        app.controller("admin.sidebar.controller", function ($scope, $rootScope, $state) {


        });

        app.controller("admin.content.controller", function ($scope, $rootScope) {
        });

        app.controller("admin.header.controller", function ($scope, $rootScope) {
        });

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

