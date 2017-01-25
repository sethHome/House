define(['apps/system3/demo/demo.controller',
], function (app) {

    app.controller("demo.controller.wizard", function ($scope) {

       

        function formWizard() {

            if ($('.wizard').length && $.fn.stepFormWizard) {
                $('.wizard').each(function () {
                    $this = $(this);
                    $(this).stepFormWizard({
                        theme: $(this).data('style') ? $(this).data('style') : "circle",
                        showNav: $(this).data('nav') ? $(this).data('nav') : "top",
                        height: "auto",
                        rtl: $('body').hasClass('rtl') ? true : false,
                        onNext: function (i, wizard) {
                            if ($this.hasClass('wizard-validation')) {
                                return $('form', wizard).parsley().validate('block' + i);
                            }
                        },
                        onFinish: function (i) {
                            if ($this.hasClass('wizard-validation')) {
                                return $('form', wizard).parsley().validate();
                            }
                        }
                    });
                });

                /* Fix issue only with tabs with Validation on error show */
                $('#validation .wizard .sf-btn').on('click', function () {
                    setTimeout(function () {
                        $(window).resize();
                        $(window).trigger('resize');
                    }, 50);
                });
            }
        }


        function formValidation() {
            if ($('.form-validation').length && $.fn.validate) {
                /* We add an addition rule to show you. Example : 4 + 8. You can other rules if you want */
                $.validator.methods.operation = function (value, element, param) {
                    return value == param;
                };
                $.validator.methods.customemail = function (value, element) {
                    return /^([-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4})+$/.test(value);
                };
                $('.form-validation').each(function () {
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
                    $(".form-validation .cancel").click(function () {
                        formValidation.resetForm();
                    });
                });
            }
        }

        $scope.$watch("$viewContentLoaded", function () {

            $('#style .form-wizard-style').on('click', 'a', function (e) {
                $('.form-wizard-style a').removeClass('current');
                $(this).addClass('current');
                var style = $(this).attr('id');
                e.preventDefault();
                $('#style .wizard-div').removeClass('current');
                $('#style .wizard-' + style).addClass('current');
            });

            $('#navigation .form-wizard-nav').on('click', 'a', function (e) {
                $('#navigation .form-wizard-nav a').removeClass('current');
                $(this).addClass('current');
                var style = $(this).attr('id');
                e.preventDefault();
                $('#navigation .wizard-div').removeClass('current');
                $('#navigation .wizard-' + style).addClass('current');
            });

            $('.nav-tabs > li > a').on('click', function () {
                /* Fix issue only with tabs, demo purpose */
                setTimeout(function () {
                    $(window).resize();
                    $(window).trigger('resize');
                }, 0);
            });

            //formWizard();

            //formValidation();
        })
    });

});
