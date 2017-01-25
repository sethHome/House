define(['apps/base/base.directive',
    'assets/global/plugins/step-form-wizard/plugins/parsley/parsley.min',
    'assets/global/plugins/step-form-wizard/js/step-form-wizard',
    'css!assets/global/plugins/step-form-wizard/css/step-form-wizard.min',
],
    function (app) {

        app.directive('wizard', function ($timeout) {

            return {
                restrict: 'C',
                link: function ($scope, element, attrs) {
                   
                    if ($.fn.stepFormWizard) {
                       
                        var wiz = $(element).stepFormWizard({
                            theme: $(element).data('style') ? $(element).data('style') : "circle",
                            showNav: $(element).data('nav') ? $(element).data('nav') : "top",
                            height: "auto",
                            rtl: $('body').hasClass('rtl') ? true : false,
                            onNext: function (i, wizard) {
                                if ($(element).hasClass('wizard-validation')) {
                                    return $('form', wizard).parsley().validate('block' + i);
                                }
                            },
                            onFinish: function (i, wizard) {
                                if ($(element).hasClass('wizard-validation')) {
                                    if (!$('form', wizard).parsley().validate()) {
                                        return;
                                    }
                                }

                                if ($scope.onFinish) {
                                    $scope.onFinish();
                                }
                            }
                        });

                       
                        window.setTimeout(function () {
                            wiz.goTo(0);
                        }, 100);
                       
                    }

                    if ($(element).hasClass('form-validation') && $.fn.validate) {
                        /* We add an addition rule to show you. Example : 4 + 8. You can other rules if you want */
                        $.validator.methods.operation = function (value, element, param) {
                            return value == param;
                        };
                        $.validator.methods.customemail = function (value, element) {
                            return /^([-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4})+$/.test(value);
                        };
                        $(element).each(function () {
                            var formValidation = $(this).validate({
                                success: "valid",
                                submitHandler: function () { alert("Form is valid! We submit it") },
                                errorClass: "form-error",
                                validClass: "form-success",
                                errorElement: "div",
                                ignore: [],
                                rules: {
                                    avatar: { extension: "jpg|png|gif|jpeg|doc|docx|pdf|xls|rar|zip" },
                                    password2: { equalTo: '#password' },
                                    calcul: { operation: 12 },
                                    url: { url: true },
                                    email: {
                                        required: {
                                            depends: function () {
                                                $(this).val($.trim($(this).val()));
                                                return true;
                                            }
                                        },
                                        customemail: true
                                    },
                                },
                                messages: {
                                    name: { required: 'Enter your name' },
                                    lastname: { required: 'Enter your last name' },
                                    firstname: { required: 'Enter your first name' },
                                    email: { required: 'Enter email address', customemail: 'Enter a valid email address' },
                                    language: { required: 'Enter your language' },
                                    mobile: { required: 'Enter your phone number' },
                                    avatar: { required: 'You must upload your avatar' },
                                    password: { required: 'Write your password' },
                                    password2: { required: 'Write your password', equalTo: '2 passwords must be the same' },
                                    calcul: { required: 'Enter the result of 4 + 8', operation: 'Result is false. Try again!' },
                                    terms: { required: 'You must agree with terms' }
                                },
                                highlight: function (element, errorClass, validClass) {
                                    $(element).closest('.form-control').addClass(errorClass).removeClass(validClass);
                                },
                                unhighlight: function (element, errorClass, validClass) {
                                    $(element).closest('.form-control').removeClass(errorClass).addClass(validClass);
                                },
                                errorPlacement: function (error, element) {
                                    if (element.hasClass("custom-file") || element.hasClass("checkbox-type") || element.hasClass("language")) {
                                        element.closest('.option-group').after(error);
                                    }
                                    else if (element.is(":radio") || element.is(":checkbox")) {
                                        element.closest('.option-group').after(error);
                                    }
                                    else if (element.parent().hasClass('input-group')) {
                                        element.parent().after(error);
                                    }
                                    else {
                                        error.insertAfter(element);
                                    }
                                },
                                invalidHandler: function (event, validator) {
                                    var errors = validator.numberOfInvalids();
                                }
                            });
                            $(element).find('.cancel').click(function () {
                                formValidation.resetForm();
                            });
                        });
                    }
                }
            };

        });
    });
