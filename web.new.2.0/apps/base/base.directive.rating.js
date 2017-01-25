define(['apps/base/base.directive'],
    function (app) {
        
        app.directive("rating", function () {
            return {
                restrict: 'E',
                link: function (scope, element, attrs) {
                    $(element).rateit({
                        readonly: $(element).data('readonly') ? $(element).data('readonly') : false, // Not editable, for example to show rating that already exist 
                        resetable: $(element).data('resetable') ? $(element).data('resetable') : false,
                        value: $(element).data('value') ? $(element).data('value') : 0, // Current value of rating
                        min: $(element).data('min') ? $(element).data('min') : 1, // Maximum of star
                        max: $(element).data('max') ? $(element).data('max') : 5, // Maximum of star
                        step: $(element).data('step') ? $(element).data('step') : 0.1
                    });
                    // Tooltip Option      
                    if ($(element).data('tooltip')) {
                        var tooltipvalues = ['bad', 'poor', 'ok', 'good', 'super']; // You can change text here 
                        $(element).bind('over', function (event, value) { $(element).attr('title', tooltipvalues[value - 1]); });
                    }
                    // Confirmation before voting option      
                    if ($(element).data('confirmation')) {
                        $(element).on('beforerated', function (e, value) {
                            value = value.toFixed(1);
                            if (!confirm('Are you sure you want to rate element item: ' + value + ' stars?')) {
                                e.preventDefault();
                            }
                            else {
                                // We disable rating after voting. If you want to keep it enable, remove element part
                                $(element).rateit('readonly', true);
                            }
                        });
                    }
                    // Disable vote after rating
                    if ($(element).data('disable-after')) {
                        $(element).bind('rated', function (event, value) {
                            $(element).rateit('readonly', true);
                        });
                    }
                    // Display rating value as text below
                    if ($(element).parent().find('.rating-value')) {
                        $(element).bind('rated', function (event, value) {
                            if (value) value = value.toFixed(1);
                            $(element).parent().find('.rating-value').text('Your rating: ' + value);
                        });
                    }
                    // Display hover value as text below     
                    if ($(element).parent().find('.hover-value')) {
                        $(element).bind('over', function (event, value) {
                            if (value) value = value.toFixed(1);
                            $(element).parent().find('.hover-value').text('Hover rating value: ' + value);
                        });
                    }
                }
            };
        });
    });
