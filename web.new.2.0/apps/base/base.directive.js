define([],
    function () {

        var app = angular.module('base.directive', []);

        app.filter('role', function () {
            return function (input, roleID) {
              
                var result = [];
                
                angular.forEach(input, function (user) {
                    if (user.Visiable && user.Roles && user.Roles.contains(function (r) { return r.Key == roleID })) {
                        result.push(user);
                    }
                });

                return result;
            };
        });

        app.filter('taskuser', function ($rootScope) {
            return function (input, def) {
               
                var result = [];

                angular.forEach(input, function (user) {

                    var u = $rootScope.SpecProcessUsers[user.ID];
                    
                    if (u && u[def.SpecialtyID] && u[def.SpecialtyID][def.ProcessID]) {

                        var tasks = u[def.SpecialtyID][def.ProcessID];

                        if (tasks.split(',').contains(function (t) { return t == def.TaskID })) {
                            result.push(user);
                        }
                    } 
                });

                if (result.length == 0) {
                    result = input;
                }

                return result;
            };
        });

        app.filter('specmanager', function ($rootScope) {
            return function (input, def) {

                var result = [];

                angular.forEach(input, function (user) {

                    var u = $rootScope.SpecProcessUsers[user.ID];

                    if (u && u[def.SpecialtyID] && u[def.SpecialtyID][def.ProcessID]) {

                        var tasks = u[def.SpecialtyID][def.ProcessID];

                        var taskID = $rootScope.ProcessModel_enum[def.ProcessID].Tasks[0].ID;

                        if (tasks.split(',').contains(function (t) { return t == taskID })) {
                            result.push(user);
                        }
                    }
                });

                if (result.length == 0) {
                    result = input;
                }

                return result;
            };
        });

        app.filter('mydept', function ($rootScope) {
            return function (input, enable) {

                if (enable) {
                    var result = [];

                    var dept = $rootScope.currentUser.Account.Dept.Key;

                    angular.forEach(input, function (user) {

                        if (user.Dept.Key == dept) {
                            result.push(user);
                        }
                    });

                    return result;
                } else {
                    return input;
                }
               
            };
        });

        app.directive('pageContent', function () {
            return {
                restrict: 'C',
                link: function (scope, element, attrs, controllers) {
                    
                    $(element).click(function (ev) {
                        
                        chatSidebar = document.getElementById('quickview-sidebar');
                        var target = ev.target;
                        if (target !== chatSidebar) {
                            if ($('#quickview-sidebar').hasClass('open')) {
                                $('#quickview-sidebar').addClass('closing');
                                $('#quickview-sidebar').removeClass('open');
                                setTimeout(function () {
                                    $('#quickview-sidebar').removeClass('closing');
                                }, 400);
                            }
                        }
                    })
                }
            };
        });

        app.directive('navSidebar', function () {
            return {
                restrict: 'C',
                link: function (scope, element, attrs) {
                    
                    var hoverTimeout;
                    $('.nav-sidebar > li').hover(function () {

                        clearTimeout(hoverTimeout);
                        $(this).siblings().removeClass('nav-hover');
                        $(this).addClass('nav-hover');
                    }, function () {
                        var $self = $(this);
                        hoverTimeout = setTimeout(function () {
                            $self.removeClass('nav-hover');
                        }, 200);
                    });

                    $('.nav-sidebar > li .children').hover(function () {
                        clearTimeout(hoverTimeout);
                        $(this).closest('.nav-parent').siblings().removeClass('nav-hover');
                        $(this).closest('.nav-parent').addClass('nav-hover');
                    }, function () {
                        var $self = $(this);
                        hoverTimeout = setTimeout(function () {
                            $(this).closest('.nav-parent').removeClass('nav-hover');
                        }, 200);
                    });
                }
            };
        });

        // Route State Load Spinner(used on page or content load)
        app.directive('ngSpinnerLoader', ['$rootScope',
            function ($rootScope) {
                return {
                    link: function (scope, element, attrs) {
                        // by defult hide the spinner bar
                        element.addClass('hide'); // hide spinner bar by default
                        // display the spinner bar whenever the route changes(the content part started loading)
                        $rootScope.$on('$routeChangeStart', function () {
                            element.removeClass('hide'); // show spinner bar
                        });
                        // hide the spinner bar on rounte change success(after the content loaded)
                        $rootScope.$on('$routeChangeSuccess', function () {

                            setTimeout(function () {

                                element.addClass('hide'); // hide spinner bar
                            }, 500);
                            $("html, body").animate({
                                scrollTop: 0
                            }, 500);
                        });
                    }
                };
            }
        ]);

        app.directive('ngViewClass', function ($location) {
            return {
                link: function (scope, element, attrs, controllers) {
                    var classes = attrs.ngViewClass ? attrs.ngViewClass.replace(/ /g, '').split(',') : [];
                    setTimeout(function () {
                        if ($(element).hasClass('ng-enter')) {
                            for (var i = 0; i < classes.length; i++) {
                                var route = classes[i].split(':')[1];
                                var newclass = classes[i].split(':')[0];

                                if (route === $location.path()) {
                                    $(element).addClass(newclass);
                                } else {
                                    $(element).removeClass(newclass);
                                }
                            }
                        }
                    })

                }
            };
        });

        app.directive('ddropdown', function ($location) {
            return {
                restrict: 'C',
                link: function (scope, element, attrs, controllers) {
                    
                    $(element).dropdownHover();
                }
            };
        });

        app.directive('fullheight', function ($window) {

            return {

                link: function (scope, element, attrs, controllers) {

                    var headerHeight = 50;
                    var height = $window.innerHeight;
                    var data_padding = $(element).data('padding') ? $(element).data('padding') : 0;

                    height = height - data_padding;

                    if (attrs.fullheight == "window") {
                        height = height;
                    }
                    else if (attrs.fullheight == "quickview") {
                        height = height - headerHeight;
                    }
                    else if (attrs.fullheight == "panel") {
                        height = height - headerHeight - 20 - 20;
                    }
                    else if (attrs.fullheight == "panelcontent") {

                        var panelHeadHeigh = $(element).parent().find('.panel-header').height();

                        height = height - headerHeight - 20 - 20 - panelHeadHeigh - 80;

                    }
                    else if (attrs.fullheight == 'panelGrid') {

                        var panelHeadHeigh = $(element).parent().find('.panel-header').height();

                        height = height - headerHeight - 20 - 20 - panelHeadHeigh - 55 - 55;
                    }
                    else if (new Number(attrs.fullheight) > 0) {
                        height = attrs.fullheight;
                    }
                    if (attrs.percent) {
                        height = height * new Number(attrs.percent);
                    }
                    if (attrs.calheight) {
                        height = height + new Number(attrs.calheight);
                    }

                    scope.fullHeight = height;

                    $(element).css("height", height + "px");
                    $(element).attr("data-height", height);
                    $(element).attr("max-height", height);
                }
            };
        });

        app.directive("scroll", function ($parse) {
            return function ($scope, $element, $attrs) {
                var scroll = {
                    element: $element,
                    attrs: $attrs,
                    init: function () {
                       
                        if ($.fn.mCustomScrollbar) {

                            var scroll_height = $(this.element).data('height') ? $(this.element).data('height') : 'auto';
                            var data_padding = $(this.element).data('padding') ? $(this.element).data('padding') : 0;
                            if ($(this.element).data('height') == 'window') {
                                thisHeight = $(this.element).height();
                                windowHeight = $(window).height() - data_padding - 50;
                                if (thisHeight < windowHeight) scroll_height = thisHeight;
                                else scroll_height = windowHeight;
                            }

                            $(this.element).mCustomScrollbar("destroy");

                            $(this.element).mCustomScrollbar({
                                scrollButtons: {
                                    enable: false
                                },
                                autoHideScrollbar: true,
                                scrollInertia: 150,
                                theme: "dark-thick",
                                set_height: scroll_height,
                                advanced: {
                                    updateOnContentResize: true,
                                    updateOnBrowserResize: true,
                                }

                            });

                            if (this.attrs.scrollTo) {

                                $(this.element).mCustomScrollbar('scrollTo', this.attrs.scrollTo);
                            }
                        }
                    },

                    scrollTo: function (scrollTo) {
                        
                        //var ele = $(".chat-msg-body");
                        //var ele2 = $(this.element);

                        //console.log(scrollTo);

                        //$(".chat-msg-body").mCustomScrollbar('scrollTo', scrollTo);
                        $(this.element).mCustomScrollbar('scrollTo', scrollTo);
                    },

                    update: function () {
                        
                        $(this.element).mCustomScrollbar('update');
                    }

                };

                if ($attrs.scroll.indexOf('.') > -1) {
                    $scope.scrollModel = scroll;

                    var parseFunc = $parse($attrs.scroll + " = scrollModel");

                    parseFunc($scope);

                } else {

                    if ($attrs.parentModel) {
                        $scope.$parent[$attrs.scroll] = scroll;
                    } else {
                        $scope[$attrs.scroll] = scroll;
                    }
                }

                if ($attrs.auto == "true") {
                    scroll.init();
                }
            };
        });

        app.directive('btnLoading', function () {
            return {

                link: function (scope, element, attrs, controllers) {

                    var hander = Ladda.create(element[0]);

                    scope[attrs.name] = hander;
                }
            };
        });

        app.directive("ngRepeatEnd", function ($parse) {
            return function (scope, element, attrs) {

                if (scope.$last) {

                    if (attrs.ngRepeatEnd) {

                        var parseFunc = $parse(attrs.ngRepeatEnd);
                        parseFunc(scope);
                    }
                }
            };
        });

        app.directive("ngRepeatSet", function ($parse) {
            return function (scope, element, attrs) {

                if (attrs.ngRepeatSet) {

                    var parseFunc = $parse(attrs.ngRepeatSet);
                    parseFunc(scope);
                }
            };
        });

        app.directive('modal', function ($parse) {

            return {
                restrict: 'AC',
                replace: false,
                link: function (scope, elem, attrs) {
                    if (attrs.onShow) {

                        $(elem).on('shown.bs.modal', function () {
                            var parseFunc = $parse(attrs.onShow);
                            parseFunc(scope);
                        })
                    }

                    if (attrs.onHide) {
                        $(elem).on('hidden.bs.modal', function () {
                            var parseFunc = $parse(attrs.onHide);
                            parseFunc(scope);
                        })
                    }
                }
            }
        });

        app.directive('barcode', function ($parse) {

            return {
                restrict: 'E',
                template: '<img />',
                scope: {
                    code: '='
                },
                link: function (scope, elem, attrs) {
                  
                    scope.$watch("code", function (newval, oldval) {
                        if (newval) {
                            $(elem).find("img").JsBarcode(scope.code);
                        }
                    });
                }
            }
        });

        app.directive('animateNumber', function () {
            return {
                restrict: 'E',
                replace: false,
                link: function (scope, elem, attrs) {

                    var value = attrs["value"];
                    var duration = parseInt(attrs["duration"], 10);

                    $(elem).animateNumbers(value, true, duration);
                }
            }
        });

        app.directive('slideIos', function () {
            return {
                restrict: 'C',
                replace: false,
                link: function (scope, elem, attrs) {
                    if ($.fn.slider) {
                        $(elem).sliderIOS();
                    }
                }
            }
        });
        
        app.directive('enterPress', function ($parse) {
            return function ($scope, element, $attrs) {

                $(element).keyup(function (e) {
                    if (e.which == 13) {
                        $scope.$safeApply(function () {
                            var parseFunc = $parse($attrs.enterPress);
                            parseFunc($scope);
                        });
                    }
                });
            };
        });

        app.directive('modalDragable', function () {
            return {
                restrict: 'C',
                link: function (scope, elem, attr) {

                    if (attr.visiable != undefined &&
                        attr.visiable == 'false') {
                        return;
                    }
                    // .modal-dialog
                    $(elem).parent().parent().draggable({
                        handle: ".modal-header"
                    });
                }
            }
        });

        app.directive('modalBlock', function () {

            function blockUI(item) {
                $(item).block({
                    message: '<svg class="circular"><circle class="path" cx="40" cy="40" r="10" fill="none" stroke-width="2" stroke-miterlimit="10"/></svg>',
                    css: {
                        border: 'none',
                        width: '14px',
                        backgroundColor: 'none'
                    },
                    overlayCSS: {
                        backgroundColor: '#fff',
                        opacity: 0.6,
                        cursor: 'wait'
                    }
                });
            }

            function unblockUI(item) {
                $(item).unblock();
            }

            return {
                restrict: 'AC',
                link: function (scope, elem, attr) {
                    
                    scope.blockHander = {
                        block: function () {
                            blockUI($(elem).parent().parent());
                        },
                        unblock: function () {
                            unblockUI($(elem).parent().parent());
                        }
                    };
                }
            }
        });

        app.directive('countup', function ($timeout) {
            return {
                restrict: 'C',
                scope: {
                    to: '='
                },
                link: function (scope, elem, attr) {

                    from = attr["from"] ? attr["from"] : 0;
                    //to = attr["to"] ? attr["to"] : 0;
                    duration = attr["duration"] ? attr["duration"] : 1;
                    delay = attr["delay"] ? attr["delay"] : 500;
                    decimals = attr["decimals"] ? attr["decimals"] : 0;

                    var options = {
                        useEasing: true,
                        useGrouping: true,
                        separator: ',',
                        prefix: attr["prefix"] ? attr["prefix"] : '',
                        suffix: attr["suffix"] ? attr["suffix"] : ''
                    }

                    scope.$watch('to', function (newval, oldval) {

                        var numAnim = new countUp($(elem).get(0), from, newval == undefined ? 0 : newval, decimals, duration, options);
                        numAnim.start();
                        //$timeout(function () {

                        //}, delay);
                    })
                }
            }
        })

        app.directive('uiSelectWrap', function uiSelectWrap($document, uiGridEditConstants) {
            return function link($scope, $elm, $attr) {
                $document.on('click', docClick);

                function docClick(evt) {
                    if ($(evt.target).closest('.ui-select-container').size() === 0) {
                        $scope.$emit(uiGridEditConstants.events.END_CELL_EDIT);
                        $document.off('click', docClick);
                    }
                }
            };
        });

        app.directive('datePickerWrap', function uiSelectWrap($document, uiGridEditConstants) {
            return function link($scope, $elm, $attr) {
                $document.on('click', docClick);

                function docClick(evt) {
                    if ($(evt.target).closest('.datepicker').size() === 0) {
                        $scope.$emit(uiGridEditConstants.events.END_CELL_EDIT);
                        $document.off('click', docClick);
                    }
                }
            };
        });

        app.directive('percentbox', function ($timeout) {
            return {
                restrict: 'C',
                link: function (scope, elem, attr) {

                    $(elem).animate({
                        width: attr.value + "%",
                    }, attr.value);
                }
            }
        });

        app.directive('timelineBlock', function ($timeout) {
            return {
                restrict: 'C',
                link: function (scope, elem, attr) {

                    var $timeline_block = $(elem);

                    //hide timeline blocks which are outside the viewport
                    if ($timeline_block.offset().top > $(window).scrollTop() + $(window).height() * 0.75) {
                        $timeline_block.find('.timeline-icon, .timeline-content').addClass('is-hidden');
                    }

                    //on scolling, show/animate timeline blocks when enter the viewport
                    $(window).on('scroll', function () {
                        $timeline_block.each(function () {
                            if ($(this).offset().top <= $(window).scrollTop() + $(window).height() * 0.75 && $(this).find('.timeline-icon').hasClass('is-hidden')) {
                                $(this).find('.timeline-icon, .timeline-content').removeClass('is-hidden').addClass('bounce-in');
                            }
                            if ($(this).offset().top > $(window).scrollTop() + $(window).height() * 0.75) {
                                $(this).find('.timeline-icon, .timeline-content').removeClass('bounce-in').addClass('is-hidden');
                            }
                        });
                    });
                }
            }
        });

        app.filter('enumMap', function ($rootScope, $timeout) {

            return function (input, name) {

                if (input == 0 || input == undefined) {
                    return "";
                }

                input = input + '';

                var enumObj = undefined;
                if (name == "user" || name == "ProcessModel") {
                    enumObj = $rootScope[name + "_enum"];
                }
                else if (name == "Object") {
                    enumObj = $rootScope.baseEnum.System3[name];
                }
                else {
                    enumObj = $rootScope.baseEnum[$rootScope.currentBusiness.Key][name];
                }

                if (enumObj == undefined) {
                    return '';
                }

                var match;
                if (!input) {
                    return '';
                } else if (result = enumObj[input]) {
                    return result;
                } else if ((match = input.match(/(.+)( \(\d+\))/)) && (result = enumObj[match[1]])) {
                    return result + match[2];
                } else {
                    return input;
                }
            };
        });

        app.filter('userImg', function ($rootScope, $timeout) {

            return function (input, name) {

                if (input == 0 || input == undefined) {
                    return "";
                }

                var user = $rootScope.user_item.find(function (u) { return u.ID == input; });

                if (user) {
                    return user.PhotoImg;
                }
            };
        });

        app.filter('userDept', function ($rootScope, $timeout) {

            return function (input, name) {

                if (input == 0 || input == undefined) {
                    return "";
                }

                var user = $rootScope.user_item.find(function (u) { return u.ID == input; });

                if (user) {
                    return user.Dept.Name;
                }
            };
        });

        app.filter('lastMessageDate', function () {
            var filterfun = function (messages) {

                if (messages == null ||
                    messages == undefined) {
                    return "";
                }

                var messageDate = messages[messages.length - 1].Date;

                var time_start = new Date(messageDate.replace('T', ' ')).getTime();;

                if (time_start == null ||
                    time_start == undefined) {
                    return "";
                }

                var time_end = new Date().getTime();

                var date3 = time_end - time_start;  //时间差的毫秒数


                //计算出相差天数
                var days = Math.floor(date3 / (24 * 3600 * 1000))

                if (days > 30) {
                    return Math.floor(days / 30) + "个月前";

                }
                if (days > 7) {
                    console.log(days)
                    return Math.floor(days / 7) + "周前";
                }
                if (days > 1) {
                    return days + '天前';
                }

                //计算出小时数
                var leave1 = date3 % (24 * 3600 * 1000)    //计算天数后剩余的毫秒数
                var hours = Math.floor(leave1 / (3600 * 1000))

                if (hours > 1) {
                    return hours + '小时前';
                }

                //计算相差分钟数
                var leave2 = leave1 % (3600 * 1000)        //计算小时数后剩余的毫秒数
                var minutes = Math.floor(leave2 / (60 * 1000))

                if (minutes > 0) {
                    return minutes + '分钟前';
                }

                return "刚刚";
            };
            return filterfun;
        });

        app.filter('messageDate', function () {
            var filterfun = function (date) {

                if (date == null ||
                    date == undefined) {
                    return "";
                }


                var messageDate = new Date(date);

                var day = messageDate.getDay();
                if (isNaN(day)) {
                    messageDate = new Date(date);
                }

                if (messageDate == null || messageDate == undefined) {
                    return "";
                }

                if (messageDate.getDay() != new Date().getDay()) {
                    // 一天前的显示完整时间
                    return messageDate.format('MM/dd hh:mm:ss');
                } else {
                    // 当天的消息只显示 时分秒
                    return messageDate.format('hh:mm:ss');
                }
            };
            return filterfun;
        });

        app.filter('strDate', function () {
            var filterfun = function (date) {

                var time_start = new Date(date.replace('T', ' ')).getTime();;

                if (time_start == null ||
                    time_start == undefined) {
                    return "";
                }

                var time_end = new Date().getTime();

                var date3 = time_end - time_start;  //时间差的毫秒数


                //计算出相差天数
                var days = Math.floor(date3 / (24 * 3600 * 1000))

                if (days > 30) {
                    return Math.floor(days / 30) + "个月前";

                }
                if (days > 7) {
                    return Math.floor(days / 7) + "周前";
                }
                if (days > 0) {
                    return days + '天前';
                }

                //计算出小时数
                var leave1 = date3 % (24 * 3600 * 1000)    //计算天数后剩余的毫秒数
                var hours = Math.floor(leave1 / (3600 * 1000))

                if (hours > 0) {
                    return hours + '小时前';
                }

                //计算相差分钟数
                var leave2 = leave1 % (3600 * 1000)        //计算小时数后剩余的毫秒数
                var minutes = Math.floor(leave2 / (60 * 1000))

                if (minutes > 0) {
                    return minutes + '分钟前';
                }

                return "刚刚";
            };
            return filterfun;
        });

        app.filter('strDateLeft', function () {
            var filterfun = function (date) {

                var time_start = new Date(date.replace('T', ' ')).getTime();;

                if (time_start == null ||
                    time_start == undefined) {
                    return "";
                }

                var time_end = new Date().getTime();

                var date3 = time_start - time_end;  //时间差的毫秒数


                //计算出相差天数
                var days = Math.floor(date3 / (24 * 3600 * 1000))

                if (days >= 30) {
                    return Math.floor(days / 30) + "个月";

                }
                if (days >= 7) {
                    return Math.floor(days / 7) + "周";
                }
                if (days > 0) {
                    return days + '天';
                }

                //计算出小时数
                var leave1 = date3 % (24 * 3600 * 1000)    //计算天数后剩余的毫秒数
                var hours = Math.floor(leave1 / (3600 * 1000))

                if (hours > 0) {
                    return hours + '小时';
                }

                //计算相差分钟数
                var leave2 = leave1 % (3600 * 1000)        //计算小时数后剩余的毫秒数
                var minutes = Math.floor(leave2 / (60 * 1000))

                if (minutes > 0) {
                    return minutes + '分钟';
                }

                return "即将";
            };
            return filterfun;
        });

        app.filter('TDate', function () {
            var filterfun = function (date, formate) {

                if (date == null || date == undefined) {
                    return "";
                }
                if (date.toTDate) {
                    return date.toTDate(formate);
                } else {
                    return date
                }
            };
            return filterfun;
        });

        app.filter('leaveDate', function () {
            var filterfun = function (date) {

                var time_start = new Date(date).getTime();;

                if (time_start == null ||
                    time_start == undefined) {
                    return "";
                }

                var time_end = new Date().getTime();

                var date3 = time_start - time_end;  //时间差的毫秒数


                //计算出相差天数
                var days = Math.floor(date3 / (24 * 3600 * 1000))

                if (days > 30) {
                    return Math.floor(days / 30) + "个月";
                }
                if (days > 7) {
                    return Math.floor(days / 7).toFixed(0) + "周";
                }
                if (days > 1) {
                    return days + '天';
                }

                //计算出小时数
                var leave1 = date3 % (24 * 3600 * 1000)    //计算天数后剩余的毫秒数
                var hours = Math.floor(leave1 / (3600 * 1000))

                if (hours > 1) {
                    return hours + '小时';
                }

                //计算相差分钟数
                var leave2 = leave1 % (3600 * 1000)        //计算小时数后剩余的毫秒数
                var minutes = Math.floor(leave2 / (60 * 1000))

                if (minutes > 0) {
                    return minutes + '分钟';
                }

                return "小于1分钟";
            };
            return filterfun;
        });

        app.filter('calendar', function (moment) {
            var filterfun = function (date) {

                return moment(date).format('a hh:mm');
            };
            return filterfun;
        });

        app.filter('countdown', function (moment) {
            var filterfun = function (date) {

                var time_start = new Date(date).getTime();
                var time_end = new Date().getTime();
                return (time_start - time_end) / 1000;

                //return moment(date, 'yyyy/MM/dd hh:mm:ss').diff(moment(), 'seconds') * 1000;

            };
            return filterfun;
        });

        app.filter('fileSize', function () {
            var filterfun = function (size) {
                if (size) {
                    return size.toFileSize();
                }
            };
            return filterfun;
        });

        app.filter("subStr", function () {
            var filterfun = function (str, length) {

                if (str == null ||
                    str == undefined) {
                    return "";
                }

                if (str.length <= length) {
                    return str;
                }

                return str.preStr(length);

            };
            return filterfun;
        });

        app.filter("money", function () {
            var filterfun = function (str, min) {

                if (str == null ||
                    str == undefined ||
                    new Number(str) == NaN ||
                    new Number(str) < 0) {

                    return "￥0.00";
                }

                if (min &&
                    new Number(str) < min) {
                    return "￥" + min;
                }

                return "￥" + new Number(str).toFixed(2);

            };
            return filterfun;
        });

        app.filter("bool", function () {
            var filterfun = function (val) {

                if (val) {
                    return "√"
                }

                return "";
            };
            return filterfun;
        });

        app.filter("users", function ($rootScope) {
            return function (val) {

                var userNameStrs = "";

                if (val) {
                    var userIDs = val.split(',');
                    for (var i = 0; i < userIDs.length; i++) {

                        userNameStrs += $rootScope["user_enum"][userIDs[i]];
                        if (i != userIDs.length - 1) {
                            userNameStrs += ","
                        }
                    }
                }
                return userNameStrs;

            };
        });

        app.filter("phasebg", function ($rootScope) {
            return function (val) {

                switch (val) {
                    case 1: return "bg-areo";
                    case 2: return "bg-red";
                    case 3: return "bg-blue";
                    case 4: return "bg-primary";
                    case 5: return "bg-yellow";
                    default: return "";
                }
            };
        });
       

        return app;
    });
