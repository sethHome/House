(function () {
    angular
      .module('validation.rule', ['validation'])
      .config(['$validationProvider', function ($validationProvider) {
          var expression = {
              empty: function (value) {
                  console.log(value);
                  return true;
              },
              required: function (value) {
                  if (typeof (value) == "Array") {
                      return !!value && value.length > 0;
                  } else {
                      return !!value;
                  }
              },
              url: /((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)/,
              email: /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/,
              number: /^\d+$/,
              money: /^([1-9][\d]{0,7}|0)(\.[\d]{1,2})?$/,
              date : /((^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(10|12|0?[13578])([-\/\._])(3[01]|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(11|0?[469])([-\/\._])(30|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(0?2)([-\/\._])(2[0-8]|1[0-9]|0?[1-9])$)|(^([2468][048]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([3579][26]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][13579][26])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][13579][26])([-\/\._])(0?2)([-\/\._])(29)$))/,
              minlength: function (value, scope, element, attrs, param) {
                  return value && value.length >= param;
              },
              maxlength: function (value, scope, element, attrs, param) {
                  if (value) {
                      return value.length <= param;
                  } else {
                      return true;
                  }
              },
              numberornull: function (value) {

                  if (value == null || value == undefined) {
                      return true;
                  }
                  else {
                      var reg = new RegExp("^[0-9]*$");
                      return reg.test(value);
                  }
              },
          };

          var defaultMsg = {
              empty: {
                  error: 'empty',
                  success: '√'
              },
              required: {
                  error: '必填项 <i class="fa fa-warning"></i>',
                  success: '√ '
              },
              url: {
                  error: 'Url格式不正确',
                  success: '√'
              },
              email: {
                  error: '邮箱地址不正确',
                  success: '√'
              },
              number: {
                  error: '请输入数字',
                  success: '√ '
              },
              numberornull: {
                  error: '请输入数字',
                  success: '√ '
              },
              money: {
                  error: '请输入金额',
                  success: '√ '
              },
              date: {
                  error: '日期格式不正确',
                  success: '√ '
              },
              minlength: {
                  error: '输入的长度太短',
                  success: '√ '
              },
              maxlength: {
                  error: '输入的长度太长',
                  success: '√ '
              }
          };
          $validationProvider.setExpression(expression).setDefaultMsg(defaultMsg);
      }]);
}).call(this);
