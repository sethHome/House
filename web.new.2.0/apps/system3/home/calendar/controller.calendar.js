define(['apps/system3/home/home.controller', 'apps/system3/home/calendar/calendar.service'], function (app) {

    app.controller("home.controller.calendar", function ($scope, calendarService, $uibModal, moment) {

        $scope.externalEvents = [];
        $scope.events = [];
        $scope.calendarView = 'month';
        $scope.viewDate = new Date();
        $scope.isCellOpen = true;
        $scope.viewChangeEnabled = true;

        $scope.addEvent = function () {
            
            var modalInstance = $uibModal.open({
                animation: false,
                windowTopClass: 'fade',
                templateUrl: 'apps/system3/home/calendar/view/calendar-add.html',
                controller: function ($scope, $uibModalInstance) {
                    $scope.start = start;
                    $scope.end = end;

                    $scope.add = function () {
                        $uibModalInstance.close($scope.event);
                    }

                    $scope.close = function () {
                        $uibModalInstance.close();
                    }
                },
            });

            modalInstance.result.then(function (event) {
                //success
                $scope.externalEvents.push({
                    title: event.Title,
                    type: 'custType',
                    typeName: event.TypeInfo.Text,
                    typeID : event.TypeInfo.Key,
                    color: event.TypeInfo.Tags.color,
                    //cssClass: event.TypeInfo.Tags.color,

                    startsAt: event.time_start,
                    endsAt: event.time_end,

                    draggable: true,
                    resizable: true,
                    
                });

            }, function () {
                //dismissed
            })
        };

        $scope.viewChangeClicked = function (date, nextView) {
            return $scope.viewChangeEnabled;
        };

        $scope.eventEdited = function (event) {
            var modalInstance = $uibModal.open({
                animation: false,
                windowTopClass: 'fade',
                templateUrl: 'apps/system3/home/calendar/view/calendar-maintain.html',
                controller: function ($scope, $uibModalInstance) {

                    $scope.calEvent = event;

                    $scope.submit = function () {

                        calendarService.update({
                            ID: event.id,
                            StartTime: event.startsAt.format('yyyy/MM/dd hh:mm:ss'),
                            EndTime: event.endsAt.format('yyyy/MM/dd hh:mm:ss'),
                            Title: event.title,
                            Type: event.typeID
                        }).then(function () {
                            $uibModalInstance.close();
                        });
                    }

                    $scope.close = function () {
                        $uibModalInstance.close();
                    }
                },
            });
        };

        $scope.eventDeleted = function (event) {

            calendarService.remove(event.id).then(function () {
                if (event.type == "custType") {
                    $scope.externalEvents.push(event);
                }

                $scope.events.removeObj(event);
            })
        }

        $scope.eventDropped = function (event, start, end) {

            var externalIndex = $scope.externalEvents.indexOf(event);

            event.startsAt = start;

            if (end) {
                event.endsAt = end;
            }

            if (externalIndex > -1) {

                // todo： 新增的时候应该用原来的时间，用拖动的日期
                calendarService.add({
                    StartTime: event.startsAt.format('yyyy/MM/dd hh:mm:ss'),
                    EndTime: event.endsAt.format('yyyy/MM/dd hh:mm:ss'),
                    Title: event.title,
                    Type: event.typeID
                }).then(function (id) {
                    event.id = id;
                    $scope.externalEvents.splice(externalIndex, 1);
                    $scope.events.push(event);
                });

            } else {
                calendarService.update({
                    ID: event.id,
                    StartTime: event.startsAt.format('yyyy/MM/dd hh:mm:ss'),
                    EndTime: event.endsAt.format('yyyy/MM/dd hh:mm:ss'),
                    Title: event.title,
                    Type: event.typeID
                });
            }
        };

        $scope.eventTimesChanged = function (event) {

        }

        $scope.toggle = function ($event, field, event) {
            $event.preventDefault();
            $event.stopPropagation();
            event[field] = !event[field];
        };

        calendarService.getMyCalendar({}).then(function (result) {
            angular.forEach(result.Source, function (item) {
                $scope.events.push({
                    id : item.ID,
                    title: item.Title,
                    type: 'warning',
                    
                    startsAt: moment(item.StartTime, "YYYY/MM/DD HH:mm:SS").toDate(),
                    endsAt: moment(item.EndTime, "YYYY/MM/DD HH:mm:SS").toDate(),

                    editable: item.CreateUser == $scope.currentUser.Account.ID,
                    deletable: item.CreateUser == $scope.currentUser.Account.ID,
                    draggable: item.CreateUser == $scope.currentUser.Account.ID,
                    resizable: item.CreateUser == $scope.currentUser.Account.ID
                });
            });
        });
    });
});

