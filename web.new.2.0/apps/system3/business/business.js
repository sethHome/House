define(['angularAMD',
    'css!apps/system3/business/css/business'],

    function (angularAMD) {
        'use strict';

        // 定义App
        var app = angular.module("module_business",
            ['ui.router', 'module_business_controller'])

        // 本App的主路由
        var mainState = {
            name: 'business',
            url: '/system3/business',
            deepStateRedirect: true,
            sticky: true,
            views: {
                'business.Sidebar@': {
                    controller: 'business.sidebar.controller',
                    templateUrl: 'apps/system3/business/view/business-silder.html'
                },
                'business.Content@': {
                    controller: 'business.content.controller',
                    templateUrl: 'apps/system3/business/view/business.html'
                },
                'header-menu@': {
                    controller: 'business.header.controller',
                    templateUrl: 'apps/system3/business/view/business-header-menu.html'
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
                mainState.system = $rootScope.modulesSys["business"];
                _$stateProvider.state(mainState);
                
                var subStateView = {
                    bid: {
                        maintain: {
                            maintain: {
                                templateUrl: "apps/system3/business/bid/view/bid-maintain.html",
                                controller: "business.controller.bid.maintain",
                                resolve: {
                                    loadController: angularAMD.$load('apps/system3/business/bid/bid.controller.maintain')
                                }
                            }
                        }
                    },
                    contract: {
                        maintain:{
                            maintain: {
                                templateUrl: "apps/system3/business/contract/view/contract-maintain.html",
                                controller: "business.controller.contract.maintain",
                                resolve: {
                                    loadController: angularAMD.$load('apps/system3/business/contract/contract.controller.maintain')
                                }
                            }
                        },
                        payee: {
                            payee: {
                                templateUrl: "apps/system3/business/contract/view/contract-payee.html",
                                controller: "business.controller.contract.payee",
                                resolve: {
                                    loadController: angularAMD.$load('apps/system3/business/contract/contract.controller.payee')
                                }
                            }
                        }
                    },
                    customer: {
                        maintain: {
                            maintain: {
                                templateUrl: "apps/system3/business/customer/view/customer-maintain.html",
                                controller: "business.controller.customer.maintain",
                                resolve: {
                                    loadController: angularAMD.$load('apps/system3/business/customer/customer.controller.maintain')
                                }
                            }
                        }
                    },
                    project: {
                        maintain: {
                            maintain: {
                                templateUrl: "apps/system3/business/project/view/project-maintain.html",
                                controller: $rootScope.maintainModel == 1 ? "business.controller.project.maintain" : undefined,
                                resolve: {
                                    loadController: angularAMD.$load('apps/system3/business/project/project.controller.maintain')
                                }
                            }
                        }
                    },
                    engineering: {
                        maintain: {
                            maintain: {
                                templateUrl: "apps/system3/business/engineering/view/engineering-maintain.html",
                                controller: $rootScope.maintainModel == 1 ? "business.controller.engineering.maintain" : undefined,
                                resolve: {
                                    loadController: angularAMD.$load('apps/system3/business/engineering/engineering.controller.maintain')
                                }
                            }
                        }
                    },
                }

                angular.forEach($rootScope.modules["business"], function (state) {
                    
                    var s = {
                        tab: true,
                        text: state.Text,
                        system: state.System,
                        name: 'business.' + state.Name,
                        url: '/' + state.Name,
                        views: {}
                    };

                    if (state.Param) {
                        s.url += state.Param;
                    };

                    s.views['business.content.' + state.Name] = {
                        templateUrl: "apps/system3/business/" + state.Name + "/view/" + state.Name + ".html",
                        controller: "business.controller." + state.Name,
                        resolve: {
                            loadController: angularAMD.$load('apps/system3/business/' + state.Name + '/' + state.Name + '.controller')
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
                                deepStateRedirect: true,
                                sticky: true,
                            };

                            if (subState.Param) {
                                ss.url += subState.Param;
                            };

                            ss.views = subStateView[state.Name][subState.Name];

                            _$stateProvider.state(ss);
                        });
                    }
                });
            }]);

        app.controller("business.sidebar.controller", ['$scope',  function ($scope) {

            //$scope.applicationService.handSilderHover();
        }]);

        app.controller("business.content.controller", function ($scope,$state, $rootScope) {
            
            if ($state.current.name == "business") {
                $state.go("business.engineering", {phase:2});
            }
        });

        app.controller("business.header.controller", function ($scope, $rootScope) {
        });

        angularAMD.processQueue();

        // 返回app和主路由
        return {
            mainState: mainState,
            module: app
        };
    });

