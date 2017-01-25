define(['apps/system3/business/business',
        'apps/system3/business/customer/customer.service'], function (app) {

            app.module.directive('customer', function (customerService) {

                return {
                    restrict: 'E',
                    link: function ($scope, element, $attrs) {
                        $scope[$attrs.model] = customerService.getCustomers({
                            txtfilter: $attrs.txtfilter,
                            type: $attrs.type
                        }).$object;
                    }
                };
            });
        });

