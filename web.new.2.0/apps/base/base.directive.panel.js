define(['apps/base/base.directive'],
    function (app) {

        app.directive("blockui", function ($parse) {

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
                link: function (scope, element, attrs) {
                    var str = attrs.blockui + " = blockInfo"
                    scope.blockInfo = {
                        block: function () {
                            blockUI($(element));
                        },
                        unblock: function () {
                            unblockUI($(element));
                        }
                    };

                    var parseFunc = $parse(str);
                    parseFunc(scope);
                }
            };
        });

        app.directive("panel", function () {

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
                restrict: 'C',
                link: function (scope, element, attrs) {
                    if (attrs.model) {
                        scope[attrs.model] = {
                            block: function () {
                                blockUI($(element));
                            },
                            unblock: function () {
                                unblockUI($(element));
                            }
                        };
                    }
                }
            };
        });

        app.directive("controlBtn", function () {

            function maximizePanel() {
                if ($('.maximized').length) {
                    var panel = $('.maximized');
                    var windowHeight = $(window).height() - 2;
                    panelHeight = panel.find('.panel-header').height() + panel.find('.panel-content').height() + 100;
                    if (panel.hasClass('maximized')) {
                        if (windowHeight > panelHeight) panel.parent().height(windowHeight);
                        else {
                            if ($('.main-content').height() > panelHeight) {
                                panel.parent().height($('.main-content').height());
                            } else {
                                panel.parent().height(panelHeight);
                            }
                        }
                    } else {
                        panel.parent().height('');
                    }
                }
            }

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
                restrict:'C',
                link: function (scope, element, attrs) {
                    
                    if (attrs.popout == "true") {
                        $(element).append($('<a href="#" class="panel-popout  tt" title="Pop Out/In"><i class="icons-office-58"></i></a>').click(function (event) {
                            event.preventDefault();
                            var panel = $(this).parents(".panel:first");
                            if (panel.hasClass("modal-panel")) {
                                $("i", this).removeClass("icons-office-55").addClass("icons-office-58");
                                panel.removeAttr("style").removeClass("modal-panel");
                                panel.find(".panel-maximize,.panel-toggle").removeClass("nevershow");
                                panel.draggable("destroy").resizable("destroy");
                            } else {
                                panel.removeClass("maximized");
                                panel.find(".panel-maximize,.panel-toggle").addClass("nevershow");
                                $("i", this).removeClass("icons-office-58").addClass("icons-office-55");
                                var w = panel.width();
                                var h = panel.height();
                                panel.addClass("modal-panel").removeAttr("style").width(w).height(h);
                                $(panel).draggable({
                                    handle: ".panel-header",
                                    containment: ".page-content"
                                }).css({
                                    "left": panel.position().left - 10,
                                    "top": panel.position().top + 2
                                }).resizable({
                                    minHeight: 150,
                                    minWidth: 200
                                });
                            }
                            window.setTimeout(function () {
                                $("body").trigger("resize");
                            }, 300);
                        }));
                    }
                    if (attrs.reload) {

                        var el = $(element).parents(".panel:first");

                        scope[attrs.reload] = {
                            blockUI : function () {
                                blockUI(el);
                            },
                            unblockUI : function () {
                                unblockUI(el);
                            }
                        };
                        
                        //$(element).append($('<a href="#" class="panel-reload "><i class="icon-reload"></i></a>').click(function (event) {
                        //    event.preventDefault();
                        //    event.stopPropagation();
                            
                        //    blockUI(el);
                        //    window.setTimeout(function () {
                        //        unblockUI(el);
                        //    }, 1800);
                        //}));
                    }
                    if (attrs.maximize == "true") {
                        $(element).append($('<a href="#" class="panel-maximize "><i class="icon-size-fullscreen"></i></a>').click(function (event) {
                            event.preventDefault();
                            var panel = $(this).parents(".panel:first");
                            $body.toggleClass("maximized-panel");
                            panel.removeAttr("style").toggleClass("maximized");
                            maximizePanel();
                            if (panel.hasClass("maximized")) {
                                panel.parents(".portlets:first").sortable("destroy");
                                $(window).trigger('resize');
                            } else {
                                $(window).trigger('resize');
                                pluginsService.sortablePortlets();
                                panel.parent().height('');
                            }
                            $("i", this).toggleClass("icon-size-fullscreen").toggleClass("icon-size-actual");
                            panel.find(".panel-toggle").toggleClass("nevershow");
                            $("body").trigger("resize");
                            
                            return false;
                        }));
                    }
                    if (attrs.close == "true") {
                        $(element).append($('<a class="panel-close"><i class="icon-trash"></i></a>').click(function (event) {
                            
                            event.preventDefault();
                            $item = $(this).parents(".panel:first");
                            if (attrs.closeConfirm) {
                                bootbox.confirm(attrs.closeConfirm, function (result) {
                                    if (result === true) {
                                        $item.addClass("animated bounceOutRight");
                                        window.setTimeout(function () {
                                            $item.remove();
                                        }, 300);
                                    }
                                });
                            } else {
                                $item.addClass("animated bounceOutRight");
                                window.setTimeout(function () {
                                    $item.remove();
                                }, 300);
                            }
                        }));
                    }
                    if (attrs.toggle == "true") {
                        $(element).append($('<a href="#" class="panel-toggle"><i class="fa fa-angle-down"></i></a>').click(function (event) {
                            event.preventDefault();
                            $(this).toggleClass("closed").parents(".panel:first").find(".panel-content").slideToggle();
                        }));
                    }
                }
            };
        });
    });
