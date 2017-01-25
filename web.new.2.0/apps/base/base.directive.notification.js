define(['apps/base/base.directive'],
    function (app) {

        app.directive("notification", function () {

            return {
                restrict: 'E',
                link: function (scope, element, attrs) {

                    $(element).hide();

                    if (attrs.position == 'bottom') {
                        openAnimation = 'animated fadeInUp';
                        closeAnimation = 'animated fadeOutDown';
                    }
                    else if (attrs.position == 'top') {
                        openAnimation = 'animated fadeIn';
                        closeAnimation = 'animated fadeOut';
                    }
                    else {
                        openAnimation = 'animated bounceIn';
                        closeAnimation = 'animated bounceOut';
                    }

                    if (attrs.container == "panel") {
                        scope[attrs.model] = {
                            show: function () {

                                $(element).parent().noty({
                                    text: $(element).html(),
                                    dismissQueue: true,
                                    layout: 'top',
                                    closeWith: ['click'],
                                    theme: 'made',
                                    maxVisible: 10,
                                    animation: {
                                        open: openAnimation,
                                        close: closeAnimation
                                    },
                                    timeout: attrs.timeout,
                                    callback: {
                                        onShow: function () {
                                            var sidebarWidth = $('.sidebar').width();
                                            var topbarHeight = $('.topbar').height();
                                            if (position == 'top' && style == 'topbar') {
                                                $('.noty_inline_layout_container').css('top', 0);
                                                if ($('body').hasClass('rtl')) {
                                                    $('.noty_inline_layout_container').css('right', 0);
                                                }
                                                else {
                                                    $('.noty_inline_layout_container').css('left', 0);
                                                }

                                            }
                                        }
                                    }
                                });
                            }
                        }
                    } else {
                        scope[attrs.model] = {
                            show: function () {
                                
                                noty({
                                    text: $(element).html(),
                                    type: attrs.type,
                                    dismissQueue: true,
                                    layout: attrs.position,
                                    closeWith: ['click'],
                                    theme: 'made',
                                    maxVisible: 10,
                                    animation: {
                                        open: openAnimation,
                                        close: closeAnimation,
                                        easing: 'swing',
                                        speed: 100
                                    },
                                    timeout: attrs.timeout,
                                    callback: {
                                        onShow: function () {
                                            
                                            leftNotfication = $('.sidebar').width();
                                            if ($('body').hasClass('rtl')) {
                                                if (position == 'top' || position == 'bottom') {
                                                    $('#noty_top_layout_container').css('margin-right', leftNotfication).css('left', 0);
                                                    $('#noty_bottom_layout_containe').css('margin-right', leftNotfication).css('left', 0);
                                                }
                                                if (position == 'topRight' || position == 'centerRight' || position == 'bottomRight') {
                                                    $('#noty_centerRight_layout_container').css('right', leftNotfication + 20);
                                                    $('#noty_topRight_layout_container').css('right', leftNotfication + 20);
                                                    $('#noty_bottomRight_layout_container').css('right', leftNotfication + 20);
                                                }
                                            }
                                            else {
                                                if (position == 'top' || position == 'bottom') {
                                                    $('#noty_top_layout_container').css('margin-left', leftNotfication).css('right', 0);
                                                    $('#noty_bottom_layout_container').css('margin-left', leftNotfication).css('right', 0);
                                                }
                                                if (position == 'topLeft' || position == 'centerLeft' || position == 'bottomLeft') {
                                                    $('#noty_centerLeft_layout_container').css('left', leftNotfication + 20);
                                                    $('#noty_topLeft_layout_container').css('left', leftNotfication + 20);
                                                    $('#noty_bottomLeft_layout_container').css('left', leftNotfication + 20);
                                                }
                                            }

                                        }
                                    }
                                });
                            }
                        }
                    }
                }
            };
        });
    });
