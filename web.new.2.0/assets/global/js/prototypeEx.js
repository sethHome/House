//================================String====================================

// 去除左右空格
String.prototype.trim = function () {
    var str = this,
    str = str.replace(/^\s\s*/, ''),
    ws = /\s/,
    i = str.length;
    while (ws.test(str.charAt(--i)));
    return str.slice(0, i + 1);
}

// 去除左边空格
String.prototype.trimStart = function () {
    return this.replace(/(^\s*)/g, "");
};

// 去除右边空格
String.prototype.trimEnd = function () {
    return this.replace(/(\s*$)/g, "");
};

// 去除左右空格
String.prototype.clearSpaceToOne = function () {
    return this.replace(/(^\s*)|(\s*$)/g, "").replace(/\s+/g, " ");
};

// 判断字符是否是 str 开头
String.prototype.startWith = function (str) {
    if (str == null || str == "" || this.length == 0 || str.length > this.length)
        return false;
    if (this.substr(0, str.length) == str)
        return true;
    else
        return false;
    return true;
};

// 判断字符是否是 str 结尾
String.prototype.endWith = function (str) {
    if (str == null || str == "" || this.length == 0 || str.length > this.length)
        return false;
    if (this.substring(this.length - str.length) == str)
        return true;
    else
        return false;
    return true;
};

// 得到字符长度，中文算2个字符
String.prototype.getByteLength = function () {
    var ch, bytenum = 0;
    var pt = /[^\x00-\xff]/;
    for (var i = 0; i < this.length; i++) {
        ch = this.substr(i, 1);
        if (ch.match(pt)) {
            bytenum += 2;
        } else {
            bytenum += 1;
        }
    }
    return bytenum;
};

// 字符截取 num 长度，并在结尾加 ostr
String.prototype.getByteLengthString = function (num, ostr) {
    var ch, bytenum = 0;
    var rs = "";
    var pt = /[^\x00-\xff]/;
    for (var i = 0; i < this.length; i++) {
        ch = this.substr(i, 1);
        if (ch.match(pt)) {
            bytenum += 2;
            if (bytenum > num) {
                return rs + ostr;
            }
        } else {
            bytenum += 1;
            if (bytenum == num) {
                return rs + ostr;
            }
        }
        rs += ch;
    }
    return rs;
};

// 仿 C# String.Format
String.prototype.format = function () {
    var args = arguments;

    if (arguments.length == 1 && toString.apply(arguments[0]) === '[object Array]') {
        args = arguments[0];
    }

    return this.replace(/\{(\d+)\}/g, function (m, i) {
        return args[i];
    });
};

// 转换为 数字 类型
String.prototype.toInt = function () {
    return parseInt(this, 10);
};

// 仿 C# String.Format
//String.format = function () {
//    if (arguments.length == 0)
//        return null;
//    var str = arguments[0];
//    for (var i = 1; i < arguments.length; i++) {
//        var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
//        str = str.replace(re, arguments[i]);
//    }
//    return str;
//};

/**
* 过滤字符串里面的html标记，类似于 innerText
*/
String.prototype.stripTags = function () {
    return this.replace(/<\/?[^>]+>/gi, '');
};

/**
* 类似 escape，转换 " & < > 为 &quot; &amp; %lt; &gt;
*/
String.prototype.escapeHTML = function () {
    var div = document.createElement('div');
    var text = document.createTextNode(this);
    div.appendChild(text);
    return div.innerHTML;
};

/**
* 和上面相反
*/
String.prototype.unescapeHTML = function () {
    var div = document.createElement('div');
    div.innerHTML = this.stripTags();
    return div.childNodes[0] ? div.childNodes[0].nodeValue : '';
};

// 生成重复字符串
String.prototype.times = function (n) {
    if (n == 1) {
        return this;
    }
    var s = this.times(Math.floor(n / 2));
    s += s;
    if (n % 2) {
        s += this;
    }
    return s;
}

// 得到安全字符串（用于搜索）
String.prototype.getSafeText = function () {
    var _regSafeText = /(\'|\-{2,2})+/;
    return this.replace(_regSafeText, "");
};

// 反序列化提交参数对象 如把 name=aa&sex=1 转换为 {name:"aaa",sex="1"}
String.prototype.deserialize = function (separator) {
    var hash = {};
    var match = this.trim().match(/([^?#]*)(#.*)?$/);
    if (!match) return {};
    var querys = match[1].split(separator || '&');
    for (var i = 0; i < querys.length; i++) {
        var pair = querys[i].split('=')
        if (pair[0]) {
            var key = decodeURIComponent(pair.shift());
            var value = pair.length > 1 ? pair.join('=') : pair[0];
            if (value != undefined) value = decodeURIComponent(value);
            hash[key] = value;
        }
    }
    return hash;
}

String.prototype.toTDate = function (format) {
    var date = this;

    if (date == null || date == undefined) {
        return "";
    }

    var d = new Date(date.replace('T', ' '));

    if (d == null ||
        d == undefined ||
        d.getFullYear() == 1900) {
        return "";
    }

    if (format) {
        return d.format(format);
    } else {
        return d.format('yyyy/MM/dd');
    }
}

String.prototype.preStr = function (length) {
    if (this.length > length) {
        return this.substr(0, length) + "...";
    } else {
        return this;
    }
}

String.prototype.PadLeft = function (totalWidth, paddingChar) {

    if (paddingChar != null) {

        return this.PadHelper(totalWidth, paddingChar, false);

    } else {

        return this.PadHelper(totalWidth, ' ', false);

    }

}

String.prototype.PadRight = function (totalWidth, paddingChar) {

    if (paddingChar != null) {

        return this.PadHelper(totalWidth, paddingChar, true);

    } else {

        return this.PadHelper(totalWidth, ' ', true);

    }



}

String.prototype.PadHelper = function (totalWidth, paddingChar, isRightPadded) {



    if (this.length < totalWidth) {

        var paddingString = new String();

        for (i = 1; i <= (totalWidth - this.length) ; i++) {

            paddingString += paddingChar;

        }



        if (isRightPadded) {

            return (this + paddingString);

        } else {

            return (paddingString + this);

        }

    } else {

        return this;

    }

}

String.prototype.removeFileExtension = function () {
    var reg = /\.\w+$/;
    return this.replace(reg, '');
}

//================================Data====================================

Date.prototype.format = function (format) {
    // 时间格式化
    var o = {
        "M+": this.getMonth() + 1,                      //month 
        "d+": this.getDate(),                           //day 
        "h+": this.getHours(),                          //hour 
        "m+": this.getMinutes(),                        //minute 
        "s+": this.getSeconds(),                        //second 
        "q+": Math.floor((this.getMonth() + 3) / 3),    //quarter 
        "S": this.getMilliseconds()                     //millisecond 
    }
    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
};

//日期增加 "2000/12/31".dateAdd('d',1,);
Date.prototype.dateAdd = function (strInterval, Number) {
    var dtTmp = this;
    switch (strInterval) {
        case 's': return new Date(Date.parse(dtTmp) + (1000 * Number));
        case 'n': return new Date(Date.parse(dtTmp) + (60000 * Number));
        case 'h': return new Date(Date.parse(dtTmp) + (3600000 * Number));
        case 'd': return new Date(Date.parse(dtTmp) + (86400000 * Number));
        case 'w': return new Date(Date.parse(dtTmp) + ((86400000 * 7) * Number));
        case 'q': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number * 3, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        case 'm': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        case 'y': return new Date((dtTmp.getFullYear() + Number), dtTmp.getMonth(), dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
    }
};

//+---------------------------------------------------  
//| 比较日期差 dtEnd 格式为日期型或者 有效日期格式字符串  
//+---------------------------------------------------
Date.prototype.dateDiff = function (strInterval, dtEnd) {
    var dtStart = this;
    if (typeof dtEnd == 'string')//如果是字符串转换为日期型  
    {
        dtEnd = new Date(dtEnd);
    }
    switch (strInterval) {
        case 's': return parseInt((dtEnd - dtStart) / 1000);
        case 'n': return parseInt((dtEnd - dtStart) / 60000);
        case 'h': return parseInt((dtEnd - dtStart) / 3600000);
        case 'd': return parseInt((dtEnd - dtStart) / 86400000);
        case 'w': return parseInt((dtEnd - dtStart) / (86400000 * 7));
        case 'm': return (dtEnd.getMonth() + 1) + ((dtEnd.getFullYear() - dtStart.getFullYear()) * 12) - (dtStart.getMonth() + 1);
        case 'y': return dtEnd.getFullYear() - dtStart.getFullYear();
    }
};

//+---------------------------------------------------  
//| 把日期分割成数组  
//+---------------------------------------------------
Date.prototype.toArray = function () {
    var myDate = this;
    var myArray = Array();
    myArray[0] = myDate.getFullYear();
    myArray[1] = myDate.getMonth();
    myArray[2] = myDate.getDate();
    myArray[3] = myDate.getHours();
    myArray[4] = myDate.getMinutes();
    myArray[5] = myDate.getSeconds();
    return myArray;
};

//+---------------------------------------------------  
//| 取得日期数据信息  
//| 参数 interval 表示数据类型  
//| y 年 m月 d日 w星期 ww周 h时 n分 s秒  
//+---------------------------------------------------
Date.prototype.datePart = function (interval) {
    var myDate = this;
    var partStr = '';
    var week = ['日', '一', '二', '三', '四', '五', '六'];
    switch (interval) {
        case 'y': partStr = myDate.getFullYear(); break;
        case 'm': partStr = myDate.getMonth() + 1; break;
        case 'd': partStr = myDate.getDate(); break;
        case 'w': partStr = week[myDate.getDay()]; break;
        case 'ww': partStr = myDate.WeekNumOfYear(); break;
        case 'h': partStr = myDate.getHours(); break;
        case 'n': partStr = myDate.getMinutes(); break;
        case 's': partStr = myDate.getSeconds(); break;
    }
    return partStr;
};

//+---------------------------------------------------  
//| 取得当前日期所在月的最大天数  
//+---------------------------------------------------
Date.prototype.maxDayOfDate = function () {
    var myDate = this;
    var ary = myDate.toArray();
    var date1 = (new Date(ary[0], ary[1] + 1, 1));
    var date2 = date1.dateAdd(1, 'm', 1);
    var result = dateDiff(date1.format('yyyy-MM-dd'), date2.format('yyyy-MM-dd'));
    return result;
};


//================================Array====================================

if (typeof Array.prototype['max'] == 'undefined') {
    Array.prototype.map = function (fn, thisObj) {
        var scope = thisObj || window;
        var a = [];
        for (var i = 0, j = this.length; i < j; ++i) {
            a.push(fn.call(scope, this[i], i, this));
        }
        return a;
    };
    Array.prototype.max = function () {
        return Math.max.apply({}, this)
    };
    Array.prototype.min = function () {
        return Math.min.apply({}, this)
    };
}

Array.prototype.indexOf = function (oValue) {
    /// <summary>在数组中查找对应值，返回所在位置</summary>
    /// <param name="oValue" type="Object">要进行匹配的对象</param>
    var n = -1;
    for (var i = 0; i < this.length; i++) {
        if (this[i] == oValue) {
            n = i;
            break;
        }
    }
    return n;
};

Array.prototype.isIndexOf = function (oValue) {
    /// <summary>在数组中查找有无对应值</summary>
    /// <param name="oValue" type="Object">要进行匹配的对象</param>
    return this.indexOf(oValue) == -1 ? false : true;
};

Array.prototype.each = function (fn) {
    if (typeof Array.prototype.forEach === "function") {
        this.forEach(fn)
    } else {
        var scope = arguments[1] || window;
        for (var i = 0, j = this.length; i < j; ++i) {
            fn.call(scope, this[i], i, this);
        }
    }
};

Array.range = function (start, end) {
    var _range = []
    for (var i = start, l = end - start; i < l; i++) {
        _range.push(i)
    }
    return _range
};

Array.indexOf = function (args, value) {
    /// <summary>在数组中查找对应值，返回所在位置</summary>
    /// <param name="oValue" type="Object">要进行匹配的对象</param>
    var n = -1;
    for (var i = 0; i < args.length; i++) {
        if (args[i] == value) {
            n = i;
            break;
        }
    }
    return n;
};

Array.isIndexOf = function (argso, value) {
    /// <summary>在数组中查找有无对应值</summary>
    /// <param name="oValue" type="Object">要进行匹配的对象</param>
    if (value != "") {
        for (var i = 0; i < argso.length; i++) {
            if (argso[i] == value) {
                return true;
            }
        }
    }
    return false;
};

Array.prototype.custRemove = function (callBack) {

    for (var i = 0; i < this.length; i++) {
        if (callBack(this[i])) {
            this.splice(i, 1);
        }
    }
};

Array.prototype.removeObj = function (obj) {

    for (var i = 0; i < this.length; i++) {
        if (this[i] === obj) {
            this.splice(i, 1);
        }
    }
};

Array.prototype.contains = function (callBack) {

    for (var i = 0; i < this.length; i++) {

        if (callBack(this[i])) {
            return true;
        }
    }

    return false;
}

Array.prototype.insertAt = function (index, item) {
    this.splice(index, 0, item);
};

Array.prototype.get = function (callBack) {

    for (var i = 0; i < this.length; i++) {

        if (callBack(this[i])) {
            return this[i];
        }
    }
};

Array.prototype.find = function (callBack) {

    for (var i = 0; i < this.length; i++) {

        if (callBack(this[i])) {
            return this[i];
        }
    }
};

Array.prototype.count = function (callBack) {

    var result = 0;
    for (var i = 0; i < this.length; i++) {

        if (callBack(this[i])) {
            result++;
        }
    }

    return result;
};

Array.prototype.where = function (callBack) {

    var result = [];
    for (var i = 0; i < this.length; i++) {

        if (callBack(this[i])) {
            result.push(this[i]);
        }
    }

    return result;
};


Array.prototype.allIndex = function (callBack) {

    var all = [];

    for (var i = 0; i < this.length; i++) {

        if (callBack(this[i])) {
            all.push(i);
        }
    }

    return all;
}

Number.prototype.toBinary = function () {
    return (this & value) > 0;
}


Number.prototype.toFileSize = function () {
    if (this == 0) {
        return '空文件'
    }

    var size = parseFloat(this);
    var rank = 0;
    var rankchar = 'Bytes';
    while (size > 1024) {
        size = size / 1024;
        rank++;
    }
    if (rank == 1) {
        rankchar = "KB";
    }
    else if (rank == 2) {
        rankchar = "MB";
    }
    else if (rank == 3) {
        rankchar = "GB";
    }
    return size.toFixed(2) + " " + rankchar;
}