define(['angularAMD'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_document",
            ['ui.router', 'module_document_controller'])

        // 本App的主路由
        var mainState = {
            name: 'document',
            url: '/system3/document',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'document.Sidebar@': {
                    controller: 'document.sidebar.controller',
                    templateUrl: 'apps/system3/document/view/document-silder.html'
                },
                'document.Content@': {
                    controller: 'document.content.controller',
                    templateUrl: 'apps/system3/document/view/document.html'
                },
                'header-menu@': {
                    controller: 'document.header.controller',
                    templateUrl: 'apps/system3/document/view/document-header-menu.html'
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
                mainState.system = $rootScope.modulesSys["document"];
                _$stateProvider.state(mainState);


                angular.forEach($rootScope.modules["document"], function (state) {

                    var s = {
                        tab: true,
                        text: state.Text,
                        system: state.System,
                        name: 'document.' + state.Name,
                        url: '/' + state.Name,

                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['document.content.' + state.Name] = {
                        templateUrl: "apps/system3/document/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "document.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system3/document/' + state.Name + '/' + state.Name + '.controller')
                        }
                    };

                    _$stateProvider.state(s);

                });
            }]);

        app.controller("document.sidebar.controller", function ($scope, $rootScope,$state) {

            $scope.querys = [
                { id: 1, Text: '工程类型', Name:'EngineeringType', Items: $rootScope.getBaseData("EngineeringType") },
                { id: 2, Text: '工程阶段', Name: 'EngineeringPhase', Items: $rootScope.getBaseData("EngineeringPhase")},
                { id: 3, Text: '电压等级', Name: 'VolLev', Items: $rootScope.getBaseData("VolLev")},
                {
                    id: 1, Text: '工程时间', Name: 'DateRange', Items: [
                        { Value: 1, Text: '最近一个月' },
                        { Value: 2, Text: '最近三个月' },
                        { Value: 3, Text: '最近半年' },
                        { Value: 4, Text: '一年内' },
                        { Value: 5, Text: '一年前' }
                    ]
                },
            ];

            $scope.clear = function () {
                angular.forEach($scope.querys, function (item) {
                    item.currentMenu = undefined;
                });
                $rootScope.$broadcast("$document_query", {});
            }

            $scope.goto = function (parentMenu, menu) {
                
                parentMenu.currentMenu = menu;
                $scope.currentParentMenu = parentMenu;

                var filter = {};
                angular.forEach($scope.querys, function (item) {
                    if (item.currentMenu) {
                        filter[item.Name] = item.currentMenu.Value;
                    }
                });

                $rootScope.$broadcast("$document_query", filter);
            }
        });

        app.controller("document.content.controller", function ($scope,$state, $rootScope) {
            
            if ($state.current.name == "document") {
                $state.go("document.query");
            }
        });

        app.controller("document.header.controller", function ($scope, $rootScope) {
        });

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

