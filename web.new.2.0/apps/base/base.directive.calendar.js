define(['apps/base/base.directive'],
    function (app) {

        app.directive('calendarDragable', function () {
            return {
                restrict: 'A',
                link: function (scope, elem, attr) {

                    // create an Event Object (http://arshaw.com/fullcalendar/docs/event_data/Event_Object/)
                    // it doesn't need to have a start or end
                    var eventObject = {
                        title: scope.item.Text, // use the element's text as the event title
                        color: scope.item.Tags.color
                    };
                    // store the Event Object in the DOM element so we can get to it later
                    $(elem).data('eventObject', eventObject);
                    // make the event draggable using jQuery UI
                    $(elem).draggable({
                        zIndex: 999,
                        revert: true,      // will cause the event to go back to its
                        revertDuration: 0,  //  original position after the drag.
                    });
                }
            }
        });

        app.directive('calendar', function ($uibModal) {

            return {
                restrict: 'EA',
                scope: {
                    events: '=',
                    dropRemove: '=',
                    droppable: '=',
                    editable: '=',
                    selectable: '='
                },
                controller: function ($scope, $rootScope, $uibModal) {


                },
                link: function ($scope, element, attrs) {

                    /*  Initialize the calendar  */
                    var date = new Date();
                    var d = date.getDate();
                    var m = date.getMonth();
                    var y = date.getFullYear();
                    var form = '';
                    var today = new Date($.now());

                    var calendar = $(element).fullCalendar({
                        lang: 'zh-cn',
                        slotDuration: '00:15:00', /* If we want to split day time each 15minutes */
                        minTime: '08:00:00',
                        maxTime: '19:00:00',
                        defaultView: attrs.defaultView,
                        handleWindowResize: true,
                        height: $(window).height() - 200,
                        header: {
                            left: 'prev,next today',
                            center: 'title',
                            right: 'month,agendaWeek,agendaDay'
                        },
                        events: function (start, end, timezone, callback) {
                            
                            $scope.$watch("events", function (newval, oldval) {
                                if (newval) {
                                    callback(newval);
                                }
                            });
                        },
                        
                        editable: $scope.editable,
                        droppable: $scope.droppable, // this allows things to be dropped onto the calendar !!!
                        selectable: $scope.selectable,

                        eventLimit: true, // allow "more" link when too many events

                        drop: function (date) {
                            // retrieve the dropped element's stored Event Object
                            var originalEventObject = $(this).data('eventObject');

                            // we need to copy it, so that multiple events don't have a reference to the same object
                            var copiedEventObject = $.extend({}, originalEventObject);
                            // assign it the date that was reported
                            copiedEventObject.start = date;
                            copiedEventObject['className'] = copiedEventObject.color;

                            // render the event on the calendar
                            // the last `true` argument determines if the event "sticks" (http://arshaw.com/fullcalendar/docs/event_rendering/renderEvent/)
                            $(element).fullCalendar('renderEvent', copiedEventObject, true);

                            // remove the source element
                            if ($scope.dropRemove) {
                                $(this).remove();
                            }
                        },

                        eventClick: function (calEvent, jsEvent, view) {
                            $uibModal.open({
                                animation: false,
                                windowTopClass: 'fade',
                                templateUrl: 'apps/home/calendar/view/calendar-maintain.html',
                                controller: function ($scope, $uibModalInstance) {
                                    $scope.calEvent = calEvent;
                                    $scope.submit = function () {
                                        calendar.fullCalendar('updateEvent', $scope.calEvent);
                                        $uibModalInstance.close();
                                    }

                                    $scope.remove = function () {
                                        calendar.fullCalendar('removeEvents', function (ev) {
                                            return (ev._id == calEvent._id);
                                        });

                                        $uibModalInstance.close();
                                    };

                                    $scope.close = function () {
                                        $uibModalInstance.close();
                                    }
                                },
                            });
                        },

                        select: function (start, end, allDay) {
                            $uibModal.open({
                                animation: false,
                                windowTopClass: 'fade',
                                templateUrl: 'apps/home/calendar/view/calendar-add.html',
                                controller: function ($scope, $uibModalInstance) {
                                    $scope.start = start;
                                    $scope.end = end;

                                    $scope.add = function () {

                                        calendar.fullCalendar('renderEvent', {
                                            title: $scope.event.Title,
                                            start: start,
                                            end: end,
                                            allDay: false,
                                            className: $scope.event.TypeInfo.Tags["color"]
                                        }, true);
                                        calendar.fullCalendar('unselect');
                                        $uibModalInstance.close();
                                    }

                                    $scope.close = function () {
                                        $uibModalInstance.close();
                                    }
                                },
                            });
                        }
                    });

                    
                }
            };
        });
    });
