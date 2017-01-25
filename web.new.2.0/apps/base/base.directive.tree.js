define(['apps/base/base.directive', 'jstree'],
    function (app) {

        app.directive('tree', ['$parse', function ($parse) {

            var treeDir = {
                restrict: 'EA',

                fetchResource: function ($scope, cb) {

                    return $scope.load({ $call: cb });
                },
                manageEvents: function (s, e, a) {

                    if (a.changed) {
                        treeDir.tree.on('changed.jstree', function (e, data) {
                         
                            if (typeof (s[a.changed]) == "function") {
                                s[a.changed](e, data);
                            } else {
                                var parseFunc = $parse(a.changed);
                                parseFunc(s);
                            }
                        });
                    }

                    if (a.ready) {
                        treeDir.tree.on('ready.jstree', function (e, data) {
                            if (typeof (s[a.ready]) == "function") {
                                s[a.ready](e, data);
                            } else {
                                var parseFunc = $parse(a.ready);
                                parseFunc(s);
                            }
                        });
                    }
                },
                managePlugins: function (s, e, a, config) {
                    if (a.treePlugins) {
                        config.plugins = a.treePlugins.split(',');
                        config.core = config.core || {};
                        config.core.check_callback = config.core.check_callback || true;

                        if (config.plugins.indexOf('state') >= 0) {
                            config.state = config.state || {};
                            config.state.key = a.treeStateKey;
                        }

                        if (config.plugins.indexOf('search') >= 0) {
                            if (a.search) {
                                s.$watch(a.search, function (newVal, oldVal) {
                                    if (newVal) {
                                        treeDir.tree.jstree(true).search(newVal, false, true);
                                    } else {
                                        treeDir.tree.jstree(true).clear_search();
                                    }
                                });
                            }
                        }

                        if (config.plugins.indexOf('checkbox') >= 0) {
                            config.checkbox = config.checkbox || {};
                            config.checkbox.keep_selected_style = false;

                            config.checkbox.three_state = false;

                            if (a.threeState == "true") {
                                config.checkbox.three_state = true;
                            }
                        }

                        if (config.plugins.indexOf('contextmenu') >= 0) {

                            if (s.treeContextmenu) {
                                config.contextmenu = config.contextmenu || {};

                                config.contextmenu.items = function (data) {

                                    return s.treeContextmenu(data);
                                }
                            }
                        }

                        if (config.plugins.indexOf('types') >= 0) {
                            if (a.treeTypes) {
                                config.types = s[a.treeTypes];
                                //console.log(config);
                            }

                            config.types = {
                                "disabled":{
                                    "icon": "fa fa-ban"
                                },
                                "folder": {
                                    "icon": "icon-folder"
                                },
                                "file": {
                                    "icon": "fa fa-file-text"
                                },
                                "user": {
                                    "icon": "icon-user"
                                },
                                "users": {
                                    "icon": "icon-users"
                                },
                                "cube": {
                                    "icon": "fa fa-cube"
                                },
                                "cubes": {
                                    "icon": "fa fa-cubes"
                                },
                                "star": {
                                    "icon": "fa fa-star"
                                },
                                "edit": {
                                    "icon": "fa fa-edit"
                                },
                                "database": {
                                    "icon": "fa fa-database"
                                },
                                "cloud": {
                                    "icon": "fa fa-cloud"
                                },
                                "filter": {
                                    "icon": "fa fa-filter"
                                },
                                "circle": {
                                    "icon": "fa fa-circle-o"
                                },
                                "list": {
                                    "icon": "fa fa-th-list"
                                }
                            }
                        }

                        if (config.plugins.indexOf('themes') >= 0) {
                            config.themes = {
                                "theme": "classic",
                                "dots": false,
                                "icons": true
                            }
                        }

                        if (config.plugins.indexOf('dnd') >= 0) {
                            if (a.treeDnd) {
                                config.dnd = s[a.treeDnd];
                                //console.log(config);
                            }
                        }
                    }
                    return config;
                },
                link: function (s, e, a) { // scope, element, attribute \O/
                    $(function () {
                        var config = {};

                        // users can define 'core'
                        config.core = {};
                        if (a.treeCore) {
                            config.core = $.extend(config.core, s[a.treeCore]);
                        }

                        // clean Case
                        a.treeData = a.treeData ? a.treeData.toLowerCase() : '';
                        a.treeSrc = a.treeSrc ? a.treeSrc.toLowerCase() : '';

                        if (a.treeData == 'html') {
                            treeDir.fetchResource(s, function (data) {
                                e.html(data);
                                treeDir.init(s, e, a, config);
                            });
                        } else if (a.treeData == 'json') {
                            treeDir.fetchResource(s, function (data) {
                                config.core.data = data;
                                treeDir.init(s, e, a, config);
                            });
                        } else if (a.treeData == 'scope') {
                            s.$watch(a.treeModel, function (n, o) {

                                if (n) {
                                    config.core.data = s[a.treeModel];
                                    $(e).jstree('destroy');
                                    treeDir.init(s, e, a, config);
                                }
                            }, true);

                            // Trigger it initally
                            // Fix issue #13
                            //config.core.data = s[a.treeModel];
                            //treeDir.init(s, e, a, config);
                        } else if (a.treeAjax) {
                            config.core.data = {
                                'url': a.treeAjax,
                                'data': function (node) {
                                    return {
                                        'id': node.id != '#' ? node.id : 1
                                    };
                                }
                            };
                            treeDir.init(s, e, a, config);
                        }
                    });

                },
                init: function (s, e, a, config) {
                    treeDir.managePlugins(s, e, a, config);

                    this.tree = $(e).jstree(config);

                    if (a.treeApi) {
                        s[a.treeApi] = $(e).jstree(true);
                    }

                    treeDir.manageEvents(s, e, a);
                }
            };

            return treeDir;

        }]);
    });
