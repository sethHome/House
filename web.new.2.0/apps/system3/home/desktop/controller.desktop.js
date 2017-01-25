define(['angularAMD', 'apps/system3/home/home.controller',
    'apps/system3/production/provide/provide.controller.maintain',
    'apps/system3/office/news/news.service',
    'apps/system3/home/desktop/controller.quicklink',
    'apps/system3/home/desktop/desktop.service',
    'apps/system3/home/calendar/calendar.service'], function (angularAMD, app) {

        app.controller("home.controller.desktop", function ($rootScope, $scope, $uibModal,
            desktopService, calendarService, newsService, moment) {

            $scope.$watch('$viewContentLoaded', function () {

                //$.StartScreen();

                var tiles = $(".tile, .tile-small, .tile-sqaure, .tile-wide, .tile-large, .tile-big, .tile-super");

                $.each(tiles, function () {
                    var tile = $(this);
                    setTimeout(function () {
                        tile.css({
                            opacity: 1,
                            "-webkit-transform": "scale(1)",
                            "transform": "scale(1)",
                            "-webkit-transition": ".3s",
                            "transition": ".3s"
                        });
                    }, Math.floor(Math.random() * 500));
                });

                $(".tile-group").animate({
                    left: 0
                });
            });

            $scope.myStatistics = desktopService.getMyStatistics().$object;

            // 获取关注工程

            $scope.followEngs = [];

            desktopService.getFollowEngs().then(function (result) {

                angular.forEach(result.Source, function (item) {
                    item.Notes = desktopService.getFollowEngNotes(item.ID).$object;
                });

                $scope.followEngs = result.Source;
            })

            newsService.getNewsList({ pagesize: 10, pageindex: 1 }).then(function (result) {

                $scope.news = result.Source.where(function (item) { return item.Type != 1 });

                $scope.gonggao = result.Source.find(function (item) { return item.Type == 1 });
            });

            $scope.myTodayEvent = calendarService.getMyCalendar({ today: true }).$object;

            $scope.myTaskCount = desktopService.getMyTaskCount().$object;

            $scope.setContent = function (content) {
                if (content) {
                    var dd = content.replace(/<\/?.+?>/g, "");
                    return dd.replace(/&nbsp;/g, "");//dds为得到后的内容 
                }
            }

            $scope.setPage = function () {

                var tiles = $(".tile, .tile-small, .tile-sqaure, .tile-wide, .tile-large, .tile-big, .tile-super");

                $.each(tiles, function () {
                    var tile = $(this);
                    setTimeout(function () {
                        tile.css({
                            opacity: 1,
                            "-webkit-transform": "scale(1)",
                            "transform": "scale(1)",
                            "-webkit-transition": ".3s",
                            "transition": ".3s"
                        });
                    }, Math.floor(Math.random() * 500));
                });

                $(".tile-group").animate({
                    left: 0
                });
            }

            $scope.eventBegin = function (event) {

                if (PmEx) {
                    var typeStr = $scope.DayEventType_enum[event.Type];
                    PmEx.Notify("今日日程", typeStr, event.Title);
                } else {
                    $scope.$safeApply(function () {
                        event.isBegin = true;
                        if ($scope.beginShow) {
                            $uibModal.open({
                                animation: false,
                                templateUrl: 'apps/system3/home/desktop/view/event-show.html',
                                size: 'topfull',

                                controller: function ($scope, $uibModalInstance, event) {
                                    $scope.event = event;
                                    $scope.close = function () {
                                        $uibModalInstance.dismiss('cancel');
                                    }
                                },
                                resolve: {
                                    event: function () {
                                        return event;
                                    }
                                }
                            });
                        }
                    })
                }

                
            }

            $scope.isFinish = function (event) {

                return moment() > moment(event.EndTime);
            }

            $scope.eventLoaded = function () {
                $scope.eventScroll.init();
                $scope.beginShow = true;
            }

            $scope.$on("$ReloadUserTask", function (e, arg) {
                $scope.loadTasks();
            });

            // 打开提资审核窗口
            $scope.openProvideModel = function (task) {

                $scope.modalInstance = $uibModal.open({
                    animation: false,
                    size: 'lg',
                    windowTopClass: 'fade',
                    templateUrl: 'apps/system3/production/provide/view/provide-maintain.html',
                    controller: 'production.controller.provide.maintain',
                    resolve: {
                        provideParams: function () {
                            return {
                                provideInfo: task.SpecialtyProvide,
                                task: task,
                                isEdit: true,
                                modelType: 'window',
                            }
                        },

                    }
                });

                $scope.modalInstance.result.then(function () {
                    $scope.provideTasks = {};
                    $scope.provideTasks = desktopService.getMyProvideTasks().$object;
                }, function () {
                    //dismissed
                });
            }

            // 打开表单流转窗口
            $scope.openFormModel = function (task) {

                var info = {};
                var objItem = $rootScope.getBaseData("Object").find(function (item) { return item.Tags["name"] == task.ObjectKey });
                info.controller = objItem.Tags["controller"];
                info.template = objItem.Tags["template"];
                info.controllerUrl = objItem.Tags["controllerUrl"];

                //angular.forEach($rootScope.Object_item, function (item) {
                //    if (item.Tags["name"] == task.ObjectKey) {
                //        info.controller = item.Tags["controller"];
                //        info.template = item.Tags["template"];
                //        info.controllerUrl = item.Tags["controllerUrl"];
                //    }
                //});

                $scope.modalInstance = $uibModal.open({
                    animation: false,
                    size: 'lg',
                    windowTopClass: 'fade',
                    templateUrl: info.template,
                    controller: info.controller,
                    resolve: {
                        loadController: angularAMD.$load(info.controllerUrl),
                        modelParam: function () {
                            return {
                                objID: task.ObjectID,
                                taskID: task.ID
                            }
                        },
                    }
                });

                $scope.modalInstance.result.then(function () {
                    $scope.formTasks = {};
                    $scope.formTasks = desktopService.getMyFormTasks().$object;
                }, function () {
                    //dismissed
                });
            }

            $scope.myQuickLinks = [];
            $scope.myMenus = [];
            $scope.getMyMenus = function (menus, icon, bg) {
                angular.forEach(menus, function (menu) {
                    if (menu.Href) {
                        var link = {
                            key: menu.Key,
                            text: menu.Text,
                            icon: menu.Icon ? menu.Icon : icon,
                            bg: bg,
                            href: menu.Href,
                            param: menu.Param
                        };

                        if (menu.IsFavorite) {
                            link.selected = true;
                            $scope.myQuickLinks.push(link);
                        }

                        $scope.myMenus.push(link);
                    }

                    if (menu.SubMenus && menu.SubMenus.length > 0) {
                        $scope.getMyMenus(menu.SubMenus, menu.Icon ? menu.Icon : icon, bg);
                    }
                });
            }
            var menuBgs = {
                'business': 'bg-green',
                'demo': 'bg-aero',
                'home': 'bg-blue',
                'production': 'bg-purple',
                'statistics': 'bg-pink',
                'system': 'bg-orange',
                'office': 'bg-purple',
                'document': 'bg-dark',
                'mail': 'bg-yellow',
            };

            angular.forEach($rootScope.currentBusiness.Menus, function (menu) {
                $scope.getMyMenus(menu.SubMenus, menu.Icon, menuBgs[menu.Href]);
            });

            $scope.openQuickLinkMaintain = function () {

                $uibModal.open({
                    animation: true,
                    templateUrl: 'apps/system3/home/desktop/view/quick-link.html',
                    controller: 'quickLinkController',
                    windowTopClass: 'fade modal-slideleft',
                    resolve: {
                        myMenus: function () {
                            return $scope.myMenus;
                        },
                        myQuickLinks: function () {
                            return $scope.myQuickLinks
                        }
                    }
                });
            };
        });
    });
