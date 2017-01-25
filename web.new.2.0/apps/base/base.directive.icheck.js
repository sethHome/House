define(['apps/base/base.directive'],
    function (app) {

        app.directive('icheck', function ($timeout, $parse) {
            return {
                require: 'ngModel',
                link: function ($scope, element, $attrs, ngModel) {
                    return $timeout(function () {
                       
                        $scope.$watch($attrs['ngModel'], function (newValue) {
                            $(element).iCheck('update');
                        })

                        $scope.$watch($attrs['ngDisabled'], function (newValue) {
                            $(element).iCheck(newValue ? 'disable' : 'enable');
                            $(element).iCheck('update');
                        })

                        return $(element).iCheck({
                            checkboxClass: 'icheckbox_square-blue',
                            radioClass: 'iradio_square-blue'

                        }).on('ifChanged', function (event) {
                            if ($(element).attr('type') === 'checkbox' && $attrs['ngModel']) {
                                $scope.$apply(function () {
                                    return ngModel.$setViewValue(event.target.checked);
                                });
                            }
                            if ($(element).attr('type') === 'radio' && $attrs['ngModel']) {
                                return $scope.$apply(function () {

                                    var value = $attrs['value'];
                                    return ngModel.$setViewValue(value);
                                   
                                });
                            }
                        });
                    });
                }
            };
        });
    });
