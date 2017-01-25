define(['angularAMD',
    'angular-ui-grid',
    'calendar','timer',
    'ngload!apps/system3/production/production.controller',
    'css!apps/system3/home/home'], function (angularAMD) {

        var controller = angular.module("module_home_controller",
            ['ui.grid',
            'ui.grid.resizeColumns',
            'ui.grid.selection',
            'ui.grid.treeView',
            'ui.grid.edit',
            'ui.grid.cellNav',
            'mwl.calendar', 'ui.bootstrap',
            'base.service','timer',
            'module_production_controller']);

        controller.controller("home.sidebar.controller", ['$scope', function ($scope) {
            
        }]);

        controller.controller("home.content.controller", function ($scope, $state, $rootScope, calendarConfig, moment) {
            
            calendarConfig.dateFormatter = 'moment';
            calendarConfig.i18nStrings = {
                eventsLabel: '日程',
                timeLabel: '时间',
                weekNumber: '第{week}周'
            },

            moment.locale('zh-cn');


            if ($state.current.name == "home") {
                $state.go("home.desktop");
            }
        });

        controller.controller("home.header.controller", function ($scope, $rootScope) {


        });

        angularAMD.processQueue();

        return controller;
    });
