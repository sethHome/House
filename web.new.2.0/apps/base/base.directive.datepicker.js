define(['apps/base/base.directive'],
    function (app) {

        app.directive('datePicker', function ($rootScope, $timeout, $parse, $filter) {
            return {
                restrict: 'A',
                link: function ($scope, $element, $attrs) {

                    var ngmodel = $attrs.ngModel;

                    //if (ngmodel) {
                    //    var filterStr = ngmodel + " = " + ngmodel + ".toTDate()";
                    //    var func = $parse(filterStr);
                    //    func($scope);
                    //}

                    $($element).bootstrapDatepicker({
                        startView: $attrs.view ? $attrs.view : 0, // 0: month view , 1: year view, 2: multiple year view
                        language: $attrs.lang ? $attrs.lang : "cn",
                        forceParse: $attrs.parse ? $attrs.parse : false,
                        daysOfWeekDisabled: $attrs.dayDisabled ? $attrs.dayDisabled : "", // Disable 1 or various day. For monday and thursday: 1,3
                        calendarWeeks: $attrs.calendarWeek ? $attrs.calendarWeek : false, // Display week number 
                        autoclose: $attrs.autoclose ? $attrs.autoclose : true,
                        todayHighlight: $attrs.todayHighlight ? $attrs.todayHighlight : true, // Highlight today date
                        toggleActive: $attrs.toggleActive ? $attrs.toggleActive : true, // Close other when open
                        multidate: $attrs.multidate ? $attrs.multidate.multidate : false, // Allow to select various days
                        orientation: $attrs.orientation ? $attrs.orientation : "auto", // Allow to select various days,
                        rtl: $('html').hasClass('rtl') ? true : false
                    }).on("changeDate", function (e) {
                        
                        $rootScope.$safeApply(function () {
                            if (ngmodel) {
                                var str = ngmodel + " = '" + e.format(null, 'yyyy/MM/dd') + "'";
                                var parseFunc = $parse(str);
                                parseFunc($scope);
                            }
                        });
                    });

                    $parse("dateVal = " + ngmodel)($scope);
                    if ($scope.dateVal) {
                        $($element).bootstrapDatepicker("update", new Date($scope.dateVal));
                    }
                }
            };
        });

        
    });
