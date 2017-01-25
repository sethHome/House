define(['apps/base/base.service'], function (app) {

    app.factory("notificationService", function (Restangular, userApiUrl, userApiVersion) {

        var restSrv = Restangular.withConfig(function (configSetter) {
            configSetter.setBaseUrl(userApiUrl + userApiVersion);
        })

        return {
            getEffects: function (max, read) {
                if (max == undefined) {
                    max = 10;
                }
                return restSrv.one("notification").get({
                    pagesize: max,
                    effect: 1,
                    read: read
                });
            },

            read: function (id) {
                return restSrv.one("notification", id).customPUT({},'read');
            },

            showException: function (msg,exType, stackTrace) {

                var notifContentDebug = '<div class="alert  media fade in bg-red"><p>' + msg + '<br />' + exType + '</p><hr /><p>' + stackTrace + '</p></div>';
                
                var openAnimation = 'animated fadeIn';
                var closeAnimation = 'animated fadeOut';
                noty({
                    text: notifContentDebug,
                    type: "danger",
                    dismissQueue: true,
                    layout: "top",
                    closeWith: ['click'],
                    theme: 'made',
                    maxVisible: 10,
                    animation: {
                        open: openAnimation,
                        close: closeAnimation,
                        easing: 'swing',
                        speed: 100
                    },
                    timeout: undefined,
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
    });


});
