define(['apps/base/base.directive', 'ckeditor'],
    function () {

        var app = angular.module('ngCkeditor', []);
        var $defer, loaded = false;

        app.run(['$q', '$timeout', function ($q, $timeout) {
            $defer = $q.defer();

            if (angular.isUndefined(CKEDITOR)) {
                throw new Error('CKEDITOR not found');
            }
            CKEDITOR.disableAutoInline = true;
            function checkLoaded() {
                if (CKEDITOR.status === 'loaded') {

                    loaded = true;
                    $defer.resolve();

                } else {
                    checkLoaded();
                }
            }

            CKEDITOR.on('loaded', checkLoaded);
            $timeout(checkLoaded, 100);
        }]);

        app.directive('ckeditor', ['$timeout', '$q', 'attachUpload', function ($timeout, $q, attachUpload) {

            return {
                restrict: 'AC',
                require: ['ngModel', '^?form'],
                scope: false,
                link: function (scope, element, attrs, ctrls) {
                    var ngModel = ctrls[0];
                    var form = ctrls[1] || null;
                    var EMPTY_HTML = '<p></p>',
                        isTextarea = element[0].tagName.toLowerCase() === 'textarea',
                        data = [],
                        isReady = false;

                    if (!isTextarea) {
                        element.attr('contenteditable', true);
                    }

                    var onLoad = function () {
                        var options = {
                            removePlugins: 'elementspath',
                            resize_enabled: false,
                            extraPlugins: 'uploadimage',
                            uploadUrl: attachUpload + '/open',
                            toolbar: attrs["toolbar"] ? attrs["toolbar"] : 'full',
                            toolbar_full: [ //jshint ignore:line
                                {
                                    name: 'basicstyles',
                                    items: ['Bold', 'Italic', 'Strike', 'Underline']
                                },
                                { name: 'paragraph', items: ['BulletedList', 'NumberedList', 'Blockquote'] },
                                { name: 'editing', items: ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock'] },
                                { name: 'links', items: ['Link', 'Unlink', 'Anchor'] },
                                { name: 'tools', items: ['SpellChecker', 'Maximize'] },
                                '/',
                                {
                                    name: 'styles',
                                    items: ['Format', 'Font', 'FontSize', 'TextColor', 'PasteText', 'PasteFromWord', 'RemoveFormat']
                                },
                                { name: 'insert', items: ['Image', 'Table', 'SpecialChar', 'Smiley'] },
                                { name: 'forms', items: ['Outdent', 'Indent'] },
                                { name: 'clipboard', items: ['Undo', 'Redo'] },
                                { name: 'document', items: ['PageBreak', 'Source'] },
                                
                            ],
                            toolbar_chat: [ //jshint ignore:line
                                {
                                    name: 'styles',
                                    items: ['Font','FontSize', 'TextColor', 'Smiley']
                                },

                            ],
                            disableNativeSpellChecker: false,
                            
                            height: attrs["height"] ? attrs["height"] : '250px',
                            width: '100%',
                            // The following options are not necessary and are used here for presentation purposes only.
                            // They configure the Styles drop-down list and widgets to use classes.

                            stylesSet: [
                                { name: 'Narrow image', type: 'widget', widget: 'image', attributes: { 'class': 'image-narrow' } },
                                { name: 'Wide image', type: 'widget', widget: 'image', attributes: { 'class': 'image-wide' } }
                            ],

                            // Load the default contents.css file plus customizations for this sample.
                            contentsCss: [CKEDITOR.basePath + 'contents.css', 'http://sdk.ckeditor.com/samples/assets/css/widgetstyles.css'],

                            // Configure the Enhanced Image plugin to use classes instead of styles and to disable the
                            // resizer (because image size is controlled by widget styles or the image takes maximum
                            // 100% of the editor width).
                            image2_alignClasses: ['image-align-left', 'image-align-center', 'image-align-right'],
                            image2_disableResizer: true
                        };
                        options = angular.extend(options, scope[attrs.ckeditor]);

                        var instance = (isTextarea) ? CKEDITOR.replace(element[0], options) : CKEDITOR.inline(element[0], options),
                            configLoaderDef = $q.defer();

                        element.bind('$destroy', function () {
                            if (instance && CKEDITOR.instances[instance.name]) {
                                CKEDITOR.instances[instance.name].destroy();
                            }
                        });
                        var setModelData = function (setPristine) {
                            var data = instance.getData();
                            if (data === '') {
                                data = null;
                            }
                            $timeout(function () { // for key up event
                                if (setPristine !== true || data !== ngModel.$viewValue) {
                                    ngModel.$setViewValue(data);
                                }

                                if (setPristine === true && form) {
                                    form.$setPristine();
                                }
                            }, 0);
                        }, onUpdateModelData = function (setPristine) {
                            if (!data.length) {
                                return;
                            }

                            var item = data.pop() || EMPTY_HTML;
                            isReady = false;
                            instance.setData(item, function () {
                                setModelData(setPristine);
                                isReady = true;
                            });
                        };

                        instance.on('pasteState', setModelData);
                        instance.on('change', setModelData);
                        instance.on('blur', setModelData);
                        //instance.on('key',          setModelData); // for source view

                        instance.on('instanceReady', function () {
                            scope.$broadcast('ckeditor.ready');
                            scope.$apply(function () {
                                onUpdateModelData(true);
                            });

                            instance.document.on('keyup', setModelData);
                        });
                        instance.on('customConfigLoaded', function () {
                            configLoaderDef.resolve();
                        });

                        ngModel.$render = function () {
                            data.push(ngModel.$viewValue);
                            if (isReady) {
                                onUpdateModelData();
                            }
                        };
                    };

                    if (CKEDITOR.status === 'loaded') {
                        loaded = true;
                    }
                    if (loaded) {
                        onLoad();
                    } else {
                        $defer.promise.then(onLoad);
                    }
                }
            };
        }]);

        return app;
    });
