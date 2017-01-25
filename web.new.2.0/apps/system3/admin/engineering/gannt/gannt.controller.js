define([
'apps/system3/admin/admin',
'apps/system3/admin/admin.service',
'css!assets/bower_components/angular-motion/dist/angular-motion',
'css!assets/bower_components/angular-ui-tree/dist/angular-ui-tree.min',
'css!assets/bower_components/angular-gantt-1.2.6/assets/angular-gantt',
'css!assets/bower_components/angular-gantt-1.2.6/assets/angular-gantt-plugins'], function (app) {

    app.module.service('taskStateColor', function Sample() {
        return {
            0: '#D2BDBD',   // empty
            1: '#F1C232',   // waiting
            2: '#3C8CF8'    // done
        };
    });

    app.module.service('Sample', function Sample() {
        return {
            getSampleData: function() {
                return [
                        // Order is optional. If not specified it will be assigned automatically
                        {name: 'Milestones', height: '3em', sortable: false, classes: 'gantt-row-milestone', color: '#45607D', tasks: [
                            // Dates can be specified as string, timestamp or javascript date object. The data attribute can be used to attach a custom object
                            {name: 'Kickoff', color: '#93C47D', from: '2013-10-07T09:00:00', to: '2013-10-07T10:00:00', data: 'Can contain any custom data or object'},
                            {name: 'Concept approval', color: '#93C47D', from: new Date(2013, 9, 18, 18, 0, 0), to: new Date(2013, 9, 18, 18, 0, 0), est: new Date(2013, 9, 16, 7, 0, 0), lct: new Date(2013, 9, 19, 0, 0, 0)},
                            {name: 'Development finished', color: '#93C47D', from: new Date(2013, 10, 15, 18, 0, 0), to: new Date(2013, 10, 15, 18, 0, 0)},
                            {name: 'Shop is running', color: '#93C47D', from: new Date(2013, 10, 22, 12, 0, 0), to: new Date(2013, 10, 22, 12, 0, 0)},
                            {name: 'Go-live', color: '#93C47D', from: new Date(2013, 10, 29, 16, 0, 0), to: new Date(2013, 10, 29, 16, 0, 0)}
                        ], data: 'Can contain any custom data or object'},
                        {name: 'Status meetings', tasks: [
                            {name: 'Demo #1', color: '#9FC5F8', from: new Date(2013, 9, 25, 15, 0, 0), to: new Date(2013, 9, 25, 18, 30, 0)},
                            {name: 'Demo #2', color: '#9FC5F8', from: new Date(2013, 10, 1, 15, 0, 0), to: new Date(2013, 10, 1, 18, 0, 0)},
                            {name: 'Demo #3', color: '#9FC5F8', from: new Date(2013, 10, 8, 15, 0, 0), to: new Date(2013, 10, 8, 18, 0, 0)},
                            {name: 'Demo #4', color: '#9FC5F8', from: new Date(2013, 10, 15, 15, 0, 0), to: new Date(2013, 10, 15, 18, 0, 0)},
                            {name: 'Demo #5', color: '#9FC5F8', from: new Date(2013, 10, 24, 9, 0, 0), to: new Date(2013, 10, 24, 10, 0, 0)}
                        ]},
                        {name: 'Kickoff', movable: {allowResizing: false}, tasks: [
                            {name: 'Day 1', color: '#9FC5F8', from: new Date(2013, 9, 7, 9, 0, 0), to: new Date(2013, 9, 7, 17, 0, 0),
                                progress: {percent: 100, color: '#3C8CF8'}, movable: false},
                            {name: 'Day 2', color: '#9FC5F8', from: new Date(2013, 9, 8, 9, 0, 0), to: new Date(2013, 9, 8, 17, 0, 0),
                                progress: {percent: 100, color: '#3C8CF8'}},
                            {name: 'Day 3', color: '#9FC5F8', from: new Date(2013, 9, 9, 8, 30, 0), to: new Date(2013, 9, 9, 12, 0, 0),
                                progress: {percent: 100, color: '#3C8CF8'}}
                        ]},
                        {name: 'Create concept', tasks: [
                            {name: 'Create concept', content: '<i class="fa fa-cog" ng-click="scope.handleTaskIconClick(task.model)"></i> {{task.model.name}}', color: '#F1C232', from: new Date(2013, 9, 10, 8, 0, 0), to: new Date(2013, 9, 16, 18, 0, 0), est: new Date(2013, 9, 8, 8, 0, 0), lct: new Date(2013, 9, 18, 20, 0, 0),
                                progress: 100}
                        ]},
                        {name: 'Finalize concept', tasks: [
                            {name: 'Finalize concept', color: '#F1C232', from: new Date(2013, 9, 17, 8, 0, 0), to: new Date(2013, 9, 18, 18, 0, 0),
                                progress: 100}
                        ]},
                        {name: 'Development', children: ['Sprint 1', 'Sprint 2', 'Sprint 3', 'Sprint 4'], content: '<i class="fa fa-file-code-o" ng-click="scope.handleRowIconClick(row.model)"></i> {{row.model.name}}'},
                        {name: 'Sprint 1', tooltips: false, tasks: [
                            {name: 'Product list view', color: '#F1C232', from: new Date(2013, 9, 21, 8, 0, 0), to: new Date(2013, 9, 25, 15, 0, 0),
                                progress: 25}
                        ]},
                        {name: 'Sprint 2', tasks: [
                            {name: 'Order basket', color: '#F1C232', from: new Date(2013, 9, 28, 8, 0, 0), to: new Date(2013, 10, 1, 15, 0, 0)}
                        ]},
                        {name: 'Sprint 3', tasks: [
                            {name: 'Checkout', color: '#F1C232', from: new Date(2013, 10, 4, 8, 0, 0), to: new Date(2013, 10, 8, 15, 0, 0)}
                        ]},
                        {name: 'Sprint 4', tasks: [
                            {name: 'Login & Signup & Admin Views', color: '#F1C232', from: new Date(2013, 10, 11, 8, 0, 0), to: new Date(2013, 10, 15, 15, 0, 0)}
                        ]},
                        {name: 'Hosting'},
                        {name: 'Setup', tasks: [
                            {name: 'HW', color: '#F1C232', from: new Date(2013, 10, 18, 8, 0, 0), to: new Date(2013, 10, 18, 12, 0, 0)}
                        ]},
                        {name: 'Config', tasks: [
                            {name: 'SW / DNS/ Backups', color: '#F1C232', from: new Date(2013, 10, 18, 12, 0, 0), to: new Date(2013, 10, 21, 18, 0, 0)}
                        ]},
                        {name: 'Server', parent: 'Hosting', children: ['Setup', 'Config']},
                        {name: 'Deployment', parent: 'Hosting', tasks: [
                            {name: 'Depl. & Final testing', color: '#F1C232', from: new Date(2013, 10, 21, 8, 0, 0), to: new Date(2013, 10, 22, 12, 0, 0), 'classes': 'gantt-task-deployment'}
                        ]},
                        {name: 'Workshop', tasks: [
                            {name: 'On-side education', color: '#F1C232', from: new Date(2013, 10, 24, 9, 0, 0), to: new Date(2013, 10, 25, 15, 0, 0)}
                        ]},
                        {name: 'Content', tasks: [
                            {name: 'Supervise content creation', color: '#F1C232', from: new Date(2013, 10, 26, 9, 0, 0), to: new Date(2013, 10, 29, 16, 0, 0)}
                        ]},
                        {name: 'Documentation', tasks: [
                            {name: 'Technical/User documentation', color: '#F1C232', from: new Date(2013, 10, 26, 8, 0, 0), to: new Date(2013, 10, 28, 18, 0, 0)}
                        ]}
                ];
            },
            getSampleTimespans: function() {
                return [
                        {
                            from: new Date(2013, 9, 21, 8, 0, 0),
                            to: new Date(2013, 9, 25, 15, 0, 0),
                            name: 'Sprint 1 Timespan'
                            //priority: undefined,
                            //classes: [],
                            //data: undefined
                        }
                ];
            }
        };
    })
    app.module.controller('admin.controller.engineering.gannt', ['$scope', '$timeout', '$log', 'ganttUtils', 'GanttObjectModel', 'adminService', 'ganttMouseOffset', 'ganttDebounce', 'moment','taskStateColor',
        function ($scope, $timeout, $log, ganttUtils, GanttObjectModel, adminService, ganttMouseOffset, ganttDebounce, moment, taskStateColor) {
        var objectModel;
        var dataToRemove;

        $scope.options = {
            mode: 'custom',
            scale: 'day',
            sortMode: undefined,
            sideMode: 'Tree',
            daily: false,
            maxHeight: false,
            width: false,
            zoom: 1,
            tooltips: {
                content: '<i class="fa fa-user" /> {{task.model.user | enumMap:"user"}}&nbsp;{{task.model.name}} </br>' +
                    '<small>' +
                    '<i class="fa fa-clock-o" /> {{task.isMilestone() === true && getFromLabel() || getFromLabel() + \' - \' + getToLabel()}}' +
                    '</small>'
            },
            columns: ['model.name', 'from', 'to'],
            treeTableColumns: ['from', 'to'],
            columnsHeaders: { 'model.name': '名称', 'from': '开始日期', 'to': '结束日期' },
            columnsClasses: { 'model.name': 'gantt-column-name', 'from': 'gantt-column-from', 'to': 'gantt-column-to' },
            columnsFormatters: {
                'from': function (from) {
                    return from !== undefined ? from.format('lll') : undefined;
                },
                'to': function (to) {
                    return to !== undefined ? to.format('lll') : undefined;
                }
            },
            treeHeaderContent: '<i class="fa fa-align-justify"></i> {{getHeader()}}',
            columnsHeaderContents: {
                'model.name': '<i class="fa fa-align-justify"></i> {{getHeader()}}',
                'from': '<i class="fa fa-calendar"></i> {{getHeader()}}',
                'to': '<i class="fa fa-calendar"></i> {{getHeader()}}'
            },
            autoExpand: 'none',
            taskOutOfRange: 'truncate',
            fromDate: moment(null),
            toDate: undefined,
            rowContent: '<i class="fa fa-align-justify"></i> {{row.model.name}}',
            taskContent: '<i class="fa fa-tasks"></i> {{task.model.name}}',
            allowSideResizing: true,
            labelsEnabled: true,
            currentDate: 'column',
            currentDateValue: new Date(),
            draw: false,
            readOnly: false,
            groupDisplayMode: 'group',
            filterTask: '',
            filterRow: '',
            timeFrames: {
                'day': {
                    start: moment('8:00', 'HH:mm'),
                    end: moment('20:00', 'HH:mm'),
                    working: true,
                    default: true
                },
                'noon': {
                    start: moment('12:00', 'HH:mm'),
                    end: moment('13:30', 'HH:mm'),
                    working: false,
                    default: true
                },
                'weekend': {
                    working: false
                },
                'holiday': {
                    working: false,
                    color: 'red',
                    classes: ['gantt-timeframe-holiday']
                }
            },
            dateFrames: {
                'weekend': {
                    evaluator: function (date) {
                        return date.isoWeekday() === 6 || date.isoWeekday() === 7;
                    },
                    targets: ['weekend']
                },
                '11-november': {
                    evaluator: function (date) {
                        return date.month() === 10 && date.date() === 11;
                    },
                    targets: ['holiday']
                }
            },
            timeFramesNonWorkingMode: 'visible',
            columnMagnet: '15 minutes',
            timeFramesMagnet: true,
            canDraw: function (event) {
                var isLeftMouseButton = event.button === 0 || event.button === 1;
                //$scope.options.draw && !$scope.options.readOnly &&
                return isLeftMouseButton;
            },
            drawTaskFactory: function () {
                return {
                    id: ganttUtils.randomUuid(),  // Unique id of the task.
                    name: 'Drawn task', // Name shown on top of each task.
                    color: '#AA8833' // Color of the task in HEX format (Optional).
                };
            },
            api: function (api) {
                // API Object is used to control methods and events from angular-gantt.
                $scope.api = api;

                api.core.on.ready($scope, function () {
                   

                    // When gantt is ready, load data.
                    // `data` attribute could have been used too.

                    $scope.$watch("currentEng", function (newval, oldval) {
                        if (newval) {
                            $scope.load(newval.ID);
                        }
                        
                    });

                    // Add some DOM events
                    api.directives.on.new($scope, function (directiveName, directiveScope, element) {
                        if (directiveName === 'ganttTask') {
                            element.bind('click', function (event) {
                                event.stopPropagation();
                                logTaskEvent('task-click', directiveScope.task);
                            });
                            element.bind('mousedown touchstart', function (event) {
                                event.stopPropagation();
                                $scope.live.row = directiveScope.task.row.model;
                                if (directiveScope.task.originalModel !== undefined) {
                                    $scope.live.task = directiveScope.task.originalModel;
                                } else {
                                    $scope.live.task = directiveScope.task.model;
                                }
                                $scope.$digest();
                            });
                        } else if (directiveName === 'ganttRow') {
                            element.bind('click', function (event) {
                                event.stopPropagation();
                                logRowEvent('row-click', directiveScope.row);
                            });
                            element.bind('mousedown touchstart', function (event) {
                                event.stopPropagation();
                                $scope.live.row = directiveScope.row.model;
                                $scope.$digest();
                            });
                        } else if (directiveName === 'ganttRowLabel') {
                            element.bind('click', function () {
                                logRowEvent('row-label-click', directiveScope.row);
                            });
                            element.bind('mousedown touchstart', function () {
                                $scope.live.row = directiveScope.row.model;
                                $scope.$digest();
                            });
                        }
                    });

                    api.tasks.on.rowChange($scope, function (task) {
                        $scope.live.row = task.row.model;
                    });

                    objectModel = new GanttObjectModel(api);
                });
            }
        };

        $scope.handleTaskIconClick = function (taskModel) {
            alert('Icon from ' + taskmodel.name + ' task has been clicked.');
        };

        $scope.handleRowIconClick = function (rowModel) {
            alert('Icon from ' + rowmodel.name + ' row has been clicked.');
        };

        $scope.expandAll = function () {
            $scope.api.tree.expandAll();
        };

        $scope.collapseAll = function () {
            $scope.api.tree.collapseAll();
        };

        $scope.$watch('options.sideMode', function (newValue, oldValue) {
            if (newValue !== oldValue) {
                $scope.api.side.setWidth(undefined);
                $timeout(function () {
                    $scope.api.columns.refresh();
                });
            }
        });

        $scope.canAutoWidth = function (scale) {
            if (scale.match(/.*?hour.*?/) || scale.match(/.*?minute.*?/)) {
                return false;
            }
            return true;
        };

        $scope.getColumnWidth = function (widthEnabled, scale, zoom) {
            if (!widthEnabled && $scope.canAutoWidth(scale)) {
                return undefined;
            }

            if (scale.match(/.*?week.*?/)) {
                return 150 * zoom;
            }

            if (scale.match(/.*?month.*?/)) {
                return 300 * zoom;
            }

            if (scale.match(/.*?quarter.*?/)) {
                return 500 * zoom;
            }

            if (scale.match(/.*?year.*?/)) {
                return 800 * zoom;
            }

            return 40 * zoom;
        };

        // Reload data action
        $scope.load = function (id) {
           
            adminService.getGanntSource(id).then(function (source) {

                var timespans = {
                    from: new Date(),
                    to: new Date(),
                    name: 'Sprint 1 Timespan'
                    //priority: undefined,
                    //classes: [],
                    //data: undefined
                }

                var ganttData = [];
                angular.forEach(source, function (item) {
                    var data = {
                        id: item.ID,
                        name: item.Name,
                        parent: item.Parent,
                    };

                    if (item.Tasks) {
                        data.tasks = [];

                        angular.forEach(item.Tasks, function (task) {

                            if (task.from < timespans.from) {
                                timespans.from = task.from;
                            }
                            if (task.to > timespans.to) {
                                timespans.to = task.to;
                            }
                            // todo
                            data.tasks.push({
                                name: task.Name,
                                user: task.User,
                                from: task.From == null ? undefined : task.From,
                                to: task.To == null ? undefined : task.To,
                                est: task.Est == null ? undefined : task.Est,
                                lct: task.Lct == null ? undefined : task.Lct,
                                color: taskStateColor[task.State],
                            });
                        });
                    }

                    //data.color = '#319DB5';

                    ganttData.push(data);
                });
                //timespans.from = timespans.from.setDate(timespans.from.getDate() + 1);
                //timespans.from.setDate(2);
                //timespans.to.setDate(2);
                //$scope.timespans = timespans;

                $scope.data = ganttData;
                dataToRemove = undefined;

            });

            //$scope.data = Sample.getSampleData();
            //dataToRemove = undefined;
            //$scope.timespans = Sample.getSampleTimespans();
        };

        $scope.reload = function () {
            $scope.load();
        };

        // Remove data action
        $scope.remove = function () {
            $scope.api.data.remove(dataToRemove);
        };

        // Clear data action
        $scope.clear = function () {
            $scope.data = [];
        };


        // Visual two way binding.
        $scope.live = {};

        var debounceValue = 1000;

        var listenTaskJson = ganttDebounce(function (taskJson) {
            if (taskJson !== undefined) {
                var task = angular.fromJson(taskJson);
                objectModel.cleanTask(task);
                var model = $scope.live.task;
                angular.extend(model, task);
            }
        }, debounceValue);
        $scope.$watch('live.taskJson', listenTaskJson);

        var listenRowJson = ganttDebounce(function (rowJson) {
            if (rowJson !== undefined) {
                var row = angular.fromJson(rowJson);
                objectModel.cleanRow(row);
                var tasks = row.tasks;

                delete row.tasks;
                var rowModel = $scope.live.row;

                angular.extend(rowModel, row);

                var newTasks = {};
                var i, l;

                if (tasks !== undefined) {
                    for (i = 0, l = tasks.length; i < l; i++) {
                        objectModel.cleanTask(tasks[i]);
                    }

                    for (i = 0, l = tasks.length; i < l; i++) {
                        newTasks[tasks[i].id] = tasks[i];
                    }

                    if (rowModel.tasks === undefined) {
                        rowModel.tasks = [];
                    }
                    for (i = rowModel.tasks.length - 1; i >= 0; i--) {
                        var existingTask = rowModel.tasks[i];
                        var newTask = newTasks[existingTask.id];
                        if (newTask === undefined) {
                            rowModel.tasks.splice(i, 1);
                        } else {
                            objectModel.cleanTask(newTask);
                            angular.extend(existingTask, newTask);
                            delete newTasks[existingTask.id];
                        }
                    }
                } else {
                    delete rowModel.tasks;
                }

                angular.forEach(newTasks, function (newTask) {
                    rowModel.tasks.push(newTask);
                });
            }
        }, debounceValue);
        $scope.$watch('live.rowJson', listenRowJson);

        $scope.$watchCollection('live.task', function (task) {
            $scope.live.taskJson = angular.toJson(task, true);
            $scope.live.rowJson = angular.toJson($scope.live.row, true);
        });

        $scope.$watchCollection('live.row', function (row) {
            $scope.live.rowJson = angular.toJson(row, true);
            if (row !== undefined && row.tasks !== undefined && row.tasks.indexOf($scope.live.task) < 0) {
                $scope.live.task = row.tasks[0];
            }
        });

        $scope.$watchCollection('live.row.tasks', function () {
            $scope.live.rowJson = angular.toJson($scope.live.row, true);
        });

        // Event utility function
        var addEventName = function (eventName, func) {
            return function (data) {
                return func(eventName, data);
            };
        };
    }]);
});
