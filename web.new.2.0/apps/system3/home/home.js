define(['angularAMD',
    'ngload!apps/system3/home/home.controller',
    'ngload!apps/system3/office/office'],

    function (angularAMD) {
        'use strict';
       
        // 定义App
        var app = angular.module("module_home",
            ['ui.router', 'module_home_controller', 'module_office'])

        // 本App的主路由
        var mainState = {
            name: 'home',
            url: '/system3/home',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'home.Sidebar@': {
                    controller: 'home.sidebar.controller',
                    templateUrl: 'apps/system3/home/view/home-silder.html'
                },
                'home.MegaMenu@': {
                    templateUrl: 'apps/system3/home/view/home-mega.html'
                },
                'home.Content@': {
                    controller: 'home.content.controller',
                    templateUrl: 'apps/system3/home/view/home.html'
                },
                'header-menu@': {
                    controller: 'home.header.controller',
                    templateUrl: 'apps/system3/home/view/home-header-menu.html'
                },
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
                mainState.system = $rootScope.modulesSys["home"];
                _$stateProvider.state(mainState);

                angular.forEach($rootScope.modules["home"], function (state) {

                    var s = {
                        tab: true,
                        text: state.Text,
                        system: state.System,
                        name: 'home.' + state.Name,
                        url: '/' + state.Name,
                        deepStateRedirect: true,
                        sticky: true,
                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['home.content.' + state.Name] = {
                        templateUrl: "apps/system3/home/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "home.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system3/home/' + state.Name + '/controller.' + state.Name)
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

